namespace PulseParser
{
    partial class PulseParser
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PulseParser));
            this.buttonBrowse = new System.Windows.Forms.Button();
            this.printPreviewDialog1 = new System.Windows.Forms.PrintPreviewDialog();
            this.progressBar = new System.Windows.Forms.PictureBox();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.pictureBoxScoper = new System.Windows.Forms.PictureBox();
            this.hScrollBarPan = new System.Windows.Forms.HScrollBar();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxSpan = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.trackBar = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.progressBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScoper)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonBrowse
            // 
            this.buttonBrowse.Location = new System.Drawing.Point(12, 12);
            this.buttonBrowse.Name = "buttonBrowse";
            this.buttonBrowse.Size = new System.Drawing.Size(98, 34);
            this.buttonBrowse.TabIndex = 0;
            this.buttonBrowse.Text = "脉冲文件";
            this.buttonBrowse.UseVisualStyleBackColor = true;
            this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
            // 
            // printPreviewDialog1
            // 
            this.printPreviewDialog1.AutoScrollMargin = new System.Drawing.Size(0, 0);
            this.printPreviewDialog1.AutoScrollMinSize = new System.Drawing.Size(0, 0);
            this.printPreviewDialog1.ClientSize = new System.Drawing.Size(400, 300);
            this.printPreviewDialog1.Enabled = true;
            this.printPreviewDialog1.Icon = ((System.Drawing.Icon)(resources.GetObject("printPreviewDialog1.Icon")));
            this.printPreviewDialog1.Name = "printPreviewDialog1";
            this.printPreviewDialog1.Visible = false;
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(116, 17);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(348, 22);
            this.progressBar.TabIndex = 7;
            this.progressBar.TabStop = false;
            // 
            // timerUpdate
            // 
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // pictureBoxScoper
            // 
            this.pictureBoxScoper.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pictureBoxScoper.BackColor = System.Drawing.SystemColors.Window;
            this.pictureBoxScoper.Location = new System.Drawing.Point(12, 52);
            this.pictureBoxScoper.Name = "pictureBoxScoper";
            this.pictureBoxScoper.Size = new System.Drawing.Size(626, 262);
            this.pictureBoxScoper.TabIndex = 10;
            this.pictureBoxScoper.TabStop = false;
            this.pictureBoxScoper.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxScoper_Paint);
            this.pictureBoxScoper.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxScoper_MouseDown);
            this.pictureBoxScoper.MouseEnter += new System.EventHandler(this.pictureBoxScoper_MouseEnter);
            this.pictureBoxScoper.MouseLeave += new System.EventHandler(this.pictureBoxScoper_MouseLeave);
            this.pictureBoxScoper.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBoxScoper_MouseMove);
            this.pictureBoxScoper.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxScoper_MouseUp);
            this.pictureBoxScoper.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.pictureBoxScoper_MouseWheel);
            // 
            // hScrollBarPan
            // 
            this.hScrollBarPan.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hScrollBarPan.Location = new System.Drawing.Point(12, 320);
            this.hScrollBarPan.Name = "hScrollBarPan";
            this.hScrollBarPan.Size = new System.Drawing.Size(626, 22);
            this.hScrollBarPan.TabIndex = 11;
            this.hScrollBarPan.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hScrollBarPan_Scroll);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(470, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 12;
            this.label1.Text = "显示宽度";
            // 
            // textBoxSpan
            // 
            this.textBoxSpan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSpan.Location = new System.Drawing.Point(526, 18);
            this.textBoxSpan.Name = "textBoxSpan";
            this.textBoxSpan.ReadOnly = true;
            this.textBoxSpan.Size = new System.Drawing.Size(75, 21);
            this.textBoxSpan.TabIndex = 13;
            this.textBoxSpan.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBoxSpan.TextChanged += new System.EventHandler(this.textBoxSpan_TextChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(607, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 12);
            this.label2.TabIndex = 14;
            this.label2.Text = "x20ns";
            // 
            // trackBar
            // 
            this.trackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBar.Location = new System.Drawing.Point(472, 247);
            this.trackBar.Name = "trackBar";
            this.trackBar.Size = new System.Drawing.Size(148, 45);
            this.trackBar.TabIndex = 15;
            this.trackBar.Visible = false;
            this.trackBar.Scroll += new System.EventHandler(this.trackBar_Scroll);
            this.trackBar.ValueChanged += new System.EventHandler(this.trackBar_ValueChanged);
            // 
            // PulseParser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(650, 350);
            this.Controls.Add(this.trackBar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxSpan);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.hScrollBarPan);
            this.Controls.Add(this.pictureBoxScoper);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.buttonBrowse);
            this.Name = "PulseParser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "脉冲分析";
            this.Load += new System.EventHandler(this.PulseParser_Load);
            this.SizeChanged += new System.EventHandler(this.PulseParser_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.progressBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxScoper)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonBrowse;
        private System.Windows.Forms.PrintPreviewDialog printPreviewDialog1;
        private System.Windows.Forms.PictureBox progressBar;
        private System.Windows.Forms.Timer timerUpdate;
        private System.Windows.Forms.PictureBox pictureBoxScoper;
        private System.Windows.Forms.HScrollBar hScrollBarPan;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxSpan;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TrackBar trackBar;
    }
}

