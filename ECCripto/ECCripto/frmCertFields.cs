using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ECCripto
{
    public partial class frmCertFields : Form
    {
        CertFields fields;
        public frmCertFields()
        {
            InitializeComponent();

            fields = new CertFields();
        }

        private void txtCN_TextChanged(object sender, EventArgs e)
        {
            fields.CN = txtCN.Text;
        }

        private void txtCI_TextChanged(object sender, EventArgs e)
        {
            fields.CI = txtCI.Text;
        }

        private void txtOU_TextChanged(object sender, EventArgs e)
        {
            fields.OU = txtOU.Text;
        }

        private void txtO_TextChanged(object sender, EventArgs e)
        {
            fields.O = txtO.Text;
        }

        private void txtL_TextChanged(object sender, EventArgs e)
        {
            fields.L = txtL.Text;
        }

        private void txtST_TextChanged(object sender, EventArgs e)
        {
            fields.ST = txtST.Text;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            fields.EmailAddress = txtEmail.Text;
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            fields.CI = fields.CN = fields.EmailAddress = fields.L = fields.O = fields.OU = fields.ST = "";
            Close();
        }

        public CertFields CertFields { get { return fields; } }
    }
}
