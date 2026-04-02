namespace Domain.ViewModel.HotelManagement.HotelReport;

public class HkSalesReportVm
{
    public string StrFromDate { get; set; }
    public string?StrToDate { get; set; }

    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }

    public string TranNo { get; set; }
    public string BillNumber { get; set; }
    public long? BillId { get; set; }
    public DateTime TransDate { get; set; }

    public double Amount { get; set; }
    public double Discount { get; set; }
    public double DiscountedAmount { get; set; }
    public double NetTotal { get; set; }
    public double ServiceCharge { get; set; }
    public double VAT { get; set; }
    public double CashReceived { get; set; }
    public double CardReceived { get; set; }
    public double CompanyCredit { get; set; }
    public double RoomCredit { get; set; }

    public string BookingNo { get; set; }
    public string RoomNo { get; set; }
    public string PaymentDetails { get; set; }
    public string TranBy { get; set; }
}
