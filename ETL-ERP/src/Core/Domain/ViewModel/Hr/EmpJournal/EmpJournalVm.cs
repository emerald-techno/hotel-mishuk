using Domain.ConfigurationModel;
using Domain.Utility;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Hr.EmpJournal
{
    public class EmpJournalVm
    {
        public long Id { get; set; }

        [Required]
        [StringLength(80)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Title { get; set; }

        [StringLength(400)]
        public string About { get; set; }

        [StringLength(120)]
        public string DocUrl { get; set; }
        public DateTime SubmitDate { get; set; }
        public long EmployeeId { get; set; }
        public bool IsAjaxPost { get; set; }

        #region File Upload

        public IFormFile DocFile { get; set; }

        public AppFile AppFile { get; set; }

        public string FolderPath => $"\\EmpJournal";

        public AppFile GetAppFileToUploadFolder()
        {
            if (AppFile != null) { return AppFile; }

            var fileUpload = new AppFileUploadHelper
            {
                FormFile = DocFile?.IsNullOrEmpty() == true ? null : DocFile,
                FolderPath = FolderPath,
                FileNamePrefix = "JOURNAL"
            };
            AppFile = Utility.Utility.ConvertFormFileToAppFile(fileUpload)?.FirstOrDefault();
            if (DocFile != null) DocUrl = Utility.Utility.GetFileSmallPath(AppFile?.FileUrl);
            return AppFile;
        }

        #endregion
    }
}
