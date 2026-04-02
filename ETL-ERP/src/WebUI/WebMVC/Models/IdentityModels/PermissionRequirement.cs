using Microsoft.AspNetCore.Authorization;

namespace WebMVC.Models.IdentityModels
{
    internal class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; private set; }

        public PermissionRequirement(string permission)
        {
            this.Permission = permission;
        }
    }
}