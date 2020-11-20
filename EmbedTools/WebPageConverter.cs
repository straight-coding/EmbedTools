using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using System.IO.Compression;
using System.Diagnostics;

namespace EmbedTools
{
    public partial class WebPageConverter : UserControl
    {
        Thread _compressThread;
        readonly int ATTRI_GZIP = 0x00000001;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct FileHeader
        {
            public UInt32 nIndex;
            public UInt32 nSize;
            public UInt64 tLastModified;
            public UInt32 nAttributes;
            public UInt32 nReserved;
            public UInt32 nOffsetFileData; //file content offset
            public UInt32 nOffsetNextFile; // ==> _http_file_info*
        }

        static int CompareTwoPath(String a, String b)
        {
            return b.Length.CompareTo(a.Length);
        }

        internal class CompressorEventArgs : EventArgs
        {
            public string Folder { get; set; }
            public string OutoutFile { get; set; }
            public string Extensions { get; set; }

            public CompressorEventArgs()
            {
                Folder = "";
                OutoutFile = "";
                Extensions = "";
            }
        }

        public WebPageConverter()
        {
            InitializeComponent();
        }

        private void WebPageConverter_Load(object sender, EventArgs e)
        {
            this.textBoxExtensions.Text = "*.css;*.js;*.html;*.htm";
            this.textBoxWebRoot.Text = "D:\\straight\\LPC407x-NoOS-LWIP-MBEDTLS-HTTPD-KEIL\\LambdaIOT\\httpd\\webroot";
            this.textBoxOutput.Text = "D:\\straight\\LPC407x-NoOS-LWIP-MBEDTLS-HTTPD-KEIL\\LambdaIOT\\httpd\\fs_data_.c";

            int top = this.labelGzip.Location.Y + this.labelGzip.Height + 4;
            int margin = 4;
            this.listBoxLog.Location = new Point(margin, top + margin);
            this.listBoxLog.Size = new System.Drawing.Size(this.Size.Width - 2 * margin, this.Size.Height - 2 * margin - top);
        }

