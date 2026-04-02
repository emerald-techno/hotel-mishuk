namespace Domain.ViewModel.Restaurant.FoodOrder;

public class AssignRoomVm
{
    public long OrderId { get; set; }
    public long RoomId { get; set; }
    public long CustomerId { get; set; }
    public bool IsRoomService { get; set; }
}
