using Renci.SshNet;
using System;
using System.IO;
using System.Net.Security;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SSHClient
{
    public partial class SSHClient : Form
    {
        private SshClient sshClient;
        private ShellStream sshStream;

        private Boolean connected = false;

        public SSHClient()
        {
            InitializeComponent();
        }

        private void SSHClient_Load(object sender, EventArgs e)
        {
            this.textBoxServer.Text = "";
            this.textBoxUser.Text = "";
            this.textBoxPassword.Text = "";

            this.textBoxCommand.Text = "ls";
        }

        private async void connect()
        {
            sshClient = new SshClient(this.textBoxServer.Text, this.textBoxUser.Text, this.textBoxPassword.Text);
            // need this otherwise will timeout after 10 minutes or so
            sshClient.KeepAliveInterval = TimeSpan.FromMinutes(1);
            try
            {
                sshClient.Connect();
                this.buttonConnect.Text = "Disconnect";
                connected = true;

                sshStream = sshClient.CreateShellStream("Tail", 0, 0, 0, 0, 1024);

                var outputTask = ProcessOutputAsync(sshStream);
                //var inputTask = ProcessInputAsync(sshStream);

                await Task.WhenAll(outputTask);//, inputTask);

                outputTask.Dispose();
                sshStream.Dispose();
                sshClient.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}\r\n{ex.StackTrace}");
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (connected && (sshClient != null))
            {
                sshStream.Dispose();
                sshClient.Disconnect();

                this.buttonConnect.Text = "Connect";
                connected = false;
                return;
            }

            this.textBoxResponse.Text = "";

            connect();
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (sshStream.CanWrite)
                sshStream.WriteLine(this.textBoxCommand.Text);
        }

        private async Task ProcessOutputAsync(ShellStream shellStream)
        {
            using (var reader = new StreamReader(shellStream))
            {
                while (shellStream.CanRead)
                {
                    if (!shellStream.DataAvailable)
                        await Task.Delay(100);
                    else
                    {
                        long count = shellStream.Length;
                        for (int i = 0; i < count; i++)
                        {
                            int ch = reader.Read();
                            this.textBoxResponse.Text += (char)(ch);
                        }
                    }
                }
            }
        }
    }
}
