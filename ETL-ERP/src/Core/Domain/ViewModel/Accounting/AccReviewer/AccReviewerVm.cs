using Domain.ModelInterface;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Accounting.AccReviewer
{
    public class AccReviewerVm : IDataTableSearch
    {
        public long Id { get; set; }

        [DisplayName("Reviewer Type: ")]
        [StringLength(1)]
        public string ReviewerType { get; set; }

        [DisplayName("Serial No: ")]
        public int SlNo { get; set; }

        [StringLength(120, ErrorMessage = "Remarks can not be more than 120 characters")]
        public string Remarks { get; set; }

        //--------------FK-------------------------
        public long ReviewrId { get; set; }
        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
    }
}
