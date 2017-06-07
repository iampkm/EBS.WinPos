namespace EBS.WinPos.Tool
{
    partial class frmRepair
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.btnUpdateSummary = new System.Windows.Forms.Button();
            this.btnUpOrder = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.btnCheck = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtServiceDB = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtSqlite = new System.Windows.Forms.TextBox();
            this.lblMsg = new System.Windows.Forms.Label();
            this.btnUpdate = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dtpDate
            // 
            this.dtpDate.Location = new System.Drawing.Point(47, 95);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(154, 21);
            this.dtpDate.TabIndex = 17;
            // 
            // btnUpdateSummary
            // 
            this.btnUpdateSummary.Location = new System.Drawing.Point(426, 92);
            this.btnUpdateSummary.Name = "btnUpdateSummary";
            this.btnUpdateSummary.Size = new System.Drawing.Size(137, 23);
            this.btnUpdateSummary.TabIndex = 16;
            this.btnUpdateSummary.Text = "上传对账汇总数据";
            this.btnUpdateSummary.UseVisualStyleBackColor = true;
            this.btnUpdateSummary.Click += new System.EventHandler(this.btnUpdateSummary_Click);
            // 
            // btnUpOrder
            // 
            this.btnUpOrder.Location = new System.Drawing.Point(321, 93);
            this.btnUpOrder.Name = "btnUpOrder";
            this.btnUpOrder.Size = new System.Drawing.Size(75, 23);
            this.btnUpOrder.TabIndex = 15;
            this.btnUpOrder.Text = "上传订单";
            this.btnUpOrder.UseVisualStyleBackColor = true;
            this.btnUpOrder.Click += new System.EventHandler(this.btnUpOrder_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(47, 144);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(609, 288);
            this.dataGridView1.TabIndex = 14;
            // 
            // btnCheck
            // 
            this.btnCheck.Location = new System.Drawing.Point(220, 93);
            this.btnCheck.Name = "btnCheck";
            this.btnCheck.Size = new System.Drawing.Size(75, 23);
            this.btnCheck.TabIndex = 13;
            this.btnCheck.Text = "检查差异单据";
            this.btnCheck.UseVisualStyleBackColor = true;
            this.btnCheck.Click += new System.EventHandler(this.btnCheck_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(45, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 12;
            this.label2.Text = "服务器连接：";
            // 
            // txtServiceDB
            // 
            this.txtServiceDB.Location = new System.Drawing.Point(128, 54);
            this.txtServiceDB.Name = "txtServiceDB";
            this.txtServiceDB.Size = new System.Drawing.Size(485, 21);
            this.txtServiceDB.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(45, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 10;
            this.label1.Text = "Sqlite文件：";
            // 
            // txtSqlite
            // 
            this.txtSqlite.Location = new System.Drawing.Point(128, 18);
            this.txtSqlite.Name = "txtSqlite";
            this.txtSqlite.Size = new System.Drawing.Size(485, 21);
            this.txtSqlite.TabIndex = 9;
            // 
            // lblMsg
            // 
            this.lblMsg.AutoSize = true;
            this.lblMsg.Location = new System.Drawing.Point(49, 126);
            this.lblMsg.Name = "lblMsg";
            this.lblMsg.Size = new System.Drawing.Size(0, 12);
            this.lblMsg.TabIndex = 18;
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(590, 92);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(84, 23);
            this.btnUpdate.TabIndex = 19;
            this.btnUpdate.Text = "修改订单";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // frmRepair
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(814, 451);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.lblMsg);
            this.Controls.Add(this.dtpDate);
            this.Controls.Add(this.btnUpdateSummary);
            this.Controls.Add(this.btnUpOrder);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.btnCheck);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtServiceDB);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtSqlite);
            this.Name = "frmRepair";
            this.Text = "订单数据修复工具";
            this.Load += new System.EventHandler(this.frmRepair_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.Button btnUpdateSummary;
        private System.Windows.Forms.Button btnUpOrder;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnCheck;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtServiceDB;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSqlite;
        private System.Windows.Forms.Label lblMsg;
        private System.Windows.Forms.Button btnUpdate;
    }
}

