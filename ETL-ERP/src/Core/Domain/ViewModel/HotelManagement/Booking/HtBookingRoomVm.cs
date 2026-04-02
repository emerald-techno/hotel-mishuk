using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.ViewModel.HotelManagement.RoomBooking;

public class HtBookingRoomVm
{
    public long Id { get; set; }
    public long RoomCategoryId { get; set; }
    public string RoomCategoryName { get; set; }
    public long? RoomId { get; set; }
    public string RoomNo { get; set; }
    public long? ComplementaryId { get; set; }
    public string ComplementaryName { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime CheckOutTime { get; set; }
    public DateTime? ActualCheckInTime { get; set; }
    public DateTime? ActualCheckOutTime { get; set; }
    public string CheckInTimeStr { get; set; }
    public string CheckOutTimeStr { get; set; }
    public string ActualCheckInTimeStr { get; set; }
    public string ActualCheckOutTimeStr { get; set; }
    public double Rent { get; set; }
    public double ServiceCharge { get; set; }
    public double Vat { get; set; }
    public double Tax { get; set; }
    public double Discount { get; set; }
    public double NetRent { get; set; }
    public double? TotalGuest { get; set; } = 0;
    public double? Adult { get; set; } = 0;
    public double? Child { get; set; } = 0;
    public short? ExtraBed { get; set; } = 0;
    public double? ExtraBedCharge { get; set; } = 0;
    public double? ExtraBedAsServiceCharge { get; set; } = 0;
    public double RoomRent { get; set; } = 0;
    public double RoomServiceCharge { get; set; } = 0;
    public bool IsHalfDay { get; set; }
    public bool IsDayUse { get; set; }
    public bool IsCharged { get; set; }
    public int BookingDayStatus { get; set; } // 1 = Half Day, 2 = Day Use
    public int? CleaningStatus { get; set; } = 0;
    public string CleaningStatusText => CleaningStatus switch
    {

        0 => "Vacant & Clean",
        1 => "Vacant & Dirty",
        2 => "Occupied",
        3 => "Check Out",
        4 => "Out of Order",
        _ => string.Empty
    };

    [NotMapped]
    public double RoomDiscount { get; set; }
    [NotMapped]
    public double Days { get; set; }
}

public class HtBookingRoomDto
{
    public long Id { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime CheckOutTime { get; set; }
    public DateTime? ActualCheckInTime { get; set; }
    public DateTime? ActualCheckOutTime { get; set; }
    public double Rent { get; set; }
    public double ServiceCharge { get; set; }
    public double Vat { get; set; }
    public double Tax { get; set; }
    public double Discount { get; set; }
    public double NetRent { get; set; }
    public double TotalGuest { get; set; }
    public double Adult { get; set; }
    public double Child { get; set; }
    public long RoomCategoryId { get; set; }
    public string RoomCategoryName { get; set; }
    public long BookingId { get; set; }
    public string BookingNo { get; set; }
    public long? RoomId { get; set; }
    public string RoomNo { get; set; }
    public double RoomRent { get; set; }
    public double RoomServiceCharge { get; set; }
    public long? ComplementaryId { get; set; }
    public string ComplementaryName { get; set; }
    public short? ExtraBed { get; set; } = 0;
    public double? ExtraBedCharge { get; set; } = 0;
    public int BookingDayStatus { get; set; } // 1 = Half Day, 2 = Day Use
    public int CleaningStatus { get; set; } = 0;
    public bool IsCharged { get; set; }
    public string CleaningStatusText => CleaningStatus switch
    {

        0 => "Vacant & Clean",
        1 => "Vacant & Dirty",
        2 => "Occupied",
        3 => "Check Out",
        4 => "Out of Order",
        _ => string.Empty
    };
}
public class BookingRoomInfoDto
{
    public long BookingId { get; set; }
    public string BookingNo { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime CheckOutTime { get; set; }
    public long? RoomId { get; set; }
    public string RoomNo { get; set; }
    public double TotalGuest { get; set; }
    public string GuestName { get; set; }
    public string GuestMobile { get; set; }
}