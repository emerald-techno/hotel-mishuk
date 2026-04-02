using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace WebMVC.Models.UserRole;

public class SaveUserRoleModel
{
    public long Id { get; set; }
    
    [Required]
    [Remote("IsExistsName", "Role", ErrorMessage = "Role Already Exists", AdditionalFields = "InitialName")]
    public string Name { get; set; }
    public IList<string> Permissions { get; set; }
    public RolePermissionHelper ViewPermission { get; set; }

    public SaveUserRoleModel()
    {
        this.Permissions = new List<string>();
        this.ViewPermission = new RolePermissionHelper();
    }

}
