using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.HotelManagement;

public class HtBillingDetail : IAuditable
{
    public long Id { get; set; }
    public double Quantity { get; set; }
    public double Rate { get; set; }
    public double Amount { get; set; }
    public double VAT { get; set; }
    public double Tax { get; set; }
    public double Discount { get; set; }
    public double ServiceCharge { get; set; } = 0;
    public double ExtraBedCharge { get; set; } = 0;
    public double NetAmount { get; set; }

    [StringLength(150)]
    public string Remarks { get; set; }

    [StringLength(250)]
    public string AuditRemarks { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public DateTime? ServiceDate { get; set; }
    public DateTime? AuditDate { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsComplimentary { get; set; }

    // --- Fk ---

    public long BillId { get; set; }
    public HtBilling Bill { get; set; }
    public long ServiceId { get; set; }
    public HtService Service { get; set; }
    public long? BookingRoomId { get; set; }
    public HtBookingRoom BookingRoom { get; set; }
    public long? BookingHallId { get; set; }
    public HtBookingHall BookingHall { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public ApplicationUser UpdatedBy { get; set; }
    public long? AuditById { get; set; }
    public ApplicationUser AuditBy { get; set; }
}