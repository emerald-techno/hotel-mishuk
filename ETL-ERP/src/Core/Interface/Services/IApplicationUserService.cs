using Domain.ConfigurationModel;
using Domain.Entities.Identity;
using Domain.Result;
using Domain.Utility.Common;
using Domain.ViewModel.QueryModel;
using Domain.ViewModel.User;

namespace Interface.Services;

public interface IApplicationUserService
{
    Task<QueryResult<ApplicationUser>> GetAllAsync(UserQuery queryObj);
    Task<ApplicationUser> GetByIdAsync(long id);
    Task<ApplicationUser> GetByUserNameAsync(string userName);
    Task<(Result Result, long Id)> AddAsync(ApplicationUser command, long userRoleId, string newPassword, long? employeeId);
    Task<(Result Result, long Id)> UpdateAsync(ApplicationUser command, long userRoleId);
    Task<Result> DeleteAsync(long id);
    Task<Result> ActiveInactiveAsync(long id);
    Task<bool> IsExistsUserNameAsync(string name, string initialName);
    Task<bool> IsExistsEmailAsync(string email, string initialEmail);
    Task<long> GetUsersCountAsync();
    Task ChanagePasswordAsync(long id, string currentPassword, string newPassword);
    Task<ApplicationUser> FindByNameAsync(string userName);
    Task<bool> ResetPassword(long id, string encryptPassword);
    Task<DataTablePagination<UserSearchVm, UserSearchVm>>
        SearchAsync(DataTablePagination<UserSearchVm, UserSearchVm> model);

}
