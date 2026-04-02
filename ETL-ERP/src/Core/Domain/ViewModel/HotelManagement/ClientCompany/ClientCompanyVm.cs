using Domain.ConfigurationModel;
using Domain.Utility;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.HotelManagement.ClientCompany;

public class ClientCompanyVm
{
    public long Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; }

    [StringLength(350)]
    public string Address { get; set; }

    [StringLength(30)]
    public string Mobile { get; set; }

    [StringLength(100)]
    public string Email { get; set; }

    [StringLength(100)]
    public string WebSite { get; set; }

    [StringLength(100)]
    public string LogoUrl { get; set; }
    public DateTime ActionDate { get; set; }
    public long? ActionById { get; set; }
    public bool IsAjaxPost { get; set; }

    #region File Upload

    public IFormFile LogoFile { get; set; }
    public AppFile AppFile { get; set; }
    public string FolderPath => $"\\ClientCompany";

    public AppFile GetAppFileToUploadFolder()
    {
        if (AppFile != null) { return AppFile; }

        var fileUpload = new AppFileUploadHelper
        {
            FormFile = LogoFile?.IsNullOrEmpty() == true ? null : LogoFile,
            FolderPath = FolderPath,
            FileNamePrefix = "PHOTO"
        };
        AppFile = Utility.Utility.ConvertFormFileToAppFile(fileUpload)?.FirstOrDefault();
        if (LogoFile != null) LogoUrl = Utility.Utility.GetFileSmallPath(AppFile?.FileUrl);
        return AppFile;
    }

    #endregion
}
