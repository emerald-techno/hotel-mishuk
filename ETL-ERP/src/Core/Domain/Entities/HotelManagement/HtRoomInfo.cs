using DocumentFormat.OpenXml.Wordprocessing;
using Domain.Entities.Identity;
using Domain.Enums.AppEnums;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.HotelManagement;

public class HtRoomInfo : IAuditable
{
    public long Id { get; set; }

    [Required]
    [StringLength(25)]
    [Column(TypeName = "VARCHAR")]
    public string RoomNo { get; set; }

    [StringLength(500)]
    [Column(TypeName = "VARCHAR")]
    public string RoomInformation { get; set; }
    public bool IsAc { get; set; }
    public bool IsBelcony { get; set; }

    [StringLength(35)]
    public string BedInfo { get; set; }
    public short NumberOfBed { get; set; }
    public short Person { get; set; }

    [StringLength(150)]
    public string OtherInfo { get; set; }

    [StringLength(30)]
    public string RoomSize { get; set; }
    public double Rent { get; set; }
    public double Vat { get; set; }
    public double ServiceCharge { get; set; }
    public double TotalRent { get; set; }

    public BookingStatusEnum BookingStatus { get; set; }
    public CleaningStatusEnum CleaningStatus { get; set; }
    public AvailabilityStatusEnum HouseKeeperAvailabilityStatus { get; set; }

    [StringLength(120)]
    public string Remakrs { get; set; }
    [StringLength(120)]
    public string OOORemakrs { get; set; }
    public DateTime? OOODate { get; set; }

    [StringLength(120)]
    [Column(TypeName = "VARCHAR")]
    public string PhotoUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }

    // --- Fk ---

    public long RoomCategoryId { get; set; }
    public HtRoomCategory RoomCategory { get; set; }
    public long FloorId { get; set; }
    public HtFloorInfo Floor { get; set; }
    public long? BedTypeId { get; set; }
    public HtBedType BedType { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public ApplicationUser UpdatedBy { get; set; }
}
