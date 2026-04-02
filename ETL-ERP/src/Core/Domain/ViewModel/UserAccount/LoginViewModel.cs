using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.UserAccount;

public class LoginViewModel
{
    //[DisplayName("Username")]
    //public string UserName { get; set; }
    
    [Required]
    public string Email { get; set; }


    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [Display(Name = "Remember me ?")]
    public bool RememberMe { get; set; }

    public bool IsFromView { get; set; }
    public string GrantType { get; set; }
    public string Scope { get; set; }

    public string Message { get; set; }

    public IEnumerable<SelectListItem> RoleLookUp { get; set; }

    [DisplayName("Role")]
    public long RoleId { get; set; }
    public string ReturnUrl { get; set; }
    public string CustomReturnUrl { get; set; }
}
