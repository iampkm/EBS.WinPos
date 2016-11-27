using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using EBS.Infrastructure;  
namespace EBS.WinPos.Service
{
   public class PrinterService
    {
        string ipPort = "127.0.0.1";
        int _port = 9100;
        public PrinterService()  
        {  
        }

        public PrinterService(string IpPort)  
        {  
            this.ipPort = IpPort;//打印机端口   
        }  
  
        ///   <summary>   
        ///   输出文字到打印机   
        ///   </summary>   
        ///   <param   name= "str "> 要打印的内容 </param>   
        public void PrintLine(string str)  
        {  
            //建立连接  
            IPAddress ipa = IPAddress.Parse(ipPort);
            IPEndPoint ipe = new IPEndPoint(ipa, _port);//9100为小票打印机指定端口  
            Socket soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);  
            soc.Connect(ipe);  
  
            //string str= "hello,123456789,大家好! ";  
  
            byte[] b = System.Text.Encoding.GetEncoding("GB2312").GetBytes(str);  
            soc.Send(b);  
            soc.Close();  
        }  
  
  
        public void PrintPic(Bitmap bmp)  
        {  
            //把ip和端口转化为IPEndPoint实例  
            IPEndPoint ip_endpoint = new IPEndPoint(IPAddress.Parse(ipPort), _port);  
  
            //创建一个Socket  
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);  
  
  
            //连接到服务器  
            socket.Connect(ip_endpoint);  
            //应对同步Connect超时过长的办法，猜测应该是先用异步方式建立以个连接然后，  
            //确认连接是否可用，然后报错或者关闭后，重新建立一个同步连接                      
  
            //socket.SendTimeout = 1000;  
  
            //初始化打印机，并打印  
  
            Byte[] byte_send = Encoding.GetEncoding("gb18030").GetBytes("\x1b\x40");  
  
            //发送测试信息  
            socket.Send(byte_send, byte_send.Length, 0);  
  
  
            byte[] data = new byte[] { 0x1B, 0x33, 0x00 };  
            socket.Send(data, data.Length, 0);  
            data[0] = (byte)'\x00';  
            data[1] = (byte)'\x00';  
            data[2] = (byte)'\x00';    // Clear to Zero.  
  
            Color pixelColor;  
  
            // ESC * m nL nH 点阵图  
            byte[] escBmp = new byte[] { 0x1B, 0x2A, 0x00, 0x00, 0x00 };  
  
            escBmp[2] = (byte)'\x21';  
  
            //nL, nH  
            escBmp[3] = (byte)(bmp.Width % 256);  
            escBmp[4] = (byte)(bmp.Width / 256);  
  
            // data  
            for (int i = 0; i < (bmp.Height / 24) + 1; i++)  
            {  
                socket.Send(escBmp, escBmp.Length, 0);  
  
                for (int j = 0; j < bmp.Width; j++)  
                {  
                    for (int k = 0; k < 24; k++)  
                    {  
                        if (((i * 24) + k) < bmp.Height)   // if within the BMP size  
                        {  
                            pixelColor = bmp.GetPixel(j, (i * 24) + k);  
                            if (pixelColor.R == 0)  
                            {  
                                data[k / 8] += (byte)(128 >> (k % 8));  
                            }  
                        }  
                    }  
  
                    socket.Send(data, 3, 0);  
                    data[0] = (byte)'\x00';  
                    data[1] = (byte)'\x00';  
                    data[2] = (byte)'\x00';    // Clear to Zero.  
                }  
                  
                byte_send = Encoding.GetEncoding("gb18030").GetBytes("\n");  
  
                //发送测试信息  
                socket.Send(byte_send, byte_send.Length, 0);  
            } // data  
  
            byte_send = Encoding.GetEncoding("gb18030").GetBytes("\n");  
  
            //发送测试信息  
            socket.Send(byte_send, byte_send.Length, 0);  
            socket.Close();  
        }  
  
  
        ///   <summary>   
        ///   打开钱箱   
        ///   </summary>   
        public void OpenCashBox()  
        {  
            IPAddress ipa = IPAddress.Parse(ipPort);
            IPEndPoint ipe = new IPEndPoint(ipa, _port);//9100为小票打印机指定端口  
            Socket soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);  
            soc.Connect(ipe);  
            char[] c = { Convert.ToChar(27), 'p', Convert.ToChar(0), Convert.ToChar(60), Convert.ToChar(255) };  
            byte[] b = System.Text.Encoding.GetEncoding("GB2312").GetBytes(c);  
            soc.Send(b);  
            soc.Close();  
        }  


        // 打印
        //打印文字（端口号 字符）
        public  void PrintString(int Port, string val)
        {
            System.IO.Ports.SerialPort sp = new System.IO.Ports.SerialPort();
            sp.PortName = "COM" + Port.ToString();
            try
            {
                sp.Open();
            }
            catch
            {
                throw new AppException("端口被占用");
            }
            List<byte> data = new List<byte>();
            string[] lines = val.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                byte[] content = System.Text.Encoding.Default.GetBytes(lines[i].Replace("\r", ""));
                byte[] wapbt = { 0x0a };
                data.AddRange(content);
                data.AddRange(wapbt);
            }
            byte[] cutbt = { 0x1d, 0x56, 0x42, 0x11 };
            data.AddRange(cutbt);
            byte[] databt = data.ToArray();
            sp.Write(databt, 0, databt.Length);
            sp.Close();
        }
    }
}
