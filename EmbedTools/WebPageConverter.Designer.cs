namespace EmbedTools
{
    partial class WebPageConverter
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
            this.buttonBrowseOutput = new System.Windows.Forms.Button();
            this.textBoxOutput = new System.Windows.Forms.TextBox();
            this.buttonConvert = new System.Windows.Forms.Button();
            this.buttonBrowseWeb = new System.Windows.Forms.Button();
            this.textBoxWebRoot = new System.Windows.Forms.TextBox();
            this.listBoxLog = new System.Windows.Forms.ListBox();
            this.labelGzip = new System.Windows.Forms.Label();
            this.textBoxExtensions = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonBrowseOutput
            // 
            this.buttonBrowseOutput.Location = new System.Drawing.Point(10, 45);
            this.buttonBrowseOutput.Name = "buttonBrowseOutput";
            this.buttonBrowseOutput.Size = new System.Drawing.Size(118, 29);
            this.buttonBrowseOutput.TabIndex = 0;
            this.buttonBrowseOutput.Text = "Output File ...";
            this.buttonBrowseOutput.UseVisualStyleBackColor = true;
            this.buttonBrowseOutput.Click += new System.EventHandler(this.buttonBrowseOutput_Click);
            // 
            // textBoxOutput
            // 
            this.textBoxOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxOutput.Location = new System.Drawing.Point(138, 49);
            this.textBoxOutput.Name = "textBoxOutput";
            this.textBoxOutput.Size = new System.Drawing.Size(447, 21);
            this.textBoxOutput.TabIndex = 1;
            // 
            // buttonConvert
            // 
            this.buttonConvert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonConvert.Location = new System.Drawing.Point(606, 36);
            this.buttonConvert.Name = "buttonConvert";
            this.buttonConvert.Size = new System.Drawing.Size(103, 43);
            this.buttonConvert.TabIndex = 2;
            this.buttonConvert.Text = "Convert";
            this.buttonConvert.UseVisualStyleBackColor = true;
            this.buttonConvert.Click += new System.EventHandler(this.buttonConvert_Click);
            // 
            // buttonBrowseWeb
            // 
            this.buttonBrowseWeb.Location = new System.Drawing.Point(10, 10);
            this.buttonBrowseWeb.Name = "buttonBrowseWeb";
            this.buttonBrowseWeb.Size = new System.Drawing.Size(118, 29);
            this.buttonBrowseWeb.TabIndex = 0;
            this.buttonBrowseWeb.Text = "Web Root ...";
            this.buttonBrowseWeb.UseVisualStyleBackColor = true;
            this.buttonBrowseWeb.Click += new System.EventHandler(this.buttonBrowseWeb_Click);
            // 
            // textBoxWebRoot
            // 
            this.textBoxWebRoot.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxWebRoot.Enabled = false;
            this.textBoxWebRoot.Location = new System.Drawing.Point(138, 14);
            this.textBoxWebRoot.Name = "textBoxWebRoot";
            this.textBoxWebRoot.Size = new System.Drawing.Size(447, 21);
            this.textBoxWebRoot.TabIndex = 1;
            // 
            // listBoxLog
            // 
            this.listBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxLog.FormattingEnabled = true;
            this.listBoxLog.IntegralHeight = false;
            this.listBoxLog.ItemHeight = 12;
            this.listBoxLog.Location = new System.Drawing.Point(14, 113);
            this.listBoxLog.Name = "listBoxLog";
            this.listBoxLog.Size = new System.Drawing.Size(695, 354);
            this.listBoxLog.TabIndex = 3;
            // 
            // labelGzip
            // 
            this.labelGzip.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelGzip.Location = new System.Drawing.Point(10, 79);
            this.labelGzip.Name = "labelGzip";
            this.labelGzip.Size = new System.Drawing.Size(118, 29);
            this.labelGzip.TabIndex = 4;
            this.labelGzip.Text = "gzip Extensions";
            this.labelGzip.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBoxExtensions
            // 
            this.textBoxExtensions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxExtensions.Location = new System.Drawing.Point(138, 82);
            this.textBoxExtensions.Name = "textBoxExtensions";
            this.textBoxExtensions.Size = new System.Drawing.Size(447, 21);
            this.textBoxExtensions.TabIndex = 5;
            // 
            // WebPageConverter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBoxExtensions);
            this.Controls.Add(this.labelGzip);
            this.Controls.Add(this.listBoxLog);
            this.Controls.Add(this.buttonConvert);
            this.Controls.Add(this.textBoxWebRoot);
            this.Controls.Add(this.textBoxOutput);
            this.Controls.Add(this.buttonBrowseWeb);
            this.Controls.Add(this.buttonBrowseOutput);
            this.Name = "WebPageConverter";
            this.Size = new System.Drawing.Size(725, 488);
            this.Load += new System.EventHandler(this.WebPageConverter_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonBrowseOutput;
        private System.Windows.Forms.TextBox textBoxOutput;
        private System.Windows.Forms.Button buttonConvert;
        private System.Windows.Forms.Button buttonBrowseWeb;
        private System.Windows.Forms.TextBox textBoxWebRoot;
        private System.Windows.Forms.ListBox listBoxLog;
        private System.Windows.Forms.Label labelGzip;
        private System.Windows.Forms.TextBox textBoxExtensions;
    }
}
