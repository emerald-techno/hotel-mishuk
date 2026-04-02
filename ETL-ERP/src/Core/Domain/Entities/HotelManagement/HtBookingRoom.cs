using Domain.Entities.Identity;
using Domain.ModelInterface;

namespace Domain.Entities.HotelManagement;

public class HtBookingRoom : IAuditable
{
    public long Id { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime CheckOutTime { get; set; }
    public DateTime? ActualCheckInTime { get; set; }
    public DateTime? ActualCheckOutTime { get; set; }
    public double RoomRent { get; set; }
    public double RoomServiceCharge { get; set; }
    public double Rent { get; set; }
    public double ServiceCharge { get; set; }
    public double Vat { get; set; }
    public double Tax { get; set; }
    public double Discount { get; set; }
    public double NetRent { get; set; }
    public double TotalGuest { get; set; }
    public double Adult { get; set; }
    public double Child { get; set; }
    public short ExtraBed { get; set; } = 0;
    public double ExtraBedCharge { get; set; } = 0;
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }
    public int BookingDayStatus { get; set; } // 1 = Half Day, 2 = Day Use

    // --- Fk ---
    public long RoomCategoryId { get; set; }
    public HtRoomCategory RoomCategory { get; set; }
    public long BookingId { get; set; }
    public HtBookingService Booking { get; set; }
    public long? RoomId { get; set; }
    public HtRoomInfo Room { get; set; }
    public long? ComplementaryId { get; set; }
    public HtComplementary Complementary { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public ApplicationUser UpdatedBy { get; set; }
}
