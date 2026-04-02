using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Admin
{
    public class SetCountry
    {
        public long Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(50)]
        public string AbbrName { get; set; }

        [StringLength(50)]
        public string Code { get; set; }

        public bool IsDeleted { get; set; }

    }
}
