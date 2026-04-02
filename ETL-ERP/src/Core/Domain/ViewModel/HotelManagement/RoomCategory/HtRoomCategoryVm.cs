using Domain.ConfigurationModel;
using Domain.Utility;
using Domain.ViewModel.HotelManagement.RoomFacility;
using Domain.ViewModel.Website;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.HotelManagement.RoomCategory;

public class HtRoomCategoryVm
{
    public long Id { get; set; }

    [StringLength(120, ErrorMessage = "Category name can not be more than 120 characters")]
    [Required(ErrorMessage = "Category name is required")]
    public string CategoryName { get; set; }

    public bool IsAc { get; set; }

    public bool IsBalcony { get; set; }

    public long BedTypeId { get; set; }

    public int BedNumber { get; set; }

    public int Capacity { get; set; }

    public string OtherInfo { get; set; }

    public double Rent { get; set; }

    public double? OfferRate { get; set; } = 0;

    public double ServiceCharge { get; set; }

    [Required(ErrorMessage = "Total rent is required")]
    public double TotalRent { get; set; }

    public string Remarks { get; set; }

    public bool IsActive { get; set; }
    public bool IsWebSiteShow { get; set; } = true;

    public string PhotoUrl { get; set; } = String.Empty;
    //--------------------------------
    public bool HasDiscount { get; set; }
    public DiscountType? DiscountType { get; set; }
    public double? DiscountValue { get; set; }
    public DateTime? DiscountStart { get; set; }
    public DateTime? DiscountEnd { get; set; }

    //--------------------------------

    public DateTime ActionDate { get; set; }
    public long ActionById { get; set; }



    //-------------------------------

    public IEnumerable<SelectListItem> BedTypeLookUp { get; set; }
    public ICollection<HtRoomFacilityVm> RoomFacilityList { get; set; }


    //PHotoFile

    public IFormFile PhotoFile { get; set; }
    public AppFile AppFile { get; set; }
    public string FolderPath => $"RoomCategory";

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
}

public enum DiscountType
{
    Percent,
    Amount
}
