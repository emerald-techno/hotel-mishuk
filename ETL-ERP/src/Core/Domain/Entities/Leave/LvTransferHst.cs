using System.ComponentModel.DataAnnotations;
using Domain.Entities.Identity;

namespace Domain.Entities.Leave
{
    public class LvTransferHst
    {
        public long Id { get; set; }
        public DateTime TransferDate { get; set; }

        [StringLength(200)]
        public string Remarks { get; set; }
        public DateTime ActionDate { get; set; }
        public bool IsDeleted { get; set; }

        // ---- FK ----

        public long LvReviewerId { get; set; }
        public LvAppReviewer LvReviewer { get; set; }
        public long ReviewerId { get; set; }
        public ApplicationUser Reviewer { get; set; }
        public long PreReviewerId { get; set; }
        public ApplicationUser PreReviewer { get; set; }
        public long TransferById { get; set; }
        public ApplicationUser TransferBy { get; set; }

    }
}
