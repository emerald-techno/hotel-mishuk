using Domain.ModelInterface;

namespace Domain.ViewModel.Accounting.AccTranNote
{
    public class AccTranNoteSearchVm : IDataTableSearch
    {
        public long Id { get; set; }
        public DateTime NoteDate { get; set; }
        public string NoteType { get; set; }
        public string NoteDesc { get; set; }
        public int SlNo { get; set; }
        public string Remarks { get; set; }

        //----------------FK-------------------------
        public long TranMstId { get; set; }
        public long NoteById { get; set; }
        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
    }
}
