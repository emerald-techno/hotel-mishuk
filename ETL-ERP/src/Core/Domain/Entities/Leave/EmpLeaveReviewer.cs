using Domain.Entities.HR;
using Domain.Entities.Identity;
using Domain.ModelInterface;

namespace Domain.Entities.Leave
{
    public class EmpLeaveReviewer : IAuditable
    {
        public long Id { get; set; }
        public short ReviewFor { get; set; } // 1=Leave, 2=Loan, 3=PF                                           
        public short SlNo { get; set; } // 1=1ST APPROVER, 2=2ND APPROVER, 3=3RD APPROVER,... 99=FINAL APPROVER
        public bool IsFinalReviewer { get; set; }
        public DateTime ActionDate { get; set; }
        public DateTime? UpdateDate { get; set; }

        public bool IsDeleted { get; set; }


        // ---- FK ----

        public long EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public long ReviewerId { get; set; }
        public ApplicationUser Reviewer { get; set; }
        public long AltReviewerId { get; set; }
        public ApplicationUser AltReviewer { get; set; }
        public long ActionById { get; set; }
        public ApplicationUser ActionBy { get; set; }
        public long? UpdatedById { get; set; }
        public ApplicationUser UpdatedBy { get; set; }
    }
}
