using DocumentFormat.OpenXml.Wordprocessing;
using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.HotelManagement;

public class HtFloorInfo : IAuditable
{
    public long Id { get; set; }

    [Required]
    [StringLength(30)]
    [Column(TypeName = "VARCHAR(30)")]
    public string FloorName { get; set; }
    public int TotalRoom {  get; set; }
    
    [StringLength(30)]
    [Column(TypeName = "VARCHAR(30)")]
    public string StartRoomNo { get; set; }
    
    [StringLength(30)]
    [Column(TypeName = "VARCHAR(30)")]
    public string EndRoomNo { get; set; }

    [StringLength(120)]
    public string Remarks { get; set; }
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
