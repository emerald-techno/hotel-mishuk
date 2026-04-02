using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Accounting.AccTranNote
{
    public class AccTranNoteVm
    {
        public long Id { get; set; }
        public DateTime NoteDate { get; set; }

        [DisplayName("Note Date: ")]
        public string NoteDateStr { get; set; }

        [StringLength(20, ErrorMessage = "Note type can not be more than 20 characters")]
        [Required(ErrorMessage = "Note type is required")]
        [DisplayName("Note Type: ")]
        public string NoteType { get; set; } // Create, Approve, Audit, BankClear, FinalAttach, Note, AutoCreate


        [StringLength(250, ErrorMessage = "Note description can not be more than 250 characters")]
        [DisplayName("Note Description: ")]
        public string NoteDesc { get; set; }

        [DisplayName("Serial No: ")]
        public int SlNo { get; set; }


        [StringLength(120, ErrorMessage = "Remarks can not be more than 120 characters")]
        [DisplayName("Remarks: ")]
        public string Remarks { get; set; }

        //---------------FK-----------------------
        public long TranMstId { get; set; }
        public long NoteById { get; set; }
        public bool IsAjaxPost { get; set; }
    }
}
