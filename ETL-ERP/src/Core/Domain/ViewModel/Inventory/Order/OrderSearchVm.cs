using Domain.Entities.Inventory;
using Domain.ModelInterface;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Inventory.Order
{
    public class OrderSearchVm : IDataTableSearch
    {
        public long Id { get; set; }
        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public short Status { get; set; } // 0=FRESH, 1 = REVIEW, 2=REJECTED, 3 = APPROVED

        public string StatusText => Status switch { 0 => "FRESH", 1 => "REVIEW", 2 => "REJECTED", 3 => "APPROVED", _ => "--" };

        public short ReceiveStatus { get; set; } // 0=NOT, 1=PARTIAL, 2=FULL, 3=FOURCE FULL
        public string OrderType { get; set; } // I=Item, S=Service
        public string Remarks { get; set; }
        public string ReqNo { get; set; }
        public DateTime? ReqDate { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public DateTime? CompleteDate { get; set; }
        public string CompleteRemarks { get; set; }

        public string SFromDate { get; set; }
        public string SToDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public virtual ICollection<OrderDtl> OrderDetails { get; set; }
        public long SupplierId { get; set; }
        public long? ReqId { get; set; }
        public string SupplierName { get; set; }
        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
        public IEnumerable<SelectListItem> SupplierLookUp { get; set; }
        public IEnumerable<SelectListItem> RequstionLookup { get; set; }
    }
}
