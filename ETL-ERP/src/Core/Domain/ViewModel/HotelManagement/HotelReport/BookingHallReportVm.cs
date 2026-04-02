namespace Domain.ViewModel.HotelManagement.HotelReport;

public class BookingHallReportVm
{
    // Date filters
    public string StrFromDate { get; set; }
    public string StrToDate { get; set; }

    // Booking / Hall Information
    public string BookingNo { get; set; }
    public long BookingId { get; set; }
    public long BookingHallId { get; set; }
    public DateTime BookingDate { get; set; }
    public DateTime PartyDate { get; set; }
    public string HallShift { get; set; }
    public string HallName { get; set; }

    // Financials
    public double HallRent { get; set; }
    public double Rent { get; set; }
    public double ServiceCharge { get; set; }
    public double Vat { get; set; }
    public double Discount { get; set; }
    public double NetRent { get; set; }

    // Guest / Person Info
    public int TotalPerson { get; set; }
    public string GuestName { get; set; }
    public string Mobile { get; set; }

    // Audit Information
    public int? AuditById { get; set; }
    public string AuditBy { get; set; }
    public DateTime? AuditDate { get; set; }
    public string AuditRemarks { get; set; }

}
