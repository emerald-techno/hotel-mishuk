using Domain.Entities.HR;
using Domain.Entities.Identity;

namespace Domain.Entities.Leave
{
    public class LeaveCf
    {
        public long Id { get; set; }
        public int LeaveYear { get; set; } // Default Current Year
        public int LeaveBalance { get; set; } = 0; // If 0 No limit
        public int CfBalance { get; set; } = 0; // Carry Forwarded Leave
        public int LeaveEnjoyed { get; set; } = 0;
        public int LeaveSale { get; set; } = 0;
        public DateTime LastActionDate { get; set; }
        public DateTime ActionDate { get; set; }
        public bool IsDeleted { get; set; }

        //----- FK ------

        public long LeaveTypeId { get; set; }
        public LeaveType LeaveType { get; set; }
        public long EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public long ActionById { get; set; }
        public ApplicationUser ActionBy { get; set; }
    }
}
