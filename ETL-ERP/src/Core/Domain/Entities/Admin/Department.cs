using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Admin
{
    public class Department : IAuditable
    {
        public long Id { get; set; }

        [StringLength(120)]
        [Required]
        public string Name { get; set; }

        [StringLength(100)]
        [Required]
        public string Code { get; set; }

        [StringLength(120)]
        public string Remarks { get; set; }

        //------------------FK-------------------------
        public ApplicationUser ActionBy { get; set; }
        public long ActionById { get; set; }
        public DateTime ActionDate { get; set; }

        public ApplicationUser UpdatedBy { get; set; }
        public long? UpdatedById { get; set; }
        public DateTime? UpdateDate { get; set; }

        public bool IsDeleted { get; set; }
        public bool IsAcademic { get; set; }
    }
}
