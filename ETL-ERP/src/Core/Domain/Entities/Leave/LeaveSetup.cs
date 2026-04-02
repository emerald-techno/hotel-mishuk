using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Leave
{
    public class LeaveSetup : IAuditable
    {
        public long Id { get; set; }

        [Required]
        [StringLength(1)]
        public string LeaveLimitType { get; set; }  // Y = Year, L = Job Life
        public int LeaveBalance { get; set; } = 0; // If 0 No limit
        public int MaxLeave { get; set; } = 0; // Max leave at a time 
        public int MinLeave { get; set; } = 0;
        public bool IsCarryForward { get; set; }
        public int MaxCarryForward { get; set; } = 0; // 0 Means No Limit
        public int LeaveAfter { get; set; } = 0; // If 0 then from join date
        public DateTime ActiveDate { get; set; }

        [Required]
        [StringLength(1)]
        public string Gender { get; set; }// A = All, F = Female
        public bool IsActive { get; set; }
        public DateTime ActionDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }

        //----- FK ------

        public long LeaveTypeId { get; set; }
        public LeaveType LeaveType { get; set; }
        public long ActionById { get; set; }
        public ApplicationUser ActionBy { get; set; }
        public long? UpdatedById { get; set; }
        public ApplicationUser UpdatedBy { get; set; }

    }
}
