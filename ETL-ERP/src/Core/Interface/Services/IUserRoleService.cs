using Interface.Base;
using Microsoft.AspNetCore.Identity;

namespace Interface.Services;

public interface IUserRoleService : IService<IdentityUserRole<long>>
{

}
