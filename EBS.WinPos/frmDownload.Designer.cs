namespace EBS.WinPos
{
    partial class frmDownload
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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lblMsg = new System.Windows.Forms.Label();
            this.btnDownloadAll = new System.Windows.Forms.Button();
            this.btnDownloadProduct = new System.Windows.Forms.Button();
            this.chkList = new System.Windows.Forms.CheckedListBox();
            this.chkAll = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(37, 161);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(396, 23);
            this.progressBar1.TabIndex = 0;
            // 
            // lblMsg
            // 
            this.lblMsg.AutoSize = true;
            this.lblMsg.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblMsg.Location = new System.Drawing.Point(33, 115);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(55, 20);
            this.lblMsg.TabIndex = 1;
            this.lblMsg.Text = "lblMsg";
            // 
            // btnDownloadAll
            // 
            this.btnDownloadAll.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnDownloadAll.Location = new System.Drawing.Point(362, 81);
            this.btnDownloadAll.Name = "btnDownloadAll";
            this.btnDownloadAll.Size = new System.Drawing.Size(120, 31);
            this.btnDownloadAll.TabIndex = 2;
            this.btnDownloadAll.Text = "下载全量数据";
            this.btnDownloadAll.UseVisualStyleBackColor = true;
            this.btnDownloadAll.Click += new System.EventHandler(this.btnDownloadAll_Click);
            // 
            // btnDownloadProduct
            // 
            this.btnDownloadProduct.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnDownloadProduct.Location = new System.Drawing.Point(362, 12);
            this.btnDownloadProduct.Name = "btnDownloadProduct";
            this.btnDownloadProduct.Size = new System.Drawing.Size(120, 31);
            this.btnDownloadProduct.TabIndex = 2;
            this.btnDownloadProduct.Text = "下载选中数据";
            this.btnDownloadProduct.UseVisualStyleBackColor = true;
            this.btnDownloadProduct.Click += new System.EventHandler(this.btnDownloadProduct_Click);
            // 
            // chkList
            // 
            this.chkList.FormattingEnabled = true;
            this.chkList.Location = new System.Drawing.Point(37, 12);
            this.chkList.Name = "chkList";
            this.chkList.Size = new System.Drawing.Size(206, 100);
            this.chkList.TabIndex = 4;
            // 
            // chkAll
            // 
            this.chkAll.AutoSize = true;
            this.chkAll.Location = new System.Drawing.Point(268, 22);
            this.chkAll.Name = "chkAll";
            this.chkAll.Size = new System.Drawing.Size(48, 16);
            this.chkAll.TabIndex = 5;
            this.chkAll.Text = "全选";
            this.chkAll.UseVisualStyleBackColor = true;
            this.chkAll.CheckedChanged += new System.EventHandler(this.chkAll_CheckedChanged);
            // 
            // frmDownload
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(503, 235);
            this.Controls.Add(this.chkAll);
            this.Controls.Add(this.chkList);
            this.Controls.Add(this.btnDownloadProduct);
            this.Controls.Add(this.btnDownloadAll);
            this.Controls.Add(this.lblMsg);
            this.Controls.Add(this.progressBar1);
            this.Name = "frmDownload";
            this.Text = "下载数据";
            this.Load += new System.EventHandler(this.frmDownload_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lblMsg;
        private System.Windows.Forms.Button btnDownloadAll;
        private System.Windows.Forms.Button btnDownloadProduct;
        private System.Windows.Forms.CheckedListBox chkList;
        private System.Windows.Forms.CheckBox chkAll;
    }
}