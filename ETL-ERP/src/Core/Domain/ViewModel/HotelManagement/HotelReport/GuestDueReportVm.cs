using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.HotelManagement.HotelReport;

public class GuestDueReportVm
{
    public string StrQueryDate { get; set; }
    public string StrFromDate { get; set; }
    public string StrToDate { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string BookingType { get; set; }
    public long BookingId { get; set; } 
    public string BookingNo { get; set; }
    public DateTime BillDate { get; set; }
    public DateTime BookingDate { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime CheckOutTime { get; set; }
    public string BookingDateStr { get; set; }
    public string RoomNoList { get; set; }
    public long GuestId { get; set; }
    public string GuestName { get; set; }
    public string GuestMobile { get; set; }
    public long CompanyId { get; set; }
    public string CompanyName { get; set; }
    public int TotalGuest { get; set; }
    public decimal Rent { get; set; }
    public decimal ExtraBedCharge { get; set; }
    public decimal ExtraCharge { get; set; }
    public decimal Discount { get; set; }
    public decimal SpecialDiscount { get; set; }
    public decimal FoodBill { get; set; }
    public decimal Vat { get; set;}
    public decimal Tax { get; set;}
    public decimal ServiceCharge { get; set; }
    public decimal ConfBill { get; set; }
    public decimal NetRent { get; set; }
    public decimal TotalBill { get; set; }
    public decimal TotalReceive { get; set; }
    public decimal DueAmount { get; set; }

    public bool IsPirnt = false;

    public string ReporetGroupBy { get; set; } = "G";//G=GUEST WISE, C=COMPANY WISE

    public IEnumerable<SelectListItem> GuestLookUp { get; set; }
    public IEnumerable<SelectListItem> CompanyLookUp { get; set; }
    public IEnumerable<SelectListItem> GroupLookUp { get; set; }
    public IEnumerable<SelectListItem> RoomTypeLookUp { get; set; }
}
