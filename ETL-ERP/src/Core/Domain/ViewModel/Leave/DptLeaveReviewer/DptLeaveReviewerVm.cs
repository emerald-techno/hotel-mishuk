namespace Domain.ViewModel.Leave.DptLeaveReviewer
{
    public class DptLeaveReviewerVm
    {
        public long Id { get; set; }
        public short ReviewFor { get; set; } // 1=Leave, 2=Loan, 3=PF 
        public short SlNo { get; set; } // 1=1ST APPROVER, 2=2ND APPROVER, 3=3RD APPROVER,... 99=FINAL APPROVER
        public string SlNoText => SlNo switch { 1 => "FIRST APPROVER", 2 => "SECOND APPROVER", 3 => "THIRD APPROVER", 4 => "FOURTH APPROVER", 99 => "FINAL APPROVER", _ => "" };
        public bool IsFinalReviewer { get; set; }

        // --- FK --- 

        public long DepartmentId { get; set; }
        public long ReviewerId { get; set; }
        public long AltReviewerId { get; set; }
    }
}
