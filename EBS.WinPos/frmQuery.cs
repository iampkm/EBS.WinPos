using EBS.WinPos.Domain.Entity;
using EBS.WinPos.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EBS.WinPos
{
    public partial class frmQuery : Form
    {
        ProductService _productService;
        VipCardService _vipService;
        public frmQuery()
        {
            InitializeComponent();

            _productService = new ProductService();
            _vipService = new VipCardService();
        }

        public void Query()
        {
            string productCodeOrBarCode = this.txtBarCode.Text;
            Product model = _productService.GetProduct(productCodeOrBarCode);
            if (model == null)
            {
                MessageBox.Show("商品不存在");
                this.txtBarCode.Text = "";
                return;
            }
            

            //查询会员折扣
            var vipCustomer  = _vipService.GetByCode(this.txtVipCode.Text);
            var discount = vipCustomer == null ? 1 : vipCustomer.Discount;

            //int lastIndex = this.dgvData.Rows.GetLastRow(DataGridViewElementStates.Selected);
            //if (lastIndex > -1)
            //{
            //    //前一行商品如果与扫码的商品一样，就直接累加数量
            //    var preRow = this.dgvData.Rows[lastIndex];
            //    if (preRow.Cells["ProductId"].Value != null && (int)preRow.Cells["ProductId"].Value == model.Id)
            //    {
            //        preRow.Cells["Quantity"].Value = (int)preRow.Cells["Quantity"].Value + 1;
            //        preRow.Selected = true;
            //        this.txtBarCode.Text = "";
            //        ShowOrderInfo();
            //        return;
            //    }
            //}

            int index = this.dgvData.Rows.Add();
            var row = this.dgvData.Rows[index];
            row.Cells["ProductId"].Value = model.Id;
            row.Cells["ProductCode"].Value = model.Code;
            row.Cells["BarCode"].Value = model.BarCode;
            row.Cells["ProductName"].Value = model.Name;
            row.Cells["Specification"].Value = model.Specification;
            row.Cells["SalePrice"].Value = model.SalePrice;
            row.Cells["RealPrice"].Value = model.SalePrice * discount;
            row.Cells["Quantity"].Value = 1;
            row.Cells["Amount"].Value = model.SalePrice * discount * 1;
            row.Selected = true;
            this.txtBarCode.Text = "";
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            Query();
        }

        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Query();
            }
        }

        private void txtVipCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Query();
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
