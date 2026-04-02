using Domain.ConfigurationModel;
using Domain.Utility;
using Domain.ViewModel.Leave.LvAppReviewer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.ViewModel.Leave.EmpLeaveApplication
{
    public class EmpLeaveApplicationVm
    {
        public long Id { get; set; }

        [StringLength(60)]
        public string ApplicationNo { get; set; }
        public DateTime FromDate { get; set; }
        public string FromDateStr { get; set; }
        public DateTime ToDate { get; set; }
        public string ToDateStr { get; set; }
        public DateTime? AprFromDate { get; set; }
        public DateTime? AprToDate { get; set; }
        public DateTime? ActualFromDate { get; set; }
        public DateTime? ActualToDate { get; set; }
        public DateTime SubmitDate { get; set; }

        [StringLength(500)]
        public string Reason { get; set; }
        public short TotalApprovalLeave { get; set; }
        public short SalaryType { get; set; } // F=Full Salary, H=Half, W=Without
        public DateTime? CancelDate { get; set; }

        [StringLength(150)]
        public string CancelReason { get; set; }
        public short Status { get; set; } // 0 = SUBMISSION, 1 = FIRST REVIEW, 2 = SECOND REVIEW, 3 = THIRD REVIEW …. ,99 = FINAL APPROVE, 98 = REJECT, 97 = SELF CANCEL
        public string StatusText => Status switch { 0 => "SUBMISSION", 1 => "FIRST REVIEW", 2 => "SECOND REVIEW", 3 => "THIRD REVIEW", 4 => "FOURTH REVIEW", 97 => "SELF CANCEL", 98 => "REJECT", 99 => "FINAL APPROVE", _ => "" };

        [StringLength(120)]
        public string FileDoc { get; set; }

        [StringLength(200)]
        public string Remarks { get; set; }
        public DateTime ActionDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool IsDeleted { get; set; }

        public int? LeaveTypeBalance { get; set; }
        public int LeaveTaken { get; set; }

        // ---- FK ----

        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string EmpDesignation { get; set; }
        public string EmpDepartment { get; set; }
        public string EmpPhotoUrl { get; set; }
        public string Code { get; set; }
        public DateTime JoinDate { get; set; }
        public int EmployeeStatus { get; set; } // 1 = Permanent, 2 = Contractual, 3 = Adhoc, 4 = Guest
        public string EmployeeStatusText => EmployeeStatus switch { 1 => "Permanent", 2 => "Contractual", 3 => "Adhoc", 4 => "Guest", _ => "--" };
        public string Gender { get; set; } //M = Male, F = Female
        public string GenderText => Gender switch { "M" => "Male", "F" => "Female", _ => "--" };
        public long LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; }
        public long SubmitById { get; set; }

        public IEnumerable<SelectListItem> EmployeeLookUp { get; set; }
        public IEnumerable<SelectListItem> LeaveTypeLookUp { get; set; }

        [NotMapped]
        public ICollection<LeaveBalanceVm> EmpLeaveInfos { get; set; }

        [NotMapped]
        public List<EmpLeaveApplicationVm> EmpLeaveApps { get; set; }

        [NotMapped]
        public List<LeaveReviewerVm> LeaveReviewerVms { get; set; }

        [NotMapped]
        public List<LvAppReviewerVm> LvAppReviewerVms { get; set; }

        [NotMapped]
        public bool IsReviewer { get; set; }

        [NotMapped]
        public LvAppReviewerVm CurrentReviewer { get; set; }
        public bool CanCancel { get; set; }

        #region File Upload

        public IFormFile LeaveFile { get; set; }

        public AppFile AppFile { get; set; }

        public string FolderPath => $"\\EmployeeLeaveApplicationOnline";

        public AppFile GetLeaveApplicationFileToUploadFolder()
        {
            if (AppFile != null) { return AppFile; }

            var fileUpload = new AppFileUploadHelper
            {
                FormFile = LeaveFile?.IsNullOrEmpty() == true ? null : LeaveFile,
                FolderPath = FolderPath,
                FileNamePrefix = "EMPLEAVEAPP"
            };
            AppFile = Utility.Utility.ConvertFormFileToAppFile(fileUpload)?.FirstOrDefault();
            if (LeaveFile != null) FileDoc = Utility.Utility.GetFileSmallPath(AppFile?.FileUrl);
            return AppFile;
        }

        #endregion

    }
}
