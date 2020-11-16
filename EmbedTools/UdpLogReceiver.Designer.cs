namespace EmbedTools
{
    partial class UdpLogReceiver
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listBoxLog = new System.Windows.Forms.ListBox();
            this.checkBoxAutoScroll = new System.Windows.Forms.CheckBox();
            this.buttonOpenFile = new System.Windows.Forms.Button();
            this.buttonClearList = new System.Windows.Forms.Button();
            this.buttonClearLogFile = new System.Windows.Forms.Button();
            this.buttonRestart = new System.Windows.Forms.Button();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.labelPort = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listBoxLog
            // 
            this.listBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxLog.FormattingEnabled = true;
            this.listBoxLog.IntegralHeight = false;
            this.listBoxLog.ItemHeight = 12;
            this.listBoxLog.Location = new System.Drawing.Point(0, 49);
            this.listBoxLog.Name = "listBoxLog";
            this.listBoxLog.Size = new System.Drawing.Size(725, 436);
            this.listBoxLog.TabIndex = 0;
            // 
            // checkBoxAutoScroll
            // 
            this.checkBoxAutoScroll.AutoSize = true;
            this.checkBoxAutoScroll.Location = new System.Drawing.Point(13, 18);
            this.checkBoxAutoScroll.Name = "checkBoxAutoScroll";
            this.checkBoxAutoScroll.Size = new System.Drawing.Size(90, 16);
            this.checkBoxAutoScroll.TabIndex = 1;
            this.checkBoxAutoScroll.Text = "Auto-Scroll";
            this.checkBoxAutoScroll.UseVisualStyleBackColor = true;
            this.checkBoxAutoScroll.CheckedChanged += new System.EventHandler(this.checkBoxAutoScroll_CheckedChanged);
            // 
            // buttonOpenFile
            // 
            this.buttonOpenFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOpenFile.Location = new System.Drawing.Point(362, 11);
            this.buttonOpenFile.Name = "buttonOpenFile";
            this.buttonOpenFile.Size = new System.Drawing.Size(112, 29);
            this.buttonOpenFile.TabIndex = 2;
            this.buttonOpenFile.Text = "Open Log File";
            this.buttonOpenFile.UseVisualStyleBackColor = true;
            this.buttonOpenFile.Click += new System.EventHandler(this.buttonOpenFile_Click);
            // 
            // buttonClearList
            // 
            this.buttonClearList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClearList.Location = new System.Drawing.Point(483, 11);
            this.buttonClearList.Name = "buttonClearList";
            this.buttonClearList.Size = new System.Drawing.Size(112, 29);
            this.buttonClearList.TabIndex = 3;
            this.buttonClearList.Text = "Clear Log List";
            this.buttonClearList.UseVisualStyleBackColor = true;
            this.buttonClearList.Click += new System.EventHandler(this.buttonClearList_Click);
            // 
            // buttonClearLogFile
            // 
            this.buttonClearLogFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClearLogFile.Location = new System.Drawing.Point(602, 11);
            this.buttonClearLogFile.Name = "buttonClearLogFile";
            this.buttonClearLogFile.Size = new System.Drawing.Size(112, 29);
            this.buttonClearLogFile.TabIndex = 4;
            this.buttonClearLogFile.Text = "Clear Log File";
            this.buttonClearLogFile.UseVisualStyleBackColor = true;
            this.buttonClearLogFile.Click += new System.EventHandler(this.buttonClearLogFile_Click);
            // 
            // buttonRestart
            // 
            this.buttonRestart.Enabled = false;
            this.buttonRestart.Location = new System.Drawing.Point(214, 11);
            this.buttonRestart.Name = "buttonRestart";
            this.buttonRestart.Size = new System.Drawing.Size(128, 29);
            this.buttonRestart.TabIndex = 5;
            this.buttonRestart.Text = "Start";
            this.buttonRestart.UseVisualStyleBackColor = true;
            this.buttonRestart.Click += new System.EventHandler(this.buttonRestart_Click);
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(154, 16);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(54, 21);
            this.textBoxPort.TabIndex = 6;
            this.textBoxPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBoxPort.TextChanged += new System.EventHandler(this.textBoxPort_TextChanged);
            // 
            // labelPort
            // 
            this.labelPort.AutoSize = true;
            this.labelPort.Location = new System.Drawing.Point(117, 19);
            this.labelPort.Name = "labelPort";
            this.labelPort.Size = new System.Drawing.Size(29, 12);
            this.labelPort.TabIndex = 7;
            this.labelPort.Text = "Port";
            // 
            // UdpLogReceiver
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelPort);
            this.Controls.Add(this.textBoxPort);
            this.Controls.Add(this.buttonRestart);
            this.Controls.Add(this.buttonClearLogFile);
            this.Controls.Add(this.buttonClearList);
            this.Controls.Add(this.buttonOpenFile);
            this.Controls.Add(this.checkBoxAutoScroll);
            this.Controls.Add(this.listBoxLog);
            this.Name = "UdpLogReceiver";
            this.Size = new System.Drawing.Size(725, 488);
            this.Load += new System.EventHandler(this.UdpLogReceiver_Load);
            this.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.UdpLogReceiver_ControlAdded);
            this.ControlRemoved += new System.Windows.Forms.ControlEventHandler(this.UdpLogReceiver_ControlRemoved);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxLog;
        private System.Windows.Forms.CheckBox checkBoxAutoScroll;
        private System.Windows.Forms.Button buttonOpenFile;
        private System.Windows.Forms.Button buttonClearList;
        private System.Windows.Forms.Button buttonClearLogFile;
        private System.Windows.Forms.Button buttonRestart;
        private System.Windows.Forms.TextBox textBoxPort;
        private System.Windows.Forms.Label labelPort;
    }
}
