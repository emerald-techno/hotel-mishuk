using Domain.Enums.AppEnums;
using Domain.ModelInterface;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.HotelManagement.Billing;

public class BillingSearchVm : IDataTableSearch
{
    public long Id { get; set; }
    public string BillNo { get; set; }
    public DateTime BillDate { get; set; }
    public double TotalAmount { get; set; }
    public double Vat { get; set; }
    public double Tax { get; set; }
    public double Discount { get; set; }
    public double NetAmount { get; set; }
    public BillStatusEnum? BillStatus { get; set; } // 0 = Fresh, 1 = Partial Paid, 2 = Full Paid
    public string Remarks { get; set; }
    public long BookingId { get; set; }
    public string BookingNo { get; set; }
    public DateTime BookingDate { get; set; }
    public long BillById { get; set; }
    public string BillByFullName { get; set; }
    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }
    public string RoomList { get; set; }
    public string FormDateStr { get; set; }
    public string ToDateStr { get; set; }
    public bool IsOnlyCheckOutBill { get; set; }
    public IEnumerable<SelectListItem> BillStatusLookUp { get; set; }
}
