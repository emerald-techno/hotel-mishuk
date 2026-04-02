using DocumentFormat.OpenXml.Wordprocessing;
using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Domain.Enums.AppEnums;

namespace Domain.Entities.HotelManagement;

public class HtBookingService : IAuditable
{
    public long Id { get; set; }

    [Required]
    [StringLength(30)]
    public string BookingNo { get; set; }
    public DateTime BookingDate { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime CheckOutTime { get; set; }
    
    [StringLength(250)]
    [Column(TypeName = "NVARCHAR")]
    public string VisitPurpose { get; set; }

    [StringLength(1)]
    public string BookingType { get; set; } // R = Room, H = Hall
    
    [StringLength(150)]
    [Column(TypeName = "VARCHAR")]
    public string Remarks { get; set; }
    public BookingServiceStatusEnum BookingStatus { get; set; }
    public PaymentStatusEnum PaymentStatus { get; set; }
    public double Rent { get; set; }
    public double ServiceCharge { get; set; }
    public double Vat { get; set; }
    public double Tax { get; set; }
    public double Discount { get; set; }
    public double NetRent { get; set; }
    public double TotalGuest { get; set; }
    public double Adult { get; set; }
    public double Child { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public DateTime? CancelDate { get; set; }
    public long? CancelBy { get; set; }
    public string CancelReason { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime ReportDate { get; set; }
    public BookingConfirmEnum BookingConfirmStatus { get; set; }

    // --- Fk ---
    public long? OnlineBookingId { get; set; }
    public HtOnlineBooking OnlineBooking { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public ApplicationUser UpdatedBy { get; set; }
}