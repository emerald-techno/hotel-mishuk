using Domain.ModelInterface;

namespace Domain.ViewModel.Leave.EmpLeaveApplication
{
    public class EmpLeaveApplicationSearchVm : IDataTableSearch
    {
        public long Id { get; set; }
        public string ApplicationNo { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime? AprFromDate { get; set; }
        public DateTime? AprToDate { get; set; }
        public DateTime? ActualFromDate { get; set; }
        public DateTime? ActualToDate { get; set; }
        public DateTime SubmitDate { get; set; }
        public string Reason { get; set; }
        public short TotalApprovalLeave { get; set; }
        public short SalaryType { get; set; } // F=Full Salary, H=Half, W=Without
        public DateTime? CancelDate { get; set; }
        public string CancelReason { get; set; }
        public short Status { get; set; } // 0 = SUBMISSION, 1 = FIRST REVIEW, 2 = SECOND REVIEW, 3 = THIRD REVIEW …. ,99 = FINAL APPROVE, 98 = REJECT, 97 = SELF CANCEL
        public string StatusText => Status switch { 0 => "SUBMISSION", 1 => "FIRST REVIEW", 2 => "SECOND REVIEW", 3 => "THIRD REVIEW", 4 => "FOURTH REVIEW", 97 => "SELF CANCEL", 98 => "REJECT", 99 => "FINAL APPROVE", _ => "" };
        public string FileDoc { get; set; }
        public string Remarks { get; set; }
        public DateTime ActionDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }

        // ---- FK ----

        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public long LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; }
        public long SubmitById { get; set; }
        public string SubmitByName { get; set; }

        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
    }
}
