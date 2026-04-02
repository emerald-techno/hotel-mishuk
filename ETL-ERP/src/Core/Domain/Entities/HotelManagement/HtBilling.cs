using Domain.Entities.Identity;
using Domain.Enums.AppEnums;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.HotelManagement;

public class HtBilling : IAuditable
{
    public long Id { get; set; }

    [Required]
    [StringLength(40)]
    public string BillNumber { get; set; }
    public DateTime BillDate { get; set; }
    public double TotalAmount { get; set; }
    public double Vat { get; set; }
    public double Tax { get; set; }
    public double Discount { get; set; }
    public double ServiceCharge { get; set; } = 0;
    public double SpecialDiscount { get; set; } = 0;
    public double PaidAmount { get; set; }
    public double NetAmount { get; set; }
    public BillStatusEnum BillStatus { get; set; } // 0 = Fresh, 1 = Partial Paid, 2 = Full Paid

    [StringLength(150)]
    public string Remarks { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }    

    [StringLength(250)]
    public string CmpRemarks { get; set; }

    // --- Fk ---

    public long BookingId { get; set; }
    public HtBookingService Booking { get; set; }
    public long BillById { get; set; }
    public ApplicationUser BillBy { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public ApplicationUser UpdatedBy { get; set; }
    public ICollection<HtBillingDetail> BillingDetails { get; set; }
}
