using Domain.ConfigurationModel;
using Domain.Utility;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Hr.EmpPosting
{
    public class EmpPostingVm
    {
        public long Id { get; set; }
        public DateTime? DateFrom { get; set; }
        public string DateFromStr { get; set; }
        public DateTime? DateTo { get; set; }
        public string DateToStr { get; set; }

        [StringLength(1)]
        [Required(ErrorMessage = "Posting Type required")]
        [DisplayName("Posting Type: *")]
        public string PostingType { get; set; }

        public string PostingTypeText => PostingType switch { "R" => "Regular", "S" => "OSD", "P" => "Promotion", "I" => "Increment", "B" => "Promotion & Increment", "O" => "Other", _ => "--" };

        [DisplayName("Net Salary: ")]
        public double NetSalary { get; set; }

        public double PreNetSalary { get; set; }
        public string PostingDoc { get; set; }

        [DisplayName("Serial No: ")]
        public int SlNo { get; set; }

        [StringLength(350, ErrorMessage = "Remarks can not be more than 350 characters")]
        [DisplayName("Remarks: ")]
        public string Remarks { get; set; }

        //-------------------------------------------
        public long EmployeeId { get; set; }
        public long? DepartmentId { get; set; }
        public long? DesignationId { get; set; }
        public long? PreDepartmentId { get; set; }
        public long? PreDesignationId { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
        public string PreDepartmentName { get; set; }
        public string PreDesignationName { get; set; }
        public bool IsAjaxPost { get; set; }

        #region File Upload

        public IFormFile PostingFile { get; set; }

        public AppFile AppFile { get; set; }

        public string FolderPath => $"\\EmpPosting";

        public AppFile GetAppFileToUploadFolder()
        {
            if (AppFile != null) { return AppFile; }

            var fileUpload = new AppFileUploadHelper
            {
                FormFile = PostingFile?.IsNullOrEmpty() == true ? null : PostingFile,
                FolderPath = FolderPath,
                FileNamePrefix = "CERTIFICATE"
            };
            AppFile = Utility.Utility.ConvertFormFileToAppFile(fileUpload)?.FirstOrDefault();
            if (PostingFile != null) PostingDoc = Utility.Utility.GetFileSmallPath(AppFile?.FileUrl);
            return AppFile;
        }

        #endregion
    }
}
