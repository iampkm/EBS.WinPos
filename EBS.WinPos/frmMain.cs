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

namespace EBS.WinPos
{
    public partial class frmMain : Form
    {
        frmLogin _loginFrom;
        WorkScheduleService _workService;
        public frmMain()
        {
            InitializeComponent();

            _workService = new WorkScheduleService();
        }
             

        private void frmMain_Load(object sender, EventArgs e)
        {
            //_loginFrom = new frmLogin();
            //ContextService.AddFrom(this);
            //ContextService.AddFrom(_loginFrom);
            //_loginFrom.ShowDialog(this);
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //收银台
            // 检查当前班次收银员账户是否一致
            var worker = _workService.GetWorking(ContextService.CurrentAccount.StoreId, Config.PosId);
            if (worker == null)
            {
                MessageBox.Show("请先上班再开始销售", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (worker.CreatedBy != ContextService.CurrentAccount.Id)
            {
                MessageBox.Show("请先交接班再开始销售", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

           var posForm= ContextService.GetFrom(typeof(frmPos));
            if (posForm == null)
            {
                posForm = new frmPos();
                ContextService.AddFrom(posForm);
                posForm.MdiParent = ContextService.ParentForm;
                posForm.Show();
            }
            else {
                posForm.Show();
            } 
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            // 交接班
            var wrokForm= ContextService.GetFrom(typeof(frmWork));
            if (wrokForm == null)
            {
                frmWork workForm = new frmWork();
                ContextService.AddFrom(workForm);
                workForm.MdiParent = ContextService.ParentForm;
                workForm.Show();
            }
            else {
                wrokForm.Show();
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            // 我的班次
            frmMy myWork = new frmMy();
            myWork.MdiParent = this;
            myWork.Show();
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            frmQuery query = new frmQuery();
            query.MdiParent = this;
            query.Show();
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            EBS.WinPos.Service.PrinterService service = new PrinterService();
            service.PrintTest();
        }
    }
}
