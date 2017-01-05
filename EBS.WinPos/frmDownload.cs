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
    public partial class frmProgress : Form
    {
        // 窗体单例
        private static frmProgress _instance;
        public static frmProgress CreateForm()
        {
            //判断是否存在该窗体,或时候该字窗体是否被释放过,如果不存在该窗体,则 new 一个字窗体  
            if (_instance == null || _instance.IsDisposed)
            {
                _instance = new frmProgress();
            }
            return _instance;
        }
        public frmProgress()
        {
            InitializeComponent();
        }

        private void frmProgress_Load(object sender, EventArgs e)
        {

        }

        public void InitProgressBar(int max)
        {
            this.progressBar1.Maximum = max;
            this.progressBar1.Minimum = 1;
            this.progressBar1.Value = 0;
            this.progressBar1.Step = 1;
        }

        public void PerformStep()
        {
            this.progressBar1.PerformStep();
            // 完成百分比
            var persent = this.progressBar1.Value / this.progressBar1.Maximum * 100;
            this.lblMsg.Text = string.Format("已完成{0}%", persent);
        }
    }
}
