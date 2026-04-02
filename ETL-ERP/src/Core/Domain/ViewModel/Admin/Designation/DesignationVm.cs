using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Admin.Designation;

public class DesignationVm
{
    public long Id { get; set; }

    [StringLength(80, ErrorMessage = "Name can not be more than 80 characters")]
    [Required(ErrorMessage = "Name is required")]
    [Remote(action: "IsNameExist", controller: "Designation", AdditionalFields = "InitName")]
    public string Name { get; set; }
    
    [StringLength(30, ErrorMessage = "Code can not be more than 30 characters")]
    [Required(ErrorMessage = "Code is required")]
    [Remote(action: "IsCodeExist", controller: "Designation", AdditionalFields = "InitCode")]
    public string Code { get; set; }
    
    [StringLength(350, ErrorMessage = "Code can not be more than 350 characters")]
    public string Remarks { get; set; }
    public bool IsTeacher { get; set; }
    public bool IsHouseKeeper { get; set; }
    public DateTime? ActionDate { get; set; }
    public long? ActionById { get; set; }
}
