using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EBS.WinPos.Domain.Entity;
using EBS.WinPos.Domain;
using EBS.WinPos.Service;
using EBS.WinPos.Service.Dto;
namespace EBS.WinPos
{
    public partial class frmPos : Form
    {


        SaleOrderService _saleOrderService;
        ProductService _productService;
        VipCardService _vipService;

        public string CustomerId { get; set; }

        public VipCard VipCustomer { get; set; }

        public frmPos()
        {
            InitializeComponent();

            _saleOrderService = new SaleOrderService();
            _productService = new ProductService();
            _vipService = new VipCardService();
        }

        frmPay _payForm;
        public void inputEnter()
        {
            var input = this.txtBarCode.Text;
            //商品编码固定8位，少于8位，即认为是输入的金额
            if (input.Length <= 7)
            {
                CreateOrder(input);
            }
            else
            {
                InputBarCode(input);
            }
        }

        public void InputBarCode(string productCodeOrBarCode)
        {
            Product model = _productService.GetProduct(productCodeOrBarCode);
            if (model == null) { MessageBox.Show("商品不存在"); return; }
            //查询会员折扣
            var discount = this.VipCustomer == null ? 1 : this.VipCustomer.Discount;

            int lastIndex = this.dgvData.Rows.GetLastRow(DataGridViewElementStates.Selected);
            if (lastIndex > -1)
            {
                //前一行商品如果与扫码的商品一样，就直接累加数量
                var preRow = this.dgvData.Rows[lastIndex];
                if (preRow.Cells["ProductId"].Value != null && (int)preRow.Cells["ProductId"].Value == model.Id)
                {
                    preRow.Cells["Quantity"].Value = (int)preRow.Cells["Quantity"].Value + 1;
                    preRow.Selected = true;
                    this.txtBarCode.Text = "";
                    ShowOrderInfo();
                    return;
                }
            }

            int index = this.dgvData.Rows.Add();
            var row = this.dgvData.Rows[index];
            row.Cells["ProductId"].Value = model.Id;
            row.Cells["ProductCode"].Value = model.Code;
            row.Cells["BarCode"].Value = model.BarCode;
            row.Cells["ProductName"].Value = model.Name;
            row.Cells["Specification"].Value = model.Specification;
            row.Cells["SalePrice"].Value = model.SalePrice;
            row.Cells["Quantity"].Value = 1;
            row.Cells["Amount"].Value = model.SalePrice * 1;
            row.Selected = true;
            this.txtBarCode.Text = "";
            ShowOrderInfo();
        }

        public void ShowOrderInfo()
        {
            var total = 0m;
            var quantityTotal = 0;
            foreach (DataGridViewRow row in this.dgvData.Rows)
            {
                if (row.Cells["ProductId"].Value != null)
                {
                    var price = (decimal)row.Cells["SalePrice"].Value;
                    var quantity = (int)row.Cells["Quantity"].Value;
                    total += price * quantity;
                    quantityTotal += quantity;
                }
            }
            this.lblOrderTotal.Text = total.ToString();
            this.lblQuantityTotal.Text = quantityTotal.ToString();
        }

