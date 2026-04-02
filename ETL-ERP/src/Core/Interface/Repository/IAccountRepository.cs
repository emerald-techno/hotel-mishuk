using Domain.Entities.Identity;
using Interface.Base;

namespace Interface.Repository;

public interface IAccountRepository : IRepository<ApplicationUser>
{
    Task<Tuple<ApplicationUser, string[]>> GetUserAndRolesAsync(long userId);
    Task<List<Tuple<ApplicationUser, string[]>>> GetUsersAndRolesAsync(int page, int pageSize);
    Task<bool> TestCanDeleteRoleAsync(long roleId);
    Task<bool> TestCanDeleteUserAsync(long userId);
    Task<ApplicationRole> GetRoleLoadRelatedAsync(string roleName);
    Task<List<ApplicationRole>> GetRolesLoadRelatedAsync(int page, int pageSize);
}
