using Domain.ConfigurationModel;
using Domain.Utility;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Hr.EmpEducation
{
    public class EmpEducationVm
    {
        public long Id { get; set; }
        [StringLength(60, ErrorMessage = "Course Name can not be more than 60 characters")]
        [Required(ErrorMessage = "Course Name required")]
        public string CourseName { get; set; }
        [StringLength(120, ErrorMessage = "Institution Name can not be more than 120 characters")]
        [Required(ErrorMessage = "Institute Name required")]
        public string InstitutionName { get; set; }

        [StringLength(100, ErrorMessage = "Board University can not be more than 100 characters")]
        [Required(ErrorMessage = "Board University required")]
        public string BoardUniversity { get; set; }
        public int? YearFrom { get; set; }
        public int? YearTo { get; set; }
        public int? PassingYear { get; set; }
        [StringLength(35, ErrorMessage = "Subject Group can not be more than 35 characters")]
        public string SubjectGroup { get; set; }
        [StringLength(30, ErrorMessage = "Result can not be more than 30 characters")]
        public string Result { get; set; }

        [StringLength(120)]
        public string CertificateUrl { get; set; }
        public int SlNo { get; set; }
        [StringLength(350, ErrorMessage = "Remarks can not be more than 350 characters")]
        public string Remarks { get; set; }

        //-------------------------------------
        public long EmployeeId { get; set; }
        public bool IsAjaxPost { get; set; }

        #region File Upload

        public IFormFile CertificateFile { get; set; }

        public AppFile AppFile { get; set; }

        public string FolderPath => $"\\EmpEducation";

        public AppFile GetAppFileToUploadFolder()
        {
            if (AppFile != null) { return AppFile; }

            var fileUpload = new AppFileUploadHelper
            {
                FormFile = CertificateFile?.IsNullOrEmpty() == true ? null : CertificateFile,
                FolderPath = FolderPath,
                FileNamePrefix = "CERTIFICATE"
            };
            AppFile = Utility.Utility.ConvertFormFileToAppFile(fileUpload)?.FirstOrDefault();
            if (CertificateFile != null) CertificateUrl = Utility.Utility.GetFileSmallPath(AppFile?.FileUrl);
            return AppFile;
        }

        #endregion
    }
}
