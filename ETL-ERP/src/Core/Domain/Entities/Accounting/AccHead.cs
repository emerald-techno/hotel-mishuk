using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Accounting
{
    public class AccHead : IAuditable
    {

        public long Id { get; set; }
        public long ParentHeadId { get; set; }
        public int LevelId { get; set; }

        [Required]
        [StringLength(80)]
        public string HeadName { get; set; }

        [Required]
        [StringLength(50)]
        public string HeadCode { get; set; }

        [StringLength(120)]
        public string Description { get; set; }

        public bool BudgetHead { get; set; }
        public bool AssetHead { get; set; }
        public bool PettyHead { get; set; }
        public bool BankHead { get; set; }

        [StringLength(120)]
        public string Remarks { get; set; }
        public long HeadGroupCode { get; set; }

        //-----------------FK------------------------
        public long GroupId { get; set; }
        public AccGroup Group { get; set; }

        public ApplicationUser ActionBy { get; set; }
        public DateTime ActionDate { get; set; }
        public long ActionById { get; set; }

        public ApplicationUser UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public long? UpdatedById { get; set; }
        public bool IsDeleted { get; set; }
    }
}
