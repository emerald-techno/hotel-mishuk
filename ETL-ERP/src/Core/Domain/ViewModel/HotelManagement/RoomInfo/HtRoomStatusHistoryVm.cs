using Domain.Enums.AppEnums;

namespace Domain.ViewModel.HotelManagement.RoomInfo;

public class HtRoomStatusHistoryVm
{
    public long Id { get; set; }
    public RoomStatusHstEnum RoomStatus { get; set; }
    public DateTime StatusDate { get; set; }
    public bool IsCurrent { get; set; } = true;
    public bool IsDeleted { get; set; }
    public string Remarks { get; set; }
    public long RoomId { get; set; }
    public string RoomNo { get; set; }
}
