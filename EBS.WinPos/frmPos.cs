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
using Sunisoft.IrisSkin;

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
            if (input.Length < 6)
            {
                this._currentShopCat = this._currentShopCat ?? new ShopCart(ContextService.StoreId, ContextService.PosId, ContextService.CurrentAccount.Id);
                this._currentShopCat.WorkScheduleCode = _currentWork.Code;

                if (_currentShopCat.OrderAmount > 0)  // 订单
                {
                    CreateSaleOrder();
                }
                else if (_currentShopCat.OrderAmount < 0) // 退单
                {
                    CreateRefundOrder();
                }
                else {
                    MessageBox.Show("请输入正确的商品条码", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information); 
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
            this.lblPreChargeAmount.Text = "找零：0.00";
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

        public void CreateSaleOrder()
        {
            CheckItemsAndProductQuantity();
            if (_currentShopCat.OrderId == 0)
            {
                _saleOrderService.CreateOrder(_currentShopCat);
            }
         
            this.lblOrderCode.Text = "订单号：" + _currentShopCat.OrderCode;
            this.txtBarCode.Text = "";

            // 显示支付窗体  
            frmPay payForm = frmPay.CreateForm();
            payForm.CurrentOrder = _currentShopCat;
            payForm.PosForm = this;
            payForm.ShowDialog(this);

        }

        private void CheckItemsAndProductQuantity()
        {
            if (_currentShopCat.CheckProductQuantity())
            {
                MessageBox.Show("订单明细为空或商品数量异常，一行商品数量最大1000。", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!_currentShopCat.CheckOrderType())
            {
                MessageBox.Show("订单明细数量全部为正，退单全部为负，不允许混编！", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
        }

        public void CreateRefundOrder()
        {
            // 退单
            CheckItemsAndProductQuantity();    
           
            if (_currentShopCat.OrderId == 0)  //退出销售退款窗体，重新进入时，不重复创建单据
            {
                _saleOrderService.CreateSaleRefund(_currentShopCat);
            }           
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
                if (ShopCatIsEmpty()) { return; }
                DialogResult result = MessageBox.Show("你确定作废订单？", "系统信息", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    return;
                }
                if (this._currentShopCat.OrderId == 0)
                {
                    if (_currentShopCat.OrderAmount < 0) { _currentShopCat.OrderType = 2; }
                    _currentShopCat.WorkScheduleCode = _currentWork.Code;
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
                    inputEnter();  // 扫条码和 现金，支付宝，微信支付
                    break;
                case Keys.Escape:
                    CreateCancelOrder();  // 取消订单
                    break;
                case Keys.F1:             // 修改数量
                    ModifyQuantity();
                    break;
                case Keys.F2:            // 输入vip卡
                    inputCustomer();
                    break;
                case Keys.F3:            //在线支付查询
                    PayQuery();
                    break;
                case Keys.F4:            
                   
                    break;
                case Keys.F5:            // 主菜单
                    Quit();
                    break;
                case Keys.F6:            // 打印上一单
                    PrintPreOrderTicket();
                    break;
                case Keys.F7:           // 现金，支付宝，微信退款
                  //  CreateRefundOrder();
                    break;
                case Keys.F8:           //退款查询
                    RefundQuery();
                    break;
                case Keys.F9:

                    break;
                default:
                    break;
            }
        }

        private void frmPos_Load(object sender, EventArgs e)
        {
            this.lblAccountId.Text = "工号：" + ContextService.CurrentAccount.Id;
            this.lblStoreId.Text = "门店：" + ContextService.StoreId;
            lblKeys.Text = "快捷键：F1 改数量,F2 会员,ESC 作废订单 ";
            lblKeys2.Text = "F3 支付查询,F5 主菜单,F6 重打小票,F7 创建退单 ";
            this.txtBarCode.Focus();
            this.dgvData.ClearSelection();

        }

        private void PayQuery()
        {
            frmPayQuery win = frmPayQuery.CreateForm();
            win.ShowDialog(this);
        }

        private void RefundQuery()
        {

        }

        public void PrintPreOrderTicket()
        {
            this.txtBarCode.Text = "";
            if (PreShopCatIsEmpty()) { return; }
            _saleOrderService.PrintTicket(_preShopCat.OrderId);
        }
        /// <summary>
        /// 上一笔订单购物车为空
        /// </summary>
        /// <returns></returns>
        private bool PreShopCatIsEmpty()
        {
            return this._preShopCat == null || (this._preShopCat != null && this._preShopCat.Items.Count <= 0);
        }

        /// <summary>
        /// 当前购物车为空
        /// </summary>
        /// <returns></returns>
        private bool ShopCatIsEmpty()
        {
            return this._currentShopCat == null || (this._currentShopCat != null && this._currentShopCat.Items.Count <= 0);
        }

        public void Quit()
        {
            if (!ShopCatIsEmpty())
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

        public void SetVipCard(VipCard vip)
        {
            //设置vip对象
            this.VipCustomer = vip;

            //刷新当前购物车折扣  
            // var discount = this.VipCustomer == null ? 1 : this.VipCustomer.Discount;
            this._currentShopCat = this._currentShopCat ?? new ShopCart(ContextService.StoreId, ContextService.PosId, ContextService.CurrentAccount.Id);
            this._currentShopCat.OrderLevel = Domain.ValueObject.SaleOrderLevel.Vip;
            if (this._currentShopCat.Items.Count == 0)
            {
                //如果当前还没刷商品，焦点返回输入框
                this.txtBarCode.Focus();
                return;
            }
            foreach (var item in this._currentShopCat.Items)
            {
                //只取会员商品价，不按折扣计算
                var realPrice = item.SalePrice;
                var vipProduct = _vipProductService.GetByProductId(item.ProductId);
                realPrice = vipProduct == null ? item.SalePrice : vipProduct.SalePrice;
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
                    if (quantity > 1000)
                    {
                        MessageBox.Show("单行商品数量最大为1000。", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.dgvData.Focus();
                        this.dgvData.CurrentCell = this.dgvData.Rows[index].Cells["Quantity"];
                        this.dgvData.BeginEdit(true);
                        return true;
                    }
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
