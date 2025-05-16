using System.Net.Sockets;
using System.Text;
using Microsoft.Data.SqlClient;
using System.IO;

namespace TcpClientApp
{
    public partial class Form1 : Form
    {
        // TCP istemci nesnesi
        private TcpClient? tcpClient;
        // Ağ akışı
        private NetworkStream? networkStream;
        // Veritabanı bağlantı dizesi
        private string connectionString = "Data Source=SOLAIRE\\SQLEXPRESS01;Initial Catalog=TCPLogs;Integrated Security=True;TrustServerCertificate=True";
        // Veritabanı bağlantı nesnesi
        private SqlConnection? sqlConnection;
        // İstemci çalışıyor mu?
        private bool isRunning = false;

        public Form1()
        {
            InitializeComponent();

            // Form yüklendiğinde yapılacak işlemler
            // Başlangıçta disconnect butonu devre dışı olsun
            buttonDisconnect.Enabled = false;

            // Log mesajı ekleyelim
            LogMessage("Application started. Enter IP and Port information to connect.");
        }

        // Form kapatılırken açık kaynakları temizle
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Açık bağlantıları kapat
            CloseConnection();

            // Veritabanı bağlantısını kapat
            if (sqlConnection != null && sqlConnection.State == System.Data.ConnectionState.Open)
            {
                sqlConnection.Close();
                sqlConnection.Dispose();
            }

            base.OnFormClosing(e);
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            try
            {
                // Giriş alanlarından IP ve Port bilgilerini alalım
                string serverIp = textBoxServerIp.Text.Trim();

                // Port numarasını dönüştürelim (int.Parse kullanma, TryParse daha güvenli)
                if (!int.TryParse(textBoxPort.Text.Trim(), out int serverPort))
                {
                    LogMessage("Error: Please enter a valid port number!");
                    return;
                }

                // IP ve Port boş mu kontrol edelim
                if (string.IsNullOrEmpty(serverIp) || serverPort <= 0)
                {
                    LogMessage("Error: IP address and Port number are required!");
                    return;
                }

                // Bağlantı zaten açık mı kontrol edelim
                if (isRunning)
                {
                    LogMessage("Connection is already open!");
                    return;
                }

                // Veritabanını başlatalım (tablo yok ise oluşturulacak)
                InitializeDatabase();

                // TCP istemcisini oluşturalım
                tcpClient = new TcpClient();

                // Bağlantı zamanaşımı süresi ayarlayalım (3 saniye)
                var result = tcpClient.BeginConnect(serverIp, serverPort, null, null);
                var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(3));

                if (!success)
                {
                    throw new Exception("Connection timed out.");
                }

                // Bağlantıyı tamamlayalım
                tcpClient.EndConnect(result);

                // Ağ akışını alalım
                networkStream = tcpClient.GetStream();

                // İstemci çalışıyor olarak işaretleyelim
                isRunning = true;

                // Dinleme işlemini başlatalım (ayrı bir thread'de)
                Thread listenerThread = new Thread(ListenForMessages);
                listenerThread.IsBackground = true;
                listenerThread.Start();

                // Butonların durumlarını güncelleyelim
                buttonConnect.Enabled = false;
                buttonDisconnect.Enabled = true;

                // IP ve Port giriş alanlarını devre dışı bırakalım
                textBoxServerIp.Enabled = false;
                textBoxPort.Enabled = false;

