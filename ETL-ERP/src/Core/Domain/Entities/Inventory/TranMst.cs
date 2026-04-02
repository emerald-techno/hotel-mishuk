using Domain.Entities.Admin;
using Domain.Entities.HotelManagement;
using Domain.Entities.HR;
using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Inventory;

public class TranMst : IAuditable
{
    public long Id { get; set; }

    [Required]
    [StringLength(50)]
    public string TranNo { get; set; }
    public DateTime TranDate { get; set; }

    [Required]
    [StringLength(1)]
    public string TranType { get; set; } // R=Receive, I=Issue, O=Opening, S=Scrap Receive, U = Return, E = Issue Return, Q=Requisition, D=Damage, C=Consumption

    [StringLength(120)]
    public string TranFileUrl { get; set; }
    public DateTime? QcDate { get; set; }

    [StringLength(250)]
    public string QcDesc { get; set; }

    [StringLength(120)]
    public string QcFileUrl { get; set; }

    [StringLength(350)]
    public string Remarks { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime ReportDate { get; set; }

    [StringLength(250)]
    public string AuditRemarks { get; set; }
    public DateTime? AuditDate { get; set; }

    // ---- FK ----

    public long? RefTranId { get; set; }
    public virtual TranMst RefTran { get; set; }
    public long? OrderId { get; set; }
    public virtual OrderMst Order { get; set; }
    public long? SupplierId { get; set; }
    public virtual SupplierInfo Supplier { get; set; }
    public long TranById { get; set; }
    public virtual ApplicationUser TranBy { get; set; }
    public long? IssueDeptId { get; set; }
    public virtual Department IssueDept { get; set; }
    public long? IssueEmpId { get; set; }
    public virtual Employee IssueEmp { get; set; }
    public long? QcById { get; set; }
    public virtual Employee QcBy { get; set; }
    public long? ReqMstId { get; set; }
    public virtual RequsitionInfo ReqMst { get; set; }
    public long? IssueRoomId { get; set; }
    public virtual HtRoomInfo IssueRoom { get; set; }
    public long ActionById { get; set; }
    public virtual ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public virtual ApplicationUser UpdatedBy { get; set; }
    public long? LedgerId { get; set; }
    public long? AuditById { get; set; }
    public ApplicationUser AuditBy { get; set; }
    public virtual ICollection<TranDtl> TranDtls { get; set; }
}
