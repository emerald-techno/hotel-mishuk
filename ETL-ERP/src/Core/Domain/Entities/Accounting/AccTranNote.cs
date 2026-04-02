using System.ComponentModel.DataAnnotations;
using Domain.Entities.Identity;

namespace Domain.Entities.Accounting
{
    public class AccTranNote
    {
        public long Id { get; set; }
        public DateTime NoteDate { get; set; }

        [Required]
        [StringLength(20)]
        public string NoteType { get; set; } // Create, Approve, Audit, BankClear, FinalAttach, Note, AutoCreate

        [StringLength(250)]
        public string NoteDesc { get; set; }
        public short SlNo { get; set; }

        [StringLength(120)]
        public string Remarks { get; set; }

        //---------------------------------
        public long TranMstId { get; set; }
        public AccTranMst TranMst { get; set; }

        public long NoteById { get; set; }
        public ApplicationUser NoteBy { get; set; }

        public ApplicationUser ActionBy { get; set; }
        public long ActionById { get; set; }
        public DateTime ActionDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
