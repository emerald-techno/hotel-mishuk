using Domain.ConfigurationModel;
using Domain.Entities.Identity;
using Domain.Result;
using Domain.Utility.Common;
using Domain.ViewModel.QueryModel;
using Domain.ViewModel.Role;

namespace Interface.Services;

public interface IApplicationRoleService
{
    Task<QueryResult<ApplicationRole>> GetAllAsync(UserRoleQuery queryObj);
    Task<(ApplicationRole ApplicationRole, IList<string> Permissions)> GetByIdAsync(long id);
    Task<(ApplicationRole ApplicationRole, IList<string> Permissions)> GetByNameAsync(string name);
    Task<(Result Result, long Id)> AddAsync(ApplicationRole command, IList<string> permissions);
    Task<(Result Result, long Id)> UpdateAsync(ApplicationRole command, IList<string> permissions);
    Task<Result> DeleteAsync(long id);
    Task<Result> ActiveInactiveAsync(long id);
    Task<bool> IsExistsNameAsync(string name, string initialName);
    
    Task<DataTablePagination<RoleSearchVm, RoleSearchVm>>
        SearchAsync(DataTablePagination<RoleSearchVm, RoleSearchVm> vm);
}
