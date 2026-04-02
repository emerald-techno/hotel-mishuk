using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Accounting
{
    public class AccGroup : IAuditable
    {
        public long Id { get; set; }

        [Required]
        [StringLength(60)]
        public string Name { get; set; }

        [Required]
        [StringLength(60)]
        public string GroupCode { get; set; }

        [Required]
        [StringLength(40)]
        public string GroupShortCode { get; set; }
        public short SlNo { get; set; }

        [StringLength(120)]
        public string Remarks { get; set; }

        //-------------------FK------------------------------
        public ApplicationUser ActionBy { get; set; }
        public DateTime ActionDate { get; set; }
        public long ActionById { get; set; }

        public ApplicationUser UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public long? UpdatedById { get; set; }
        public bool IsDeleted { get; set; }

    }
}
