using System.ComponentModel.DataAnnotations;
using Domain.Entities.Identity;

namespace Domain.Entities.HR
{
    public class HrSetting
    {
        public long Id { get; set; }

        [Required]
        [StringLength(15)]
        public string OfficeStartTime { get; set; }

        [Required]
        [StringLength(15)]
        public string OfficeEndTime { get; set; }

        [Required]
        [StringLength(20)]
        public string HolidayOne { get; set; }

        [StringLength(20)]
        public string HolidayTwo { get; set; }

        [Required]
        [StringLength(1)]
        public string HolidayOneType { get; set; } // F=FULL DAY, H=HALF DAY

        [StringLength(1)]
        public string HolidayTwoType { get; set; } // F=FULL DAY, H=HALF DAY
        public bool IsSpecialTime { get; set; }

        [StringLength(15)]
        public string SpOfficeStartTime { get; set; }

        [StringLength(15)]
        public string SpOfficeEndTime { get; set; }


        public long? ActionById { get; set; }
        public ApplicationUser ActionBy { get; set; }
        public DateTime ActionDate { get; set; }
        public long? UpdatedById { get; set; }
        public ApplicationUser UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
