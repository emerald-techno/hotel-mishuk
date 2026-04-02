using Domain.ConfigurationModel;
using Domain.Utility;
using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DU = Domain.Utility;

namespace Domain.ViewModel.Inventory.SupplierInfo
{
    public class SupplierInfoVm
    {
        public long Id { get; set; }

        [Required]
        [DisplayName("Supplier Name *")]
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhotoDoc { get; set; }
        public string Remarks { get; set; }
        public bool IsDeleted { get; set; }

        public IFormFile PhotoFile { get; set; }

        #region File Upload 

        public AppFile AppFile { get; set; }
        public string FolderPath => $"\\Suppliers";

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
            if (PhotoFile != null) PhotoDoc = DU.Utility.GetFileSmallPath(AppFile?.FileUrl);
            return AppFile;
        }

        #endregion

    }
}
