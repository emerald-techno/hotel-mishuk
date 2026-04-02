using Domain.ConfigurationModel;
using Domain.Utility;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Hr.EmpExperience
{
    public class EmpExperienceVm
    {
        public long Id { get; set; }

        [StringLength(100, ErrorMessage = "Company Name can not be more than 100 characters")]
        [Required(ErrorMessage = "Company Name is required")]
        [DisplayName("Company Name: *")]
        public string CompanyName { get; set; }

        public DateTime? DateFrom { get; set; }
        public string DateFromStr { get; set; }
        public DateTime? DateTo { get; set; }
        public string DateToStr { get; set; }

        [StringLength(40, ErrorMessage = "Designation can not be more than 40 characters")]
        [DisplayName("Responsibility: ")]
        public string Designation { get; set; }
        [StringLength(600, ErrorMessage = "Responsibility can not be more than 600 characters")]
        public string Responsibility { get; set; }
        public double LastDrawnSalary { get; set; }

        [StringLength(300, ErrorMessage = "Left Reason can not be more than 300 characters")]
        [DisplayName("Left Reason: ")]
        public string LeftReason { get; set; }
        [StringLength(120)]
        [DisplayName("Experience File")]
        public string ExperienceUrl { get; set; }
        public int SlNo { get; set; }
        [StringLength(350, ErrorMessage = "Remarks can not be more than 350 characters")]
        [DisplayName("Remarks: ")]
        public string Remarks { get; set; }

        //--------------------------------------------------
        public long EmployeeId { get; set; }
        public bool IsAjaxPost { get; set; }

        #region File Upload

        public IFormFile ExperienceFile { get; set; }

        public AppFile AppFile { get; set; }

        public string FolderPath => $"\\EmpEducation";

        public AppFile GetAppFileToUploadFolder()
        {
            if (AppFile != null) { return AppFile; }

            var fileUpload = new AppFileUploadHelper
            {
                FormFile = ExperienceFile?.IsNullOrEmpty() == true ? null : ExperienceFile,
                FolderPath = FolderPath,
                FileNamePrefix = "EXPERIENCE"
            };
            AppFile = Utility.Utility.ConvertFormFileToAppFile(fileUpload)?.FirstOrDefault();
            if (ExperienceFile != null) ExperienceUrl = Utility.Utility.GetFileSmallPath(AppFile?.FileUrl);
            return AppFile;
        }

        #endregion
    }
}
