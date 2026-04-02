using Domain.Entities.Identity;
using Domain.Enums.AppEnums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.HotelManagement;

public class HtRoomStatusHistory
{
    public long Id { get; set; }
    public RoomStatusHstEnum RoomStatus { get; set; }
    public DateTime StatusDate { get; set; }
    public bool IsCurrent { get; set; } = true;
    public bool IsDeleted { get; set; }

    [StringLength(100)]
    public string Remarks { get; set; }
    public DateTime ActionDate { get; set; }

    // --- Fk ---

    public long RoomId { get; set; }
    public HtRoomInfo Room { get; set; }
    public long RoomStatusById { get; set; }
    public ApplicationUser RoomStatusBy { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
}
