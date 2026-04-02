using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.HR
{
    public class EmpTraining : IAuditable
    {
        public long Id { get; set; }

        [Required]
        [StringLength(100)]
        public string CourseTitle { get; set; }

        [StringLength(100)]
        public string InstituteName { get; set; }

        [StringLength(120)]
        public string Location { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        [StringLength(30)]
        public string Result { get; set; }

        [StringLength(120)]
        public string TrainingFileUrl { get; set; }

        public short SlNo { get; set; }

        [StringLength(350)]
        public string Remarks { get; set; }


        //-----------------------------------------
        public long EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public ApplicationUser ActionBy { get; set; }
        public DateTime ActionDate { get; set; }
        public long ActionById { get; set; }

        public ApplicationUser UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public long? UpdatedById { get; set; }
        public bool IsDeleted { get; set; }
    }
}
