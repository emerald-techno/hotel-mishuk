using Domain.Enums.AppEnums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.HotelManagement.Booking;

public class HtBookingHallVm
{
    public long Id { get; set; }
    public DateTime BookingDate { get; set; }
    public string BookingDateStr { get; set; }
    public HallBookingShiftEnum HallShift { get; set; } // 1 = Day Shift, 2 = Night Shift, 3 = Both
    public string HallShiftText => HallShift switch { HallBookingShiftEnum.DayShift => "Day Shift", HallBookingShiftEnum.NightShift => "Night Shift", HallBookingShiftEnum.Both => "Both Shift", _ => "--" };
    public double HallRent { get; set; }
    public double Rent { get; set; }
    public double ServiceCharge { get; set; }
    public double Vat { get; set; }
    public double Tax { get; set; }
    public double Discount { get; set; }
    public double NetRent { get; set; }
    public double TotalPerson { get; set; }

    [StringLength(150)]
    [Column(TypeName = "VARCHAR")]
    public string Remarks { get; set; }

    // --- Fk ---

    public long BookingId { get; set; }
    public long HallId { get; set; }
    public string HallName { get; set; }
}
public class HallAvaliableReportVm
{
    public string FromDateStr { get; set; }
    public string ToDateStr { get; set; }
    public long HallId { get; set; }
    public string HallName { get; set; }
    public long? BookingId { get; set; }
    public string BookingNo { get; set; }
    public DateTime BookingDate { get; set; }
    public string Day { get; set; }
    public bool IsDayAvailable { get; set; }
    public bool IsNightAvailable { get; set; }
    public bool IsBothAvailable { get; set; }
    public IEnumerable<SelectListItem> HallLookUp { get; set; }
}