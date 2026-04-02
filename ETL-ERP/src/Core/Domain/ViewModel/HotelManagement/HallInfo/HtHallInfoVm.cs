using Domain.ConfigurationModel;
using Domain.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.HotelManagement.RoomInfo;

public class HtHallInfoVm
{
    public long Id { get; set; }

    [Required(ErrorMessage = "Hall name is required")]
    [StringLength(25)]
    [Remote(action: "IsHallNameExist", controller: "HallInfo", AdditionalFields = "InitHallName")]
    public string HallName { get; set; }

    [StringLength(500)]
    public string HallInformation { get; set; }

    [StringLength(150)]
    public string OtherInfo { get; set; }
    public double Rent { get; set; }
    public double Vat { get; set; }
    public double ServiceCharge { get; set; }
    public double TotalRent { get; set; }

    [StringLength(120)]
    public string Remakrs { get; set; }

    [StringLength(120)]
    public string PhotoUrl { get; set; } = String.Empty;
    public bool IsActive { get; set; }

    //PHotoFile

    public IFormFile PhotoFile { get; set; }
    public AppFile AppFile { get; set; }
    public string FolderPath => $"Hall";

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