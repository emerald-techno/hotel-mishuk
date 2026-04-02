using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.HR
{
    public class EmpExperience : IAuditable
    {
        public long Id { get; set; }

        [Required]
        [StringLength(100)]
        public string CompanyName { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        [StringLength(40)]
        public string Designation { get; set; }

        [StringLength(600)]
        public string Responsibility { get; set; }
        public double LastDrawnSalary { get; set; }

        [StringLength(300)]
        public string LeftReason { get; set; }

        [StringLength(120)]
        public string ExperienceUrl { get; set; }

        public short SlNo { get; set; }

        [StringLength(350)]
        public string Remarks { get; set; }



        //-----------------------------------------------
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
