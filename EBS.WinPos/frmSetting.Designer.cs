namespace EBS.WinPos
{
    partial class frmSetting
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSavePosId = new System.Windows.Forms.Button();
            this.btnSaveStoreID = new System.Windows.Forms.Button();
            this.txtPosId = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbbStores = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnDownload = new System.Windows.Forms.Button();
            this.lstInfo = new System.Windows.Forms.ListBox();
            this.btnUpdateProduct = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSavePosId);
            this.groupBox1.Controls.Add(this.btnSaveStoreID);
            this.groupBox1.Controls.Add(this.txtPosId);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cbbStores);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(567, 147);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "门店设置";
            // 
            // btnSavePosId
            // 
            this.btnSavePosId.Location = new System.Drawing.Point(357, 84);
            this.btnSavePosId.Name = "btnSavePosId";
            this.btnSavePosId.Size = new System.Drawing.Size(75, 35);
            this.btnSavePosId.TabIndex = 3;
            this.btnSavePosId.Text = "保存";
            this.btnSavePosId.UseVisualStyleBackColor = true;
            this.btnSavePosId.Click += new System.EventHandler(this.btnSavePosId_Click);
            // 
            // btnSaveStoreID
            // 
            this.btnSaveStoreID.Location = new System.Drawing.Point(357, 41);
            this.btnSaveStoreID.Name = "btnSaveStoreID";
            this.btnSaveStoreID.Size = new System.Drawing.Size(75, 35);
            this.btnSaveStoreID.TabIndex = 3;
            this.btnSaveStoreID.Text = "保存";
            this.btnSaveStoreID.UseVisualStyleBackColor = true;
            this.btnSaveStoreID.Click += new System.EventHandler(this.btnSaveStoreID_Click);
            // 
            // txtPosId
            // 
            this.txtPosId.Location = new System.Drawing.Point(138, 88);
            this.txtPosId.Name = "txtPosId";
            this.txtPosId.Size = new System.Drawing.Size(172, 29);
            this.txtPosId.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 21);
            this.label2.TabIndex = 1;
            this.label2.Text = "pos机编号：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(74, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 21);
            this.label1.TabIndex = 1;
            this.label1.Text = "门店：";
            // 
            // cbbStores
            // 
            this.cbbStores.FormattingEnabled = true;
            this.cbbStores.Location = new System.Drawing.Point(138, 40);
            this.cbbStores.Name = "cbbStores";
            this.cbbStores.Size = new System.Drawing.Size(172, 29);
            this.cbbStores.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 147);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(567, 100);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "功能测试";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(35, 42);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(128, 37);
            this.button1.TabIndex = 0;
            this.button1.Text = "测试打印小票";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnDownload
            // 
            this.btnDownload.Location = new System.Drawing.Point(35, 266);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(128, 39);
            this.btnDownload.TabIndex = 2;
            this.btnDownload.Text = "下载数据";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // lstInfo
            // 
            this.lstInfo.FormattingEnabled = true;
            this.lstInfo.ItemHeight = 21;
            this.lstInfo.Location = new System.Drawing.Point(190, 253);
            this.lstInfo.Name = "lstInfo";
            this.lstInfo.Size = new System.Drawing.Size(365, 151);
            this.lstInfo.TabIndex = 3;
            // 
            // btnUpdateProduct
            // 
            this.btnUpdateProduct.Location = new System.Drawing.Point(35, 311);
            this.btnUpdateProduct.Name = "btnUpdateProduct";
            this.btnUpdateProduct.Size = new System.Drawing.Size(128, 39);
            this.btnUpdateProduct.TabIndex = 2;
            this.btnUpdateProduct.Text = "下载商品";
            this.btnUpdateProduct.UseVisualStyleBackColor = true;
            this.btnUpdateProduct.Click += new System.EventHandler(this.btnUpdateProduct_Click);
            // 
            // frmSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(567, 421);
            this.Controls.Add(this.lstInfo);
            this.Controls.Add(this.btnUpdateProduct);
            this.Controls.Add(this.btnDownload);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "frmSetting";
            this.Text = "系统设置";
            this.Load += new System.EventHandler(this.frmSetting_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtPosId;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbbStores;
        private System.Windows.Forms.Button btnSavePosId;
        private System.Windows.Forms.Button btnSaveStoreID;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.ListBox lstInfo;
        private System.Windows.Forms.Button btnUpdateProduct;
    }
}