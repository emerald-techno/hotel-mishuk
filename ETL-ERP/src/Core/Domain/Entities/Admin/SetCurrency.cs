using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Admin
{
    public class SetCurrency : IAuditable
    {
        public long Id { get; set; }

        [Required]
        [StringLength(50)]
        public string CurrencyName { get; set; }

        [Required]
        [StringLength(15)]
        public string Symbol { get; set; }

        [Required]
        [StringLength(15)]
        public string CountrySymbol { get; set; }
        public double ExRate { get; set; } = 1;

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
