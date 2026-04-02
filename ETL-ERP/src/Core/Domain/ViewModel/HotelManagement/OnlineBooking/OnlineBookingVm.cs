using Domain.Enums.AppEnums;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.HotelManagement.OnlineBooking;

public class OnlineBookingVm
{
    public long Id { get; set; }

    [StringLength(40)]
    public string OnlineBookingNumber { get; set; }
    public DateTime OnlineBookingDate { get; set; }
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

    [StringLength(100)]
    public string GuestEmail { get; set; }

    [StringLength(400)]
    public string GuestAddress { get; set; }

    [StringLength(300)]
    public string Remarks { get; set; }
    public double NetRent { get; set; }
    public double TotalRent { get; set; }
    public int RoomCount { get; set; }
    public int TotalRoomCount { get; set; }
    public ICollection<OnlineBookingDetailVm> OnlineBookingDetails { get; set; }
}

public class OnlineBookingDto
{
    public string OnlineBookingNumber { get; set; }
    public DateTime OnlineBookingDate { get; set; }
    public string GuestName { get; set; }
    public string GuestMobile { get; set; }
}

public class OnlineBookingDetailVm
{
    public long Id { get; set; }
    public DateTime CheckInDate { get; set; }
    public string CheckInDateStr { get; set; }
    public DateTime CheckOutDate { get; set; }
    public string CheckOutDateStr { get; set; }
    public long RoomCategoryId { get; set; }
    public string RoomCategoryName { get; set; }
    public double RoomRent { get; set; }
    public int RoomCount { get; set; }
    public double TotalRent { get; set; }
    public double Adult { get; set; }
    public double Child { get; set; }
    public double TotalDays { get; set; }
}