using System.ComponentModel.DataAnnotations;
using Domain.Entities.Identity;

namespace Domain.Entities.Leave
{
    public class LvAppReviewer
    {
        public long Id { get; set; }
        public short SlNo { get; set; } // 1=1ST APPROVER, 2=2ND APPROVER, 3=3RD APPROVER,... 99=FINAL APPROVER
        public short Status { get; set; } // 0=Not Response, 1=Review, 2= Approve, 3=Reject

        [StringLength(200)]
        public string Remarks { get; set; }
        public DateTime ReceiveTime { get; set; }
        public DateTime? ResponseTime { get; set; }
        public bool IsFinalReviewer { get; set; }
        public bool IsDeleted { get; set; }

        // ---- FK ----
        public long LeaveAppId { get; set; }
        public EmpLeaveApplication LeaveApp { get; set; }
        public long ReviewerId { get; set; }
        public ApplicationUser Reviewer { get; set; }
        public long AltReviewerId { get; set; }
        public ApplicationUser AltReviewer { get; set; }

    }
}
