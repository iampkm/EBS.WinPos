using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace EBS.AutoUpdater
{
   public class AutoUpdateService
    {
        public void CheckUpdate()
        {
            bool bHasError = false;
            IAutoUpdater autoUpdater = new AutoUpdater();
            try
            {
                autoUpdater.Update();
            }
            catch (WebException exp)
            {
                MessageBox.Show("未发现更新文件");
                bHasError = true;
            }
            catch (XmlException exp)
            {
                bHasError = true;
                MessageBox.Show("下载升级文件错误");
            }
            catch (NotSupportedException exp)
            {
                bHasError = true;
                MessageBox.Show("下载地址配置错误");
            }
            catch (ArgumentException exp)
            {
                bHasError = true;
                MessageBox.Show("下载升级文件错误");
            }
            catch (Exception exp)
            {
                bHasError = true;
                MessageBox.Show("升级过程中出现错误");
            }
            finally
            {
                if (bHasError == true)
                {
                    try
                    {
                        autoUpdater.RollBack();
                    }
                    catch (Exception)
                    {
                        //Log the message to your file or database
                    }
                }
            }
        }
    }
}
