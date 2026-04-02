using DocumentFormat.OpenXml.Wordprocessing;
using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.HotelManagement;

public class HtComplementary : IAuditable
{
    public long Id { get; set; }
    
    [Required]
    [StringLength(80)]
    [Column(TypeName = "VARCHAR")]
    public string Title { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsActive { get; set; }

    // --- Fk ---

    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public ApplicationUser UpdatedBy { get; set; }
}
