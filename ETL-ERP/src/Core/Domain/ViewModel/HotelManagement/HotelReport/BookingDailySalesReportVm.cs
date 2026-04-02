namespace Domain.ViewModel.HotelManagement.HotelReport;

public class BookingDailySalesReportVm
{
    public long BookingId { get; set; }
    public long GuestId { get; set; }
    public string GuestName { get; set; }
    public long RoomCategoryId { get; set; }
    public string CategoryName { get; set; }
    public long RoomId { get; set; }
    public string RoomNo { get; set; }
    public long FloorId { get; set; }
    public string FloorName { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public decimal RoomRent { get; set; }
    public decimal Rent { get; set; }
    public decimal ExtraBedCharge { get; set; }
    public decimal Discount { get; set; }
    public decimal SpecialDiscount { get; set; }
    public decimal NetRent { get; set; }
    public decimal AdvanceAmount { get; set; }
    public string BillNumber { get; set; }
    public string StrFromDate { get; set; }
    public string StrToDate { get; set; }
    public string StrQueryDate { get; set; }
    public double TotalStay { get; set;}
    public decimal FoodBill { get; set; }
    public decimal AdvanceRefund { get; set; }
    public decimal NetReceive { get; set; }
    public decimal Vat { get; set; }
    public decimal Tax { get; set; }
    public decimal ServiceCharge { get; set; }
    public decimal Due { get; set; }
    public bool IsAdvance { get; set; }
    public decimal ExtraCharge { get; set; }
    public decimal ReservationAdvance { get; set; }
    public decimal PreReceive { get; set; } = 0;
    public string RefundTranNo { get; set; }
    
    public decimal ComplimentaryAmount { get; set; }
    public short IsComplimentary { get; set; }
    public string CmpRemarks { get; set; }

    public decimal ConfBill { get; set; }
    public short BookingStatus { get; set; } = 3;



}

public class BookingDailyAdvanceReportVm
{
    public long BookingId { get; set; }
    public string BookingNo { get; set; }
    public long GuestId { get; set; }
    public string GuestName { get; set; }
    public long RoomId { get; set; }
    public string RoomNo { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public decimal RoomRent { get; set; }
    public decimal PaidAmount { get; set; }
    public DateTime PaidDate { get; set; }
    public string StrQueryDate { get; set; }
    public decimal AdvanceAmount { get; set; }
    public decimal ReservationAdvacne { get; set; }
    public string BillNumber { get; set; }

}