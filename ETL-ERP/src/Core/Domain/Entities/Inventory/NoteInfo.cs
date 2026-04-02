using System.ComponentModel.DataAnnotations;
using Domain.Entities.Identity;

namespace Domain.Entities.Inventory;

public class NoteInfo
{
    public long Id { get; set; }
    public DateTime NoteDate { get; set; }

    [StringLength(350)]
    public DateTime NoteDesc { get; set; }
    public long SlNo { get; set; } = 1;

    [StringLength(120)]
    public string NoteFileUrl { get; set; }

    [StringLength(150)]
    public string Remarks { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime ActionDate { get; set; }

    // ---- FK ----

    public long NoteById { get; set; }
    public ApplicationUser NoteBy { get; set; }
    public long? TranMstId { get; set; }
    public TranMst TranMst { get; set; }
    public long? OrderId { get; set; }
    public OrderMst Order { get; set; }
    public long? ReqMstId { get; set; }
    public RequsitionInfo ReqMst { get; set; }
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
}
