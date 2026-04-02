namespace Domain.Entities.HotelManagement;

public class HtOnlineBookingDetail
{
    public long Id { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public double RoomRent { get; set; }
    public int RoomCount { get; set; }
    public double TotalRent { get; set; }
    public double Adult { get; set; }
    public double Child { get; set; }
    public double TotalDays { get; set; }
    public long OnlineBookingId { get; set; }
    public HtOnlineBooking OnlineBooking { get; set; }
    public long RoomCategoryId { get; set; }
    public HtRoomCategory RoomCategory { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime ActionDate { get; set; }
}