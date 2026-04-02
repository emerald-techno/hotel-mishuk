using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Admin.SetHoliday
{
    public class SetHolidaySearchVm
    {
        public long Id { get; set; }
        public int HolidayYear { get; set; }
        public string HolidayName { get; set; }
        public string Type { get; set; } // M=Mandatory, O=Optional
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public short Length { get; set; } = 1;
        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
        public IEnumerable<SelectListItem> YearLookUp { get; set; }
    }
}
