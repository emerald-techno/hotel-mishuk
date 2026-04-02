using AutoMapper;
using Domain.ConfigurationModel;
using Domain.Entities.Identity;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Result;
using Domain.Utility.Common;
using Domain.Utility.Extensions;
using Domain.ViewModel.QueryModel;
using Domain.ViewModel.Role;
using Interface.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Services;

public class ApplicationRoleService : IApplicationRoleService
{
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _iMapper;

    public ApplicationRoleService(
        RoleManager<ApplicationRole> roleManager,
        ICurrentUserService currentUserService,
        IMapper iMapper)
    {
        _roleManager = roleManager;
        _currentUserService = currentUserService;
        _iMapper = iMapper;
    }

    public async Task<QueryResult<ApplicationRole>> GetAllAsync(UserRoleQuery queryObj)
    {
        var result = new QueryResult<ApplicationRole>();

        var columnsMap = new Dictionary<string, Expression<Func<ApplicationRole, object>>>()
        {
            ["name"] = v => v.Name,
        };

        var query = _roleManager.Roles.AsQueryable();

        query = query.Where(x => !x.IsDeleted &&
            x.Status != ApplicationRoleStatusEnum.SuperAdmin &&
            (string.IsNullOrWhiteSpace(queryObj.Name) || x.Name.Contains(queryObj.Name)));

        result.TotalItems = await query.CountAsync();
        query = query.ApplyOrdering(queryObj, columnsMap);
        query = query.ApplyPaging(queryObj);
        result.Items = (await query.AsNoTracking().ToListAsync());

        return result;
    }

    public async Task<DataTablePagination<RoleSearchVm, RoleSearchVm>>
        SearchAsync(DataTablePagination<RoleSearchVm, RoleSearchVm> vm)
    {
        var searchResult = _roleManager.Roles.AsQueryable().Where(c => c.Status == ApplicationRoleStatusEnum.GeneralUser && !c.IsDeleted);

        var model = vm.SearchModel;

        if (model == null) throw new Exception("Search Role not found");

        if (!string.IsNullOrEmpty(vm.Search.Value))
        {
            var value = vm.Search.Value.Trim().ToLower();
            searchResult = searchResult.Where(c => c.Name.ToLower().Contains(value));
        }

        var totalRecords = await searchResult.CountAsync();

        if (totalRecords > 0)
        {
            vm.recordsTotal = totalRecords;
            vm.recordsFiltered = totalRecords;
            vm.draw = vm.LineDraw ?? 0;

            var data = await searchResult.OrderBy(c => c.Name)
                                         .Skip(vm.Start)
                                         .Take(vm.Length)
                                         .ToListAsync();

            vm.data = _iMapper.Map<List<RoleSearchVm>>(data);

            var sl = vm.Start;

            foreach (var searchDto in vm.data)
            {
                var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                searchDto.SerialNo = ++sl;
            }
        }

        return vm;
    }

    public async Task<(ApplicationRole ApplicationRole, IList<string> Permissions)> GetByIdAsync(long id)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());

        if (role == null)
        {
            throw new NotFoundException(nameof(ApplicationRole), id);
        }

        var permissions = (await _roleManager.GetClaimsAsync(role)).Select(c => c.Value).ToList();

        return (role, permissions);
    }

    public async Task<(ApplicationRole ApplicationRole, IList<string> Permissions)> GetByNameAsync(string name)
    {
        var role = await _roleManager.FindByNameAsync(name);

        if (role == null)
        {
            throw new NotFoundException(nameof(ApplicationRole), name);
        }

        var permissions = (await _roleManager.GetClaimsAsync(role)).Select(c => c.Value).ToList();

        return (role, permissions);
    }

    public async Task<(Result Result, long Id)> AddAsync(ApplicationRole command, IList<string> permissions)
    {
        var role = new ApplicationRole()
        {
            Name = command.Name,
            Status = ApplicationRoleStatusEnum.GeneralUser
        };

        var roleSaveResult = await _roleManager.CreateAsync(role);

        if (!roleSaveResult.Succeeded)
        {
            throw new IdentityValidationException(roleSaveResult.Errors);
        };

        role = await _roleManager.FindByNameAsync(command.Name);

        foreach (var per in permissions)
        {
            var claimSaveResult = await _roleManager.AddClaimAsync(role, new Claim(CustomClaimType.Permission, per));

            if (!claimSaveResult.Succeeded)
            {
                throw new IdentityValidationException(claimSaveResult.Errors);
            };

        }

        return (roleSaveResult.ToApplicationResult(), role.Id);
    }

    public async Task<(Result Result, long Id)> UpdateAsync(ApplicationRole command, IList<string> permissions)
    {
        var role = await _roleManager.FindByIdAsync(command.Id.ToString());

        if (role == null)
        {
            throw new NotFoundException(nameof(ApplicationRole), command.Id);
        }

        role.Name = command.Name;

        var roleSaveResult = await this._roleManager.UpdateAsync(role);

        if (!roleSaveResult.Succeeded)
        {
            throw new IdentityValidationException(roleSaveResult.Errors);
        };

        // Remove Previous Permission
        var removedPermissions = await _roleManager.GetClaimsAsync(role);
        foreach (var per in removedPermissions)
        {
            var claimRemoveResult = await _roleManager.RemoveClaimAsync(role, per);

            if (!claimRemoveResult.Succeeded)
            {
                throw new IdentityValidationException(claimRemoveResult.Errors);
            };

        }

        // Add New Permission
        foreach (var per in permissions)
        {
            var claimSaveResult = await _roleManager.AddClaimAsync(role, new Claim(CustomClaimType.Permission, per));

            if (!claimSaveResult.Succeeded)
            {
                throw new IdentityValidationException(claimSaveResult.Errors);
            };

        }

        return (roleSaveResult.ToApplicationResult(), role.Id);
    }

    public async Task<Result> DeleteAsync(long id)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());

        if (role != null)
        {
            role.IsDeleted = true;
            var result = await _roleManager.UpdateAsync(role);
            return result.ToApplicationResult();
        }

        return Result.Success();
    }

    public async Task<Result> ActiveInactiveAsync(long id)
    {
        var role = await _roleManager.FindByIdAsync(id.ToString());

        if (role != null)
        {
            role.IsActive = !role.IsActive;
            var result = await _roleManager.UpdateAsync(role);
            return result.ToApplicationResult();
        }

        return Result.Success();
    }

    public async Task<bool> IsExistsNameAsync(string name, string initialName)
    {
        var result = await _roleManager.Roles.AnyAsync(x => x.Name.ToLower() == name.ToLower());
        return result ? (!string.IsNullOrEmpty(initialName) && name.ToLower() == initialName.ToLower() ? false : true) : false;
    }
}
