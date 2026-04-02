using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Leave.LvAppReviewer
{
    public class LvAppReviewerSearchVm : IDataTableSearch
    {
        public long Id { get; set; }
        public short SlNo { get; set; } // 1=1ST REVIEWER, 2=2ND REVIEWER, 3=3RD REVIEWER,... 99=FINAL APPROVER
        public string SlNoText => SlNo switch { 1 => "FIRST REVIEWER", 2 => "SECOND REVIEWER", 3 => "THIRD REVIEWER", 4 => "FOURTH REVIEWER", 99 => "FINAL APPROVE", _ => "" };
        public short Status { get; set; } // 0=Not Response, 1=Review, 2= Approve, 3=Reject
        public string StatusText => Status switch { 0 => "Not Response", 1 => "Review", 2 => "Approve", 3 => "Reject", _ => "" };

        [StringLength(200)]
        public string Remarks { get; set; }
        public DateTime ReceiveTime { get; set; }
        public DateTime? ResponseTime { get; set; }
        public bool IsFinalReviewer { get; set; }
        public bool IsDeleted { get; set; }

        // ---- FK ----
        public long LeaveAppId { get; set; }
        public string ApplicationNo { get; set; }
        public string LeaveTypeName { get; set; }
        public DateTime? AppFromDate { get; set; }
        public DateTime? AppToDate { get; set; }
        public short? AppStatus { get; set; }
        public string AppStatusText => AppStatus switch { 0 => "SUBMISSION", 1 => "FIRST REVIEW", 2 => "SECOND REVIEW", 3 => "THIRD REVIEW", 4 => "FOURTH REVIEW", 97 => "SELF CANCEL", 98 => "REJECT", 99 => "FINAL APPROVE", _ => "" };
        public string EmployeeName { get; set; }
        public long ReviewerId { get; set; }
        public string ReviewerName { get; set; }
        public long AltReviewerId { get; set; }

        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
    }
}
