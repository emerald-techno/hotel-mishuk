using Domain.Entities.HR;
using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.HouseKeeping;

public class HkTaskAssign : IAuditable
{
    public long Id { get; set; }
    public DateTime AssignDate { get; set; }
    public DateTime? CompleteDate { get; set; }

    [StringLength(120)]
    public string AssignRemarks { get; set; }

    [StringLength(120)]
    public string CompleteRemarks { get; set; }

    [StringLength(120)]
    public string AuditorRemarks { get; set; }
    public DateTime? AuditDate { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }

    // --- Fk ---

    public long AssignId { get; set; }
    public HkRoomAssign Assign { get; set; }
    public long TaskId { get; set; }
    public HkTaskName Task { get; set; }
    public long? AuditorId { get; set; }
    public Employee Auditor { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public ApplicationUser UpdatedBy { get; set; }
}
