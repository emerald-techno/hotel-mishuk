using Domain.Entities.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Domain.ViewModel.Inventory.Transaction;

public class TransactionVm
{
    public long Id { get; set; }

    [Required]
    [StringLength(50)]
    public string TranNo { get; set; }
    public long TranById { get; set; }
    public virtual ApplicationUser TranBy { get; set; }
    public string TranByName { get; set; }
    public string TranDateStr { get; set; }
    public DateTime TranDate { get; set; }

    [StringLength(1)]
    public string TranType { get; set; }
    public string TranFileUrl { get; set; }
    public DateTime? QcDate { get; set; }
    public string QcDesc { get; set; }
    public string QcFileUrl { get; set; }
    public string Remarks { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }

    public long? RefTranId { get; set; }
    public string ReceiveNo { get; set; }
    public long? ReqMstId { get; set; }
    public string ReqNo { get; set; }
    public DateTime? ReqDate { get; set; }
    public long? OrderId { get; set; }
    public string OrderNo { get; set; }
    public DateTime? OrderDate { get; set; }
    public long? SupplierId { get; set; }
    public string SupplierName { get; set; }
    public string SupplierMobile { get; set; }
    public long? IssueDeptId { get; set; }
    public string IssueDeptName { get; set; }
    public string IssueDeptCode { get; set; }
    public string IssueUserName { get; set; }
    public long? IssueEmpId { get; set; }
    public string IssueEmpName { get; set; }
    public short ReceiveStatus { get; set; }
    public long? IssueRoomId { get; set; }
    public string IssueRoomNo { get; set; }
    public long? LedgerId { get; set; }
    public long? ActionById { get; set; }
    public IEnumerable<SelectListItem> ReqLookUp { get; set; }
    public IEnumerable<SelectListItem> OrderLookUp { get; set; }
    public IEnumerable<SelectListItem> ReceiveLookUp { get; set; }
    public IEnumerable<SelectListItem> SupplierLookUp { get; set; }
    public IEnumerable<SelectListItem> IssueDptLookUp { get; set; }
    public IEnumerable<SelectListItem> IssueEmpLookUp { get; set; }
    public IEnumerable<SelectListItem> OrderReceiveStatusLookup { get; set; }
    public IEnumerable<SelectListItem> UserLookUp { get; set; }
    public IEnumerable<SelectListItem> ItemLookUp { get; set; }
    public IEnumerable<SelectListItem> UnitLookUp { get; set; }
    public IEnumerable<SelectListItem> RecvItemUnitLookUp { get; set; }
    public IEnumerable<SelectListItem> IssueLookUp { get; set; }
    public IEnumerable<SelectListItem> RoomLookUp { get; set; }
    public IEnumerable<SelectListItem> LedgerLookUp { get; set; }
    public IEnumerable<SelectListItem> ItemCategoryLookUp { get; set; }
    public virtual ICollection<TransactionDtlVm> TranDtls { get; set; }

    [NotMapped]
    public bool IsOpenignUpdate { get; set; }

    [NotMapped]
    public bool IsReceiveAutoPay { get; set; }

    [NotMapped]
    public bool IsReqAuto { get; set; }
}
