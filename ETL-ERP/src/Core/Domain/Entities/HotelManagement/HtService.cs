using Domain.Entities.Accounting;
using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.HotelManagement;

public class HtService : IAuditable
{
    public long Id { get; set; }

    [Required]
    [StringLength(80)]
    public string ServiceName { get; set; }

    [StringLength(20)]
    public string ServiceCode { get; set; }
    
    [StringLength(150)]
    public string Description { get; set; }
    public DateTime ActionDate { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsExtra { get; set; }

    // --- Fk ---

    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
    public long? UpdatedById { get; set; }
    public ApplicationUser UpdatedBy { get; set; }
    public long? LedgerId { get; set; }
    public AccLedger Ledger { get; set; }
}