namespace ECCripto
{
    partial class frmCertDetails
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnViewCert = new System.Windows.Forms.Button();
            this.btnViewStore = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtThumbprint = new System.Windows.Forms.TextBox();
            this.txtStore = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnViewCert);
            this.groupBox2.Controls.Add(this.btnViewStore);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.txtThumbprint);
            this.groupBox2.Controls.Add(this.txtStore);
            this.groupBox2.Location = new System.Drawing.Point(11, 36);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(507, 81);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "details";
            // 
            // btnViewCert
            // 
            this.btnViewCert.Location = new System.Drawing.Point(378, 44);
            this.btnViewCert.Name = "btnViewCert";
            this.btnViewCert.Size = new System.Drawing.Size(113, 24);
            this.btnViewCert.TabIndex = 4;
            this.btnViewCert.Text = "View cert";
            this.btnViewCert.UseVisualStyleBackColor = true;
            this.btnViewCert.Click += new System.EventHandler(this.btnViewCert_Click);
            // 
            // btnViewStore
            // 
            this.btnViewStore.Location = new System.Drawing.Point(378, 15);
            this.btnViewStore.Name = "btnViewStore";
            this.btnViewStore.Size = new System.Drawing.Size(113, 23);
            this.btnViewStore.TabIndex = 4;
            this.btnViewStore.Text = "Browse store";
            this.btnViewStore.UseVisualStyleBackColor = true;
            this.btnViewStore.Click += new System.EventHandler(this.btnViewStore_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Thumbprint:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(54, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Store:";
            // 
            // txtThumbprint
            // 
            this.txtThumbprint.Location = new System.Drawing.Point(92, 47);
            this.txtThumbprint.Name = "txtThumbprint";
            this.txtThumbprint.ReadOnly = true;
            this.txtThumbprint.Size = new System.Drawing.Size(280, 20);
            this.txtThumbprint.TabIndex = 1;
            // 
            // txtStore
            // 
            this.txtStore.Location = new System.Drawing.Point(92, 17);
            this.txtStore.Name = "txtStore";
            this.txtStore.ReadOnly = true;
            this.txtStore.Size = new System.Drawing.Size(280, 20);
            this.txtStore.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(384, 24);
            this.label1.TabIndex = 4;
            this.label1.Text = "Your certificate has been created and stored.";
            // 
            // frmCertDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(533, 134);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.label1);
            this.Name = "frmCertDetails";
            this.Text = "Certificate Details";
            this.Load += new System.EventHandler(this.frmCertDetails_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnViewCert;
        private System.Windows.Forms.Button btnViewStore;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtThumbprint;
        private System.Windows.Forms.TextBox txtStore;
        private System.Windows.Forms.Label label1;
    }
}