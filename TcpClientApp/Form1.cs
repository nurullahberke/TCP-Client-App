using System.Net.Sockets;
using System.Text;
using Microsoft.Data.SqlClient; // SqlClient için do?ru namespace
using System.IO;

namespace TcpClientApp
{
    public partial class Form1 : Form
    {
        // TCP istemci nesnesi - null olabilir
        private TcpClient? tcpClient;
        // A? ak??? - null olabilir
        private NetworkStream? networkStream;
        // Veritaban? ba?lant? dizesi
        private string connectionString = "Data Source=localhost;Initial Catalog=TCPLogs;Integrated Security=True";
        // Veritaban? ba?lant? nesnesi - null olabilir
        private SqlConnection? sqlConnection;
        // ?stemci çal???yor mu?
        private bool isRunning = false;

        public Form1()
        {
            InitializeComponent();
        }
    }
}