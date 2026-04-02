using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Leave
{
    public class LeaveType : IAuditable
    {
        public long Id { get; set; }

        [Required]
        public string TypeName { get; set; }

        public int Balance { get; set; }

        [StringLength(120)]
        public string Desc { get; set; }

        [Required]
        [StringLength(1)]
        public string Gender { get; set; } // A = All, F = Female

        //-----------------------------------------
        public ApplicationUser ActionBy { get; set; }
        public DateTime ActionDate { get; set; }
        public long ActionById { get; set; }

        public ApplicationUser UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public long? UpdatedById { get; set; }
        public bool IsDeleted { get; set; }
    }
}
