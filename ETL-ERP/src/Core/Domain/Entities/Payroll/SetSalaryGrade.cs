using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Payroll
{
    public class SetSalaryGrade : IAuditable
    {
        public long Id { get; set; }

        [Required]
        [StringLength(50)]
        public string GradeName { get; set; }
        public double StartingBasic { get; set; }
        public double MaxAmount { get; set; }

        [StringLength(120)]
        public string Remarks { get; set; }

        //-----------------------------------------

        public long ActionById { get; set; }
        public ApplicationUser ActionBy { get; set; }
        public DateTime ActionDate { get; set; }
        public long? UpdatedById { get; set; }
        public ApplicationUser UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
