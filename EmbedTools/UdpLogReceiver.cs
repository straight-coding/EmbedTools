/*
  UdpLogReceiver.cs
  Author: Straight Coder<simpleisrobust@gmail.com>
  Date: Nov 13, 2020
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.IO;

namespace EmbedTools
{
    public partial class UdpLogReceiver : UserControl
    {
        Thread _udpThread = null;
        long _running = 0;
        long _finished = 1;

        String _listenPort = "8899";
        Socket _udpSocket = null;

        StringBuilder _logCache = new StringBuilder();
        DateTime _lastSave      = DateTime.Now;

        public UdpLogReceiver()
        {
            InitializeComponent();
        }

        private void UdpLogReceiver_Load(object sender, EventArgs e)
        {
            Init();
        }

        private void UdpLogReceiver_ControlAdded(object sender, ControlEventArgs e)
        {
            Init();
        }

        private void UdpLogReceiver_ControlRemoved(object sender, ControlEventArgs e)
        {
            Stop();
        }

        private void Init()
        {
            this.textBoxPort.Text = _listenPort;

            this.checkBoxAutoScroll.Enabled = true;
            this.checkBoxAutoScroll.Checked = true;

            int top = this.buttonRestart.Location.Y + this.buttonRestart.Height + 4;
            int margin = 4;
            this.listBoxLog.Location = new Point(margin, top+margin);
            this.listBoxLog.Size = new System.Drawing.Size(this.Size.Width - 2 * margin, this.Size.Height-2*margin-top);
        }

        void OnLogEvent(object sender, EventArgs e)
        {
            if (InvokeRequired)
                Invoke(new EventHandler(OnLogEvent), new object[] { sender, e });
            else
            {
                String log = ((LogEventArgs)e).Message;

                this.listBoxLog.Items.Add(log);
                if (this.checkBoxAutoScroll.Enabled)
                {
                    if (this.listBoxLog.Items.Count > 0)
                        this.listBoxLog.SelectedIndex = this.listBoxLog.Items.Count - 1;
                }
            }
        }

        public Boolean Start()
        {
            try
            {
                _udpThread = new Thread(ThreadLogProc);
                _udpThread.IsBackground = true;
                _udpThread.Name = "Thread Log";

                Interlocked.Exchange(ref _finished, 0);
                _udpThread.Start();
                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            return false;
        }

        public static List<String> GetIPAddress4()
        {
            List<String> allIP = new List<string>();
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                return allIP;

            // Get a list of all network interfaces (usually one per network card, dialup, and VPN connection) 
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            /*
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                Console.WriteLine(ni.Name);
                Console.WriteLine("Operational? {0}", ni.OperationalStatus == OperationalStatus.Up);
                Console.WriteLine("MAC: {0}", ni.GetPhysicalAddress());
                Console.WriteLine("Gateways:");
                foreach (GatewayIPAddressInformation gipi in ni.GetIPProperties().GatewayAddresses)
                {
                    Console.WriteLine("\t{0}", gipi.Address);
                }
                Console.WriteLine("IP Addresses:");

                foreach (UnicastIPAddressInformation uipi in ni.GetIPProperties().UnicastAddresses)
                {
                    Console.WriteLine("\t{0} / {1}", uipi.Address, uipi.IPv4Mask);
                }
                Console.WriteLine();
            }
            foreach (NetworkInterface Interface in networkInterfaces)
            {
                if (Interface.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                    continue;
                Console.WriteLine(Interface.Description);
                UnicastIPAddressInformationCollection UnicastIPInfoCol = Interface.GetIPProperties().UnicastAddresses;
                foreach (UnicastIPAddressInformation UnicatIPInfo in UnicastIPInfoCol)
                {
                    Console.WriteLine("\tIP Address is {0}", UnicatIPInfo.Address);
                    Console.WriteLine("\tSubnet Mask is {0}", UnicatIPInfo.IPv4Mask);
                }
            }
            */
            foreach (NetworkInterface network in networkInterfaces)
            {
                if (network.OperationalStatus != OperationalStatus.Up)
                    continue;

                // Read the IP configuration for each network 
                IPInterfaceProperties properties = network.GetIPProperties();

                // Each network interface may have multiple IP addresses 
                foreach (IPAddressInformation address in properties.UnicastAddresses)
                {
                    // We're only interested in IPv4 addresses for now 
                    if (address.Address.AddressFamily != AddressFamily.InterNetwork)
                        continue;

                    // Ignore loopback addresses (e.g., 127.0.0.1) 
                    if (IPAddress.IsLoopback(address.Address))
                        continue;

                    if (network.Description.IndexOf("Loopback", 0, Math.Min(network.Description.Length, "Loopback".Length), StringComparison.InvariantCultureIgnoreCase) >= 0)
                        continue;
                    if (network.Description.IndexOf("Hyper-V", 0, Math.Min(network.Description.Length, "Hyper-V".Length), StringComparison.InvariantCultureIgnoreCase) >= 0)
                        continue;
                    if (network.Description.IndexOf("VMWare", 0, Math.Min(network.Description.Length, "VMWare".Length), StringComparison.InvariantCultureIgnoreCase) >= 0)
                        continue;
                    if (network.Description.IndexOf("VirtualBox", 0, Math.Min(network.Description.Length, "VirtualBox".Length), StringComparison.InvariantCultureIgnoreCase) >= 0)
                        continue;

                    if (network.Name.IndexOf("Loopback", 0, Math.Min(network.Name.Length, "Loopback".Length), StringComparison.InvariantCultureIgnoreCase) >= 0)
                        continue;
                    if (network.Name.IndexOf("Hyper-V", 0, Math.Min(network.Name.Length, "Hyper-V".Length), StringComparison.InvariantCultureIgnoreCase) >= 0)
                        continue;
                    if (network.Name.IndexOf("VMWare", 0, Math.Min(network.Name.Length, "VMWare".Length), StringComparison.InvariantCultureIgnoreCase) >= 0)
                        continue;
                    if (network.Name.IndexOf("VirtualBox", 0, Math.Min(network.Name.Length, "VirtualBox".Length), StringComparison.InvariantCultureIgnoreCase) >= 0)
                        continue;

                    String ipV4 = address.Address.ToString();
                    if (ipV4.StartsWith("169.254."))
                        continue;

                    allIP.Add(ipV4);
                }
            }

            return allIP;
        }

        private void ThreadLogProc()
        {
            byte[] logBuffer = new byte[8192];
            _udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            List<String> localIpList = GetIPAddress4();
            if (localIpList.Count <= 0)
                return;

            IPAddress localIPAddr = IPAddress.Parse(localIpList[0]);
            IPEndPoint IPlocal = new IPEndPoint(IPAddress.Any, Convert.ToInt32(this.textBoxPort.Text));
            try
            {
                _udpSocket.Bind(IPlocal);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            while (true)
            {
                try
                {
                    if ((_udpSocket != null) && (_udpSocket.Available > 0))
                    {
                        int nBytes = _udpSocket.Receive(logBuffer, SocketFlags.None);
                        if (nBytes > 0)
                        {
                            String receivedLog = Encoding.UTF8.GetString(logBuffer, 0, nBytes);

                            lock (_logCache)
                            {
                                _logCache.AppendLine(receivedLog);

                                TimeSpan t = DateTime.Now - _lastSave;
                                if (t.TotalSeconds > 5)
                                {
                                    String logFile = AppDomain.CurrentDomain.BaseDirectory + String.Format("log\\{0}.log", DateTime.Now.ToString("yyyyMMdd"));
                                    if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "log\\"))
                                        Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "log\\");

                                    using (StreamWriter sw = File.AppendText(logFile))
                                    {
                                        sw.Write(_logCache);

                                        _logCache.Clear();
                                        _lastSave = DateTime.Now;
                                    }
                                }
                            }

                            LogEventArgs args = new LogEventArgs();
                            args.Message = receivedLog;
                            args.Timestamp = DateTime.Now;

                            OnLogEvent(this, args);
                        }
                    }
                    else
                    {
                        try { Thread.Sleep(20); }
                        catch (Exception e) 
                        {
                            Debug.WriteLine(e.Message);
                            break; 
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    break;
                }
            }
            _udpSocket.Close();
            Interlocked.Exchange(ref _finished, 1);
        }

        public void Stop()
        {
            {
                if (_udpSocket != null)
                    _udpSocket.Close();
                _udpThread.Interrupt();

                DateTime tStart = DateTime.Now;
                while (Interlocked.Equals(_finished, 0))
                {
                    try
                    {
                        TimeSpan tSpan = DateTime.Now - tStart;
                        if (tSpan.TotalMilliseconds >= 3000)
                        {
                            //break;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                        break;
                    }
                }
            }
        }

        private void checkBoxAutoScroll_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void textBoxPort_TextChanged(object sender, EventArgs e)
        {
            String port = this.textBoxPort.Text.Trim();
            this.buttonRestart.Enabled = !String.IsNullOrEmpty(port);
        }

        private void buttonRestart_Click(object sender, EventArgs e)
        {
            String port = this.textBoxPort.Text.Trim();
            if (String.IsNullOrEmpty(port))
                return;

            if (Interlocked.Equals(this._running, 0))
            {
                if (Start())
                {
                    Interlocked.Exchange(ref this._running, 1);
                    this.buttonRestart.Text = "Stop";
                    this.listBoxLog.Items.Add(String.Format("Started listening @ {0}", this.textBoxPort.Text));
                }
            }
            else if (Interlocked.Equals(this._running, 1))
            {
                Stop();

                Interlocked.Exchange(ref this._running, 0);
                this.buttonRestart.Text = "Start";
                this.listBoxLog.Items.Add(String.Format("Stopped listening @ {0}", this.textBoxPort.Text));
            }
            this.textBoxPort.Enabled = (Interlocked.Equals(this._running, 0));
        }

        private void buttonOpenFile_Click(object sender, EventArgs e)
        {
            string logFile = AppDomain.CurrentDomain.BaseDirectory + String.Format("log\\{0}.log", DateTime.Now.ToString("yyyyMMdd"));
            if (File.Exists(logFile))
                System.Diagnostics.Process.Start(logFile);
        }

        private void buttonClearList_Click(object sender, EventArgs e)
        {
            this.listBoxLog.Items.Clear();
        }

        private void buttonClearLogFile_Click(object sender, EventArgs e)
        {
            string logFile = AppDomain.CurrentDomain.BaseDirectory + String.Format("log\\{0}.log", DateTime.Now.ToString("yyyyMMdd"));
            if (File.Exists(logFile))
            {
                try
                {
                    File.Delete(logFile);
                }
                catch (Exception err)
                {
                    Debug.WriteLine(err.Message);
                }
            }
        }
    }
}
