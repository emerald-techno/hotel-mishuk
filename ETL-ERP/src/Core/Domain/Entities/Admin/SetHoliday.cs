using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Admin
{
    public class SetHoliday : IAuditable
    {
        public long Id { get; set; }

        [MaxLength(10)]
        public int HolidayYear { get; set; }

        [Required]
        [StringLength(80)]
        public string HolidayName { get; set; }

        [Required]
        [StringLength(1)]
        public string Type { get; set; } // M=Mandatory, O=Optional
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public short Length { get; set; } = 1;

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