        void InvokeLogEvent(object sender, EventArgs e)
        {
            if (InvokeRequired)
                Invoke(new EventHandler(InvokeLogEvent), new object[] { sender, e });
            else
            {
                LogEventArgs evt = (LogEventArgs)e;
                if (evt.Type.Equals("Log", StringComparison.InvariantCultureIgnoreCase))
                {
                    String log = evt.Message;
                    this.listBoxLog.Items.Add(log);
                }
                else if (evt.Type.Equals("Result", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (evt.Message.Equals("Success", StringComparison.InvariantCultureIgnoreCase))
                        this.listBoxLog.Items.Add("Web pages are compressed successfully");
                    else if (evt.Message.Equals("Fail", StringComparison.InvariantCultureIgnoreCase))
                        this.listBoxLog.Items.Add("Web pages are compressed successfully");
                }
                if (this.listBoxLog.Items.Count > 0)
                    this.listBoxLog.SelectedIndex = this.listBoxLog.Items.Count - 1;
            }
        }

        private void ThreadCompressProc(object obj)
        {
            CompressorEventArgs e = (CompressorEventArgs)obj;
            String rootFolder = e.Folder.Replace("\\", "/");
            if (rootFolder.EndsWith("/"))
                rootFolder = rootFolder.Substring(0, rootFolder.Length-1);
            int nPrefixLen = rootFolder.Length;

            string[] exts = e.Extensions.Replace("*", "").Split(new char[] {' ', ';', ','}, StringSplitOptions.RemoveEmptyEntries);

            string[] aryFiles = Directory.GetFiles(e.Folder, "*.*", SearchOption.AllDirectories);
            int nHeaderSize = (int)System.Runtime.InteropServices.Marshal.SizeOf(typeof(FileHeader));

            List<String> gzipExts = exts.ToList();
            List<String> files = new List<string>();
            foreach (string path in aryFiles)
                files.Add(path);
            files.Sort(CompareTwoPath);

            Dictionary<string, string> gzipFiles = new Dictionary<string, string>();
            String gzipFolder = AppDomain.CurrentDomain.BaseDirectory + "gzip" + Path.DirectorySeparatorChar;
            if (!Directory.Exists(gzipFolder))
                Directory.CreateDirectory(gzipFolder);

            UInt32 nTotalSize = 0;
            foreach (string filePath in files)
            {
                FileInfo fi = new FileInfo(filePath);
                long fileSize = fi.Length;
                if (gzipExts.IndexOf(fi.Extension) >= 0)
                {
                    try
                    {
                        gzipFiles[filePath] = gzipFolder + (Guid.NewGuid().ToString() + ".gz");
                        var bytes = File.ReadAllBytes(filePath);
                        using (FileStream fs = new FileStream(gzipFiles[filePath], FileMode.CreateNew))
                        using (GZipStream zipStream = new GZipStream(fs, CompressionMode.Compress, false))
                        {
                            zipStream.Write(bytes, 0, bytes.Length);
                        }
                        fileSize = new FileInfo(gzipFiles[filePath]).Length;
                    }
                    catch (Exception er)
                    {
                        Debug.WriteLine(er.Message);
                    }
                }

                if (!gzipFiles.ContainsKey(filePath))
                {
                    gzipFiles[filePath] = filePath;
                }

                nTotalSize += (UInt32)nHeaderSize;
                nTotalSize += (UInt32)((UInt32)((filePath.Length - nPrefixLen + 1) + 3) & 0xFFFFFFFFCUL); //4-byte alignment
                nTotalSize += (UInt32)((UInt32)(fileSize + 3) & 0xFFFFFFFFCUL); //4-byte alignment

                LogEventArgs args = new LogEventArgs();
                            args.Type = "Log";
                            args.Message = filePath;
                            args.Timestamp = DateTime.Now;
                InvokeLogEvent(this, args);
            }

            int nOffset = 0;
            byte[] fsData = new byte[nTotalSize];
            Array.Clear(fsData, 0, fsData.Length);

            foreach (string filePath in files)
            {
                long fileSize = new FileInfo(gzipFiles[filePath]).Length;

                int pathSpace = (int)((UInt32)((filePath.Length - nPrefixLen + 1) + 3) & 0xFFFFFFFFCUL); //4-byte alignment
                int bodySpace = (int)((UInt32)(fileSize + 3) & 0xFFFFFFFFCUL); //4-byte alignment

                FileInfo fi = new FileInfo(filePath);

                FileHeader fileIndex = new FileHeader();
                fileIndex.nIndex++;
                fileIndex.nSize = (UInt32)fileSize;
                fileIndex.nAttributes |= (UInt32)ATTRI_GZIP;
                fileIndex.tLastModified = (UInt32)((fi.LastWriteTimeUtc.Ticks-0x089f7ff5f7b58000)/10000000);
                fileIndex.nOffsetFileData = (UInt32)(nOffset + nHeaderSize + pathSpace);
                fileIndex.nOffsetNextFile = (UInt32)(nOffset + nHeaderSize + pathSpace + bodySpace);

                IntPtr ptrHeader = Marshal.AllocHGlobal((int)nHeaderSize);
                Marshal.StructureToPtr(fileIndex, ptrHeader, true);
                Marshal.Copy(ptrHeader, fsData, nOffset, (int)nHeaderSize);
                Marshal.FreeHGlobal(ptrHeader);
                nOffset += nHeaderSize; //4-byte alignment

                byte[] fileName = Encoding.ASCII.GetBytes(filePath.Substring(nPrefixLen));
                Buffer.BlockCopy(fileName, 0, fsData, nOffset, fileName.Length);
                nOffset += pathSpace; //4-byte alignment

                byte[] fileBody = File.ReadAllBytes(gzipFiles[filePath]);
                Buffer.BlockCopy(fileBody, 0, fsData, nOffset, fileBody.Length);
                nOffset += bodySpace; //4-byte alignment
            }

            string temp;
            StringBuilder fsExport = new StringBuilder();
            fsExport.AppendLine("const unsigned char g_szWebRoot[] __attribute__((aligned(4))) = {");
            for (int i = 0; i < fsData.Length; i++)
            {
                temp = "0x" + fsData[i].ToString("X2");
                fsExport.Append(temp);
                if (i == fsData.Length - 1)
                    fsExport.AppendLine("");
                else
                {
                    fsExport.Append(",");
                    if ((i & 0x000F) == 0xF)
                        fsExport.AppendLine("");
                }
            }
            fsExport.AppendLine("};");

            File.WriteAllText(e.OutoutFile, fsExport.ToString());

            LogEventArgs evt = new LogEventArgs();
            evt.Type = "Result";
            evt.Message = "Success";
            evt.Timestamp = DateTime.Now;
            InvokeLogEvent(this, evt);
        }

        private void buttonBrowseWeb_Click(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                DialogResult result = dlg.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dlg.SelectedPath))
                {
                    this.textBoxWebRoot.Text = dlg.SelectedPath;
                    //string[] files = Directory.GetFiles(dlg.SelectedPath);

                    //System.Windows.Forms.MessageBox.Show("Files found: " + files.Length.ToString(), "Message");
                }
            }
        }

        private void buttonBrowseOutput_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "C Source|*.c|All files (*.*)|*.*";
            dlg.Title = "Save a Source File";
            if (DialogResult.OK == dlg.ShowDialog())
            {
                this.textBoxOutput.Text = dlg.FileName;
            }
        }

        private void buttonConvert_Click(object sender, EventArgs e)
        {
            CompressorEventArgs arg = new CompressorEventArgs();
            arg.Folder = this.textBoxWebRoot.Text.Trim();
            arg.OutoutFile = this.textBoxOutput.Text.Trim();
            arg.Extensions = this.textBoxExtensions.Text.Trim();

            if (String.IsNullOrEmpty(arg.Folder) || String.IsNullOrEmpty(arg.OutoutFile))
                return;

            _compressThread = new Thread(ThreadCompressProc);
            _compressThread.IsBackground = true;
            _compressThread.Name = "Compressor Thread";
            _compressThread.Start(arg);
        }
    }
}
