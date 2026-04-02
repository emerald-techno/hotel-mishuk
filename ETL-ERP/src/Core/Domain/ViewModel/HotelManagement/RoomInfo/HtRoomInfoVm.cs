using Domain.Enums.AppEnums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.HotelManagement.RoomInfo;

public class HtRoomInfoVm
{
    public long Id { get; set; }

    [Required]
    [StringLength(25)]
    [Remote(action: "IsRoomNoExist", controller: "RoomInfo", AdditionalFields = "InitRoomNo")]
    public string RoomNo { get; set; }

    [StringLength(500)]
    public string RoomInformation { get; set; }
    public bool IsAc { get; set; }
    public bool IsBelcony { get; set; }

    [StringLength(35)]
    public string BedInfo { get; set; }
    public short NumberOfBed { get; set; }
    public short Person { get; set; }

    [StringLength(150)]
    public string OtherInfo { get; set; }

    [StringLength(30)]
    public string RoomSize { get; set; }
    public double Rent { get; set; }
    public double Vat { get; set; }
    public double ServiceCharge { get; set; }
    public double TotalRent { get; set; }

    public BookingStatusEnum BookingStatus { get; set; }
    public CleaningStatusEnum CleaningStatus { get; set; }
    public string CleaningStatusText => (int)CleaningStatus switch { 0 => "Vacant & Clean", 1 => "Vacant & Dirty", 2 => "Occupied", 3 => "Check Out", 4 => "Out Of Order", _ => "" };

    [StringLength(120)]
    public string Remakrs { get; set; }

    [StringLength(120)]
    public string PhotoUrl { get; set; }
    public bool IsActive { get; set; }

    // --- Fk ---

    [Required(ErrorMessage = "Room category is required")]
    public long RoomCategoryId { get; set; }
    public string RoomCategoryName { get; set; }

    [Required(ErrorMessage = "Floor category is required")]
    public long FloorId { get; set; }
    public string FloorName { get; set; }
    public long? BedTypeId { get; set; }
    public string BedTypeName { get; set; }

    public IEnumerable<SelectListItem> RoomCategoryLookUp { get; set; }
    public IEnumerable<SelectListItem> FloorLookUp { get; set; }
    public IEnumerable<SelectListItem> BedTypeLookUp { get; set; }
    public ICollection<SaveRoomFacilityCategoryVm> RoomFacilityLookup { get; set; }
    public ICollection<SaveRoomFacilityMapVm> RoomFacilityMaps { get; set; }
}

public class SaveRoomFacilityCategoryVm
{
    public long FacilityCategoryId { get; set; }
    public string FacilityCategoryName { get; set; }
    public ICollection<SaveRoomFacilityVm> SaveRoomFacilities { get; set; }
}

public class SaveRoomFacilityVm
{
    public long FacilityId { get; set; }
    public string FacilityName { get; set; }
}

public class SaveRoomFacilityMapVm
{
    public long FacilityId { get; set; }
    public string FacilityName { get; set; }
}

public class DayWiseRoomInfoVm
{
    public long RoomId { get; set; }
    public string RoomNo { get; set; }
    public bool IsAc { get; set; }
    public bool IsBelcony { get; set; }
    public short NumberOfBed { get; set; }
    public short Person { get; set; }
    public double Rent { get; set; }
    public double Vat { get; set; }
    public double ServiceCharge { get; set; }
    public double TotalRent { get; set; }
    public double? ExtraBed { get; set; }
    public short BookingStatus { get; set; } // 1=Available, 2=Booked, 3=Occupied
    public string BookingStatusText => BookingStatus switch { 1 => "Available", 2 => "Booked", 3 => "Occupied", _ => "N/A" };
    public long RoomCategoryId { get; set; }
    public long? AssignedHouseKeeperId { get; set; }
    public string RoomCategoryName { get; set; }
    public long FloorId { get; set; }
    public string FloorName { get; set; }
    public string GuestName { get; set; }
    public string GuestMobile { get; set; }
    public DateTime? CheckInDate { get; set; }
    public DateTime? CheckOutDate { get; set; }
    public long BookingId { get; set; } = 0;
    public short CleaningStatus { get; set; }
    public string CleaningStatusText { get; set; }
    public string OOORemarks { get; set; }
    public short Status { get; set; }
    public string StatusText { get; set; }
    public bool IsTodayCheckout { get; set; }
}