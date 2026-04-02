using Domain.Entities.HotelManagement;
using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Accounting;

public class AccLedger : IAuditable
{
    public long Id { get; set; }

    [Required]
    [StringLength(90)]
    public string LedgerName { get; set; }

    [Required]
    [StringLength(50)]
    public string LedgerCode { get; set; }

    [StringLength(120)]
    public string Description { get; set; }

    [StringLength(120)]
    public string Remarks { get; set; }
    public long LedgerGroupCode { get; set; }


    //-------------------FK----------------------
    public long HeadId { get; set; }
    public AccHead Head { get; set; }

    public ApplicationUser ActionBy { get; set; }
    public DateTime ActionDate { get; set; }
    public long ActionById { get; set; }

    public ApplicationUser UpdatedBy { get; set; }
    public DateTime? UpdateDate { get; set; }
    public long? UpdatedById { get; set; }
    public bool IsDeleted { get; set; }
}
