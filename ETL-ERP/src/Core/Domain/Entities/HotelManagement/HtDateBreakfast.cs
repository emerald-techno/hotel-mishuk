using Domain.Entities.Identity;

namespace Domain.Entities.HotelManagement;

public class HtDateBreakfast
{
    public long Id { get; set; }
    public double BreakfastAmount { get; set; }
    public DateTime BreakfastDate { get; set; }
    public DateTime ActionDate { get; set; }
    public bool IsDeleted { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
}