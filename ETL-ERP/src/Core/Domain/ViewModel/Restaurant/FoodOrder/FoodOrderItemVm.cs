namespace Domain.ViewModel.Restaurant.FoodOrder;

public class FoodOrderItemVm
{
    public long Id { get; set; }
    public double Quantity { get; set; } = 1;
    public double Rate { get; set; } = 0;
    public double TotalAmount { get; set; } = 0;
    public string Description { get; set; }
    public string KotNo { get; set; }

    // --- Fk ---

    public long OrderId { get; set; }
    public string OrderNo { get; set; }
    public long FoodId { get; set; }
    public string FoodName { get; set; }
    public string FoodDescription { get; set; }
    public DateTime? ActionDate { get; set; }
    public bool IsServed { get; set; }
    public DateTime? ServedTime { get; set; }
}

public class SaveFoodOrderItemDto
{
    public double Quantity { get; set; } = 1;
    public double Rate { get; set; } = 0;
    public double TotalAmount { get; set; } = 0;
    public long OrderId { get; set; }
    public long FoodId { get; set; }
}

public class DiscountUpdateDto
{
    public double Discount { get; set; }
    public long OrderId { get; set; }
}
public class BillDiscountUpdateDto
{
    public double Discount { get; set; }
    public long BillId { get; set; }
}