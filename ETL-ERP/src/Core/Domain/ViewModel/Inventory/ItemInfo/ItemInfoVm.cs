using Domain.ConfigurationModel;
using Domain.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DU = Domain.Utility;

namespace Domain.ViewModel.Inventory.ItemInfo;

public class ItemInfoVm
{
    public long Id { get; set; }

    [Required]
    [DisplayName("Item Name *")]
    public string ItemName { get; set; }
    public string ItemCode { get; set; }
    public double ReorderQty { get; set; }
    public double MaxQty { get; set; }
    public double MinQty { get; set; }
    public double AlertQty { get; set; }
    public short LeadDay { get; set; }
    public string Remarks { get; set; }
    public long UnitId { get; set; }
    public long CategoryId { get; set; }
    public string UnitName { get; set; }
    public string CategoryName { get; set; }
    public bool IsDeleted { get; set; }
    public IEnumerable<SelectListItem> UnitLookUp { get; set; }
    public IEnumerable<SelectListItem> CategoryLookUp { get; set; }

    [StringLength(120)]
    public string PhotoDocUrl { get; set; }
    public IFormFile PhotoFile { get; set; }

    public long? LedgerId { get; set; }
    public string HeadCode { get; set; }

    #region File Upload 

    public AppFile AppFile { get; set; }
    public string FolderPath => $"\\Items";

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
