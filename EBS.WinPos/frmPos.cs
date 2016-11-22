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
using EBS.Infrastructure;
namespace EBS.WinPos
{
    public partial class frmPos : Form
    {


        SaleOrderService _saleOrderService;
        ProductService _productService;
        VipCardService _vipService;
        VipProductService _vipProductService;
        ShopCart _currentShopCat;

        public int OrderId { get; set; }

        public VipCard VipCustomer { get; set; }

        public frmPos()
        {
            InitializeComponent();

            _saleOrderService = new SaleOrderService();
            _productService = new ProductService();
            _vipService = new VipCardService();
            _vipProductService = new VipProductService();
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
            if (model == null) { 
                MessageBox.Show("商品不存在"); 
                this.txtBarCode.Text = ""; 
                return;
            }

            //查询会员折扣
            var discount = this.VipCustomer == null ? 1 : this.VipCustomer.Discount;
            var vipProduct=_vipProductService.GetByProductId(model.Id);
            //真正的销售价
            var realPrice = model.SalePrice;
            if(this.VipCustomer!=null)
            {
                realPrice = vipProduct == null ? model.SalePrice * discount : vipProduct.SalePrice;
            }           
            int lastIndex = this.dgvData.Rows.GetLastRow(DataGridViewElementStates.Selected);
            if (lastIndex > -1)
            {
                //前一行商品如果与扫码的商品一样，就直接累加数量
                var preRow = this.dgvData.Rows[lastIndex];
                if (preRow.Cells["ProductId"].Value != null && Convert.ToInt32(preRow.Cells["ProductId"].Value) == model.Id)
                {
                    preRow.Cells["Quantity"].Value = Convert.ToInt32(preRow.Cells["Quantity"].Value) + 1;
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
            row.Cells["RealPrice"].Value = realPrice;
            row.Cells["Quantity"].Value = 1;
            row.Cells["Amount"].Value = realPrice * 1;
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
                    var price = (decimal)row.Cells["RealPrice"].Value;
                    var quantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                    total += price * quantity;
                    quantityTotal += quantity;
                }
            }
            this.lblOrderTotal.Text ="￥" + total.ToString();
            this.lblQuantityTotal.Text ="数量：" + quantityTotal.ToString();
        }

        public void ShowPreOrderInfo(OrderInfo model)
        {
            this.lblPreOrderCode.Text ="上一单：" + model.OrderCode;
            this.lblPreOrderAmount.Text = "金额：￥" + model.OrderAmount.ToString();
            this.lblPreChargeAmount.Text = "找零：￥" + model.ChargeAmount.ToString();
        }

        public void CreateOrder(string money)
        {
            try
            {
                var inputAmount = 0m;
                decimal.TryParse(money, out inputAmount);
                var newOrder = SaveOrder(inputAmount);
                this.OrderId = newOrder.OrderId;  //保存新订单Id
                this.lblOrderCode.Text = newOrder.OrderCode;
                newOrder.PayAmount = inputAmount;
                this.txtBarCode.Text = "";

                // 显示支付窗体  
                if (_currentShopCat.CheckCanPay())
                {
                    _payForm = new frmPay();
                    _payForm.CurrentOrder = newOrder;
                    _payForm.PosForm = this;
                    _payForm.ShowDialog(this);
                }
                else {
                    MessageBox.Show("有商品项数量为0,不能支付！请调整数量或ESC作废订单。");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
          

        }

        public OrderInfo SaveOrder(decimal inputAmount)
        {
            //1 创建订单，并显示支付窗口
            if (this.dgvData.Rows.Count == 0)
            {
                return null;
            }
            //查询会员折扣
            var discount = this.VipCustomer == null ? 1 : this.VipCustomer.Discount;

            _currentShopCat = new ShopCart()
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
                    product.Id = Convert.ToInt32(row.Cells["ProductId"].Value);
                    product.Name = row.Cells["ProductName"].Value.ToString();
                    product.SalePrice = Convert.ToDecimal(row.Cells["SalePrice"].Value);
                    product.Code = row.Cells["ProductCode"].Value.ToString();
                    int quantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                    _currentShopCat.Items.Add(new ShopCartItem(product, quantity, discount));
                }
            }
            var newOrder = _saleOrderService.CreateOrder(_currentShopCat);
            return newOrder;
        }
      
       



