using Domain.Enums.AppEnums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.HotelManagement;

public class HtOnlineBooking
{
    public long Id { get; set; }

    [Required]
    [StringLength(40)]
    public string OnlineBookingNumber { get; set; }
    public DateTime OnlineBookingDate { get; set; }
    public DateTime ArrivalDate { get; set; }
    public DateTime DepartureDate { get; set; }
    public OnlineBookingStatusEnum Status { get; set; }

    [Required]
    [StringLength(200)]
    public string GuestName { get; set; }

    [Required]
    [StringLength(30)]
    public string GuestMobile { get; set; }

    [StringLength(30)]
    public string GuestEmail { get; set; }

    [StringLength(400)]
    public string GuestAddress { get; set; }
    
    [StringLength(400)]
    public string Remarks { get; set; }
    public double TotalRent { get; set; }
    public int TotalRoomCount { get; set; }
    public double TotalAdult { get; set; }
    public double TotalChild { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime ActionDate { get; set; }
    public ICollection<HtOnlineBookingDetail> OnlineBookingDetails { get; set; }
}
