using Domain.Enums.AppEnums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel.Report
{
    public class RsDailySalesSummaryVm
    {
        public DateTime TranDate { get; set; }
        public long OrderId { get; set; }
        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public long CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerType { get; set; }
        public int CustomerTypeId { get; set; }
        public long? RoomId { get; set; }
        public string RoomNo { get; set; }
        public long? BookingId { get; set; }
        public string BookingNo { get; set; }
        public double OrderAmount { get; set; }

        public double Vat { get; set; }
        public double Tax { get; set; }
        public double ServiceCharge { get; set; }
        public double Discount { get; set; }
        public double NetAmount { get; set; }
        public double PaidAmount { get; set; }
        public double DueAmount { get; set; }
        public double DueCollection { get; set; }
        public double AdvanceAmount { get; set; } = 0;
        public double AdvanceRefund { get; set; } = 0;
        public string TableNo { get; set; }
        public int TableId { get; set; } 


        public RsOrderPaymentStatusEnum PaymentStatus { get; set; }
        public string StrQueryDate { get; set; }
    }
}
