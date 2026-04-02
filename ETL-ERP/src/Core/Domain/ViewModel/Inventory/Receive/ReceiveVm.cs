using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Inventory.Receive
{
    public class ReceiveVm
    {
        public long Id { get; set; }

        [Required]
        [StringLength(50)]
        public string TranNo { get; set; }
        public DateTime TranDate { get; set; }

        [StringLength(1)]
        public string TranType { get; set; } // R=Receive, I=Issue, O=Opening, S=Scrap Receive, U = Return, E = Issue Return, [Q=Requisition]

        public string TranFileUrl { get; set; }
        public DateTime? QcDate { get; set; }
        public string QcDesc { get; set; }
        public string QcFileUrl { get; set; }
        public string Remarks { get; set; }
        public long? SupplierId { get; set; }


        public IEnumerable<SelectListItem> SupplierLookUp { get; set; }
        public IEnumerable<SelectListItem> ItemLookUp { get; set; }
        public IEnumerable<SelectListItem> UnitLookUp { get; set; }
        public IEnumerable<SelectListItem> RecvItemUnitLookUp { get; set; }

        public virtual ICollection<ReceiveDtlVm> TranDtls { get; set; }
    }
}
