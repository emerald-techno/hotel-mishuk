using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Notice
{
    public class NoticeInfo : IAuditable
    {
        public long Id { get; set; }
        public DateTime NoticeTime { get; set; } //DateTime

        [StringLength(250)]
        public string Subject { get; set; }
        public string Body { get; set; }

        [StringLength(120)]
        public string FileUrl { get; set; }
        public short ReceiverType { get; set; } // 0=ALL, 1=Employee, 2=Teacher,3=Student
        public DateTime? DisplayStartDate { get; set; }
        public DateTime? DisplayEndDate { get; set; }
        public short Status { get; set; } // 0= Submitted, 1=Approved, 2=Rejected       
        public DateTime? ApprovedDate { get; set; }
        public DateTime ActionDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }


        // === FK ===

        public long? ReceiverId { get; set; }
        public ApplicationUser Receiver { get; set; }
        public long? ApprovedById { get; set; }
        public ApplicationUser ApprovedBy { get; set; }
        public long SenderId { get; set; }
        public ApplicationUser Sender { get; set; }
        public long ActionById { get; set; }
        public ApplicationUser ActionBy { get; set; }
        public long? UpdatedById { get; set; }
        public ApplicationUser UpdatedBy { get; set; }
    }
}
