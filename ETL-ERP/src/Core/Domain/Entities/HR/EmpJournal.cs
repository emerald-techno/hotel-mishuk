using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.HR
{
    public class EmpJournal : IAuditable
    {
        public long Id { get; set; }

        [Required]
        [StringLength(80)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Title { get; set; }

        [StringLength(400)]
        public string About { get; set; }

        [StringLength(120)]
        public string DocUrl { get; set; }
        public DateTime SubmitDate { get; set; }

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
