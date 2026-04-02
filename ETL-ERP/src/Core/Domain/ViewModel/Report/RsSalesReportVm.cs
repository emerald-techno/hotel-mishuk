using Domain.Enums.AppEnums;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel.Report
{
    public class RsSalesReportVm
    {
        public long OrderId { get; set; }
        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public long CustomerId { get; set; }
        public string CustomerName { get; set; }
        public long? RoomId { get; set; }
        public string RoomNo { get; set; }
        public long? BookingId { get; set; }
        public string BookingNo { get; set; }
        public double OrderAmount { get; set; }
        public double PaidAmount { get; set; }
        public double DueAmount { get; set; }
        public RsOrderPaymentStatusEnum PaymentStatus { get; set; }
        public string StrQueryDate { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public decimal Quantity { get; set; } = 0;
        public decimal Rate { get; set; } = 0;
        public decimal ItemAmount { get; set; } = 0;
        public decimal VAT { get; set; }
        public decimal Tax { get; set; }
        public decimal Discount { get; set; } = 0;
        public decimal NetAmount { get; set; } = 0;
        public string GroupBy { get; set; }
       


        public IEnumerable<SelectListItem> CategoryLookup { get; set; }
        public IEnumerable<SelectListItem> ItemLookup { get; set; }
        public IEnumerable<SelectListItem> GroupByLookup { get; set; }

        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
    }
}
