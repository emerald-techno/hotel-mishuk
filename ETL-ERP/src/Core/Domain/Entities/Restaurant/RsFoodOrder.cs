using Domain.Entities.Identity;
using Domain.Enums.AppEnums;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.HotelManagement;

public class RsFoodOrder : IAuditable
{
    public long Id { get; set; }

    [Required]
    [StringLength(50)]
    public string OrderNo { get; set; }
    public DateTime OrderDate { get; set; }
    public RsOrderStatusEnum OrderStatus { get; set; }
    public RsOrderPaymentStatusEnum PaymentStatus { get; set; }
    public double OrderAmount { get; set; } = 0;
    public double VAT { get; set; } = 0;
    public double TAX { get; set; } = 0;
    public double ServiceCharge { get; set; } = 0;
    public double Discount { get; set; } = 0;
    public double NetAmount { get; set; } = 0;
    public RsOrderTypeEnum OrderType { get; set; }
    public DateTime? ReservationDate { get; set; }
    public bool? IsRoomService { get; set; }

    [StringLength(250)]
    public string OrderDesc { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public DateTime? CancelDate { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime ReportDate { get; set; }

    [StringLength(250)]
    public string AuditRemarks { get; set; }
    public DateTime? AuditDate { get; set; }
    
    // --- Fk ---

    public long CustomerId { get; set; }
    public RsCustomer Customer { get; set; }
    public long CustomerTypeId { get; set; }
    public RsCustomerType CustomerType { get; set; }
    public long? RoomId { get; set; }
    public HtRoomInfo Room { get; set; }
    public long? BookingId { get; set; }
    public HtBookingService Booking { get; set; }
    public long? BookingRoomId { get; set; }
    public HtBookingRoom BookingRoom { get; set; }
    public long? TableId { get; set; }
    public RsTable Table { get; set; }
    public long? WaiterId { get; set; }
    public RsWaiter Waiter { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public ApplicationUser UpdatedBy { get; set; }
    public long? CancelById { get; set; }
    public ApplicationUser CancelBy { get; set; }
    public long? AuditById { get; set; }
    public ApplicationUser AuditBy { get; set; }
    public ICollection<RsFoodOrderItem> RsFoodOrderItems { get; set; }
}