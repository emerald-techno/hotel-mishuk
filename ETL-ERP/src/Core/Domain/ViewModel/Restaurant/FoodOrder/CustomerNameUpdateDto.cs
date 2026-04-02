namespace Domain.ViewModel.Restaurant.FoodOrder;

public class CustomerNameUpdateDto
{
    public long OrderId { get; set; }
    public long CustomerId { get; set; }
    public long CustomerTypeId { get; set; }
}
