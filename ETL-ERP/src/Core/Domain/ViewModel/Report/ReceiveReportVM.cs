using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Report
{
    public class ReceiveReportVM
    {
        public long CategoryId { get; set; }
        public long ItemId { get; set; }
        public long UnitId { get; set; }
        public string CategoryName { get; set; }
        public string ItemName { get; set; }
        public string UnitName { get; set; }
        public double ItemQty { get; set; } = 0;
        public double UnitPrice { get; set; } = 0;
        public double TotalAmount { get; set; } = 0;

        public long SupplierId { get; set; }
        public long OrderId { get; set; }
        public string SupplierName { get; set; }
        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }

        public long TranId { get; set; }

        public long DepartmentId { get; set; }
        public string DepartmentName { get; set; }

        public IEnumerable<SelectListItem> CategoryLookUp { get; set; }
        public IEnumerable<SelectListItem> ItemLookUp { get; set; }
        public IEnumerable<SelectListItem> LedgerLookUp { get; set; }
        public IEnumerable<SelectListItem> DepartmentLookUp { get; set; }

        public long? LedgerId { get; set; }
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public string TranNo { get; set; }
        public DateTime TranDate { get; set; }
        public string StrTranDate { get; set; }
    }
}
