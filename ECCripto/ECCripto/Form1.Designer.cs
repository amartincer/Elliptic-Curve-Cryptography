namespace ECCripto
{
    partial class Form1
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
            this.labelVersion = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.pageCommon = new System.Windows.Forms.TabPage();
            this.btSaveCommon = new System.Windows.Forms.Button();
            this.txtCurveName = new System.Windows.Forms.TextBox();
            this.chkCurvaName = new System.Windows.Forms.CheckBox();
            this.btFields = new System.Windows.Forms.Button();
            this.cmbCurveName = new System.Windows.Forms.ComboBox();
            this.lbCurveName = new System.Windows.Forms.Label();
            this.btGenerateKeysCSR = new System.Windows.Forms.Button();
            this.btGenetateKeys = new System.Windows.Forms.Button();
            this.pageUser = new System.Windows.Forms.TabPage();
            this.btCreateUserP12 = new System.Windows.Forms.Button();
            this.btSaveUserCert = new System.Windows.Forms.Button();
            this.txtFriendlyName = new System.Windows.Forms.TextBox();
            this.lbFriendlyName = new System.Windows.Forms.Label();
            this.nudCountValidMonths = new System.Windows.Forms.NumericUpDown();
            this.lbCountValidMonths = new System.Windows.Forms.Label();
            this.btCreateUserCert = new System.Windows.Forms.Button();
            this.pageCA = new System.Windows.Forms.TabPage();
            this.btCreateCACertP12 = new System.Windows.Forms.Button();
            this.btSaveCA = new System.Windows.Forms.Button();
            this.txtFriendlyNameCA = new System.Windows.Forms.TextBox();
            this.lbFriendlyNameCA = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.saveFileDialog2 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.tabControl1.SuspendLayout();
            this.pageCommon.SuspendLayout();
            this.pageUser.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCountValidMonths)).BeginInit();
            this.pageCA.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.Location = new System.Drawing.Point(474, 284);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(47, 13);
            this.labelVersion.TabIndex = 17;
            this.labelVersion.Text = "(version)";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.pageCommon);
            this.tabControl1.Controls.Add(this.pageUser);
            this.tabControl1.Controls.Add(this.pageCA);
            this.tabControl1.Location = new System.Drawing.Point(12, 87);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(531, 181);
            this.tabControl1.TabIndex = 18;
            // 
            // pageCommon
            // 
            this.pageCommon.Controls.Add(this.btSaveCommon);
            this.pageCommon.Controls.Add(this.txtCurveName);
            this.pageCommon.Controls.Add(this.chkCurvaName);
            this.pageCommon.Controls.Add(this.btFields);
            this.pageCommon.Controls.Add(this.cmbCurveName);
            this.pageCommon.Controls.Add(this.lbCurveName);
            this.pageCommon.Controls.Add(this.btGenerateKeysCSR);
            this.pageCommon.Controls.Add(this.btGenetateKeys);
            this.pageCommon.Location = new System.Drawing.Point(4, 22);
            this.pageCommon.Name = "pageCommon";
            this.pageCommon.Padding = new System.Windows.Forms.Padding(3);
            this.pageCommon.Size = new System.Drawing.Size(523, 155);
            this.pageCommon.TabIndex = 0;
            this.pageCommon.Text = "Common";
            this.pageCommon.UseVisualStyleBackColor = true;
            // 
            // btSaveCommon
            // 
            this.btSaveCommon.AutoSize = true;
            this.btSaveCommon.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btSaveCommon.Location = new System.Drawing.Point(326, 105);
            this.btSaveCommon.Name = "btSaveCommon";
            this.btSaveCommon.Size = new System.Drawing.Size(42, 23);
            this.btSaveCommon.TabIndex = 7;
            this.btSaveCommon.Text = "Save";
            this.btSaveCommon.UseVisualStyleBackColor = true;
            this.btSaveCommon.Visible = false;
            this.btSaveCommon.Click += new System.EventHandler(this.btSaveCommon_Click);
            // 
            // txtCurveName
            // 
            this.txtCurveName.Location = new System.Drawing.Point(278, 41);
            this.txtCurveName.Name = "txtCurveName";
            this.txtCurveName.Size = new System.Drawing.Size(138, 20);
            this.txtCurveName.TabIndex = 6;
            this.txtCurveName.Visible = false;
            // 
            // chkCurvaName
            // 
            this.chkCurvaName.AutoSize = true;
            this.chkCurvaName.Location = new System.Drawing.Point(428, 45);
            this.chkCurvaName.Name = "chkCurvaName";
            this.chkCurvaName.Size = new System.Drawing.Size(15, 14);
            this.chkCurvaName.TabIndex = 5;
            this.chkCurvaName.UseVisualStyleBackColor = true;
            this.chkCurvaName.Visible = false;
            this.chkCurvaName.CheckedChanged += new System.EventHandler(this.chkCurvaName_CheckedChanged);
            // 
            // btFields
            // 
            this.btFields.AutoSize = true;
            this.btFields.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btFields.Location = new System.Drawing.Point(311, 76);
            this.btFields.Name = "btFields";
            this.btFields.Size = new System.Drawing.Size(75, 23);
            this.btFields.TabIndex = 4;
            this.btFields.Text = "Define fields";
            this.btFields.UseVisualStyleBackColor = true;
            this.btFields.Visible = false;
            this.btFields.Click += new System.EventHandler(this.btFields_Click);
            // 
            // cmbCurveName
            // 
            this.cmbCurveName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCurveName.FormattingEnabled = true;
            this.cmbCurveName.Location = new System.Drawing.Point(278, 41);
            this.cmbCurveName.Name = "cmbCurveName";
            this.cmbCurveName.Size = new System.Drawing.Size(138, 21);
            this.cmbCurveName.TabIndex = 3;
            this.cmbCurveName.Visible = false;
            // 
            // lbCurveName
            // 
            this.lbCurveName.AutoSize = true;
            this.lbCurveName.Location = new System.Drawing.Point(298, 25);
            this.lbCurveName.Name = "lbCurveName";
            this.lbCurveName.Size = new System.Drawing.Size(99, 13);
            this.lbCurveName.TabIndex = 2;
            this.lbCurveName.Text = "Elliptic Curve Name";
            this.lbCurveName.Visible = false;
            // 
            // btGenerateKeysCSR
            // 
            this.btGenerateKeysCSR.Location = new System.Drawing.Point(18, 87);
            this.btGenerateKeysCSR.Name = "btGenerateKeysCSR";
            this.btGenerateKeysCSR.Size = new System.Drawing.Size(157, 23);
            this.btGenerateKeysCSR.TabIndex = 1;
            this.btGenerateKeysCSR.Text = "Generate Keys and CSR file";
            this.btGenerateKeysCSR.UseVisualStyleBackColor = true;
            this.btGenerateKeysCSR.Click += new System.EventHandler(this.btGenerateKeysCSR_Click);
            // 
            // btGenetateKeys
            // 
            this.btGenetateKeys.Location = new System.Drawing.Point(18, 25);
            this.btGenetateKeys.Name = "btGenetateKeys";
            this.btGenetateKeys.Size = new System.Drawing.Size(96, 23);
            this.btGenetateKeys.TabIndex = 0;
            this.btGenetateKeys.Text = "Generate Keys";
            this.btGenetateKeys.UseVisualStyleBackColor = true;
            this.btGenetateKeys.Click += new System.EventHandler(this.btGenetateKeys_Click);
            // 
            // pageUser
            // 
            this.pageUser.Controls.Add(this.btCreateUserP12);
            this.pageUser.Controls.Add(this.btSaveUserCert);
            this.pageUser.Controls.Add(this.txtFriendlyName);
            this.pageUser.Controls.Add(this.lbFriendlyName);
            this.pageUser.Controls.Add(this.nudCountValidMonths);
            this.pageUser.Controls.Add(this.lbCountValidMonths);
            this.pageUser.Controls.Add(this.btCreateUserCert);
            this.pageUser.Location = new System.Drawing.Point(4, 22);
            this.pageUser.Name = "pageUser";
            this.pageUser.Padding = new System.Windows.Forms.Padding(3);
            this.pageUser.Size = new System.Drawing.Size(523, 155);
            this.pageUser.TabIndex = 1;
            this.pageUser.Text = "User";
            this.pageUser.UseVisualStyleBackColor = true;
            // 
            // btCreateUserP12
            // 
            this.btCreateUserP12.Location = new System.Drawing.Point(15, 70);
            this.btCreateUserP12.Name = "btCreateUserP12";
            this.btCreateUserP12.Size = new System.Drawing.Size(201, 23);
            this.btCreateUserP12.TabIndex = 9;
            this.btCreateUserP12.Text = "Create User Certificate PKCS#12";
            this.btCreateUserP12.UseVisualStyleBackColor = true;
            this.btCreateUserP12.Click += new System.EventHandler(this.btCreateUserP12_Click);
            // 
            // btSaveUserCert
            // 
            this.btSaveUserCert.AutoSize = true;
            this.btSaveUserCert.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btSaveUserCert.Location = new System.Drawing.Point(470, 67);
            this.btSaveUserCert.Name = "btSaveUserCert";
            this.btSaveUserCert.Size = new System.Drawing.Size(42, 23);
            this.btSaveUserCert.TabIndex = 8;
            this.btSaveUserCert.Text = "Save";
            this.btSaveUserCert.UseVisualStyleBackColor = true;
            this.btSaveUserCert.Visible = false;
            this.btSaveUserCert.Click += new System.EventHandler(this.btSaveUserCert_Click);
            // 
            // txtFriendlyName
            // 
            this.txtFriendlyName.Location = new System.Drawing.Point(364, 67);
            this.txtFriendlyName.Name = "txtFriendlyName";
            this.txtFriendlyName.Size = new System.Drawing.Size(100, 20);
            this.txtFriendlyName.TabIndex = 5;
            this.txtFriendlyName.Visible = false;
            // 
            // lbFriendlyName
            // 
            this.lbFriendlyName.AutoSize = true;
            this.lbFriendlyName.Location = new System.Drawing.Point(265, 70);
            this.lbFriendlyName.Name = "lbFriendlyName";
            this.lbFriendlyName.Size = new System.Drawing.Size(74, 13);
            this.lbFriendlyName.TabIndex = 4;
            this.lbFriendlyName.Text = "Friendly Name";
            this.lbFriendlyName.Visible = false;
            // 
            // nudCountValidMonths
            // 
            this.nudCountValidMonths.Location = new System.Drawing.Point(364, 28);
            this.nudCountValidMonths.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudCountValidMonths.Name = "nudCountValidMonths";
            this.nudCountValidMonths.Size = new System.Drawing.Size(41, 20);
            this.nudCountValidMonths.TabIndex = 3;
            this.nudCountValidMonths.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudCountValidMonths.Visible = false;
            // 
            // lbCountValidMonths
            // 
            this.lbCountValidMonths.AutoSize = true;
            this.lbCountValidMonths.Location = new System.Drawing.Point(265, 30);
            this.lbCountValidMonths.Name = "lbCountValidMonths";
            this.lbCountValidMonths.Size = new System.Drawing.Size(93, 13);
            this.lbCountValidMonths.TabIndex = 2;
            this.lbCountValidMonths.Text = "CountValidMonths";
            this.lbCountValidMonths.Visible = false;
            // 
            // btCreateUserCert
            // 
            this.btCreateUserCert.Location = new System.Drawing.Point(15, 21);
            this.btCreateUserCert.Name = "btCreateUserCert";
            this.btCreateUserCert.Size = new System.Drawing.Size(201, 23);
            this.btCreateUserCert.TabIndex = 1;
            this.btCreateUserCert.Text = "Create User Certificate from CSR file";
            this.btCreateUserCert.UseVisualStyleBackColor = true;
            this.btCreateUserCert.Click += new System.EventHandler(this.btCreateCert_Click);
            // 
            // pageCA
            // 
            this.pageCA.Controls.Add(this.btCreateCACertP12);
            this.pageCA.Controls.Add(this.btSaveCA);
            this.pageCA.Controls.Add(this.txtFriendlyNameCA);
            this.pageCA.Controls.Add(this.lbFriendlyNameCA);
            this.pageCA.Location = new System.Drawing.Point(4, 22);
            this.pageCA.Name = "pageCA";
            this.pageCA.Size = new System.Drawing.Size(523, 155);
            this.pageCA.TabIndex = 2;
            this.pageCA.Text = "CA";
            this.pageCA.UseVisualStyleBackColor = true;
            // 
            // btCreateCACertP12
            // 
            this.btCreateCACertP12.Location = new System.Drawing.Point(15, 20);
            this.btCreateCACertP12.Name = "btCreateCACertP12";
            this.btCreateCACertP12.Size = new System.Drawing.Size(201, 23);
            this.btCreateCACertP12.TabIndex = 13;
            this.btCreateCACertP12.Text = "Create CA Certificate PKCS#12";
            this.btCreateCACertP12.UseVisualStyleBackColor = true;
            this.btCreateCACertP12.Click += new System.EventHandler(this.btCreateCACertP12_Click);
            // 
            // btSaveCA
            // 
            this.btSaveCA.AutoSize = true;
            this.btSaveCA.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btSaveCA.Location = new System.Drawing.Point(470, 18);
            this.btSaveCA.Name = "btSaveCA";
            this.btSaveCA.Size = new System.Drawing.Size(42, 23);
            this.btSaveCA.TabIndex = 12;
            this.btSaveCA.Text = "Save";
            this.btSaveCA.UseVisualStyleBackColor = true;
            this.btSaveCA.Visible = false;
            this.btSaveCA.Click += new System.EventHandler(this.btSaveCA_Click);
            // 
            // txtFriendlyNameCA
            // 
            this.txtFriendlyNameCA.Location = new System.Drawing.Point(354, 18);
            this.txtFriendlyNameCA.Name = "txtFriendlyNameCA";
            this.txtFriendlyNameCA.Size = new System.Drawing.Size(100, 20);
            this.txtFriendlyNameCA.TabIndex = 11;
            this.txtFriendlyNameCA.Visible = false;
            // 
            // lbFriendlyNameCA
            // 
            this.lbFriendlyNameCA.AutoSize = true;
            this.lbFriendlyNameCA.Location = new System.Drawing.Point(265, 23);
            this.lbFriendlyNameCA.Name = "lbFriendlyNameCA";
            this.lbFriendlyNameCA.Size = new System.Drawing.Size(74, 13);
            this.lbFriendlyNameCA.TabIndex = 10;
            this.lbFriendlyNameCA.Text = "Friendly Name";
            this.lbFriendlyNameCA.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(532, 69);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 16;
            this.pictureBox1.TabStop = false;
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.FileName = "openFileDialog2";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(555, 307);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Generation of Elliptic Curves Certificates";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.pageCommon.ResumeLayout(false);
            this.pageCommon.PerformLayout();
            this.pageUser.ResumeLayout(false);
            this.pageUser.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudCountValidMonths)).EndInit();
            this.pageCA.ResumeLayout(false);
            this.pageCA.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage pageCommon;
        private System.Windows.Forms.Button btGenerateKeysCSR;
        private System.Windows.Forms.Button btGenetateKeys;
        private System.Windows.Forms.TabPage pageUser;
        private System.Windows.Forms.TabPage pageCA;
        private System.Windows.Forms.ComboBox cmbCurveName;
        private System.Windows.Forms.Label lbCurveName;
        private System.Windows.Forms.TextBox txtCurveName;
        private System.Windows.Forms.CheckBox chkCurvaName;
        private System.Windows.Forms.Button btFields;
        private System.Windows.Forms.Button btSaveCommon;
        private System.Windows.Forms.SaveFileDialog saveFileDialog2;
        private System.Windows.Forms.Button btCreateUserCert;
        private System.Windows.Forms.Label lbFriendlyName;
        private System.Windows.Forms.NumericUpDown nudCountValidMonths;
        private System.Windows.Forms.Label lbCountValidMonths;
        private System.Windows.Forms.Button btSaveUserCert;
        private System.Windows.Forms.TextBox txtFriendlyName;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private System.Windows.Forms.Button btCreateUserP12;
        private System.Windows.Forms.Button btCreateCACertP12;
        private System.Windows.Forms.Button btSaveCA;
        private System.Windows.Forms.TextBox txtFriendlyNameCA;
        private System.Windows.Forms.Label lbFriendlyNameCA;
    }
}

