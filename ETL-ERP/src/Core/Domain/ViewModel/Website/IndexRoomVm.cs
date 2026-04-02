using Domain.Entities.HotelManagement;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Website;

public class IndexRoomVm
{
    public int GuestCount { get; set; }
    public int BookedCount { get; set; }
    public int AvailableCount { get; set; }
    public int OccupiedCount { get; set; }
    public int TodayCheckId { get; set; }
    public int TodayCheckOut { get; set; }
    public ICollection<HtRoomInfo> Rooms { get; set; }
    public string StrQueryDate { get; set; }


    public long? RoomCategoryId { get; set; }
    public int? HouseKeeperStatus { get; set; } 
    public string HouseKeeperStatusText => HouseKeeperStatus switch { 0 => "Assinged", 1 => "Running", 2 => "Completed", 3 => "Cancel", _ => "" };
    public int? CleaningStatus { get; set; } 
    public string CleaningStatusText => CleaningStatus switch { 0 => "Vacant & Clean", 1 => "Vacant & Dirty", 2 => "Occupied", 3 => "Check Out", 4 => "Out Of Order", _ => "" };

    public IEnumerable<SelectListItem> EmployeeLookUp { get; set; }
    public IEnumerable<SelectListItem> RoomCategoryLookUp { get; set; }
    public IEnumerable<SelectListItem> AvailabilityStatusLookUp { get; set; }
    public IEnumerable<SelectListItem> CleanStatusLookUp { get; set; }
    public IEnumerable<SelectListItem> CleanStatusLookUpForUpdate { get; set; }
}
