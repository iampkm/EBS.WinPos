using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EBS.WinPos.Domain.Entity;
namespace EBS.WinPos.Service.WxPayAPI
{
   public interface IMicropay
    {
       /// <summary>
       /// 微信刷卡支付
       /// </summary>
       /// <param name="orderCode">订单编号</param>
       /// <param name="body">商品描述</param>
       /// <param name="total_fee">订单金额：单位到分，只能是整数</param>
       /// <param name="auth_code">用户微信授权码（扫的条码）</param>
       void Run(string orderCode, string body, int total_fee, string auth_code);
    }
}
