using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.HR
{
    public class EmpEducation : IAuditable
    {
        public long Id { get; set; }

        [Required]
        [StringLength(60)]
        public string CourseName { get; set; }

        [Required]
        [StringLength(120)]
        public string InstitutionName { get; set; }

        [Required]
        [StringLength(100)]
        public string BoardUniversity { get; set; }

        public short? YearFrom { get; set; }
        public short? YearTo { get; set; }
        public short? PassingYear { get; set; }

        [StringLength(35)]
        public string SubjectGroup { get; set; }

        [StringLength(30)]
        public string Result { get; set; }

        [StringLength(120)]
        public string CertificateUrl { get; set; }

        public short SlNo { get; set; }

        [StringLength(350)]
        public string Remarks { get; set; }


        //---------------------FK------------------
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
