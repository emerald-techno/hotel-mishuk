using Domain.ConfigurationModel;
using Domain.Entities.Accounting;
using Domain.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DU = Domain.Utility;

namespace Domain.ViewModel.Inventory.CategoryInfo
{
    public class CategoryInfoVm
    {
        public long Id { get; set; }

        [Required]
        [DisplayName("Category Name *")]
        public string CategoryName { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryType { get; set; } //I=Item, S=Service
        public string Remarks { get; set; }

        public long? LedgerId { get; set; }
        public string LedgerName { get; set; }

        public bool IsDeleted { get; set; }

        public string PhotoDocUrl { get; set; } = String.Empty;
        public IFormFile PhotoFile { get; set; }

        public IEnumerable<SelectListItem> CategoryTypeLookUp { get; set; }
        public IEnumerable<SelectListItem> LadgerLookUp { get; set; }

        #region File Upload
        public AppFile AppFile { get; set; }
        public string FolderPath => $"\\Categories";

        public AppFile GetAppFileToUploadFolder()
        {
            if (AppFile != null) { return AppFile; }

            var fileUpload = new AppFileUploadHelper
            {
                FormFile = PhotoFile?.IsNullOrEmpty() == true ? null : PhotoFile,
                FolderPath = FolderPath,
                FileNamePrefix = "PHOTO"
            };
            AppFile = DU.Utility.ConvertFormFileToAppFile(fileUpload)?.FirstOrDefault();
            if (PhotoFile != null) PhotoDocUrl = DU.Utility.GetFileSmallPath(AppFile?.FileUrl);
            return AppFile;
        }
        #endregion

    }
}
