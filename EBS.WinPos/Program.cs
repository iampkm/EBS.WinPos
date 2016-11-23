using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using EBS.Infrastructure;
namespace EBS.WinPos
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                //处理未捕获的异常   
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                //处理UI线程异常   
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                //处理非UI线程异常   
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMain());
            }
            catch (Exception e)
            {
                WriteLog(e);
            }
           
        }

        static void WriteLog(Exception e)
        {
            if (e is AppException)
            {
                MessageBox.Show(e.Message, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else {
                MessageBox.Show("出现错误，请联系管理员或重启系统", "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                AppContext.Log.Error(e);
            }
          
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            WriteLog(e.Exception);
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception error = e.ExceptionObject as Exception;
            WriteLog(error);
        }
    }
}
