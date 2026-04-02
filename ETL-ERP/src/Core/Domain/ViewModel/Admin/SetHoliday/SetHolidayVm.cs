using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Admin.SetHoliday
{
    public class SetHolidayVm
    {
        public long Id { get; set; }
        public int HolidayYear { get; set; }

        [Required(ErrorMessage = "Holiday name is required")]
        [StringLength(80, ErrorMessage = "Holiday name can not exceed 80 characters")]
        [Remote(action: "IsNameExist", controller: "SetHoliday", AdditionalFields = $"InitName,{nameof(HolidayYear)}")]
        public string HolidayName { get; set; }

        [Required(ErrorMessage = "Type is required")]
        [StringLength(1, ErrorMessage = "Type can not exceed 1 character")]
        public string Type { get; set; } // M=Mandatory, O=Optional

        [DisplayName("Start Date: ")]
        public DateTime StartDate { get; set; }
        public string StartDateStr { get; set; }

        [DisplayName("End Date: ")]
        public DateTime EndDate { get; set; }
        public string EndDateStr { get; set; }
        public short Length { get; set; }

        public IEnumerable<SelectListItem> TypeLookUp { get; set; }
        public IEnumerable<SelectListItem> YearLookUp { get; set; }

    }
}