        public void CreateOrder(string money)
        {
            //1 创建订单，并显示支付窗口
            if (this.dgvData.Rows.Count == 0)
            {
                return;
            }
            //查询会员折扣
            var discount = this.VipCustomer == null ? 1 : this.VipCustomer.Discount;
            var inputAmount = 0m;
            decimal.TryParse(money, out inputAmount);

            ShopCart cat = new ShopCart()
            {
                StoreId = ContextService.CurrentAccount.StoreId,
                Editor = ContextService.CurrentAccount.Id,
                PayAmount = inputAmount
            };
            // 明细
            foreach (DataGridViewRow row in this.dgvData.Rows)
            {
                Product product = new Product();
                if (row.Cells["ProductId"].Value != null)
                {
                    product.Id = (int)row.Cells["ProductId"].Value;
                    product.Name = (string)row.Cells["ProductName"].Value.ToString();
                    product.SalePrice = (decimal)row.Cells["SalePrice"].Value;
                    product.Code = row.Cells["ProductCode"].Value.ToString();
                    int quantity = (int)row.Cells["Quantity"].Value;
                    cat.Items.Add(new ShopCartItem(product, quantity, discount));
                }
            }
            var newOrder = _saleOrderService.CreateOrder(cat);
            this.lblOrderCode.Text = newOrder.OrderCode;
            newOrder.PayAmount = inputAmount;
            this.txtBarCode.Text = "";
            // 模拟
            //var newOrder = new OrderInfo()
            //{
            //    OrderId = 1,
            //    OrderAmount = 89.3m,
            //    OrderCode = "1500121356",
            //    PayAmount = inputAmount
            //};

            // 显示支付窗体          
            _payForm = new frmPay();
            _payForm.CurrentOrder = newOrder;
            _payForm.PosForm = this;
            _payForm.ShowDialog(this);

        }
        /// <summary>
        /// 按ESC 取消订单
        /// </summary>
        public void Cancel()
        {
            this.ClearAll();
        }
        /// <summary>
        /// 减数量
        /// </summary>
        public void DeleteQuantity()
        {
            int lastIndex = this.dgvData.Rows.GetLastRow(DataGridViewElementStates.Selected);
            if (lastIndex > -1)
            {
                //前一行商品如果与扫码的商品一样，就直接累加数量
                var preRow = this.dgvData.Rows[lastIndex];
                if (preRow.Cells["ProductId"].Value != null)
                {
                    preRow.Cells["Quantity"].Value = (int)preRow.Cells["Quantity"].Value - 1;
                    preRow.Selected = true;

                    if ((int)preRow.Cells["Quantity"].Value == 0)
                    {
                        this.dgvData.Rows.RemoveAt(lastIndex);
                        //前一行选中
                        if (this.dgvData.Rows[lastIndex - 1] != null)
                        {
                            this.dgvData.Rows[lastIndex - 1].Selected = true;
                        }
                    }
                }
            }
            // 如果数量为 0，删除该行
            ShowOrderInfo();
        }



        public void ClearAll()
        {
            this.dgvData.Rows.Clear();
            this.lblOrderTotal.Text = "";
            this.lblQuantityTotal.Text = "";
            this.lblOrderCode.Text = "";
            this.VipCustomer = null;
        }

        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    inputEnter();
                    break;
                case Keys.Escape:
                    Cancel();
                    break;
                case Keys.Delete:
                    DeleteQuantity();
                    break;
                case Keys.F1:
                    break;
                case Keys.F2:
                    MinusQuantity();
                    break;
                case Keys.F3:
                    inputCustomer();
                    break;
                default:
                    break;
            }
        }

        public void MinusQuantity()
        {
            DeleteQuantity();
        }

        public void PlusQuantity()
        {
            int lastIndex = this.dgvData.Rows.GetLastRow(DataGridViewElementStates.Selected);
            if (lastIndex > -1)
            {
                //前一行商品如果与扫码的商品一样，就直接累加数量
                var preRow = this.dgvData.Rows[lastIndex];
                if (preRow.Cells["ProductId"].Value != null)
                {
                    preRow.Cells["Quantity"].Value = (int)preRow.Cells["Quantity"].Value + 1;
                    preRow.Selected = true;
                }
            }
            // 如果数量为 0，删除该行
            ShowOrderInfo();
        }

        public void inputCustomer()
        {
            frmCustomerId vipForm = new frmCustomerId();
            vipForm.PosFrom = this;
            vipForm.ShowDialog(this);
        }

        public void SetVipCard(string code)
        {
            this.VipCustomer = _vipService.GetByCode(code);
            this.lblDiscount.Text = this.VipCustomer == null ? "" : this.VipCustomer.Discount.ToString();
            this.ClearAll();
            this.txtBarCode.Focus();
        }

        private void frmPos_Load(object sender, EventArgs e)
        {
            // 如果没有点上班，不显示该界面
            this.lblAccountId.Text = "工号：" + ContextService.CurrentAccount.Id;
            this.lblStoreId.Text = "门店：" + ContextService.CurrentAccount.StoreId;
            // this.lblStoreId.Text = "门店："
            this.txtBarCode.Focus();

        }



        private void dgvData_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(e.RowBounds.Location.X,
           e.RowBounds.Location.Y,
           dgvData.RowHeadersWidth - 4,
           e.RowBounds.Height);

            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                dgvData.RowHeadersDefaultCellStyle.Font,
                rectangle,
                Color.Black,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
        }

        private void frmPos_Resize(object sender, EventArgs e)
        {
            // this.plBarCode.Width = this.Width / 2;
        }

        //dgvData.RowHeadersDefaultCellStyle.ForeColor


    }
}
