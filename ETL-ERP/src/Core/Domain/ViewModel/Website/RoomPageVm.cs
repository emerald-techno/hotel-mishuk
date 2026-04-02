using Domain.Entities.HotelManagement;
using Domain.Enums.AppEnums;
using Domain.ViewModel.HotelManagement.RoomCategory;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Website;

public class RoomPageVm
{
    public string StrFromDate { get; set; }
    public string StrToDate { get; set; }
    public int? RoomCount { get; set; }
    public long? RoomCategoryId { get; set; }

    public ICollection<HtRoomCategoryVm> RoomCategories { get; set; }
}

public class MakeReservationVm
{
    public DateTime ArrivalDate { get; set; }
    public DateTime DepartureDate { get; set; }
    public double Adult { get; set; }
    public double Child { get; set; }
    public OnlineBookingStatusEnum Status { get; set; }

    [Required]
    [StringLength(200)]
    public string GuestName { get; set; }

    [Required]
    [StringLength(30)]
    public string GuestMobile { get; set; }

    [StringLength(400)]
    public string GuestAddress { get; set; }
    public double Rent { get; set; }
    public int RoomCount { get; set; }
    public long RoomCategoryId { get; set; }
    public string RoomCategoryName { get; set; }
    public double RoomCategoryRent { get; set; }
    public double RoomCategorySc { get; set; }
    public bool IsDeleted { get; set; }
}
public class CheckOutVm
{
    public long Id { get; set; }
    public string OnlineBookingNumber { get; set; }
    public DateTime OnlineBookingDate { get; set; }
    public DateTime ArrivalDate { get; set; }
    public DateTime DepartureDate { get; set; }
    public double Adult { get; set; }
    public double Child { get; set; }
    public OnlineBookingStatusEnum Status { get; set; }
    public string GuestName { get; set; }
    public string GuestMobile { get; set; }
    public string GuestAddress { get; set; }
    public double Rent { get; set; }
    public double ServiceCharge { get; set; }
    public int RoomCount { get; set; }
    public long RoomCategoryId { get; set; }
    public HtRoomCategory RoomCategory { get; set; }
}

public class RoomFacilityVm
{
    public string FacilityName { get; set; }
    public string CategoryName { get; set; }
    public long CategoryId { get; set; }
    public string RoomNo { get; set; }
}
