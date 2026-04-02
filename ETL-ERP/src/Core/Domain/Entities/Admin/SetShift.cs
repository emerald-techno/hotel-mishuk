using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Admin
{
    public class SetShift : IAuditable
    {
        public long Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ShiftName { get; set; }

        [StringLength(15)]
        public string StartTime { get; set; }
        public short WorkingHour { get; set; } = 8; // in Hours

        [StringLength(15)]
        public string LunchTime { get; set; }
        public short LunchHour { get; set; } = 60; // in Minutes

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
