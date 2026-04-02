using Domain.ConfigurationModel;
using Domain.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Restaurant.FoodCategory;

public class FoodCategoryVm
{
    public long Id { get; set; }

    [StringLength(100, ErrorMessage = "Food category name can not be more than 100 characters")]
    [Required(ErrorMessage = "Food category name is required")]
    public string CategoryName { get; set; } // Bengali, Indian, Chinese
    public string Remarks { get; set; }
    public string PhotoUrl { get; set; } = string.Empty;
    public IEnumerable<SelectListItem> FoodCategoryLookUp { get; set; }
    public long? ParentCategoryId { get; set; }
    public string ParentCategoryName { get; set; }
    public bool IsActive { get; set; }
    public long ActionById { get; set; }
    public DateTime ActionDate { get; set; }

    #region Photo

    public IFormFile PhotoFile { get; set; }
    public AppFile AppFile { get; set; }
    public string FolderPath => $"FoodCategory";

    public AppFile GetAppFileToUploadFolder()
    {
        if (AppFile != null) { return AppFile; }

        var fileUpload = new AppFileUploadHelper
        {
            FormFile = PhotoFile?.IsNullOrEmpty() == true ? null : PhotoFile,
            FolderPath = FolderPath,
            FileNamePrefix = "PHOTO"
        };
        AppFile = Utility.Utility.ConvertFormFileToAppFile(fileUpload)?.FirstOrDefault();
        if (PhotoFile != null) PhotoUrl = Utility.Utility.GetFileSmallPath(AppFile?.FileUrl);
        return AppFile;
    }

    #endregion
}
