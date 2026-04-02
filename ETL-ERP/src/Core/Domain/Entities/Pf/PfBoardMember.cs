using Domain.Entities.HR;
using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Pf
{
    public class PfBoardMember : IAuditable
    {
        public long Id { get; set; }

        [Required]
        [StringLength(60)]
        public string Name { get; set; }

        [StringLength(60)]
        public string MemberRole { get; set; }
        public short SlNo { get; set; } // Default = 1
        public DateTime JoinDate { get; set; }

        [StringLength(160)]
        public string PhotoUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime? DisableDate { get; set; }

        [StringLength(160)]
        public string Remarks { get; set; }

        //-----------------------------------------
        public long? EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public long ActionById { get; set; }
        public ApplicationUser ActionBy { get; set; }
        public DateTime ActionDate { get; set; }

        public long? UpdatedById { get; set; }
        public ApplicationUser UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }

        public bool IsDeleted { get; set; }
    }
}
