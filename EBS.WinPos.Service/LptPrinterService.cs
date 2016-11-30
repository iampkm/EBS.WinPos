using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace EBS.WinPos.Service
{
   public class LptPrinterService:IPosPrinter
    {
        private string LptStr = "lpt1";

        public LptPrinterService()
        {

        }
        public LptPrinterService(string l_LPT_Str)
        {

            LptStr = l_LPT_Str;
        }
        [StructLayout(LayoutKind.Sequential)]
        private struct OVERLAPPED
        {
            int Internal;
            int InternalHigh;
            int Offset;
            int OffSetHigh;
            int hEvent;
        }


        //调用DLL.
        [DllImport("kernel32.dll")]
        private static extern int CreateFile(string lpFileName, uint dwDesiredAccess, int dwShareMode, int lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, int hTemplateFile);
        [DllImport("kernel32.dll")]
        private static extern bool WriteFile(int hFile, byte[] lpBuffer, int nNumberOfBytesToWrite, ref int lpNumberOfBytesWritten, ref OVERLAPPED lpOverlapped);
        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(int hObject);
        private int iHandle;


        /// <summary>
        /// 打开端口
        /// </summary>
        /// <returns></returns>
        public bool Open()
        {
            iHandle = CreateFile(LptStr, 0x40000000, 0, 0, 3, 0, 0);
            // iHandle = CreateFile(LptStr, GENERIC_WRITE, 0, 0, OPEN_EXISTING, 0, 0);

            if (iHandle != -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 打印字符串，通过调用该方法可以打印需要的字符串
        /// </summary>
        /// <param name="Mystring"></param>
        /// <returns></returns>
        public bool Write(String Mystring)
        {
            //如果端口为打开，则提示，打开，则打印
            if (iHandle != -1)
            {
                OVERLAPPED x = new OVERLAPPED();
                int i = 0;
                //byte[] mybyte = System.Text.Encoding.Default.GetBytes(Mystring);
                byte[] mybyte = Encoding.GetEncoding("GB2312").GetBytes(Mystring);
                bool b = WriteFile(iHandle, mybyte, mybyte.Length, ref i, ref x);
                return b;
            }
            else
            {
                throw new Exception("不能连接到打印机!");
            }
        }
        /// <summary>
        /// 打印命令，通过参数，可以打印小票打印机的一些命令，比如换行，行间距，打印位图等。
        /// </summary>
        /// <param name="mybyte"></param>
        /// <returns></returns>
        public bool Write(byte[] mybyte)
        {
            //如果端口为打开，则提示，打开，则打印
            if (iHandle != -1)
            {
                OVERLAPPED x = new OVERLAPPED();
                int i = 0;
                return WriteFile(iHandle, mybyte, mybyte.Length, ref i, ref x);
            }
            else
            {
                throw new Exception("不能连接到打印机!");
            }
        }

        /// <summary>
        /// 关闭端口
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            return CloseHandle(iHandle);
        }

        /// <summary>
        /// 打印图片方法
        /// </summary>
        public void PrintPicture(Image img)
        {
            //获取图片
            Bitmap bmp = new Bitmap(img);

            //设置字符行间距为n点行
            //byte[] data = new byte[] { 0x1B, 0x33, 0x00 };
            string send = "" + (char)(27) + (char)(51) + (char)(0);
            byte[] data = new byte[send.Length];
            for (int i = 0; i < send.Length; i++)
            {
                data[i] = (byte)send[i];
            }
            this.Write(data);

            data[0] = (byte)'\x00';
            data[1] = (byte)'\x00';
            data[2] = (byte)'\x00';    // Clear to Zero.

            Color pixelColor;


            //ESC * m nL nH d1…dk   选择位图模式
            // ESC * m nL nH
            byte[] escBmp = new byte[] { 0x1B, 0x2A, 0x00, 0x00, 0x00 };

            escBmp[2] = (byte)'\x21';

            //nL, nH
            escBmp[3] = (byte)(bmp.Width % 256);
            escBmp[4] = (byte)(bmp.Width / 256);

            //循环图片像素打印图片
            //循环高
            for (int i = 0; i < (bmp.Height / 24 + 1); i++)
            {
                //设置模式为位图模式
                this.Write(escBmp);
                //循环宽
                for (int j = 0; j < bmp.Width; j++)
                {
                    for (int k = 0; k < 24; k++)
                    {
                        if (((i * 24) + k) < bmp.Height)  // if within the BMP size
                        {
                            pixelColor = bmp.GetPixel(j, (i * 24) + k);
                            if (pixelColor.R == 0)
                            {
                                data[k / 8] += (byte)(128 >> (k % 8));

                            }
                        }
                    }
                    //一次写入一个data，24个像素
                    this.Write(data);

                    data[0] = (byte)'\x00';
                    data[1] = (byte)'\x00';
                    data[2] = (byte)'\x00';    // Clear to Zero.
                }

                //换行，打印第二行
                byte[] data2 = { 0xA };
                this.Write(data2);
            } // data
            this.Write("\n\n");
        }

        public void Print(string content)
        {
            this.Write(content);
        }
    }
}
