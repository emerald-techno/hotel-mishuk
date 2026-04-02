using System.ComponentModel.DataAnnotations;
using Domain.ConfigurationModel;
using Domain.Utility;
using Domain.ViewModel.Restaurant.FoodIngredient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Restaurant.FoodItem;

public class FoodItemVm
{
    public long Id { get; set; }

    [Required]
    [StringLength(100)]
    [Remote(action: "IsNameExist", controller: "FoodItem", AdditionalFields = "InitItemName, CategoryId")]
    public string ItemName { get; set; } // Bengali, Indian, Chinese

    [Required]
    [StringLength(50)]
    [Remote(action: "IsCodeExist", controller: "FoodItem", AdditionalFields = "InitCode")]
    public string ItemCode { get; set; }

    [StringLength(120)]
    public string PhotoUrl { get; set; }

    [StringLength(300)]
    public string Description { get; set; }
    public double VAT { get; set; } = 0;
    public double Rate { get; set; } = 0;
    public double OfferRate { get; set; } = 0;
    public double NetRate { get; set; } = 0;
    public string Remarks { get; set; }
    public bool IsActive { get; set; }
    public bool IsSetMenuItem { get; set; } = false;
    public long? ActionById { get; set; }
    public DateTime? ActionDate { get; set; }

    // --- Fk ---

    public long CategoryId { get; set; }
    public string CategoryName { get; set; }
    public IEnumerable<SelectListItem> FoodCategoryLookUp { get; set; }

    public IEnumerable<SelectListItem> ItemLookup { get; set; }
    public IEnumerable<SelectListItem> UnitLookup { get; set; }
    public IEnumerable<SelectListItem> FoodItemLookup { get; set; }
    public ICollection<FoodIngredientVM> FoodIngredientVMs { get; set; }
    public ICollection<FoodSetItemVm> FoodSetItemVms { get; set; }

    #region Photo

    public IFormFile PhotoFile { get; set; }
    public AppFile AppFile { get; set; }
    public string FolderPath => $"FoodItem";

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

public class FoodSetItemVm
{
    public long Id { get; set; }
    public bool IsEnable { get; set; }
    public long FoodItemId { get; set; }
    public string FoodItemName { get; set; }
    public long SetFoodItemId { get; set; }
    public string SetFoodItemName { get; set; }
    public double SetFoodItemPrice { get; set; } = 0;
    public double Quantity { get; set; } = 0;
}