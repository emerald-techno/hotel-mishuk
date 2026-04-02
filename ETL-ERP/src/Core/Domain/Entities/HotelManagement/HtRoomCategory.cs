using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.HotelManagement;

public class HtRoomCategory : IAuditable
{
    public long Id { get; set; }

    [Required]
    [StringLength(160)]
    public string CategoryName { get; set; }
    public bool IsAc { get; set; }
    public bool IsBalcony { get; set; }
    public int BedNumber { get; set; } = 1;
    public int Capacity { get; set; } = 1;

    [StringLength(600)]
    public string OtherInfo { get; set; }
    public double Rent { get; set; }
    public double? OfferRate { get; set; } = 0;
    public double ServiceCharge { get; set; }
    public double Vat { get; set; }
    public double TotalRent { get; set; }

    [StringLength(120)]
    public string PhotoUrl { get; set; }

    [StringLength(150)]
    public string Remarks { get; set; }
    public bool IsActive { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsWebSiteShow { get; set; }

    // --- Fk ---

    public long? BedTypeId { get; set; }
    public HtBedType BedType { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public ApplicationUser UpdatedBy { get; set; }
}
