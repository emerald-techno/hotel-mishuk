using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Admin
{
    public class SetFincYear : IAuditable
    {
        public long Id { get; set; }

        [Required]
        [StringLength(30)]
        public string YearName { get; set; }
        public DateTime YearStartDate { get; set; }
        public DateTime YearEndDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        // --- Fk ---
        public DateTime ActionDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public long ActionById { get; set; }
        public ApplicationUser ActionBy { get; set; }
        public long? UpdatedById { get; set; }
        public ApplicationUser UpdatedBy { get; set; }
    }
}
