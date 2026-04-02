using Domain.ModelInterface;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Leave.EmpLeaveReviewer
{
    public class EmpLeaveReviewerSearchVm : IDataTableSearch
    {
        public long Id { get; set; }
        public short ReviewFor { get; set; } // 1=Leave, 2=Loan, 3=PF 
        public short SlNo { get; set; } // 1=1ST APPROVER, 2=2ND APPROVER, 3=3RD APPROVER,... 99=FINAL APPROVER
        public string SlNoText => SlNo switch { 1 => "FIRST APPROVER", 2 => "SECOND APPROVER", 3 => "THIRD APPROVER", 4 => "FOURTH APPROVER", 99 => "FINAL APPROVER", _ => "" };
        public bool IsFinalReviewer { get; set; }

        //---------FK------------------

        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public long ReviewerId { get; set; }
        public string ReviewerName { get; set; }
        public long AltReviewerId { get; set; }
        public string AltReviewerName { get; set; }

        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }


        public IEnumerable<SelectListItem> EmployeeLookUp { get; set; }
        public IEnumerable<SelectListItem> ReviewerLookUp { get; set; }
        public IEnumerable<SelectListItem> AltReviewerLookUp { get; set; }
        public IEnumerable<SelectListItem> SLNoLookUp { get; set; }
    }
}
