using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Attendance;

public class DutyShift : IAuditable
{
    public long Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string ShiftName { get; set; }

    [Required]
    [StringLength(20)]
    public string ShiftCode { get; set; }
    
    [Required]
    [StringLength(1)]
    public string ShiftType { get; set; } // P=Permanent, D=Duty.. Default=P
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    [Required]
    [StringLength(1)]
    public string PayType { get; set; } // M=Monthly, S=ShiftWise.. Default=M

    public DateTime ActionDate { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? UpdateDate { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public ApplicationUser UpdatedBy { get; set; }
}
