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
namespace EBS.WinPos
{
    public partial class frmPos : Form
    {

        Repository _db;
        public frmPos()
        {
            InitializeComponent();

            _db = new Repository();


        }

        frmPay _payForm;
        public void inputEnter()
        {
            var input = this.txtBarCode.Text;
            _payForm = new frmPay();
            _payForm.ShowDialog(this);

        }

        public void Cancel()
        {

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
                default:
                    break;
            }
        }

        private void frmPos_Load(object sender, EventArgs e)
        {
            this.txtBarCode.Focus();

            mockData();
        }

        public void mockData()
        {
            List<SaleOrderItem> list = new List<SaleOrderItem>();
            for (int i = 1; i < 100; i++)
            {
                decimal price = (100 / 3 * i);
                SaleOrderItem item = new SaleOrderItem()
                {
                    ProductName = "商品" + i.ToString(),
                    ProductCode = "1234560" + i.ToString(),
                    BarCode = "b1234560" + i.ToString(),
                    Quantity = 1,
                    SalePrice = Math.Round(price, 2),
                    Specification = "12ml",
                };
                list.Add(item);
            }
            this.dgvData.AutoGenerateColumns = false;
            this.dgvData.DataSource = list;
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

        //dgvData.RowHeadersDefaultCellStyle.ForeColor


    }
}
