using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace ECCripto
{
    public partial class frmCertDetails : Form
    {
        public StoreLocation CertStoreLocation { get; set; }
        public StoreName CertStoreName { get; set; }
        public X509Certificate2 Certificate { get; set; }

        public frmCertDetails()
        {
            InitializeComponent();
        }

        private void frmCertDetails_Load(object sender, EventArgs e)
        {
            txtStore.Text = string.Format("{0}/{1}", CertStoreLocation, CertStoreName);
            txtThumbprint.Text = Certificate.Thumbprint;
        }

        private void btnViewStore_Click(object sender, EventArgs e)
        {
            X509Store store = new X509Store(CertStoreName, CertStoreLocation);
            store.Open(OpenFlags.ReadOnly);

            X509Certificate2UI.SelectFromCollection(store.Certificates, txtStore.Text, "", X509SelectionFlag.SingleSelection);
        }

        private void btnViewCert_Click(object sender, EventArgs e)
        {
            X509Certificate2UI.DisplayCertificate(Certificate);
        }
    }
}
