using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Accounting
{
    public class AccReviewer : IAuditable
    {
        public long Id { get; set; }

        [Required]
        [StringLength(1)]
        public string ReviewerType { get; set; } //A = Approver, R = Reconcile, U = Audit
        public short SlNo { get; set; }

        [StringLength(120)]
        public string Remarks { get; set; }

        //-----------------FK-----------------------
        public long ReviewrId { get; set; }
        public AccReviewer Reviewer { get; set; }

        public ApplicationUser ActionBy { get; set; }
        public DateTime ActionDate { get; set; }
        public long ActionById { get; set; }

        public ApplicationUser UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public long? UpdatedById { get; set; }
        public bool IsDeleted { get; set; }
    }
}
