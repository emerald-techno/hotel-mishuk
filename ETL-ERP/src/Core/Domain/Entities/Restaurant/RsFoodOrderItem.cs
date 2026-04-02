using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.HotelManagement;

public class RsFoodOrderItem : IAuditable
{
    public long Id { get; set; }
    public double Quantity { get; set; } = 1;
    public double Rate { get; set; } = 0;
    public double TotalAmount { get; set; } = 0;

    [StringLength(250)]
    public string Description { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }

    [StringLength(60)]
    public string KotNo { get; set; }
    public bool IsServed { get; set; }
    public DateTime? ServedTime { get; set; }

    // --- Fk ---

    public long OrderId { get; set; }
    public RsFoodOrder Order { get; set; }
    public long FoodId { get; set; }
    public RsFoodItem Food { get; set; }
    public long? ServedById { get; set; }
    public ApplicationUser ServedBy { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public ApplicationUser UpdatedBy { get; set; }
}