using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;

namespace EBS.WinPos.Service
{
    /// 客显  
    /// </summary>  
    public class CustomerDisplay
    {
        #region 成员变量  

        private string spPortName;
        private int spBaudRate;
        private StopBits spStopBits;
        private int spDataBits;

        #endregion --成员变量  

        #region 属性  
        /// <summary>  
        /// 客显发送类型  
        /// </summary>  
        public CustomerDispiayType DispiayType { get; set; }
        #endregion --属性  

        #region 构造函数  
        /// <summary>  
        /// 构造函数  
        /// </summary>  
        /// <param name="_spPortName">端口名称（COM1,COM2，COM3...）</param>  
        /// <param name="_spBaudRate">通信波特率（2400,9600....）</param>  
        /// <param name="_spStopBits">停止位</param>  
        /// <param name="_spDataBits">数据位</param>  
        public CustomerDisplay(string _spPortName, int _spBaudRate, string _spStopBits, int _spDataBits)
        {
            this.spBaudRate = _spBaudRate;
            this.spDataBits = _spDataBits;
            this.spPortName = _spPortName;
            this.spStopBits = (StopBits)(Enum.Parse(typeof(StopBits), _spStopBits));
        }
        #endregion --构造函数  

        #region Method  
        #region 公共方法  

        /// <summary>  
        /// 数据信息展现  
        /// </summary>  
        /// <param name="data">发送的数据（清屏可以为null或者空）</param>  
        public void DisplayData(string data)
        {
            SerialPort serialPort = new SerialPort();
            serialPort.PortName = spPortName;
            serialPort.BaudRate = spBaudRate;
            serialPort.StopBits = spStopBits;
            serialPort.DataBits = spDataBits;
            serialPort.Open();

            //先清屏  
            serialPort.WriteLine(((char)12).ToString());
            //指示灯  
            string str = ((char)27).ToString() + ((char)115).ToString() + ((char)(int)this.DispiayType).ToString();
            serialPort.WriteLine(str);

            //发送数据  
            if (!string.IsNullOrEmpty(data))
            {
                serialPort.WriteLine(((char)27).ToString() + ((char)81).ToString() + ((char)65).ToString() + data + ((char)13).ToString());
            }

            serialPort.Close();
        }

        #endregion --公共方法  
        #endregion --Method  
    }

    /// <summary>  
    /// 客显类型  
    /// </summary>  
    public enum CustomerDispiayType
    {
        /// <summary>  
        /// 清屏  
        /// </summary>  
        Clear,
        /// <summary>  
        /// 单价  
        /// </summary>  
        Price,
        /// <summary>  
        /// 合计  
        /// </summary>  
        Total,
        /// <summary>  
        /// 收款  
        /// </summary>  
        Recive,
        /// <summary>  
        /// 找零  
        /// </summary>  
        Change
    }

}

// 调用示例
//Console.Write("清屏");  
//               CustomerDisplay disliay = new CustomerDisplay("COM2", 9600, StopBits.One, 8);

//disliay.DispiayType = CustomerDispiayType.Clear;  
//               disliay.DisplayData(null);                
//               System.Threading.Thread.Sleep(1000);  
  
//               Console.Write("找零");  
              
//               disliay.DispiayType = CustomerDispiayType.Change;  
//               disliay.DisplayData("230.34");  
//               System.Threading.Thread.Sleep(1000);  
  
//               Console.Write("收款");  
//               disliay.DispiayType = CustomerDispiayType.Recive;  
//               disliay.DisplayData("10.34");  
                 
//               System.Threading.Thread.Sleep(1000);  
  
//               Console.Write("总计");  
//               disliay.DispiayType = CustomerDispiayType.Total;  
//               disliay.DisplayData("99230.34");  
           
//               System.Threading.Thread.Sleep(1000);  
  
//               Console.Write("单价");  
//               disliay.DispiayType = CustomerDispiayType.Price;  
//               disliay.DisplayData("18.34");  
                 
//               System.Threading.Thread.Sleep(2000);  
