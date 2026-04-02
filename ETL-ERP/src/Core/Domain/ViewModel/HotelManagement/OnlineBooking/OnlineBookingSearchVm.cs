using Domain.Enums.AppEnums;
using Domain.ModelInterface;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.HotelManagement.OnlineBooking;

public class OnlineBookingSearchVm : IDataTableSearch
{
    public long Id { get; set; }
    public string OnlineBookingNumber { get; set; }
    public DateTime OnlineBookingDate { get; set; }
    public DateTime ArrivalDate { get; set; }
    public DateTime DepartureDate { get; set; }
    public double TotalAdult { get; set; }
    public double TotalChild { get; set; }
    public OnlineBookingStatusEnum? Status { get; set; }
    public string GuestName { get; set; }
    public string GuestMobile { get; set; }
    public string GuestAddress { get; set; }
    public double TotalRent { get; set; }
    public long TotalRoomCount { get; set; }
    public string CategoryList { get; set; }
    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }
    public bool IsPendingOnly { get; set; }

    public IEnumerable<SelectListItem> StatusLookUp { get; set; }
}
