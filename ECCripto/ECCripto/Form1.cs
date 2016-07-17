using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Org.BouncyCastle.Crypto;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Crypto.Parameters;
using System.Collections;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Utilities.Collections;
using System.Security.Cryptography;
using System.IO;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.X509.Extension;
using Org.BouncyCastle.Asn1.CryptoPro;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.Nist;
using Org.BouncyCastle.Asn1.TeleTrust;
using Org.BouncyCastle.Crypto.Generators;
using ECCGenerator;


namespace ECCripto
{
    public partial class Form1 : Form
    {
        CertFields certFields;
        Button btCommon,btUser;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateVersion();
            FillCombos();
            saveFileDialog1.Filter = openFileDialog1.Filter = "PKCS #12 file (*.p12)|*.p12";
        }

        void FillCombos()
        {
            cmbCurveName.DataSource = CurvesNames;
        }

        private void UpdateVersion()
        {
            labelVersion.Text = "v" + this.GetType().Assembly.GetName().Version.ToString();
        }

        CertFields ShowDialogDefineFields()
        {
            CertFields res=null;
            var frm = new frmCertFields();
            frm.FormClosing += (x, y) => { res = frm.CertFields; };
            frm.ShowDialog();
            return res;
        }

        #region common
        private void btGenetateKeys_Click(object sender, EventArgs e)
        {
            lbCurveName.Visible = cmbCurveName.Visible = chkCurvaName.Visible = btSaveCommon.Visible = true;
            txtCurveName.Visible = btFields.Visible = false;
            btCommon=btGenetateKeys;
        }

        private void btGenerateKeysCSR_Click(object sender, EventArgs e)
        {
            lbCurveName.Visible = cmbCurveName.Visible = chkCurvaName.Visible = btSaveCommon.Visible = btFields.Visible = true;
            txtCurveName.Visible = false;
            btCommon = btGenerateKeysCSR;
        }

        private void chkCurvaName_CheckedChanged(object sender, EventArgs e)
        {
            if (chkCurvaName.Checked)
            {
                cmbCurveName.Visible = false;
                txtCurveName.Visible = true;
            }
            else 
            {
                cmbCurveName.Visible = true;
                txtCurveName.Visible = false;
                txtCurveName.Text = "";
            }
        }

        private void btFields_Click(object sender, EventArgs e)
        {
            certFields = ShowDialogDefineFields();
        }

        List<string> CurvesNames 
        {
            get { return new List<string> { "Prime256v1", "SecP384r1", "secp521r1" }; }
        }

