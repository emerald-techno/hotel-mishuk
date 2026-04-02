using Domain.Enums.AppEnums;
using Domain.ViewModel.Restaurant.OrderPayment;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Restaurant.FoodOrder;

public class FoodOrderVm
{
    public long Id { get; set; }
    public string OrderNo { get; set; }
    public DateTime OrderDate { get; set; }
    public string OrderDateStr { get; set; }
    public RsOrderStatusEnum OrderStatus { get; set; }
    public RsOrderPaymentStatusEnum PaymentStatus { get; set; }
    public RsOrderTypeEnum OrderType { get; set; }
    public double OrderAmount { get; set; } = 0;
    public double VAT { get; set; } = 0;
    public double TAX { get; set; } = 0;
    public double ServiceCharge { get; set; } = 0;
    public double Discount { get; set; } = 0;
    public double NetAmount { get; set; } = 0;
    public string OrderDesc { get; set; }
    public DateTime? ReservationDate { get; set; }
    public string ReservationDateStr { get; set; }
    public bool IsRoomService { get; set; }
    public long? AuditById { get; set; }
    [StringLength(250)]
    public string AuditRemarks { get; set; }
    public DateTime? AuditDate { get; set; }
    public bool IsFoodOrderAudited => (AuditDate.HasValue && AuditById.HasValue);

    // --- Fk ---

    public long CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string CustomerMobile { get; set; }
    public string CustomerAddress { get; set; }
    public long CustomerTypeId { get; set; }
    public string CustomerTypeName { get; set; }
    public string CustomerTypeCode { get; set; }
    public long? RoomId { get; set; }
    public string RoomNo { get; set; }
    public long? BookingId { get; set; }
    public string BookingNo { get; set; }
    public long? TableId { get; set; }
    public string TableNo { get; set; }
    public long? WaiterId { get; set; }
    public string WaiterName { get; set; }
    public string WaiterCode { get; set; }
    public string ActionByName { get; set; }
    public IEnumerable<SelectListItem> CustomerTypeLookUp { get; set; }
    public IEnumerable<SelectListItem> CustomerTypeWithoutHotelLookUp { get; set; }
    public IEnumerable<SelectListItem> CustomerLookUp { get; set; }
    public IEnumerable<SelectListItem> RoomLookUp { get; set; }
    public IEnumerable<SelectListItem> TableLookUp { get; set; }
    public IEnumerable<SelectListItem> PayModeLookUp { get; set; }
    public IEnumerable<SelectListItem> WaiterLookUp { get; set; }
    public IEnumerable<SelectListItem>FoodItemLookUp { get; set; }
    public ICollection<FoodOrderItemVm> RsFoodOrderItems { get; set; }
    public ICollection<RsOrderPaymentVm> OrderPayments { get; set; }

    //--- payment property


    public string PaidDateStr { get; set; }
    public PayModeEnum? PayMode { get; set; }
    public double PayAmount { get; set; } = 0;
    public double PaidAmount { get; set; } = 0;
    public double AlreadyRefundAmount { get; set; } = 0;
}
