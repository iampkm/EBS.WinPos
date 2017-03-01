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
        ShopCart _preShopCat;
        WorkScheduleService _workScheduleService;
        WorkSchedule _currentWork;
        public VipCard VipCustomer { get; set; }
        private static frmPos _instance;
        public static frmPos CreateForm()
        {
            //判断是否存在该窗体,或时候该字窗体是否被释放过,如果不存在该窗体,则 new 一个字窗体  
            if (_instance == null || _instance.IsDisposed)
            {
                _instance = new frmPos();
            }
            return _instance;
        }
        public frmPos()
        {
            InitializeComponent();

            _saleOrderService = new SaleOrderService();
            _productService = new ProductService();
            _vipService = new VipCardService();
            _vipProductService = new VipProductService();
            _workScheduleService = new WorkScheduleService();
            // 添加班次信息
            _currentWork = _workScheduleService.GetWorking(ContextService.StoreId, ContextService.PosId);
           

        }

       
        public void inputEnter()
        {
            var input = this.txtBarCode.Text;
            //商品编码固定6位，少于6位，即认为是输入的金额
            this._currentShopCat = this._currentShopCat ?? new ShopCart(ContextService.StoreId, ContextService.PosId, ContextService.CurrentAccount.Id);
            if (input.Length <6)
            {
                this._currentShopCat.WorkScheduleCode = _currentWork.Code;
                if (this._currentShopCat.OrderAmount > 0)
                {
                    CreateSaleOrder(input);
                }
                else if (this._currentShopCat.OrderAmount < 0)
                {
                    CreateSaleRefund(input);
                }
                else
                {
                    MessageBox.Show("请输入正确的商品条码或金额", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information); return;
                }
            }
            else
            {
                InputBarCode(input);
            }
        }

        public void InputBarCode(string productCodeOrBarCode)
        {
            if (string.IsNullOrEmpty(productCodeOrBarCode)) { return; }
            productCodeOrBarCode = productCodeOrBarCode.Trim();
            Product model = _productService.GetProduct(productCodeOrBarCode);
            if (model == null)
            {
                MessageBox.Show("商品不存在");
                this.txtBarCode.Text = "";
                return;
            }
            this._currentShopCat = this._currentShopCat ?? new ShopCart(ContextService.StoreId, ContextService.PosId, ContextService.CurrentAccount.Id);
            //查询会员折扣
           // var discount = this.VipCustomer == null ? 1 : this.VipCustomer.Discount;          
            //真正的销售价
            var realPrice = model.SalePrice;
            if (this.VipCustomer != null)
            {
                var vipProduct = _vipProductService.GetByProductId(model.Id);
                realPrice = vipProduct == null ? model.SalePrice : vipProduct.SalePrice;
            }           
            //加入购物车
            this._currentShopCat.AddShopCart(new ShopCartItem(model, 1, realPrice));           
            ShowOrderInfo();
            this.txtBarCode.Text = "";
            var lastIndex = this.dgvData.Rows.Count - 1;
            this.dgvData.Rows[lastIndex].Selected = true;

            if (realPrice <= 0)
            {
                MessageBox.Show(string.Format("警告：商品[{0}]售价为0,请确认后再销售", model.Name), "系统信息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        public void ShowOrderInfo()
        {
            this.lblOrderTotal.Text = "总金额：" + _currentShopCat.OrderAmount.ToString("F2");
            this.lblQuantityTotal.Text = "总件数：" + _currentShopCat.TotalQuantity.ToString();
            this.lblDiscount.Text = "总优惠：" + _currentShopCat.TotalDiscountAmount.ToString("F2");
            this.lblOrderCode.Text = "订单号：" + _currentShopCat.OrderCode;
            this.lblPreChargeAmount.Text = "找零：0.00";
            //刷新gridview
            this.dgvData.AutoGenerateColumns = false;
            this.dgvData.DataSource = new List<ShopCartItem>();
            this.dgvData.DataSource = this._currentShopCat.Items;
            this.dgvData.ClearSelection();
            this.dgvData.FirstDisplayedScrollingRowIndex = this.dgvData.RowCount - 1;

        }

        public void ShowPreOrderInfo()
        {           
            var model = this._currentShopCat;
            _preShopCat = model;
            this.lblPreOrderCode.Text = "上一单：" + model.OrderCode;
            this.lblPreOrderAmount.Text = "金额：" + model.OrderAmount.ToString("F2");
            this.lblPreChargeAmount.Text = "找零：" + model.ChargeAmount.ToString("F2");
        }
        /// <summary>
        /// 上一单面板显示作废信息
        /// </summary>
        public void ShowCancelOrderInfo()
        {
            var model = this._currentShopCat;
            this._preShopCat = model;
            this.lblPreOrderCode.Text = "上一单：" + model.OrderCode;
            this.lblPreOrderAmount.Text = "金额：" + model.OrderAmount.ToString("F2");
            this.lblPreChargeAmount.Text = "找零：0.00" ;
        }

        public void ClearAll()
        {
            this.dgvData.DataSource = new List<ShopCartItem>();
            this.lblOrderTotal.Text = "总金额：0.00";
            this.lblQuantityTotal.Text = "总件数：0";
            this.lblOrderCode.Text = "订单号：";
            this.lblDiscount.Text = "总优惠：0.00";
            this.VipCustomer = null;
            this._currentShopCat = null;
        }

        /// <summary>
        /// 清空订单明细
        /// </summary>
        public void ClearItems()
        {
            ShowPreOrderInfo();
            this.dgvData.DataSource = new List<ShopCartItem>();
            this.VipCustomer = null;
            this._currentShopCat = null;
        }

        public void CreateSaleOrder(string money)
        {
            var inputAmount = 0m;
            decimal.TryParse(money, out inputAmount);
            _currentShopCat.PayAmount = inputAmount;
            if (_currentShopCat.Items.Count == 0)
            {
                MessageBox.Show("商品明细为空", "系统信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            _saleOrderService.CreateOrder(_currentShopCat);
            this.lblOrderCode.Text = "订单号：" + _currentShopCat.OrderCode;
            this.txtBarCode.Text = "";

            // 显示支付窗体  
            if (_currentShopCat.CheckCanPay())
            {               
                frmPay payForm = frmPay.CreateForm();
                payForm.CurrentOrder = _currentShopCat;
                payForm.PosForm = this;
                payForm.ShowDialog(this);
            }
            else
            {
                MessageBox.Show("有商品项数量为0,不能支付！请调整数量或ESC作废订单。", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        public void CreateSaleRefund(string money)
        {
            // 退单

            if (string.IsNullOrEmpty(money))
            {
                _currentShopCat.PayAmount = _currentShopCat.OrderAmount; ;
            }
            else
            {
                var inputAmount = 0m;
                decimal.TryParse(money, out inputAmount);
                _currentShopCat.PayAmount = inputAmount;
            }
            if (_currentShopCat.Items.Count == 0)
            {
                MessageBox.Show("退单无明细为空", "系统信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            _currentShopCat.OrderType = 2; // 销售退单
            _saleOrderService.CreateSaleRefund(_currentShopCat);
            this.lblOrderCode.Text = "订单号：" + _currentShopCat.OrderCode;
            this.txtBarCode.Text = "";

            if (_currentShopCat.OrderAmount < 0)
            {
                frmRefund refundFrom = frmRefund.CreateForm();
                refundFrom.CurrentOrder = _currentShopCat;
                refundFrom.PosForm = this;
                refundFrom.ShowDialog(this);
            }
            else
            {
                MessageBox.Show("退单金额必须小于 0", "系统信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }
        /// <summary>
        /// 按ESC 取消订单
        /// </summary>
        public void CreateCancelOrder()
        {
            try
            {
                if (this._currentShopCat == null) { return; }
                DialogResult result = MessageBox.Show("你确定作废订单？", "系统信息", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    return;
                }
                if (this._currentShopCat.OrderId == 0)
                {
                    if (_currentShopCat.OrderAmount < 0) { _currentShopCat.OrderType = 2; }
                    _currentShopCat.WorkScheduleCode = _currentWork.Code;
                    if (_currentShopCat.Items.Count == 0)
                    {
                        MessageBox.Show("商品明细为空", "系统信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    _saleOrderService.CreateOrder(_currentShopCat);
                }
                _saleOrderService.CancelOrder(_currentShopCat.OrderId, ContextService.CurrentAccount.Id);
                MessageBox.Show("订单作废成功", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //显示历史单据信息            
                ShowCancelOrderInfo();
                this.ClearAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }



      
        private void txtBarCode_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    inputEnter();
                    break;
                case Keys.Escape:
                    CreateCancelOrder();
                    break;
                case Keys.F1:  // +
                    ModifyQuantity();
                    break;
                case Keys.F2:  // - 
                    // MinusQuantity();
                    inputCustomer();
                    break;
                case Keys.F5: //主菜单
                    Quit();
                    break;
                case Keys.F6:
                    PrintPreOrderTicket();
                    break;
                default:
                    break;
            }
        }

        public void PrintPreOrderTicket()
        {
            this.txtBarCode.Text = "";
            if (_preShopCat == null) { return; }
            _saleOrderService.PrintTicket(_preShopCat.OrderId);
        }

        public void Quit()
        {
            if (this._currentShopCat != null)
            {
                MessageBox.Show("收银台还有没完成的订单，请先完结订单。", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            frmMain mainForm = frmMain.CreateForm();
            mainForm.Show();
            this.Close();
        }



        public void ModifyQuantity()
        {
            int lastIndex = this.dgvData.Rows.GetLastRow(DataGridViewElementStates.Selected);
            if (lastIndex < 0)
            {
                throw new AppException("请选择要修改数量的商品");
            }
            //设置焦点
            this.dgvData.Focus();
            this.dgvData.CurrentCell = this.dgvData.Rows[lastIndex].Cells["Quantity"];
            this.dgvData.BeginEdit(true);

        }



        public void inputCustomer()
        {
            frmVipCard vipForm = frmVipCard.CreateForm();
            vipForm.PosFrom = this;
            vipForm.ShowDialog(this);
        }

        public void SetVipCard(string code)
        {
            this.VipCustomer = _vipService.GetByCode(code);
            //刷新当前购物车折扣  
           // var discount = this.VipCustomer == null ? 1 : this.VipCustomer.Discount;
            this._currentShopCat = this._currentShopCat ?? new ShopCart(ContextService.StoreId, ContextService.PosId, ContextService.CurrentAccount.Id);
            foreach (var item in this._currentShopCat.Items)
            {                
                //只取会员商品价，不按折扣计算
                var realPrice = item.SalePrice;
                if (this.VipCustomer != null)
                {
                    var vipProduct = _vipProductService.GetByProductId(item.ProductId);
                    realPrice = vipProduct == null ? item.SalePrice : vipProduct.SalePrice;
                }
                item.ChangeRealPrice(realPrice);
            }
            // 重新绑定
            this.ShowOrderInfo();
            if (this.dgvData.CurrentCell != null)
            {
                this.dgvData.CurrentCell.Selected = true;
            }
            this.txtBarCode.Focus();
        }

        private void frmPos_Load(object sender, EventArgs e)
        {
            this.lblAccountId.Text = "工号：" + ContextService.CurrentAccount.Id;
            this.lblStoreId.Text = "门店：" + ContextService.StoreId;
            lblKeys.Text = "快捷键：F1 改数量,F2 会员,ESC 作废订单 ";
            lblKeys2.Text = "快捷键：F5 主菜单,F6 重打小票 ";
            this.txtBarCode.Focus();
            this.dgvData.ClearSelection();

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

        }

        //dgvData.RowHeadersDefaultCellStyle.ForeColor

        private void TextBoxDec_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 45  - 号
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != 45) //&& e.KeyChar != '.' e.KeyChar != 8 &&
            {
                e.Handled = true;
            }
        }

        private void dgvData_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (this.dgvData.CurrentCell.ColumnIndex == 8)  //数量列
            {
                e.Control.KeyPress += new KeyPressEventHandler(TextBoxDec_KeyPress);
            }
        }


        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                if (this.dgvData.IsCurrentCellInEditMode)   //如果当前单元格处于编辑模式   
                {
                    var index = this.dgvData.CurrentCell.RowIndex;
                    var quantity = Convert.ToInt32(this.dgvData.Rows[index].Cells["Quantity"].EditedFormattedValue.ToString());
                    var pid = this.dgvData.Rows[index].Cells["ProductId"].Value.ToString();
                    var productId = Convert.ToInt32(pid);
                    this._currentShopCat.ChangeQuantity(productId, quantity);
                    this.ShowOrderInfo(); //重新刷新                  
                    this.dgvData.Rows[index].Selected = true;
                    txtBarCode.Focus();
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }


        // this.panel1.Height = 36;
        // this.panel3.Height = 180;
        // this.panel2.Height = this.Height - this.panel1.Height - this.panel3.Height;
    }
}
