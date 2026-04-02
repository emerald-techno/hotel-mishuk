using Domain.Enums.AppEnums;
using DU = Domain.Utility;

namespace Domain.ViewModel.HotelManagement.RoomDayAudit;

public class RoomDayAuditVm
{
    public long Id { get; set; }
    public DateTime BusinessDate { get; set; }
    public int Adult { get; set; }
    public int Child { get; set; }
    public int TotalGuest => Adult + Child;
    public bool IsStay { get; set; }
    public bool IsOccupied { get; set; }
    public bool IsCharged { get; set; }
    public AuditStayStatus Status { get; set; }
    public double RackRate { get; set; }
    public double Rate { get; set; }
    public double BaseRate { get; set; }
    public double ServiceCharge { get; set; }
    public double Vat { get; set; }
    public double DiscountPercent => RackRate > 0 ?  DU.AppUtility.CalculatePercentage((RackRate - Rate), RackRate) : 0;
    public double NetAmount { get; set; }
    public string Remarks { get; set; }
    public DateTime ActionDate { get; set; } = DateTime.UtcNow;
    public long BookingRoomId { get; set; }
    public long RoomId { get; set; }
    public string RoomNo { get; set; }
    public long RoomCategoryId { get; set; }
    public string RoomCategoryName { get; set; }
    public string GuestName { get; set; }
    public string GuestMobile { get; set; }
    public long ActionById { get; set; }
    public long? BookingId { get; set; }
    public string BookingNo { get; set; }
    public DateTime? BookingDate { get; set; }
    public bool IsAllAudited { get; set; }
    public AuditStatusVm AuditedInfo { get; set; }
}

public class AuditStatusVm
{
    public bool IsAllAudited { get; set; }
    public bool IsRoomAuditGenerated { get; set; }
    public bool IsAllRoomCharged { get; set; }
    public bool IsFoPaymentAudited { get; set; }
    public bool IsExtraServiceAudited { get; set; }
    public bool IsFoodOrderAudited { get; set; }
    public bool IsRestPaymentAudited { get; set; }
    public bool IsBanquetAudited { get; set; }
}