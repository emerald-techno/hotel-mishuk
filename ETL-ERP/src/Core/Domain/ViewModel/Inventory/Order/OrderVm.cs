using Domain.Entities.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Inventory.Order
{
    public class OrderVm
    {
        public long Id { get; set; }

        [Required]
        public string OrderNo { get; set; }

        [Required]
        public string OrderDateStr { get; set; }
        public DateTime OrderDate { get; set; }

        public string DeliveryDeadlineStr { get; set; }
        public DateTime? DeliveryDeadline { get; set; }

        public DateTime? CompleteDate { get; set; }

        public string CompleteRemarks { get; set; }

        public string Remarks { get; set; }
        public string OrderType { get; set; } // I=Item, S=Service
        public short Status { get; set; }
        public long SupplierId { get; set; }
        public string SupplierName { get; set; }
        public long? ReqId { get; set; }
        public string RequsitionNo { get; set; }
        public long? CsId { get; set; }
        public long? ItemId { get; set; }
        public long? UnitId { get; set; }

        public long ActionById { get; set; }
        public virtual ApplicationUser ActionBy { get; set; }

        public IEnumerable<SelectListItem> SupplierLookUp { get; set; }
        public IEnumerable<SelectListItem> RequsitionLookUp { get; set; }
        public IEnumerable<SelectListItem> CsLookUp { get; set; }
        public IEnumerable<SelectListItem> ItemLookUp { get; set; }
        public IEnumerable<SelectListItem> UnitLookUp { get; set; }
        public IEnumerable<SelectListItem> OrderTypeLookUp { get; set; }

        public string StatusText => Status switch { 0 => "FRESH", 1 => "REVIEW", 2 => "REJECTED", 3 => "APPROVED", _ => "--" };
        public short ReceiveStatus { get; set; } // 0=NOT, 1=PARTIAL, 2=FULL, 3=FOURCE FULL
        public string ReceiveStatusText => ReceiveStatus switch { 0 => "Not Receive", 1 => "PARTIAL", 2 => "FULL", 3 => "FOURCE FULL", _ => "--" };

        public virtual ICollection<OrderDtlVm> OrderDtls { get; set; }
        public string SupplierMobile { get; set; }
        public string SupplierAddress { get; set; }
    }
}
