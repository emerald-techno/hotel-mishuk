using Domain.Entities.Identity;
using Domain.Enums.AppEnums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Accounting;

public class PayModeDetail
{
    public long Id { get; set; }

    [Required]
    [StringLength(250)]
    public string Name { get; set; }

    [Required]
    [StringLength(10)]
    public string Code { get; set; }

    [StringLength(350)]
    public string Remarks { get; set; }
    public PayModeEnum PayMode { get; set; }
    public double ComRate { get; set; } = 0;
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }

    //-------------------FK----------------------

    public long PayLedgerId { get; set; }
    public AccLedger PayLedger { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
}