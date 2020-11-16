/*
  CertificateTool.cs
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
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Operators;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Specialized;
using CreateCertificate;

namespace EmbedTools
{
    public partial class CertificateTool : UserControl
    {
        BackgroundWorker _certWorker = new BackgroundWorker();
        OrderedDictionary _certProps = new OrderedDictionary();

        public CertificateTool()
        {
            InitializeComponent();
        }

        private void CertificateTool_Load(object sender, EventArgs e)
        {
            this.comboBoxKeySize.Items.Add("512");
            this.comboBoxKeySize.Items.Add("768");
            this.comboBoxKeySize.Items.Add("1024");
            this.comboBoxKeySize.Items.Add("2048");
            this.comboBoxKeySize.Items.Add("4096");
            this.comboBoxKeySize.SelectedIndex = 0;

            this.comboBoxAlgorithm.Items.Add("SHA256WithRSA");
            this.comboBoxAlgorithm.Items.Add("SHA512WithRSA");
            this.comboBoxAlgorithm.Items.Add("SHA1WithRSA");
            this.comboBoxAlgorithm.SelectedIndex = 2;

            this.textBoxCommon.Text = "CN=Straight";
            this.textBoxSubject.Text = "CN=Straight Server";
            this.textBoxIssuer.Text = "CN=Straight Root CA";
            this.textBoxCountry.Text = "Straight";
            this.textBoxState.Text = "Straight";
            this.textBoxOrg.Text = "Straight";
            this.textBoxPassword.Text = "straight";

            this.textBoxFolder.Text = AppDomain.CurrentDomain.BaseDirectory;

            _certWorker.WorkerReportsProgress = true;
            _certWorker.WorkerSupportsCancellation = true;
        }

        private void buttonGen_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(this.textBoxPassword.Text.Trim()))
            {
                MessageBox.Show("Password is empty.",
                                "Warning",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning,
                                MessageBoxDefaultButton.Button1,
                                MessageBoxOptions.DefaultDesktopOnly,
                                false);
                return;
            }

            Application.UseWaitCursor = true;
            this.buttonGen.Text = "Waiting ...";
            this.buttonGen.Enabled = false;

            _certProps.Clear();

            _certProps.Add("Key Size", this.comboBoxKeySize.Text.Trim());
            _certProps.Add("Algorithm", this.comboBoxAlgorithm.Text.Trim());
            _certProps.Add("Common Name", this.textBoxCommon.Text.Trim());
            _certProps.Add("Subject", this.textBoxSubject.Text.Trim());
            _certProps.Add("Issuer", this.textBoxIssuer.Text.Trim());
            _certProps.Add("Country", this.textBoxCountry.Text.Trim());
            _certProps.Add("State or Province", this.textBoxState.Text.Trim());
            _certProps.Add("Organization", this.textBoxOrg.Text.Trim());
            _certProps.Add("Password", this.textBoxPassword.Text.Trim());
            _certProps.Add("ExportTo", this.textBoxFolder.Text.Trim());

            if (_certWorker.IsBusy)
            {
                _certWorker.CancelAsync();
            }
            else if (_certWorker.IsBusy != true)
            {
                _certWorker.DoWork += backgroundWorker_DoWork;
                _certWorker.ProgressChanged += backgroundWorker_ProgressChanged;
                _certWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
                _certWorker.WorkerReportsProgress = true;
                _certWorker.RunWorkerAsync();
            }
        }

        private void backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            X509Certificate2 caCert = CertificateAPI.CreateCertificateAuthorityCertificate(2048, "CN=Straight RootCA", null, null);

            if (_certWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            X509Certificate2 serverCert = CertificateAPI.IssueCertificate(512, "CN=Straight Server", caCert, new[] { "server", "straight" }, new[] { KeyPurposeID.IdKPServerAuth });
            
            if (_certWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            CertificateAPI.ExportSourceCode(serverCert, (string)_certProps["ExportTo"] + "lwip_cert.c");
            //CertificateAPI.ExportPfx(serverCert, "straight-server.pfx", (string)_certProps["Password"]);

            _certWorker.ReportProgress(100);
        }

        private void backgroundWorker_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            //progressBar1.Value = e.ProgressPercentage;
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                //this.labelStatus.Text = "Process was cancelled";
            }
            else if (e.Error != null)
            {
                //this.labelStatus.Text = "There was an error running the process. The thread aborted";
            }
            else
            {
                //this.labelStatus.Text = "Process was completed";
            }

            Application.UseWaitCursor = false;
            this.buttonGen.Text = "Generate Self-signed Certificate";
            this.buttonGen.Enabled = true;

            if (DialogResult.Yes == MessageBox.Show("Certificate is created, would you like to open it ?",
                                        "Warning", MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Warning,
                                        MessageBoxDefaultButton.Button1,
                                        MessageBoxOptions.DefaultDesktopOnly, false))
            {
                System.Diagnostics.Process.Start(this.textBoxFolder.Text + "lwip_cert.c");
            }
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
        }
    }
}
