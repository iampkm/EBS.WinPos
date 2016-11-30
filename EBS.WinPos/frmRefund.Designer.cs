namespace EBS.WinPos
{
    partial class frmRefund
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.lstPaymentWay = new System.Windows.Forms.ListBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.lblOrderAmount = new System.Windows.Forms.Label();
            this.txtPayAmount = new System.Windows.Forms.TextBox();
            this.txtLicenseCode = new System.Windows.Forms.TextBox();
            this.txtRefundAccount = new System.Windows.Forms.TextBox();
            this.txtOnlinePayAmount = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.lblOnlinePayAmount = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lstPaymentWay);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(259, 407);
            this.panel1.TabIndex = 1;
            // 
            // lstPaymentWay
            // 
            this.lstPaymentWay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstPaymentWay.Font = new System.Drawing.Font("微软雅黑", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lstPaymentWay.FormattingEnabled = true;
            this.lstPaymentWay.ItemHeight = 38;
            this.lstPaymentWay.Items.AddRange(new object[] {
            "现金",
            "支付宝",
            "微信",
            "银联",
            "Pos刷卡"});
            this.lstPaymentWay.Location = new System.Drawing.Point(0, 0);
            this.lstPaymentWay.Name = "lstPaymentWay";
            this.lstPaymentWay.Size = new System.Drawing.Size(259, 407);
            this.lstPaymentWay.TabIndex = 1;
            this.lstPaymentWay.SelectedValueChanged += new System.EventHandler(this.lstPaymentWay_SelectedValueChanged);
            this.lstPaymentWay.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lstPaymentWay_KeyDown);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.lblOrderAmount);
            this.panel2.Controls.Add(this.txtPayAmount);
            this.panel2.Controls.Add(this.txtLicenseCode);
            this.panel2.Controls.Add(this.txtRefundAccount);
            this.panel2.Controls.Add(this.txtOnlinePayAmount);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.btnSave);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.lblOnlinePayAmount);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(259, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(396, 407);
            this.panel2.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(41, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 25);
            this.label1.TabIndex = 4;
            this.label1.Text = "应退款：";
            // 
            // lblOrderAmount
            // 
            this.lblOrderAmount.AutoSize = true;
            this.lblOrderAmount.Font = new System.Drawing.Font("微软雅黑", 42F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblOrderAmount.Location = new System.Drawing.Point(108, 13);
            this.lblOrderAmount.Name = "lblOrderAmount";
            this.lblOrderAmount.Size = new System.Drawing.Size(266, 75);
            this.lblOrderAmount.TabIndex = 3;
            this.lblOrderAmount.Text = "￥120.00";
            // 
            // txtPayAmount
            // 
            this.txtPayAmount.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtPayAmount.Location = new System.Drawing.Point(139, 91);
            this.txtPayAmount.Name = "txtPayAmount";
            this.txtPayAmount.Size = new System.Drawing.Size(208, 35);
            this.txtPayAmount.TabIndex = 2;
            this.txtPayAmount.TextChanged += new System.EventHandler(this.txtPayAmount_TextChanged);
            this.txtPayAmount.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPayAmount_KeyDown);
            // 
            // txtLicenseCode
            // 
            this.txtLicenseCode.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtLicenseCode.Location = new System.Drawing.Point(139, 226);
            this.txtLicenseCode.Name = "txtLicenseCode";
            this.txtLicenseCode.PasswordChar = '*';
            this.txtLicenseCode.Size = new System.Drawing.Size(208, 35);
            this.txtLicenseCode.TabIndex = 3;
            this.txtLicenseCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtLicenseCode_KeyDown);
            // 
            // txtRefundAccount
            // 
            this.txtRefundAccount.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtRefundAccount.Location = new System.Drawing.Point(139, 181);
            this.txtRefundAccount.Name = "txtRefundAccount";
            this.txtRefundAccount.Size = new System.Drawing.Size(208, 35);
            this.txtRefundAccount.TabIndex = 3;
            this.txtRefundAccount.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtRefundAccount_KeyDown);
            // 
            // txtOnlinePayAmount
            // 
            this.txtOnlinePayAmount.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtOnlinePayAmount.Location = new System.Drawing.Point(139, 136);
            this.txtOnlinePayAmount.Name = "txtOnlinePayAmount";
            this.txtOnlinePayAmount.Size = new System.Drawing.Size(208, 35);
            this.txtOnlinePayAmount.TabIndex = 3;
            this.txtOnlinePayAmount.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtOnlinePayAmount_KeyDown);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.Color.Blue;
            this.label4.Location = new System.Drawing.Point(87, 366);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(161, 28);
            this.label4.TabIndex = 3;
            this.label4.Text = "按ESC 取消退单";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(3, 231);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(126, 25);
            this.label3.TabIndex = 0;
            this.label3.Text = "店长授权码：";
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.SystemColors.Control;
            this.btnSave.Font = new System.Drawing.Font("微软雅黑", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSave.Location = new System.Drawing.Point(27, 269);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(330, 76);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "退 款";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(22, 186);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(107, 25);
            this.label5.TabIndex = 0;
            this.label5.Text = "退款账号：";
            // 
            // lblOnlinePayAmount
            // 
            this.lblOnlinePayAmount.AutoSize = true;
            this.lblOnlinePayAmount.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblOnlinePayAmount.Location = new System.Drawing.Point(41, 141);
            this.lblOnlinePayAmount.Name = "lblOnlinePayAmount";
            this.lblOnlinePayAmount.Size = new System.Drawing.Size(88, 25);
            this.lblOnlinePayAmount.TabIndex = 0;
            this.lblOnlinePayAmount.Text = "退网银：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(22, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 25);
            this.label2.TabIndex = 0;
            this.label2.Text = "退现金￥：";
            // 
            // frmRefund
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(655, 407);
            this.ControlBox = false;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.KeyPreview = true;
            this.Name = "frmRefund";
            this.Text = "选择退款方式";
            this.Load += new System.EventHandler(this.frmRefund_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmRefund_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox lstPaymentWay;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblOrderAmount;
        private System.Windows.Forms.TextBox txtPayAmount;
        private System.Windows.Forms.TextBox txtOnlinePayAmount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblOnlinePayAmount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLicenseCode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtRefundAccount;
        private System.Windows.Forms.Label label5;
    }
}