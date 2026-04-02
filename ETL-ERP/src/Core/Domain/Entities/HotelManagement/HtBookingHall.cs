using Domain.Entities.Identity;
using Domain.Enums.AppEnums;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.HotelManagement;

public class HtBookingHall : IAuditable
{
    public long Id { get; set; }
    public DateTime BookingDate { get; set; }
    public HallBookingShiftEnum HallShift { get; set; } // 1 = Day Shift, 2 = Night Shift, 3 = Both
    public double HallRent { get; set; }
    public double Rent { get; set; }
    public double ServiceCharge { get; set; }
    public double Vat { get; set; }
    public double Tax { get; set; }
    public double Discount { get; set; }
    public double NetRent { get; set; }
    public double TotalPerson { get; set; }

    [StringLength(150)]
    [Column(TypeName = "VARCHAR")]
    public string Remarks { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }

    [StringLength(250)]
    public string AuditRemarks { get; set; }
    public DateTime? AuditDate { get; set; }

    // --- Fk ---

    public long BookingId { get; set; }
    public HtBookingService Booking { get; set; }
    public long HallId { get; set; }
    public HtHallInfo Hall { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public ApplicationUser UpdatedBy { get; set; }
    public long? AuditById { get; set; }
    public ApplicationUser AuditBy { get; set; }
}