using Domain.ModelInterface;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.HotelManagement.RoomInfo;

public class HtRoomInfoSearchVm : IDataTableSearch
{
    public long Id { get; set; }
    public string RoomNo { get; set; }
    public string RoomInformation { get; set; }
    public bool IsAc { get; set; }
    public bool IsBelcony { get; set; }
    public string BedInfo { get; set; }
    public short NumberOfBed { get; set; }
    public short Person { get; set; }
    public string OtherInfo { get; set; }
    public string RoomSize { get; set; }
    public double Rent { get; set; }
    public double Vat { get; set; }
    public double ServiceCharge { get; set; }
    public double TotalRent { get; set; }
    public int? BookingStatus { get; set; }
    public string BookingStatusText => BookingStatus switch { 0 => "Available", 1 => "Booked", _ => "" };
    public int? CleaningStatus { get; set; }
    public string CleaningStatusText => CleaningStatus switch { 0 => "Vacant & Clean", 1 => "Vacant & Dirty", 2 => "Occupied", 3 => "Check Out", 4 => "Out Of Order", _ => "" };
    public int? HouseKeeperAvailabilityStatus { get; set; }
    public string AvailabilityStatusText => HouseKeeperAvailabilityStatus switch { 0 => "Available", 1 => "Occupied", 2 => "Out Of Order", 3 => "Checked-Out", _ => "" };
    public string Remakrs { get; set; }
    public string PhotoUrl { get; set; }
    public int BookingCount { get; set; }
    public int AvailableCount { get; set; }
    public int VcCount { get; set; }
    public int OccCount { get; set; }
    public int VdCount { get; set; }
    public int CoCount { get; set; }
    public long? HouseKeeperAssignId { get; set; }
    public string HouseKeeperName { get; set; }
    public int? HouseKeeperStatus { get; set; }
    public string HouseKeeperStatusText => HouseKeeperStatus switch { 0 => "Assinged", 1 => "Running", 2 => "Completed", 3 => "Cancel", _ => "" };

    // --- Fk ---

    public long RoomCategoryId { get; set; }
    public string RoomCategoryName { get; set; }
    public long FloorId { get; set; }
    public string FloorName { get; set; }
    public long? BedTypeId { get; set; }
    public long? HouseKeeperId { get; set; }
    public string BedTypeName { get; set; }


    // -- DataTable Propertry

    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }

    public IEnumerable<SelectListItem> RoomCategoryLookUp { get; set; }
    public IEnumerable<SelectListItem> FloorLookUp { get; set; }
    public IEnumerable<SelectListItem> BedTypeLookUp { get; set; }
    public IEnumerable<SelectListItem> BookedStatusLookUp { get; set; }
    public IEnumerable<SelectListItem> CleanStatusLookUp { get; set; }
    public IEnumerable<SelectListItem> HouseKeeperLookUp { get; set; }
    public IEnumerable<SelectListItem> AvailabilityStatusLookUp { get; set; }
}
