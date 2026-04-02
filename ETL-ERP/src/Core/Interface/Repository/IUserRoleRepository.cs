using Interface.Base;
using Microsoft.AspNetCore.Identity;

namespace Interface.Repository;

public interface IUserRoleRepository : IRepository<IdentityUserRole<long>>
{
}
