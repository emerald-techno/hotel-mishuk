using Domain.Enums.AppEnums;
using Domain.ModelInterface;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Restaurant.FoodOrder;

public class FoodOrderSearchVm : IDataTableSearch
{
    public long Id { get; set; }
    public string OrderNo { get; set; }
    public DateTime OrderDate { get; set; }
    public string OrderDateStr { get; set; }
    public RsOrderStatusEnum? OrderStatus { get; set; }
    public string OrderStatusText => OrderStatus switch
    {
        RsOrderStatusEnum.Pending => "Pending",
        RsOrderStatusEnum.Served => "Served",
        RsOrderStatusEnum.Canceled => "Canceled",
        _ => ""
    };
    public RsOrderPaymentStatusEnum? PaymentStatus { get; set; }
    public double OrderAmount { get; set; } = 0;
    public double VAT { get; set; } = 0;
    public double TAX { get; set; } = 0;
    public double ServiceCharge { get; set; } = 0;
    public double Discount { get; set; } = 0;
    public double NetAmount { get; set; } = 0;
    public string OrderDesc { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }
    public string FormDateStr { get; set; }
    public string ToDateStr { get; set; }
    public RsOrderTypeEnum? OrderType { get; set; }

    // --- Fk ---

    public long CustomerId { get; set; }
    public string CustomerName { get; set; }
    public long CustomerTypeId { get; set; }
    public string CustomerTypeName { get; set; }
    public long? RoomId { get; set; }
    public string RoomNo { get; set; }
    public long? TableId { get; set; }
    public string TableNo { get; set; }
    public long? WaiterId { get; set; }
    public string WaiterName { get; set; }
    public string SaleByFullName { get; set; }
    public ICollection<FoodOrderItemVm> RsFoodOrderItems { get; set; }
    public IEnumerable<SelectListItem> FoodOrderStatusLookUp { get; set; }
    public IEnumerable<SelectListItem> OrderPaymentStatusLookUp { get; set; }

    // --- Datatable ---

    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }
}
