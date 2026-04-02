using Domain.ConfigurationModel;
using Domain.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Pf.EmpLoan;

public class EmpLoanMstVm
{
    public long Id { get; set; }
    public DateTime LoanPassDate { get; set; }

    [Required]
    public string LoanPassDateStr { get; set; }
    public DateTime LoanPayDate { get; set; }

    [Required]
    public string LoanPayDateStr { get; set; }
    public double LoanAmount { get; set; }
    public double InterestRate { get; set; }
    public double InsAmount { get; set; }
    public DateTime FirstInsDate { get; set; } // First Installment Date
    public string FirstInsDateStr { get; set; }
    public string LoanFileUrl { get; set; }
    public short Status { get; set; } // 0=Running , 1=Complete, 2=Held
    public string StatusText => Status switch { 0 => "Running", 1 => "Completed", 2 => "Held", _ => "N/A" };

    [StringLength(350)]
    public string Remarks { get; set; }
    public long EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public IEnumerable<SelectListItem> EmployeeLookup { get; set; }
    public ICollection<EmpLoanDtlVm> EmpLoanDtls { get; set; }

    #region File Upload

    public IFormFile LoanFile { get; set; }

    public AppFile AppFile { get; set; }

    public string FolderPath => $"\\EmpLoan";

    public AppFile GetAppFileToUploadFolder()
    {
        if (AppFile != null) { return AppFile; }

        var fileUpload = new AppFileUploadHelper
        {
            FormFile = LoanFile?.IsNullOrEmpty() == true ? null : LoanFile,
            FolderPath = FolderPath,
            FileNamePrefix = "CERTIFICATE"
        };
        AppFile = Utility.Utility.ConvertFormFileToAppFile(fileUpload)?.FirstOrDefault();
        if (LoanFile != null) LoanFileUrl = Utility.Utility.GetFileSmallPath(AppFile?.FileUrl);
        return AppFile;
    }

    #endregion

    public string PaidDateStr { get; set; }
}

public class InstalmentPaidVm
{
    public long Id { get; set; }
    public bool IsPaid { get; set; }
    public string PaidDateStr { get; set; }
    public string Remarks { get; set; }
    public double WaiverAmount { get; set; } = 0;
}
