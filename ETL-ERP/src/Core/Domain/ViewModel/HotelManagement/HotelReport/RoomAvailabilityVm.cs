using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.HotelManagement.HotelReport;

public class RoomAvailabilityVm
{
    public string FromDateStr { get; set; }
    public string ToDateStr { get; set; }
    public long CategoryId { get; set; }
    public IEnumerable<SelectListItem> CategoryLookup { get; set; }
}