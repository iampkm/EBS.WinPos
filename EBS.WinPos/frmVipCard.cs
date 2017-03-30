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
    public partial class frmVipCard : Form
    {
        public frmPos PosFrom { get; set; }
        private static frmVipCard _instance;
        public static frmVipCard CreateForm()
        {
            //判断是否存在该窗体,或时候该字窗体是否被释放过,如果不存在该窗体,则 new 一个字窗体  
            if (_instance == null || _instance.IsDisposed)
            {
                _instance = new frmVipCard();
            }
            return _instance;
        }

        VipCardService _vipService;

        public frmVipCard()
        {
            InitializeComponent();

            _vipService = new VipCardService();
        }

        private void frmDiscount_Load(object sender, EventArgs e)
        {
            this.txtCode.Focus();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SaveCode();
        }

        public void SaveCode()
        {
            var vipCustomer = _vipService.GetByCode(this.txtCode.Text);
            if (vipCustomer == null)
            {
                MessageBox.Show("您输入的会员卡错误,请重试！", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.txtCode.Focus();
                this.txtCode.SelectAll();
                return;
            }

            this.PosFrom.SetVipCard(vipCustomer);           
            this.Close();
        }

        private void txtCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SaveCode();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
