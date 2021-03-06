﻿namespace EBS.WinPos
{
    partial class frmMy
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
            this.plTop = new System.Windows.Forms.Panel();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnQuery = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.plBody = new System.Windows.Forms.Panel();
            this.dgvData = new System.Windows.Forms.DataGridView();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CreatedByName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StartDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.EndDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CashAmount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.plBottom = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCashAmount = new System.Windows.Forms.TextBox();
            this.plTop.SuspendLayout();
            this.plBody.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            this.plBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // plTop
            // 
            this.plTop.Controls.Add(this.btnBack);
            this.plTop.Controls.Add(this.btnPrint);
            this.plTop.Controls.Add(this.btnQuery);
            this.plTop.Controls.Add(this.label2);
            this.plTop.Controls.Add(this.dtpDate);
            this.plTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.plTop.Location = new System.Drawing.Point(0, 0);
            this.plTop.Name = "plTop";
            this.plTop.Size = new System.Drawing.Size(812, 100);
            this.plTop.TabIndex = 0;
            // 
            // btnBack
            // 
            this.btnBack.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.btnBack.Location = new System.Drawing.Point(633, 22);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(90, 41);
            this.btnBack.TabIndex = 2;
            this.btnBack.Text = "返 回";
            this.btnBack.UseVisualStyleBackColor = true;
            this.btnBack.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.btnPrint.Location = new System.Drawing.Point(482, 23);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(141, 41);
            this.btnPrint.TabIndex = 2;
            this.btnPrint.Text = "打印收银汇总";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // btnQuery
            // 
            this.btnQuery.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.btnQuery.Location = new System.Drawing.Point(381, 23);
            this.btnQuery.Name = "btnQuery";
            this.btnQuery.Size = new System.Drawing.Size(91, 41);
            this.btnQuery.TabIndex = 2;
            this.btnQuery.Text = "查 询";
            this.btnQuery.UseVisualStyleBackColor = true;
            this.btnQuery.Click += new System.EventHandler(this.btnQuery_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(33, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(112, 27);
            this.label2.TabIndex = 1;
            this.label2.Text = "上班日期：";
            // 
            // dtpDate
            // 
            this.dtpDate.Location = new System.Drawing.Point(151, 23);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(209, 34);
            this.dtpDate.TabIndex = 0;
            // 
            // plBody
            // 
            this.plBody.Controls.Add(this.dgvData);
            this.plBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plBody.Location = new System.Drawing.Point(0, 100);
            this.plBody.Name = "plBody";
            this.plBody.Size = new System.Drawing.Size(812, 416);
            this.plBody.TabIndex = 0;
            // 
            // dgvData
            // 
            this.dgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvData.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.CreatedByName,
            this.StartDate,
            this.EndDate,
            this.CashAmount});
            this.dgvData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvData.Location = new System.Drawing.Point(0, 0);
            this.dgvData.MultiSelect = false;
            this.dgvData.Name = "dgvData";
            this.dgvData.RowHeadersWidth = 51;
            this.dgvData.RowTemplate.Height = 23;
            this.dgvData.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvData.Size = new System.Drawing.Size(812, 416);
            this.dgvData.TabIndex = 0;
            this.dgvData.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvData_CellClick);
            this.dgvData.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.dgvData_RowPostPaint);
            // 
            // Id
            // 
            this.Id.DataPropertyName = "Id";
            this.Id.HeaderText = "编号";
            this.Id.MinimumWidth = 6;
            this.Id.Name = "Id";
            this.Id.ReadOnly = true;
            this.Id.Visible = false;
            this.Id.Width = 125;
            // 
            // CreatedByName
            // 
            this.CreatedByName.DataPropertyName = "CreatedByName";
            this.CreatedByName.HeaderText = "员工";
            this.CreatedByName.MinimumWidth = 6;
            this.CreatedByName.Name = "CreatedByName";
            this.CreatedByName.ReadOnly = true;
            this.CreatedByName.Width = 125;
            // 
            // StartDate
            // 
            this.StartDate.DataPropertyName = "StartDate";
            this.StartDate.HeaderText = "上班时间";
            this.StartDate.MinimumWidth = 6;
            this.StartDate.Name = "StartDate";
            this.StartDate.ReadOnly = true;
            this.StartDate.Width = 200;
            // 
            // EndDate
            // 
            this.EndDate.DataPropertyName = "EndDate";
            this.EndDate.HeaderText = "下班时间";
            this.EndDate.MinimumWidth = 6;
            this.EndDate.Name = "EndDate";
            this.EndDate.ReadOnly = true;
            this.EndDate.Width = 200;
            // 
            // CashAmount
            // 
            this.CashAmount.DataPropertyName = "CashAmount";
            this.CashAmount.HeaderText = "收现金额";
            this.CashAmount.MinimumWidth = 6;
            this.CashAmount.Name = "CashAmount";
            this.CashAmount.ReadOnly = true;
            this.CashAmount.Width = 125;
            // 
            // plBottom
            // 
            this.plBottom.Controls.Add(this.btnSave);
            this.plBottom.Controls.Add(this.label1);
            this.plBottom.Controls.Add(this.txtCashAmount);
            this.plBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.plBottom.Location = new System.Drawing.Point(0, 416);
            this.plBottom.Name = "plBottom";
            this.plBottom.Size = new System.Drawing.Size(812, 100);
            this.plBottom.TabIndex = 0;
            // 
            // btnSave
            // 
            this.btnSave.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.btnSave.Location = new System.Drawing.Point(449, 25);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(90, 40);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "保 存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 27);
            this.label1.TabIndex = 1;
            this.label1.Text = "收现金额：";
            // 
            // txtCashAmount
            // 
            this.txtCashAmount.Font = new System.Drawing.Font("微软雅黑", 14F);
            this.txtCashAmount.Location = new System.Drawing.Point(151, 25);
            this.txtCashAmount.Name = "txtCashAmount";
            this.txtCashAmount.Size = new System.Drawing.Size(266, 38);
            this.txtCashAmount.TabIndex = 0;
            this.txtCashAmount.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtCashAmount_KeyDown);
            // 
            // frmMy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 27F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(812, 516);
            this.Controls.Add(this.plBottom);
            this.Controls.Add(this.plBody);
            this.Controls.Add(this.plTop);
            this.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "frmMy";
            this.Text = "我的班次";
            this.Load += new System.EventHandler(this.frmMy_Load);
            this.plTop.ResumeLayout(false);
            this.plTop.PerformLayout();
            this.plBody.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            this.plBottom.ResumeLayout(false);
            this.plBottom.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel plTop;
        private System.Windows.Forms.Panel plBody;
        private System.Windows.Forms.DataGridView dgvData;
        private System.Windows.Forms.Panel plBottom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCashAmount;
        private System.Windows.Forms.Button btnQuery;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewTextBoxColumn CreatedByName;
        private System.Windows.Forms.DataGridViewTextBoxColumn StartDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn EndDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn CashAmount;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnPrint;
    }
}