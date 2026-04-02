using Domain.Entities.Identity;

namespace Domain.Entities.HotelManagement;

public class HtBusinessDay
{
    public long Id { get; set; }
    public DateTime BusinessDate { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime? EndDateTime { get; set; }
    public short AuditStatus { get; set; } // 0=open, 1=closed
    public double TotalRoomRevenue { get; set; }
    public double TotalFoodAndBeverageRevenue { get; set; }
    public double TotalHallRevenue { get; set; }
    public double TotalOtherRevenue { get; set; }
    public DateTime ActionDate { get; set; } = DateTime.UtcNow;
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
}
