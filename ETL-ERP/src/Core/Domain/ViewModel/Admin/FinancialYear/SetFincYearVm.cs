using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Admin.FinancialYear;

public class SetFincYearVm
{
    public long Id { get; set; }

    [Required]
    [StringLength(30)]
    [Remote(action: "IsYearExist", controller: "SetFincYear", AdditionalFields = "InitYear")]
    public string YearName { get; set; }
    public DateTime YearStartDate { get; set; }
    public string YearStartDateStr { get; set; }
    public DateTime YearEndDate { get; set; }
    public string YearEndDateStr { get; set; }
    public bool IsActive { get; set; }
}
