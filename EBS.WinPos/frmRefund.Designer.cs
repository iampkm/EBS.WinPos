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
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblOrderAmount = new System.Windows.Forms.Label();
            this.lblAmount = new System.Windows.Forms.Label();
            this.txtRefundCode = new System.Windows.Forms.TextBox();
            this.txtLicenseCode = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblOnlinePay = new System.Windows.Forms.Label();
            this.lblPaymentWay = new System.Windows.Forms.Label();
            this.lblRefundOrder = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lblOrderAmount);
            this.panel2.Controls.Add(this.lblAmount);
            this.panel2.Controls.Add(this.txtRefundCode);
            this.panel2.Controls.Add(this.txtLicenseCode);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.btnCancel);
            this.panel2.Controls.Add(this.btnSave);
            this.panel2.Controls.Add(this.lblOnlinePay);
            this.panel2.Controls.Add(this.lblPaymentWay);
            this.panel2.Controls.Add(this.lblRefundOrder);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(873, 509);
            this.panel2.TabIndex = 2;
            // 
            // lblOrderAmount
            // 
            this.lblOrderAmount.AutoSize = true;
            this.lblOrderAmount.Font = new System.Drawing.Font("微软雅黑", 42F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblOrderAmount.ForeColor = System.Drawing.Color.Red;
            this.lblOrderAmount.Location = new System.Drawing.Point(150, 209);
            this.lblOrderAmount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblOrderAmount.Name = "lblOrderAmount";
            this.lblOrderAmount.Size = new System.Drawing.Size(330, 90);
            this.lblOrderAmount.TabIndex = 3;
            this.lblOrderAmount.Text = "￥120.00";
            // 
            // lblAmount
            // 
            this.lblAmount.AutoSize = true;
            this.lblAmount.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblAmount.Location = new System.Drawing.Point(27, 252);
            this.lblAmount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAmount.Name = "lblAmount";
            this.lblAmount.Size = new System.Drawing.Size(134, 31);
            this.lblAmount.TabIndex = 4;
            this.lblAmount.Text = "退款金额：";
            // 
            // txtRefundCode
            // 
            this.txtRefundCode.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtRefundCode.Location = new System.Drawing.Point(165, 23);
            this.txtRefundCode.Margin = new System.Windows.Forms.Padding(4);
            this.txtRefundCode.Name = "txtRefundCode";
            this.txtRefundCode.Size = new System.Drawing.Size(422, 42);
            this.txtRefundCode.TabIndex = 0;
            this.txtRefundCode.TextChanged += new System.EventHandler(this.txtRefundCode_TextChanged);
            this.txtRefundCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtRefundCode_KeyDown);
            // 
            // txtLicenseCode
            // 
            this.txtLicenseCode.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtLicenseCode.Location = new System.Drawing.Point(165, 311);
            this.txtLicenseCode.Margin = new System.Windows.Forms.Padding(4);
            this.txtLicenseCode.Name = "txtLicenseCode";
            this.txtLicenseCode.PasswordChar = '*';
            this.txtLicenseCode.Size = new System.Drawing.Size(422, 42);
            this.txtLicenseCode.TabIndex = 1;
            this.txtLicenseCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtLicenseCode_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(5, 311);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(158, 31);
            this.label3.TabIndex = 0;
            this.label3.Text = "店长授权码：";
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.Red;
            this.btnSave.Font = new System.Drawing.Font("微软雅黑", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSave.ForeColor = System.Drawing.Color.Snow;
            this.btnSave.Location = new System.Drawing.Point(97, 381);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(283, 95);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "确认退款";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblOnlinePay
            // 
            this.lblOnlinePay.AutoSize = true;
            this.lblOnlinePay.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblOnlinePay.ForeColor = System.Drawing.Color.Blue;
            this.lblOnlinePay.Location = new System.Drawing.Point(64, 81);
            this.lblOnlinePay.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblOnlinePay.Name = "lblOnlinePay";
            this.lblOnlinePay.Size = new System.Drawing.Size(718, 124);
            this.lblOnlinePay.TabIndex = 0;
            this.lblOnlinePay.Text = "1.现金退款可不录入小票，按Enter键退款。\r\n2. 支付宝微信，输入小票单号或支付记录的商户订单号，\r\n按Enter键退款。\r\n3. 客户扫码退款，微信输入“" +
    "W”，支付宝“A”，按Enter键退款";
            // 
            // lblPaymentWay
            // 
            this.lblPaymentWay.AutoSize = true;
            this.lblPaymentWay.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblPaymentWay.ForeColor = System.Drawing.Color.Red;
            this.lblPaymentWay.Location = new System.Drawing.Point(612, 29);
            this.lblPaymentWay.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPaymentWay.Name = "lblPaymentWay";
            this.lblPaymentWay.Size = new System.Drawing.Size(110, 31);
            this.lblPaymentWay.TabIndex = 0;
            this.lblPaymentWay.Text = "现金退款";
            // 
            // lblRefundOrder
            // 
            this.lblRefundOrder.AutoSize = true;
            this.lblRefundOrder.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblRefundOrder.Location = new System.Drawing.Point(29, 29);
            this.lblRefundOrder.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRefundOrder.Name = "lblRefundOrder";
            this.lblRefundOrder.Size = new System.Drawing.Size(134, 31);
            this.lblRefundOrder.TabIndex = 0;
            this.lblRefundOrder.Text = "小票单号：";
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
            this.btnCancel.Font = new System.Drawing.Font("微软雅黑", 26.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCancel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnCancel.Location = new System.Drawing.Point(422, 381);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(283, 95);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "返 回";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmRefund
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(873, 509);
            this.ControlBox = false;
            this.Controls.Add(this.panel2);
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmRefund";
            this.Text = "销售退款";
            this.Load += new System.EventHandler(this.frmRefund_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmRefund_KeyDown);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblAmount;
        private System.Windows.Forms.Label lblOrderAmount;
        private System.Windows.Forms.TextBox txtRefundCode;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblOnlinePay;
        private System.Windows.Forms.Label lblRefundOrder;
        private System.Windows.Forms.TextBox txtLicenseCode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblPaymentWay;
        private System.Windows.Forms.Button btnCancel;
    }
}