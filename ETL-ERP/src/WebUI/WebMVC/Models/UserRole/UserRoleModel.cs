using Domain.Enums;

namespace WebMVC.Models.UserRole;

public class UserRoleModel
{
    public long Id { get; set; }
    public string Name { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsActive { get; set; }
    public ApplicationRoleStatusEnum? Status { get; set; }
    public IList<string> Permissions { get; set; }

    public UserRoleModel()
    {
        this.Permissions = new List<string>();
    }
}