        private void btSaveCommon_Click(object sender, EventArgs e)
        {
            string curveName = "";
            if (cmbCurveName.Visible)
            {
                if (cmbCurveName.SelectedItem == null)
                {
                    MessageBox.Show("Debe escoger o escribir el nombre de una curva elíptica.");
                    return;
                }
                curveName = cmbCurveName.SelectedItem.ToString();
            }
            else 
            {
                if (txtCurveName.Text == "")
                {
                    MessageBox.Show("Debe escoger o escribir el nombre de una curva elíptica.");
                    return;
                }
                curveName = txtCurveName.Text;
            }

            try
            {
                if (btCommon == btGenetateKeys)
                {
                    MessageBox.Show("Escoja donde se guardará el fichero con las llaves");
                    if (saveFileDialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        Generator.CreateKeys(curveName, saveFileDialog2.FileName);
                        HideCommonFields();
                    }
                }
                else if (btCommon == btGenerateKeysCSR)
                {
                    MessageBox.Show("Escoja dondo se guardará el fichero CSR");
                    if (saveFileDialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        var csrFileName = saveFileDialog2.FileName;
                        MessageBox.Show("Escoja donde se guardará el fichero con las llaves");
                        if (saveFileDialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            var keysPath = saveFileDialog2.FileName;
                            Generator.GenerateCSRKeysFile(csrFileName, keysPath, curveName, certFields.CN, certFields.CI, certFields.OU, certFields.O, certFields.L, certFields.ST, certFields.EmailAddress);
                            HideCommonFields();
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("Ha ocurrido un error durante la generación. Revise los parámetros introducidos");
            }
        }

        void HideCommonFields()
        {
            lbCurveName.Visible = txtCurveName.Visible = cmbCurveName.Visible = chkCurvaName.Visible = btFields.Visible = btSaveCommon.Visible = false;
        }
        #endregion

        #region User

        private void btCreateCert_Click(object sender, EventArgs e)
        {
            lbCountValidMonths.Visible = nudCountValidMonths.Visible = lbFriendlyName.Visible = txtFriendlyName.Visible = btSaveUserCert.Visible = true;
            btUser = btCreateUserCert;
        }

        private void btCreateUserP12_Click(object sender, EventArgs e)
        {
            lbFriendlyName.Visible = txtFriendlyName.Visible = btSaveUserCert.Visible = true;
            lbCountValidMonths.Visible = nudCountValidMonths.Visible = false;
            btUser = btCreateUserP12;
        }

        private void btSaveUserCert_Click(object sender, EventArgs e)
        {
            if (txtFriendlyName.Text == "")
            {
                MessageBox.Show("Especifique el valor descriptivo que tendrá el nuevo certificado");
                return;
            }

            if (btUser == btCreateUserCert)
            {
                #region
                MessageBox.Show("Especifique el fichero PKCS#12 de la Entidad Certificadora (CA)");
                if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var password = "";
                    var frm = new FormPassword();
                    frm.FormClosing += (x, y) =>
                    {
                        password = frm.Password;
                    };
                    frm.ShowDialog();

                    MessageBox.Show("Especifique el fichero de solicitud CSR");
                    if (openFileDialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        MessageBox.Show("Especifique donde se guardará el certificado");
                        if (saveFileDialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            try { Generator.CreateUserCert(saveFileDialog2.FileName, openFileDialog1.FileName, password, (int)nudCountValidMonths.Value, txtFriendlyName.Text, openFileDialog2.FileName); }
                            catch { MessageBox.Show("Ha ocurrido un error durante la generación. Revise los parámetros introducidos"); }

                            lbCountValidMonths.Visible = nudCountValidMonths.Visible = lbFriendlyName.Visible = txtFriendlyName.Visible = btSaveUserCert.Visible = false;
                        }
                    }
                #endregion
                }
            }
            else if (btUser == btCreateUserP12)
            {
                #region
                MessageBox.Show("Especifique el fichero que contiene las llaves");
                if (openFileDialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var keysPath = openFileDialog2.FileName;

                    MessageBox.Show("Especifique el fichero que contiene el certificado de usuario");
                    if (openFileDialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        var userCertPath = openFileDialog2.FileName;

                        MessageBox.Show("Especifique donde se guardará el fichero PKCS#12");
                        if (saveFileDialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            var frm = new FormPassword();
                            var password="";
                            frm.FormClosing += (x, y) => { password = frm.Password; };
                            frm.ShowDialog();
                            try { Generator.GenerateUserCertPkcs12(saveFileDialog2.FileName, password, keysPath, userCertPath, txtFriendlyName.Text); }
                            catch { MessageBox.Show("Ha ocurrido un error durante la generación. Revise los parámetros introducidos"); }
                            lbFriendlyName.Visible = txtFriendlyName.Visible = btSaveUserCert.Visible = false;
                        }
                    }
                }
                #endregion
            }
        }

        #endregion

        #region CA

        private void btCreateCACertP12_Click(object sender, EventArgs e)
        {
            lbFriendlyNameCA.Visible = txtFriendlyNameCA.Visible = btSaveCA.Visible = true;
        }

        private void btSaveCA_Click(object sender, EventArgs e)
        {
            if (txtFriendlyNameCA.Text == "")
            {
                MessageBox.Show("Especifique el valor descriptivo que tendrá el nuevo certificado");
                return;
            }

            MessageBox.Show("Especifique el fichero que contiene las llaves");
            if (openFileDialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var keysPath = openFileDialog2.FileName;
                MessageBox.Show("Especifique el fichero que contiene el certificado de la Entidad Certificadora (CA)");
                if (openFileDialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    MessageBox.Show("Especifique donde se guardará el fichero PKCS#12");
                    if (saveFileDialog2.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        var frm = new FormPassword();
                        var password = "";
                        frm.FormClosing += (x, y) => { password = frm.Password; };
                        frm.ShowDialog();
                        try { Generator.GenerateCACertPkcs12(saveFileDialog2.FileName, password, keysPath, openFileDialog2.FileName, txtFriendlyNameCA.Text); }
                        catch { MessageBox.Show("Ha ocurrido un error durante la generación. Revise los parámetros introducidos"); }
                        lbFriendlyNameCA.Visible = txtFriendlyNameCA.Visible = btSaveCA.Visible = false;
                    }
                }
            }
        }

        #endregion
    }
}
