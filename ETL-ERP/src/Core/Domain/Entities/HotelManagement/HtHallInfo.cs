using DocumentFormat.OpenXml.Wordprocessing;
using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.HotelManagement;

public class HtHallInfo : IAuditable
{
    public long Id { get; set; }

    [Required]
    [StringLength(25)]
    [Column(TypeName = "VARCHAR")]
    public string HallName { get; set; }

    [StringLength(500)]
    [Column(TypeName = "VARCHAR")]
    public string HallInformation { get; set; }

    [StringLength(150)]
    public string OtherInfo { get; set; }
    public double Rent { get; set; }
    public double Vat { get; set; }
    public double ServiceCharge { get; set; }
    public double TotalRent { get; set; }

    [StringLength(120)]
    public string Remakrs { get; set; }

    [StringLength(120)]
    [Column(TypeName = "VARCHAR")]
    public string PhotoUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }

    // --- Fk ---

    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public ApplicationUser UpdatedBy { get; set; }
}
