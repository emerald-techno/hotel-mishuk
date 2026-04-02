using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel.Report
{
    public class StockRegisterVm
    {
        public long CategoryId { get; set; }
        public long ItemId { get; set; }
        public long UnitId { get; set; }
        public string CategoryType { get; set; }
        public string CategoryTypeName => CategoryType switch { "I" => "Item", "S" => "Service", _ => "--" };
        public string CategoryName { get; set; }
        public string ItemName { get; set; }
        public string UnitName { get; set; }
        public double TotalRcvQty { get; set; }
        public double TotalIssueQty { get; set; }
        public double Stock { get; set; }
        public int? StockStatus { get; set; }
        public double TotalRcvAmount { get; set; }
        public double TotalIssueAmount { get; set; }
        public double StockAmount { get; set; }
        public decimal UnitPrice { get; set; }

        public double TotalInQty { get; set; } = 0;
        public double TotalInAmount { get; set; } = 0;
        public double TotalOutQty { get; set; } = 0;
        public double TotalOutAmount { get; set; } = 0;
        public double ClosingQty { get; set; } = 0;
        public double ClosingAmount { get; set; } = 0;
        public double ReceiveAmount { get; set; } = 0;
        public double ReceiveQty { get; set; } = 0;
        public double IssueReturnQty { get; set; } = 0;
        public double IssueReturnAmount { get; set; } = 0;
        public double IssueQty { get; set; } = 0;
        public double IssueAmount { get; set; } = 0;
        public double ReceiveReturnQty { get; set; } = 0;
        public double ReceiveReturnAmount { get; set; } = 0;
        public double OpeningQty { get; set; } = 0;
        public double OpeningAmount { get; set; } = 0;
        public double LeftOverQty { get; set; } = 0;
        public double LeftOverAmount { get; set; } = 0;


        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }

        public long DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public IEnumerable<SelectListItem> DepartmentLookUp { get; set; }
        public IEnumerable<SelectListItem> CategoryLookUp { get; set; }
        public IEnumerable<SelectListItem> StockStatusLookUp { get; set; }
        public IEnumerable<SelectListItem> ItemLookUp { get; set; }

        // Computed rate properties
        public decimal OpeningRate => OpeningQty != 0 ? (decimal)OpeningAmount / (decimal)OpeningQty : 0;
        public decimal ReceiveRate => ReceiveQty != 0 ? (decimal)ReceiveAmount / (decimal)ReceiveQty : 0;
        public decimal IssueRate => IssueQty != 0 ? (decimal)IssueAmount / (decimal)IssueQty : 0;
        public decimal IssueReturnRate => IssueReturnQty != 0 ? (decimal)IssueReturnAmount / (decimal)IssueReturnQty : 0;
        public decimal ReceiveReturnRate => ReceiveReturnQty != 0 ? (decimal)ReceiveReturnAmount / (decimal)ReceiveReturnQty : 0;
        public decimal TotalInRate => TotalInQty != 0 ? (decimal)TotalInAmount / (decimal)TotalInQty : 0;
        public decimal TotalOutRate => TotalOutQty != 0 ? (decimal)TotalOutAmount / (decimal)TotalOutQty : 0;
        public decimal ClosingRate => ClosingQty != 0 ? (decimal)ClosingAmount / (decimal)ClosingQty : 0;
        public decimal LeftOverRate => LeftOverQty != 0 ? (decimal)LeftOverAmount / (decimal)LeftOverQty : 0;
    }

}
