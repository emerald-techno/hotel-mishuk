using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace WebMVC.Models.User;

public class UserModel
{
    public long Id { get; set; }

    [Display(Name = "User Name")]
    public string UserName { get; set; }
    public string Email { get; set; }

    [Display(Name = "Mobile")]
    public string Mobile { get; set; }

    [Display(Name = "Full Name")]
    public string FullName { get; set; }
    public string Gender { get; set; }

    [Display(Name = "Image")]
    public string PhotoUrl { get; set; }

    [Display(Name = "User Role")]
    public string UserRoleName { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsActive { get; set; }
    public ApplicationUserStatusEnum? Status { get; set; }
}
