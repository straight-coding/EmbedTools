namespace EmbedTools
{
    partial class EmbedTools
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EmbedTools));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageLog = new System.Windows.Forms.TabPage();
            this.tabPageCertificate = new System.Windows.Forms.TabPage();
            this.tabPageWebPacker = new System.Windows.Forms.TabPage();
            this.udpLogReceiver1 = new UdpLogReceiver();
            this.certificateTool1 = new CertificateTool();
            this.webPageConverter1 = new WebPageConverter();
            this.tabControl1.SuspendLayout();
            this.tabPageLog.SuspendLayout();
            this.tabPageCertificate.SuspendLayout();
            this.tabPageWebPacker.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPageLog);
            this.tabControl1.Controls.Add(this.tabPageCertificate);
            this.tabControl1.Controls.Add(this.tabPageWebPacker);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(760, 533);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPageLog
            // 
            this.tabPageLog.Controls.Add(this.udpLogReceiver1);
            this.tabPageLog.Location = new System.Drawing.Point(4, 22);
            this.tabPageLog.Name = "tabPageLog";
            this.tabPageLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLog.Size = new System.Drawing.Size(752, 507);
            this.tabPageLog.TabIndex = 0;
            this.tabPageLog.Text = "UDP log";
            this.tabPageLog.UseVisualStyleBackColor = true;
            // 
            // tabPageCertificate
            // 
            this.tabPageCertificate.Controls.Add(this.certificateTool1);
            this.tabPageCertificate.Location = new System.Drawing.Point(4, 22);
            this.tabPageCertificate.Name = "tabPageCertificate";
            this.tabPageCertificate.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCertificate.Size = new System.Drawing.Size(752, 507);
            this.tabPageCertificate.TabIndex = 1;
            this.tabPageCertificate.Text = "Certificates";
            this.tabPageCertificate.UseVisualStyleBackColor = true;
            // 
            // udpLogReceiver1
            // 
            this.udpLogReceiver1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.udpLogReceiver1.Location = new System.Drawing.Point(3, 3);
            this.udpLogReceiver1.Name = "udpLogReceiver1";
            this.udpLogReceiver1.Size = new System.Drawing.Size(746, 501);
            this.udpLogReceiver1.TabIndex = 0;
            // 
            // certificateTool1
            // 
            this.certificateTool1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.certificateTool1.Location = new System.Drawing.Point(3, 3);
            this.certificateTool1.Name = "certificateTool1";
            this.certificateTool1.Size = new System.Drawing.Size(746, 501);
            this.certificateTool1.TabIndex = 0;
            // 
            // tabPageWebPacker
            // 
            this.tabPageWebPacker.Controls.Add(this.webPageConverter1);
            this.tabPageWebPacker.Location = new System.Drawing.Point(4, 22);
            this.tabPageWebPacker.Name = "tabPageWebPacker";
            this.tabPageWebPacker.Size = new System.Drawing.Size(752, 507);
            this.tabPageWebPacker.TabIndex = 2;
            this.tabPageWebPacker.Text = "Web Page Compressor";
            this.tabPageWebPacker.UseVisualStyleBackColor = true;
            // 
            // webPageConverter1
            // 
            this.webPageConverter1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webPageConverter1.Location = new System.Drawing.Point(0, 0);
            this.webPageConverter1.Name = "webPageConverter1";
            this.webPageConverter1.Size = new System.Drawing.Size(752, 507);
            this.webPageConverter1.TabIndex = 0;
            // 
            // EmbedTools
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 553);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "EmbedTools";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Embed Tools";
            this.tabControl1.ResumeLayout(false);
            this.tabPageLog.ResumeLayout(false);
            this.tabPageCertificate.ResumeLayout(false);
            this.tabPageWebPacker.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageLog;
        private System.Windows.Forms.TabPage tabPageCertificate;
        private UdpLogReceiver udpLogReceiver1;
        private CertificateTool certificateTool1;
        private System.Windows.Forms.TabPage tabPageWebPacker;
        private WebPageConverter webPageConverter1;
    }
}

