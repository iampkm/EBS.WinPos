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
    public partial class frmDownload : Form
    {
        // 窗体单例
        private static frmDownload _instance;
        public static frmDownload CreateForm()
        {
            //判断是否存在该窗体,或时候该字窗体是否被释放过,如果不存在该窗体,则 new 一个字窗体  
            if (_instance == null || _instance.IsDisposed)
            {
                _instance = new frmDownload();
            }
            return _instance;
        }
        public frmDownload()
        {
            InitializeComponent();
        }
    }
}
