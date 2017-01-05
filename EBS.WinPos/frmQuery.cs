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
        VipProductService _vipProductService;
        private static frmQuery _instance;
        public static frmQuery CreateForm()
        {
            //判断是否存在该窗体,或时候该字窗体是否被释放过,如果不存在该窗体,则 new 一个字窗体  
            if (_instance == null || _instance.IsDisposed)
            {
                _instance = new frmQuery();
            }
            return _instance;
        }
        public frmQuery()
        {
            InitializeComponent();

            _productService = new ProductService();
            _vipService = new VipCardService();
            _vipProductService = new VipProductService();
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
            var realPrice = model.SalePrice; 
            if (vipCustomer != null)
            {
                var vipProduct = _vipProductService.GetByProductId(model.Id);
                //真正的销售价                              
                realPrice = vipProduct == null ? model.SalePrice * discount : vipProduct.SalePrice;
              
            }
           
            
            this.dgvData.Rows.Clear();
            int index = this.dgvData.Rows.Add();
            var row = this.dgvData.Rows[index];
            row.Cells["ProductId"].Value = model.Id;
            row.Cells["ProductCode"].Value = model.Code;
            row.Cells["BarCode"].Value = model.BarCode;
            row.Cells["ProductName"].Value = model.Name;
            row.Cells["Specification"].Value = model.Specification;
            row.Cells["SalePrice"].Value = model.SalePrice.ToString("F2");
            row.Cells["RealPrice"].Value = realPrice.ToString("F2");
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
