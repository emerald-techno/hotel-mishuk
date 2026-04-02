using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.HotelManagement;

public class HtRoomFacility : IAuditable
{
    public long Id { get; set; }

    [Required]
    [StringLength(50)]
    public string FacilityName { get; set; }

    [StringLength(250)]
    public string Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }


    // --- Fk ---

    public long FacilityCategoryId { get; set; }
    public HtRoomFacilityCategory FacilityCategory { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public ApplicationUser UpdatedBy { get; set; }
}
