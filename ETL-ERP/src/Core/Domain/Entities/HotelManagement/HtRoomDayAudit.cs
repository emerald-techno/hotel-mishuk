using Domain.Entities.Identity;
using Domain.Enums.AppEnums;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.HotelManagement;

public class HtRoomDayAudit
{
    public long Id { get; set; }
    public DateTime BusinessDate { get; set; }
    public int Adult { get; set; }
    public int Child { get; set; }
    public int TotalGuest => Adult + Child;
    public bool IsStay { get; set; }
    public bool IsOccupied { get; set; }      // room is occupied
    public bool IsCharged { get; set; }       // charge posted to folio
    public AuditStayStatus Status { get; set; }     // 0=Planed,1=Stayed,2=CheckOut,3=NoShow
    public double RackRate { get; set; } // room orginal rate : 10000
    public double Rate { get; set; } // after discount rate : 5000 (50% discount)
    public double BaseRate { get; set; } // room tarrif : 3953
    public double ServiceCharge { get; set; } // service charge : 395 (10% sc)
    public double Vat { get; set; } // value added tax : 652 (15% sc)
    public double NetAmount { get; set; } // net amount : 5000

    [StringLength(500)]
    public string Remarks { get; set; }
    public DateTime ActionDate { get; set; } = DateTime.UtcNow;
    public long BookingRoomId { get; set; }
    public HtBookingRoom BookingRoom { get; set; }
    public long RoomId { get; set; }
    public HtRoomInfo Room { get; set; }
    public long RoomCategoryId { get; set; }
    public HtRoomCategory RoomCategory { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
}
