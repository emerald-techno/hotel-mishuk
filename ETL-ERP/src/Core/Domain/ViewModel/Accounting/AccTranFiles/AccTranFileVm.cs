using Domain.ConfigurationModel;
using Domain.Utility;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Accounting.AccTranFiles
{
    public class AccTranFileVm
    {
        public long Id { get; set; }
        [StringLength(80, ErrorMessage = "File name can not be more than 80 characters")]
        public string FileName { get; set; }

        public DateTime UploadDate { get; set; }

        [StringLength(120)]
        public string UploadFile { get; set; }

        public int SlNo { get; set; }
        [StringLength(120, ErrorMessage = "Remarks can not be more tha 120 characters")]
        public string Remarks { get; set; }

        //-----------------FK-----------------------
        public long TranMstId { get; set; }
        public bool IsAjaxPost { get; set; }

        #region File Upload

        public IFormFile UploadFilePick { get; set; }

        public AppFile AppFile { get; set; }

        public string FolderPath => $"\\VoucherFile";

        public AppFile GetAppFileToUploadFolder()
        {
            if (AppFile != null) { return AppFile; }

            var fileUpload = new AppFileUploadHelper
            {
                FormFile = UploadFilePick?.IsNullOrEmpty() == true ? null : UploadFilePick,
                FolderPath = FolderPath,
                FileNamePrefix = "DOC"
            };
            AppFile = Utility.Utility.ConvertFormFileToAppFile(fileUpload)?.FirstOrDefault();
            if (UploadFilePick != null) UploadFile = Utility.Utility.GetFileSmallPath(AppFile?.FileUrl);
            return AppFile;
        }

        #endregion
    }
}
