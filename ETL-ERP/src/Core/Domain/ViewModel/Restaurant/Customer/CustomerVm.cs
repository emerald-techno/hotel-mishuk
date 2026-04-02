using Domain.ConfigurationModel;
using Domain.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Restaurant.Customer;

public class CustomerVm
{
    public long Id { get; set; }
    public string CustomerCode { get; set; }
    public string Salutation { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Mobile { get; set; }
    public string Email { get; set; }
    public DateTime Dob { get; set; }
    public string DobStr { get; set; }
    public string Gender { get; set; } // M=Male,F=Female
    public string Address { get; set; }
    public string PhotoUrl { get; set; }
    public string Note { get; set; }

    // --- Fk ---

    public long? GuestId { get; set; }
    public long? EmployeeId { get; set; }
    public string GuestName { get; set; }
    public long? CompanyId { get; set; }
    public IEnumerable<SelectListItem> SalutationLookUp { get; set; }
    public IEnumerable<SelectListItem> GenderLookUp { get; set; }
    public IEnumerable<SelectListItem> GuestLookUp { get; set; }
    public IEnumerable<SelectListItem> EmployeeLookUp { get; set; }
    public IEnumerable<SelectListItem> CompanyLookUp { get; set; }

    #region Photo

    public IFormFile PhotoFile { get; set; }
    public AppFile AppFile { get; set; }
    public string FolderPath => $"Customer";

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
