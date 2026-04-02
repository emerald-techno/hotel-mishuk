using Domain.ConfigurationModel;
using Domain.Utility;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Hr.EmpTraining
{
    public class EmpTrainingVm
    {
        public long Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Course Title can not be more than 100 characters")]
        public string CourseTitle { get; set; }

        [StringLength(100, ErrorMessage = "Institute Name can not be more than 100 characters")]
        public string InstituteName { get; set; }

        [StringLength(120, ErrorMessage = "Location can not be more than 120 characters")]
        public string Location { get; set; }
        public DateTime? DateFrom { get; set; }
        public string DateFromStr { get; set; }
        public DateTime? DateTo { get; set; }
        public string DateToStr { get; set; }

        [StringLength(30, ErrorMessage = "Result can not be more than 120 characters")]
        public string Result { get; set; }

        [StringLength(120)]
        public string TrainingFileUrl { get; set; }
        public int SlNo { get; set; }

        [StringLength(350, ErrorMessage = "Remarks can not be more than 350 characters")]
        public string Remarks { get; set; }

        //-----------------------------------------------
        public long EmployeeId { get; set; }
        public bool IsAjaxPost { get; set; }

        #region File Upload

        public IFormFile TrainingFile { get; set; }

        public AppFile AppFile { get; set; }

        public string FolderPath => $"\\EmpTraining";

        public AppFile GetAppFileToUploadFolder()
        {
            if (AppFile != null) { return AppFile; }

            var fileUpload = new AppFileUploadHelper
            {
                FormFile = TrainingFile?.IsNullOrEmpty() == true ? null : TrainingFile,
                FolderPath = FolderPath,
                FileNamePrefix = "TRAINING"
            };
            AppFile = Utility.Utility.ConvertFormFileToAppFile(fileUpload)?.FirstOrDefault();
            if (TrainingFile != null) TrainingFileUrl = Utility.Utility.GetFileSmallPath(AppFile?.FileUrl);
            return AppFile;
        }

        #endregion
    }
}
