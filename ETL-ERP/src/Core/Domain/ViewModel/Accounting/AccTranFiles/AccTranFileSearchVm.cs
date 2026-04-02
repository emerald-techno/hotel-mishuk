using Domain.ModelInterface;

namespace Domain.ViewModel.Accounting.AccTranFiles
{
    public class AccTranFileSearchVm : IDataTableSearch
    {
        public long Id { get; set; }
        public string FileName { get; set; }
        public DateTime UploadDate { get; set; }
        public string UploadFile { get; set; }
        public int SlNo { get; set; }
        public string Remarks { get; set; }

        //---------------FK-------------------
        public long TranMstId { get; set; }
        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
    }
}
