namespace EBS.WinPos
{
    partial class frmPayQuery
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
            this.txtOrderCode = new System.Windows.Forms.TextBox();
            this.lblRefundOrder = new System.Windows.Forms.Label();
            this.lstOrderInfo = new System.Windows.Forms.ListBox();
            this.btnQuery = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lstPayInfo = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtOrderCode
            // 
            this.txtOrderCode.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtOrderCode.Location = new System.Drawing.Point(185, 28);
            this.txtOrderCode.Margin = new System.Windows.Forms.Padding(4);
            this.txtOrderCode.Name = "txtOrderCode";
            this.txtOrderCode.Size = new System.Drawing.Size(401, 42);
            this.txtOrderCode.TabIndex = 11;
            // 
            // lblRefundOrder
            // 
            this.lblRefundOrder.AutoSize = true;
            this.lblRefundOrder.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.lblRefundOrder.Location = new System.Drawing.Point(43, 34);
            this.lblRefundOrder.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblRefundOrder.Name = "lblRefundOrder";
            this.lblRefundOrder.Size = new System.Drawing.Size(134, 31);
            this.lblRefundOrder.TabIndex = 12;
            this.lblRefundOrder.Text = "小票单号：";
            // 
            // lstOrderInfo
            // 
            this.lstOrderInfo.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.lstOrderInfo.FormattingEnabled = true;
            this.lstOrderInfo.ItemHeight = 30;
            this.lstOrderInfo.Items.AddRange(new object[] {
            "单号：XXX1234",
            "支付状态：abc",
            "支付信息：1123",
            "支付信息：1123",
            "支付信息：1123",
            "支付信息：1123",
            "支付信息：1123",
            "支付信息：1123",
            "支付信息：1123",
            "支付信息：1123",
            "支付信息：1123",
            "支付信息：1123"});
            this.lstOrderInfo.Location = new System.Drawing.Point(26, 125);
            this.lstOrderInfo.Name = "lstOrderInfo";
            this.lstOrderInfo.Size = new System.Drawing.Size(435, 424);
            this.lstOrderInfo.TabIndex = 14;
            // 
            // btnQuery
            // 
            this.btnQuery.BackColor = System.Drawing.SystemColors.HotTrack;
            this.btnQuery.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.btnQuery.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnQuery.Location = new System.Drawing.Point(609, 28);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(117, 42);
            this.btnQuery.TabIndex = 15;
            this.btnQuery.Text = "查 询";
            this.btnQuery.UseVisualStyleBackColor = false;
            this.btnQuery.Click += new System.EventHandler(this.BtnQuery_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.btnCancel.Location = new System.Drawing.Point(421, 567);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(110, 42);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.Text = "返 回";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // lstPayInfo
            // 
            this.lstPayInfo.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.lstPayInfo.FormattingEnabled = true;
            this.lstPayInfo.ItemHeight = 30;
            this.lstPayInfo.Items.AddRange(new object[] {
            "单号：XXX1234",
            "支付状态：abc",
            "支付信息：1123",
            "支付信息：1123",
            "支付信息：1123",
            "支付信息：1123",
            "支付信息：1123",
            "支付信息：1123",
            "支付信息：1123",
            "支付信息：1123",
            "支付信息：1123",
            "支付信息：1123"});
            this.lstPayInfo.Location = new System.Drawing.Point(505, 125);
            this.lstPayInfo.Name = "lstPayInfo";
            this.lstPayInfo.Size = new System.Drawing.Size(489, 424);
            this.lstPayInfo.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.label1.Location = new System.Drawing.Point(20, 91);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 31);
            this.label1.TabIndex = 12;
            this.label1.Text = "订单信息：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.label2.Location = new System.Drawing.Point(507, 91);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(134, 31);
            this.label2.TabIndex = 12;
            this.label2.Text = "支付信息：";
            // 
            // frmPayQuery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1006, 621);
            this.Controls.Add(this.btnQuery);
            this.Controls.Add(this.lstPayInfo);
            this.Controls.Add(this.lstOrderInfo);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblRefundOrder);
            this.Controls.Add(this.txtOrderCode);
            this.KeyPreview = true;
            this.Name = "frmPayQuery";
            this.Text = "微信，支付宝收款，退款查询";
            this.Load += new System.EventHandler(this.FrmPayQuery_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtOrderCode;
        private System.Windows.Forms.Label lblRefundOrder;
        private System.Windows.Forms.ListBox lstOrderInfo;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ListBox lstPayInfo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}