using Domain.Entities.Admin;
using Domain.Entities.HR;
using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Inventory;

public class RequsitionInfo : IAuditable
{
    public long Id { get; set; }

    [Required]
    [StringLength(30)]
    public string ReqNo { get; set; }
    public DateTime ReqDate { get; set; }

    [Required]
    [StringLength(1)]
    public string Priority { get; set; } // N=Normal, H=High, A=Argent
    public short Status { get; set; } // 0=FRESH, 1 = REVIEW, 2=REJECTED, 3 = APPROVED
    public DateTime? ApprovedDate { get; set; }
    public short IsStatus { get; set; } // 0=NOT, 1= PARTIAL, 2 = FULL, 3 FOURCE FULL –Issue Status

    [StringLength(120)]
    public string Remarks { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }


    //--- FK ---

    public long? ReqById { get; set; }
    public Employee ReqBy { get; set; }
    public long? DeptId { get; set; }
    public Department Dept { get; set; }
    public long SubmitById { get; set; }
    public ApplicationUser SubmitBy { get; set; }
    public long? ApprovedById { get; set; }
    public ApplicationUser ApprovedBy { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public ApplicationUser UpdatedBy { get; set; }
    public virtual ICollection<RequsitionInfoDtl> RequsitionInfoDtls { get; set; }
}
