using Domain.ConfigurationModel;
using Domain.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.UserAccount;

public class RegisterViewModel
{
    public long Id { get; set; }

    [Required]
    [DisplayName("Full Name")]
    public string FullName { get; set; }
    public string Mobile { get; set; }
    public DateTime? DoB { get; set; }

    [Required]
    [StringLength(1)]
    public string Gender { get; set; }


    public string UserName { get; set; }

    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }


    [DisplayName("Contact No")]
    [RegularExpression(@"^(\d{11})$", ErrorMessage = "Contact No is not valid, Required '11' Digits !")]
    public string ContactNo { get; set; }
    public bool IsMaskEmail { get; set; }

    [DisplayName("Role *")]
    public short RoleId { get; set; }

    //[DisplayName("Designation *")]
    //public short DesignationId { get; set; }

    public IEnumerable<SelectListItem> RoleLookUp { get; set; }

    [StringLength(120)]
    public string PhotoUrl { get; set; }
    public IFormFile PhotoFile { get; set; }

    public RegisterViewModel GetUserModel() => AppUtility.GetUserModel("", FullName, Email, Mobile, Password, ConfirmPassword, PhotoUrl);

    #region File Upload 

    public AppFile AppFile { get; set; }
    public string FolderPath => $"\\Users";

    public AppFile GetAppFileToUploadFolder()
    {
        if (AppFile != null) { return AppFile; }

        var fileUpload = new AppFileUploadHelper { FormFile = PhotoFile?.IsNullOrEmpty() == true ? null : PhotoFile, FolderPath = FolderPath, FileNamePrefix = "PHOTO" };
        AppFile = Utility.Utility.ConvertFormFileToAppFile(fileUpload)?.FirstOrDefault();
        if (PhotoFile != null) PhotoUrl = Utility.Utility.GetFileSmallPath(AppFile?.FileUrl);
        return AppFile;
    }

    #endregion
}
