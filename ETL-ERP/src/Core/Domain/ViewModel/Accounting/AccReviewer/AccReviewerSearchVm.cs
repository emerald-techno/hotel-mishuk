using Domain.ModelInterface;

namespace Domain.ViewModel.Accounting.AccReviewer
{
    public class AccReviewerSearchVm : IDataTableSearch
    {
        public long Id { get; set; }
        public string ReviewerType { get; set; }
        public int SlNo { get; set; }
        public string Remarks { get; set; }

        //-------------------FK-------------------
        public long ReviewrId { get; set; }
        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
    }
}
