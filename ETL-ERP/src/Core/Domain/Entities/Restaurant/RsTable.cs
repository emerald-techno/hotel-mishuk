using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.HotelManagement;

public class RsTable : IAuditable
{
    public long Id { get; set; }

    [Required]
    [StringLength(100)]
    public string TableNo { get; set; }
    public short Capacity { get; set; } = 0;
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }

    // --- Fk ---

    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public ApplicationUser UpdatedBy { get; set; }
}
