namespace TcpClientApp
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            labelServerIp = new Label();
            labelPort = new Label();
            contextMenuStrip1 = new ContextMenuStrip(components);
            textBoxServerIp = new TextBox();
            textBoxPort = new TextBox();
            textBoxLogs = new TextBox();
            buttonConnect = new Button();
            buttonDisconnect = new Button();
            SuspendLayout();
            // 
            // labelServerIp
            // 
            labelServerIp.AutoSize = true;
            labelServerIp.Location = new Point(83, 63);
            labelServerIp.Name = "labelServerIp";
            labelServerIp.Size = new Size(55, 15);
            labelServerIp.TabIndex = 0;
            labelServerIp.Text = "Server IP:";
            // 
            // labelPort
            // 
            labelPort.AutoSize = true;
            labelPort.Location = new Point(322, 66);
            labelPort.Name = "labelPort";
            labelPort.Size = new Size(32, 15);
            labelPort.TabIndex = 1;
            labelPort.Text = "Port:";
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(61, 4);
            // 
            // textBoxServerIp
            // 
            textBoxServerIp.Location = new Point(144, 60);
            textBoxServerIp.Name = "textBoxServerIp";
            textBoxServerIp.Size = new Size(150, 23);
            textBoxServerIp.TabIndex = 3;
            textBoxServerIp.Text = "127.0.0.1";
            // 
            // textBoxPort
            // 
            textBoxPort.Location = new Point(360, 63);
            textBoxPort.Name = "textBoxPort";
            textBoxPort.Size = new Size(70, 23);
            textBoxPort.TabIndex = 4;
            textBoxPort.Text = "8080";
            // 
            // textBoxLogs
            // 
            textBoxLogs.Location = new Point(12, 91);
            textBoxLogs.Multiline = true;
            textBoxLogs.Name = "textBoxLogs";
            textBoxLogs.ReadOnly = true;
            textBoxLogs.ScrollBars = ScrollBars.Vertical;
            textBoxLogs.Size = new Size(760, 358);
            textBoxLogs.TabIndex = 5;
            // 
            // buttonConnect
            // 
            buttonConnect.Location = new Point(492, 62);
            buttonConnect.Name = "buttonConnect";
            buttonConnect.Size = new Size(100, 23);
            buttonConnect.TabIndex = 6;
            buttonConnect.Text = "Connect";
            buttonConnect.UseVisualStyleBackColor = true;
            // 
            // buttonDisconnect
            // 
            buttonDisconnect.Enabled = false;
            buttonDisconnect.Location = new Point(598, 62);
            buttonDisconnect.Name = "buttonDisconnect";
            buttonDisconnect.Size = new Size(100, 23);
            buttonDisconnect.TabIndex = 7;
            buttonDisconnect.Text = "Disconnect";
            buttonDisconnect.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 461);
            Controls.Add(buttonDisconnect);
            Controls.Add(buttonConnect);
            Controls.Add(textBoxLogs);
            Controls.Add(textBoxPort);
            Controls.Add(textBoxServerIp);
            Controls.Add(labelPort);
            Controls.Add(labelServerIp);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "TCP Client Application";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelServerIp;
        private Label labelPort;
        private ContextMenuStrip contextMenuStrip1;
        private TextBox textBoxServerIp;
        private TextBox textBoxPort;
        private TextBox textBoxLogs;
        private Button buttonConnect;
        private Button buttonDisconnect;
    }
}
