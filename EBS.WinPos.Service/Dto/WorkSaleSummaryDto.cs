using EBS.WinPos.Domain.ValueObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EBS.WinPos.Service.Dto
{
   public class WorkSaleSummaryDto
    {
        public string CreatedByName { get; set; }

        public string StoreName { get; set; }

        public int PosId { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal TotalOnlineAmount { get; set; }

        public PaymentWay PaymentWay { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

    }
}
