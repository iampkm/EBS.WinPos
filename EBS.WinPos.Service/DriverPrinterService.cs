using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using EBS.Infrastructure;
namespace EBS.WinPos.Service
{
    public class DriverPrinterService : IPosPrinter
    {
        PrintDocument _printDocument;
        public DriverPrinterService()
        {
            _printDocument = new PrintDocument();
            _printDocument.BeginPrint += _printDocument_BeginPrint;
            _printDocument.PrintPage += _printDocument_PrintPage;
            _printDocument.EndPrint += _printDocument_EndPrint;
        }

        string _content;

        public void Print(string content)
        {
            _content = content;
            try
            {
                _printDocument.Print();
            }
            catch (Exception ex)
            {
                throw new AppException("小票打印异常，请重试或联系管理员",ex);
            }          
        }
       
        Brush _brush;
        Font _titleFont;
        Graphics _g;

        void _printDocument_BeginPrint(object sender, PrintEventArgs e)
        {
            _brush = new SolidBrush(Color.Black);
            _titleFont = new Font("宋体", 9);
            PageSettings setting = new PageSettings();
            setting.Margins = new Margins(50,100,50,100);
            _printDocument.DefaultPageSettings = setting;
        }
      

        void _printDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
             _g = e.Graphics;
           // string title = "社区卫生服务站 处方笺";
           // g.DrawString(_content, titleFont, b, new PointF((e.PageBounds.Width - g.MeasureString(_content, titleFont).Width) / 2, 20));
            _g.DrawString(_content, _titleFont, _brush, 0,0);
        }

        void _printDocument_EndPrint(object sender, PrintEventArgs e)
        {
            _g.Dispose();

        }
        
    }
}
