using Domain.Entities.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Hr.HrSetting
{
    public class HrSettingVm
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Office Start Time required")]
        [StringLength(15)]
        [DisplayName("Office Start Time: ")]
        public string OfficeStartTime { get; set; }

        [Required(ErrorMessage = "Office End Time required")]
        [StringLength(15)]
        [DisplayName("Office End Time: ")]
        public string OfficeEndTime { get; set; }

        [Required(ErrorMessage = "Holiday One required")]
        [StringLength(20)]
        [DisplayName("Holiday One: ")]
        public string HolidayOne { get; set; }

        [StringLength(20)]
        [DisplayName("Holiday Two: ")]
        public string HolidayTwo { get; set; }

        [Required(ErrorMessage = "Holiday One Type required")]
        [StringLength(1)]
        [DisplayName("Holiday One Type: ")]
        public string HolidayOneType { get; set; } // F=FULL DAY, H=HALF DAY
        public string HolidayOneTypeText => HolidayOneType switch { "F" => "FULL DAY", "H" => "HALF DAY", _ => "__" };


        [StringLength(1)]
        [DisplayName("Holiday Two Type: ")]
        public string HolidayTwoType { get; set; } // F=FULL DAY, H=HALF DAY
        public string HolidayTwoTypeText => HolidayTwoType switch { "F" => "FULL DAY", "H" => "HALF DAY", _ => "__" };

        [DisplayName("Is Special Time: ")]
        public bool IsSpecialTime { get; set; }

        [StringLength(15)]
        [DisplayName("Special Office Start Time: ")]
        public string SpOfficeStartTime { get; set; }

        [StringLength(15)]
        [DisplayName("Special Office End Time: ")]
        public string SpOfficeEndTime { get; set; }


        public long? ActionById { get; set; }
        public ApplicationUser ActionBy { get; set; }
        public DateTime ActionDate { get; set; }
        public long? UpdatedById { get; set; }
        public ApplicationUser UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public IEnumerable<SelectListItem> HolidayLookUp { get; set; }
    }
}