        public void ClearAll()
        {
            this.dgvData.Rows.Clear();
            this.lblOrderTotal.Text = "￥0";
            this.lblQuantityTotal.Text = "数量：0";
            this.lblOrderCode.Text = "单号：";
            this.lblDiscount.Text = "折扣：";
            this.VipCustomer = null;
            this._currentShopCat = null;
            this.OrderId = 0;
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
                case Keys.F1:  // +
                    ModifyQuantity();
                    break;
                case Keys.F2:  // - 
                   // MinusQuantity();
                    inputCustomer();
                    break;
               
                default:
                    break;
            }
        }

        /// <summary>
        /// 按ESC 取消订单
        /// </summary>
        public void Cancel()
        {
            try
            {
                if (this.OrderId == 0)
                {
                    SaveOrder(0);
                }
                _saleOrderService.CancelOrder(this.OrderId, ContextService.CurrentAccount.Id);
                MessageBox.Show("作废成功!");
                this.ClearAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
          
            // 打印作废小票
        }

        public void ModifyQuantity()
        {
            int lastIndex = this.dgvData.Rows.GetLastRow(DataGridViewElementStates.Selected);
            if (lastIndex < 0) {
                throw new AppException("请选择要修改数量的商品");
            }
            //设置焦点
            this.dgvData.Focus();
            this.dgvData.CurrentCell = this.dgvData.Rows[lastIndex].Cells["Quantity"];
            this.dgvData.BeginEdit(true);

        }

       

        public void inputCustomer()
        {
            frmCustomerId vipForm = new frmCustomerId();
            vipForm.PosFrom = this;
            vipForm.ShowDialog(this);
        }

        public void SetVipCard(string code)
        {
            this.ClearAll();
            this.VipCustomer = _vipService.GetByCode(code);
            this.lblDiscount.Text ="折扣："+ this.VipCustomer == null ? "" : this.VipCustomer.Discount.ToString();  
            this.txtBarCode.Focus();
        }

        private void frmPos_Load(object sender, EventArgs e)
        {
            // 如果没有点上班，不显示该界面
            this.lblAccountId.Text = "工号：" + ContextService.CurrentAccount.Id;
            this.lblStoreId.Text = "门店：" + ContextService.CurrentAccount.StoreId;
            // this.lblStoreId.Text = "门店："
            lblKeys.Text = "快捷键：F1 改数量,F2 会员,ESC 作废订单 ";
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

        private void TextBoxDec_KeyPress(object sender, KeyPressEventArgs e)
        {
           
           if (!Char.IsDigit(e.KeyChar) && e.KeyChar != 8) //&& e.KeyChar != '.' e.KeyChar != 8 &&
            {
                e.Handled = true;
            }            
          
        }

        private void dgvData_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (this.dgvData.CurrentCell.ColumnIndex == 7)  //数量列
            {
                e.Control.KeyPress += new KeyPressEventHandler(TextBoxDec_KeyPress);

                //DataGridViewTextBoxColumn
                if (e.Control.GetType().Name == "DataGridViewTextBoxEditingControl")
                {
                    var qtyBox = e.Control as DataGridViewTextBoxEditingControl;
                    //添加事件
                    qtyBox.TextChanged += qtyBox_TextChanged;
                }
            }
        }

        void qtyBox_TextChanged(object sender, EventArgs e)
        {
            int column = dgvData.CurrentCellAddress.X;
            int row = dgvData.CurrentCellAddress.Y;
            var qtyBox = (DataGridViewTextBoxEditingControl)sender;
            this.dgvData[column, row].Value = qtyBox.Text;
            ShowOrderInfo();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                //this.OnKeyPress(new KeyPressEventArgs('r'));
                //return true;
                txtBarCode.Focus();
            }
           
             return base.ProcessCmdKey(ref msg, keyData);
        }

    }
}
