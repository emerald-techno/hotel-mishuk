using Domain.ConfigurationModel;
using Domain.Enums.AppEnums;
using Domain.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.HotelManagement.GuestInfo;

public class HtGuestInfoVm
{
    public long Id { get; set; }
    public string GuestCode { get; set; }

    [Required]
    public string Salutation { get; set; }

    [Required]
    public string FirstName { get; set; }

    [StringLength(80, ErrorMessage = "Last name can not be more than 80 characters")]
    public string LastName { get; set; }

    [Required]
    [StringLength(80)]
    public string Mobile { get; set; }

    [StringLength(40, ErrorMessage = "Email can not be more than 40 characters")]
    public string Email { get; set; }

    [StringLength(120, ErrorMessage = "Occupation can not be more than 120 characters")]
    public string Occupation { get; set; }
    public string Gender { get; set; }
    public string GenderText => Gender switch { "M" => "Male", "F" => "Female", _ => "--" };

    [StringLength(250, ErrorMessage = "Address can not be more than 250 characters")]
    public string Address { get; set; }
    public long CountryId { get; set; }
    public string CountryName { get; set; }
    public long? DistrictId { get; set; }
    public string DistrictName { get; set; }
    public DateTime? Dob { get; set; }
    public string DobString { get; set; }
    public IdentityTypeEnum? IdentityType { get; set; }

    [StringLength(120, ErrorMessage = "Identity number can not be more than 120 characters")]
    public string IdentityNo { get; set; }

    [StringLength(120)]
    public string IdentityPhotoUrl { get; set; }

    [StringLength(120)]
    public string PhotoUrl { get; set; }
    public bool IsVip { get; set; }

    [StringLength(150)]
    public string Note { get; set; }
    public long? CompanyId { get; set; }
    public string CompanyName { get; set; }
    public string CompanyMobile { get; set; }

    public IEnumerable<SelectListItem> ClientCompanyLookUp { get; set; }
    public IEnumerable<SelectListItem> SalutationLookUp { get; set; }
    public IEnumerable<SelectListItem> GenderLookUp { get; set; }
    public IEnumerable<SelectListItem> IdentityTypeLookUp { get; set; }
    public IEnumerable<SelectListItem> CountryLookUp { get; set; }
    public IEnumerable<SelectListItem> DistrictLookUp { get; set; }

    //PHotoFile

    public IFormFile PhotoFile { get; set; }
    public IFormFile IdentityFile { get; set; }
    public AppFile AppFile { get; set; }
    public AppFile AppIdentityFile { get; set; }
    public string FolderPath => $"\\GuestInfo";

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

    public AppFile GetIdentityFileToUploadFolder()
    {
        if (AppIdentityFile != null) { return AppIdentityFile; }

        var fileUpload = new AppFileUploadHelper
        {
            FormFile = IdentityFile?.IsNullOrEmpty() == true ? null : IdentityFile,
            FolderPath = FolderPath,
            FileNamePrefix = "Identity"
        };
        AppIdentityFile = Utility.Utility.ConvertFormFileToAppFile(fileUpload)?.FirstOrDefault();
        if (IdentityFile != null) IdentityPhotoUrl = Utility.Utility.GetFileSmallPath(AppIdentityFile?.FileUrl);
        return AppIdentityFile;
    }
}

public class HtGuestDetailsVm
{
    public string GuestName { get; set; }
    public string Mobile { get; set; }
    public string Email { get; set; }
    public string Occupation { get; set; }
    public string GenderText { get; set; }
    public string Address { get; set; }
    public string PhotoUrl { get; set; }
    public string VipStatus { get; set; }
    public string BookingNo { get; set; }
    public string BookingDate { get; set; }
    public string CheckInTime { get; set; }
    public string CheckOutTime { get; set; }
    public string VisitPurpose { get; set; }

    public string BookingStatus { get; set; }
    public string PaymentStatus { get; set; }
}