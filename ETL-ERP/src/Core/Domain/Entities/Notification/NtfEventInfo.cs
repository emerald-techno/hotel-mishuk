using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Notification;

public class NtfEventInfo : IAuditable
{
    public long Id { get; set; }

    [Required]
    [StringLength(100)]
    public string EventName { get; set; }

    [Required]
    [StringLength(100)]
    public string EventCode { get; set; }

    [Required]
    [StringLength(1)]
    public string EventType { get; set; } // P=PUBLIC, E=EMPLOYEE, A=ADMIN

    public short NotifyBefore { get; set; }
    public bool IsEmailNtf { get; set; }
    public bool IsSmsNtf { get; set; }

    [StringLength(350)]
    public string Remarks { get; set; }

    //-----------------------------------------
    public ApplicationUser ActionBy { get; set; }
    public DateTime ActionDate { get; set; }
    public long ActionById { get; set; }

    public ApplicationUser UpdatedBy { get; set; }
    public DateTime? UpdateDate { get; set; }
    public long? UpdatedById { get; set; }
    public bool IsDeleted { get; set; }
}
