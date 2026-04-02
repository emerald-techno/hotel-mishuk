using DocumentFormat.OpenXml.Wordprocessing;
using Domain.Entities.Admin;
using Domain.Entities.Identity;
using Domain.Enums.AppEnums;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.HotelManagement;

public class HtGuestInfo : IAuditable
{
    public long Id { get; set; }

    [Required]
    [StringLength(50)]
    [Column(TypeName = "VARCHAR")]
    public string GuestCode { get; set; }

    [Required]
    [StringLength(20)]
    [Column(TypeName = "VARCHAR")]
    public string Salutation { get; set; }

    [Required]
    [StringLength(80)]
    public string FirstName { get; set; }

    [StringLength(80)]
    public string LastName { get; set; }

    [Required]
    [StringLength(80)]
    public string Mobile { get; set; }

    [StringLength(40)]
    [Column(TypeName = "VARCHAR")]
    public string Email { get; set; }

    [StringLength(40)]
    [Column(TypeName = "VARCHAR")]
    public string Occupation { get; set; }

    [StringLength(1)]
    [Column(TypeName = "CHAR")]
    public string Gender { get; set; }

    [StringLength(250)]
    [Column(TypeName = "VARCHAR")]
    public string Address { get; set; }
    public DateTime? Dob { get; set; }
    public IdentityTypeEnum? IdentityType { get; set; }

    [StringLength(120)]
    [Column(TypeName = "VARCHAR")]
    public string IdentityNo { get; set; }

    [StringLength(120)]
    [Column(TypeName = "VARCHAR")]
    public string IdentityPhotoUrl { get; set; }

    [StringLength(120)]
    [Column(TypeName = "VARCHAR")]
    public string PhotoUrl { get; set; }
    public bool IsVip { get; set; }

    [StringLength(150)]
    public string Note { get; set; }

    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }

    // --- Fk ---
    public long? CompanyId { get; set; }
    public ClientCompany Company { get; set; }
    public long CountryId { get; set; }
    public SetCountry Country { get; set; }
    public long? DueLedgerId { get; set; }
    public long? DistrictId { get; set; }
    public SetDistrict District { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public ApplicationUser UpdatedBy { get; set; }
}
