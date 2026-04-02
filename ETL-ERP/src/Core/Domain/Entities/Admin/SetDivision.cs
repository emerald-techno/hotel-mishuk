using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Admin
{
    public class SetDivision : IAuditable
    {
        public long Id { get; set; }

        [Required]
        [StringLength(60)]
        public string Name { get; set; }

        [StringLength(60)]
        public string NameBn { get; set; }

        [StringLength(30)]
        public string Code { get; set; }


        //-------------------FK------------------------
        public long CountryId { get; set; }
        public SetCountry Country { get; set; }

        public ApplicationUser ActionBy { get; set; }
        public long ActionById { get; set; }
        public DateTime ActionDate { get; set; }

        public ApplicationUser UpdatedBy { get; set; }
        public long? UpdatedById { get; set; }

        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
