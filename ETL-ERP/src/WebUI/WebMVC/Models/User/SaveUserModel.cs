using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WebMVC.Models.User;

public class SaveUserModel
{
    public long Id { get; set; }

    [Display(Name = "Full Name")]
    public string FullName { get; set; }
    
    [Required]
    [Remote("IsExistsUserName", "User", ErrorMessage = "Name Already Exists", AdditionalFields = "InitialUserName")]
    [Display(Name = "User Name")]
    public string UserName { get; set; }
    
    [Required]
    [Remote("IsExistsEmail", "User", ErrorMessage = "Email Already Exists", AdditionalFields = "InitialEmail")]
    public string Email { get; set; }
    public string Mobile { get; set; }

    [StringLength(120)]
    public string PhotoUrl { get; set; }
    
    [Required]
    [Display(Name = "User Role")]
    public long UserRoleId { get; set; }
    public string UserRoleName { get; set; }
    public long? EmployeeId { get; set; }
    public IEnumerable<SelectListItem> RoleLookUp { get; set; }
    public IEnumerable<SelectListItem> EmployeeLookUp { get; set; }
}