                LogMessage($"Connection successful: {serverIp}:{serverPort}");
            }
            catch (Exception ex)
            {
                LogMessage($"Connection error: {ex.Message}");

                // Hata durumunda bağlantıyı kapatalım
                CloseConnection();
            }
        }

        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                // Bağlantıyı kapat
                CloseConnection();
                LogMessage("Connection closed.");
            }
            catch (Exception ex)
            {
                LogMessage($"Connection closing error: {ex.Message}");
            }
        }

        // Log mesajını TextBox'a ekler ve yeni satıra geçer
        private void LogMessage(string message)
        {
            // UI thread'inde çalıştığımızdan emin olalım
            if (textBoxLogs.InvokeRequired)
            {
                textBoxLogs.Invoke(new Action<string>(LogMessage), message);
                return;
            }

            // Mesajın başına zaman damgası ekleyelim
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";

            // TextBox'a mesajı ekleyelim
            textBoxLogs.AppendText(logMessage + Environment.NewLine);

            // Son eklenen mesajı görünür yapmak için aşağı kaydıralım
            textBoxLogs.SelectionStart = textBoxLogs.Text.Length;
            textBoxLogs.ScrollToCaret();
        }

        // Veritabanı bağlantısını başlatır ve gerekirse tabloyu oluşturur
        private void InitializeDatabase()
        {
            try
            {
                // Veritabanı bağlantısını oluştur
                sqlConnection = new SqlConnection(connectionString);

                // Bağlantıyı aç
                sqlConnection.Open();

                // Tablo oluştur sorgusu (eğer yoksa)
                string createTableQuery = @"
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TCPMessages')
                    BEGIN
                        CREATE TABLE TCPMessages (
                            Id INT IDENTITY(1,1) PRIMARY KEY,
                            MessageText NVARCHAR(MAX),
                            ReceivedTime DATETIME DEFAULT GETDATE()
                        )
                    END";

                // Sorguyu çalıştır
                using (SqlCommand command = new SqlCommand(createTableQuery, sqlConnection))
                {
                    command.ExecuteNonQuery();
                }

                // Bağlantıyı kapat
                sqlConnection.Close();

                LogMessage("Database connection and table ready.");
            }
            catch (Exception ex)
            {
                LogMessage($"Database error: {ex.Message}");
            }
        }

        private void CloseConnection()
        {
            // İstemci durumunu güncelle
            isRunning = false;

            // Ağ akışını kapat
            if (networkStream != null)
            {
                networkStream.Close();
                networkStream.Dispose();
                networkStream = null;
            }

            // TCP istemcisini kapat
            if (tcpClient != null)
            {
                tcpClient.Close();
                tcpClient.Dispose();
                tcpClient = null;
            }

            // Kontrollerin durumunu güncelle
            if (buttonConnect.InvokeRequired)
            {
                buttonConnect.Invoke(new Action(() => {
                    UpdateUIAfterDisconnect();
                }));
            }
            else
            {
                UpdateUIAfterDisconnect();
            }
        }

        // UI kontrollerini bağlantı kesildikten sonra güncelleme
        private void UpdateUIAfterDisconnect()
        {
            buttonConnect.Enabled = true;
            buttonDisconnect.Enabled = false;
            textBoxServerIp.Enabled = true;
            textBoxPort.Enabled = true;
        }

        private void ListenForMessages()
        {
            try
            {
                byte[] buffer = new byte[4096];
                int bytesRead;

                // İstemci çalıştığı sürece mesajları dinleyelim
                while (isRunning && networkStream != null)
                {
                    // Eğer veri varsa oku
                    if (networkStream.DataAvailable && networkStream.CanRead)
                    {
                        bytesRead = networkStream.Read(buffer, 0, buffer.Length);

                        if (bytesRead > 0)
                        {
                            // Veriyi string'e dönüştür
                            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                            // Log mesajını ekrana yaz
                            LogMessage($"Incoming Data: {message}");

                            // Veritabanına kaydet
                            SaveMessageToDatabase(message);
                        }
                    }

                    // Her döngüde biraz bekleyelim (CPU kullanımını azaltmak için)
                    Thread.Sleep(100);
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Listening error: {ex.Message}");
                CloseConnection();
            }
        }

        private void SaveMessageToDatabase(string message)
        {
            try
            {
                // Veritabanı bağlantısını aç
                if (sqlConnection == null)
                {
                    sqlConnection = new SqlConnection(connectionString);
                }

                if (sqlConnection.State != System.Data.ConnectionState.Open)
                {
                    sqlConnection.Open();
                }

                // SQL komutunu hazırla
                string insertQuery = "INSERT INTO TCPMessages (MessageText) VALUES (@MessageText)";

                using (SqlCommand command = new SqlCommand(insertQuery, sqlConnection))
                {
                    // Parametreyi ekle (SQL injection'a karşı koruma)
                    command.Parameters.AddWithValue("@MessageText", message);

                    // Komutu çalıştır
                    command.ExecuteNonQuery();
                }

                // Bağlantıyı kapat
                sqlConnection.Close();

                LogMessage("Message saved to database.");
            }
            catch (Exception ex)
            {
                LogMessage($"Error saving to database: {ex.Message}");
            }
        }
    }
}