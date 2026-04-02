using Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.Identity
{
    public class ApplicationRole : IdentityRole<long>
    {
        public ApplicationRoleStatusEnum Status { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public IList<ApplicationUserRole> UserRoles { get; set; }

        public ApplicationRole() : base()
        {
            IsActive = true;
            IsDeleted = false;
            UserRoles = new List<ApplicationUserRole>();
        }
        public ApplicationRole(string roleName) : base(roleName)
        {
            IsActive = true;
            IsDeleted = false;
            UserRoles = new List<ApplicationUserRole>();
        }
    }
}
