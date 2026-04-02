namespace Domain.Entities.HotelManagement;

public class HtRoomCategoryDiscountMap
{
    public long Id { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public DiscountTypeEnum DiscountType { get; set; }
    public double DiscountAmount { get; set; }
    public long RoomCategoryId { get; set; }
    public HtRoomCategory RoomCategory { get; set; }
    public long ActionById { get; set; }
    public DateTime ActionDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }

}

public enum DiscountTypeEnum
{
    Percent,
    Amount
}
