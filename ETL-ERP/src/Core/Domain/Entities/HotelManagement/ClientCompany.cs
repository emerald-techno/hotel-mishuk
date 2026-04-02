using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.HotelManagement;

public class ClientCompany : IAuditable
{
    public long Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; }

    [StringLength(350)]
    public string Address { get; set; }

    [StringLength(30)]
    public string Mobile { get; set; }

    [StringLength(100)]
    public string Email { get; set; }

    [StringLength(100)]
    public string WebSite { get; set; }

    [StringLength(100)]
    public string LogoUrl { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }

    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public ApplicationUser UpdatedBy{ get; set; }
}
