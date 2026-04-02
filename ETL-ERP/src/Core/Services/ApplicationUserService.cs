using AutoMapper;
using Domain.ConfigurationModel;
using Domain.Entities.HR;
using Domain.Entities.Identity;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Result;
using Domain.Utility.Common;
using Domain.Utility.Extensions;
using Domain.ViewModel.QueryModel;
using Domain.ViewModel.User;
using Interface.Services;
using Interface.Services.Hr;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Services;

public class ApplicationUserService : IApplicationUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _iMapper;
    private readonly IEmployeeService _iEmployeeService;

    public ApplicationUserService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ICurrentUserService currentUserService,
        IMapper iMapper,
        IEmployeeService iEmployeeService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _currentUserService = currentUserService;
        _iMapper = iMapper;
        _iEmployeeService = iEmployeeService;
    }

    public async Task<QueryResult<ApplicationUser>> GetAllAsync(UserQuery queryObj)
    {
        var result = new QueryResult<ApplicationUser>();

        var columnsMap = new Dictionary<string, Expression<Func<ApplicationUser, object>>>()
        {
            ["fullName"] = v => v.FullName,
            ["userName"] = v => v.UserName,
            ["email"] = v => v.Email,
            //["UserRoleId"] = v => v.UserRoles.FirstOrDefault().RoleId
        };

        var query = _userManager.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).AsQueryable();

        query = query.Where(x => !x.IsDeleted &&
            x.Status != ApplicationUserStatusEnum.SuperAdmin &&
            (string.IsNullOrWhiteSpace(queryObj.FullName) || x.FullName.Contains(queryObj.FullName)) &&
            (string.IsNullOrWhiteSpace(queryObj.UserName) || x.UserName.Contains(queryObj.UserName)) &&
            (queryObj.UserRoleId == null || x.UserRoles.FirstOrDefault().RoleId.Equals(queryObj.UserRoleId)) &&
            (string.IsNullOrWhiteSpace(queryObj.Email) || x.Email.Contains(queryObj.Email)));

        result.TotalItems = await query.CountAsync();
        query = query.ApplyOrdering(queryObj, columnsMap);
        query = query.ApplyPaging(queryObj);
        result.Items = (await query.AsNoTracking().ToListAsync());

        return result;
    }

    public async Task<DataTablePagination<UserSearchVm, UserSearchVm>>
        SearchAsync(DataTablePagination<UserSearchVm, UserSearchVm> vm)
    {
        var searchResult = _userManager.Users
            .Include(s => s.UserRoles)
                .ThenInclude(c => c.Role)
            .AsQueryable()
            .Where(c => c.Status == ApplicationUserStatusEnum.GeneralUser && !c.IsDeleted);

        var model = vm.SearchModel;

        if (model == null) throw new Exception("Search User not found");

        if (!string.IsNullOrEmpty(vm.Search.Value))
        {
            var value = vm.Search.Value.Trim().ToLower();
            searchResult = searchResult.Where(c => c.FullName.ToLower().Contains(value) || c.Mobile.ToLower().Contains(value));
        }

        var totalRecords = await searchResult.CountAsync();

        if (totalRecords > 0)
        {
            vm.recordsTotal = totalRecords;
            vm.recordsFiltered = totalRecords;
            vm.draw = vm.LineDraw ?? 0;

            var data = await searchResult.OrderBy(c => c.ActionDate)
                                         .Skip(vm.Start)
                                         .Take(vm.Length)
                                         .ToListAsync();

            vm.data = _iMapper.Map<List<UserSearchVm>>(data);

            var sl = vm.Start;

            foreach (var searchDto in vm.data)
            {
                var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                searchDto.SerialNo = ++sl;
                searchDto.UserRoleName = filterData?.UserRoles.FirstOrDefault()?.Role.Name;
            }
        }

        return vm;
    }

    public async Task<ApplicationUser> GetByIdAsync(long id)
    {
        var query = _userManager.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).AsQueryable();

        var user = await query.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null)
        {
            throw new NotFoundException(nameof(ApplicationUser), id);
        }

        return user;
    }

    public async Task<ApplicationUser> GetByUserNameAsync(string userName)
    {
        var query = _userManager.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).AsQueryable();

        var user = await query.FirstOrDefaultAsync(u => u.UserName == userName);

        if (user == null)
        {
            throw new NotFoundException(nameof(ApplicationUser), userName);
        }

        return user;
    }

    #region Add

    public async Task<(Result Result, long Id)> AddAsync(ApplicationUser command, long userRoleId, string newPassword, long? employeeId)
    {
        var user = new ApplicationUser
        {
            UserName = command.UserName,
            Email = command.Email,
            PhoneNumber = command.PhoneNumber,

            FullName = command.FullName,
            Gender = command.Gender,
            Mobile = command.Mobile,
            PhotoUrl = command.PhotoUrl ?? string.Empty,

            Status = ApplicationUserStatusEnum.GeneralUser,
            ActionDate = DateTime.Now,
            ActionById = _currentUserService.UserId > 0 ? _currentUserService.UserId : null
        };

        var userSaveResult = await _userManager.CreateAsync(user, newPassword);

        if (!userSaveResult.Succeeded)
        {
            throw new IdentityValidationException(userSaveResult.Errors);
        };

        // Add New User Role
        user = await _userManager.FindByNameAsync(user.UserName);
        var role = await _roleManager.FindByIdAsync(userRoleId.ToString());

        if (role == null)
        {
            throw new NotFoundException(nameof(ApplicationRole), userRoleId);
        }

        var roleSaveResult = await _userManager.AddToRoleAsync(user, role.Name);

        if (!roleSaveResult.Succeeded)
        {
            throw new IdentityValidationException(roleSaveResult.Errors);
        };

        if (roleSaveResult.Succeeded && employeeId != null && employeeId > 0)
        {
            var employee = _iEmployeeService.GetById(employeeId ?? 0);
            if (employee == null)
            {
                throw new NotFoundException(nameof(Employee), employeeId);
            }
            employee.UserId = user.Id;
            var employeeResult = await _iEmployeeService.UpdateAsync(employee);
            if (!employeeResult) throw new Exception("Employee User Sync Failed...!");
        }

        return (userSaveResult.ToApplicationResult(), user.Id);
    }

    #endregion

    public async Task<(Result Result, long Id)> UpdateAsync(ApplicationUser command, long userRoleId)
    {
        var user = await this._userManager.FindByIdAsync(command.Id.ToString());

        if (user == null)
        {
            throw new NotFoundException(nameof(ApplicationUser), command.Id);
        }

        user.UserName = command.UserName;
        user.Email = command.Email;
        user.PhoneNumber = command.PhoneNumber;

        user.FullName = command.FullName;
        user.Mobile = command.Mobile;
        user.Gender = command.Gender;
        user.PhotoUrl = command.PhotoUrl ?? user.PhotoUrl;

        user.UpdateDate = DateTime.Now;
        user.UpdatedById = _currentUserService.UserId;

        var userSaveResult = await _userManager.UpdateAsync(user);

        if (!userSaveResult.Succeeded)
        {
            throw new IdentityValidationException(userSaveResult.Errors);
        };

        // Remove Previous User Role
        var previousUserRoles = await _userManager.GetRolesAsync(user);
        if (previousUserRoles.Any())
        {
            var roleRemoveResult = await _userManager.RemoveFromRolesAsync(user, previousUserRoles);

            if (!roleRemoveResult.Succeeded)
            {
                throw new IdentityValidationException(roleRemoveResult.Errors);
            };

        }

        // Add New User Role
        var role = await _roleManager.FindByIdAsync(userRoleId.ToString());

        if (user == null)
        {
            throw new NotFoundException(nameof(ApplicationRole), userRoleId);
        }

        var roleSaveResult = await _userManager.AddToRoleAsync(user, role.Name);

        if (!roleSaveResult.Succeeded)
        {
            throw new IdentityValidationException(roleSaveResult.Errors);
        };


        return (userSaveResult.ToApplicationResult(), user.Id);
    }

    public async Task<Result> DeleteAsync(long id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user != null)
        {
            user.IsDeleted = true;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new IdentityValidationException(result.Errors);
            };

            return result.ToApplicationResult();
        }

        return Result.Success();
    }

    public async Task<Result> ActiveInactiveAsync(long id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());

        if (user != null)
        {
            user.IsActive = !user.IsActive;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new IdentityValidationException(result.Errors);
            };

            return result.ToApplicationResult();
        }

        return Result.Success();
    }

    public async Task<bool> IsExistsUserNameAsync(string name, string initialName)
    {
        var result = await _userManager.Users.AnyAsync(x => x.UserName.ToLower() == name.ToLower());
        return result ? (!string.IsNullOrEmpty(initialName) && name.ToLower() == initialName.ToLower() ? false : true) : false;
    }

    public async Task<bool> IsExistsEmailAsync(string email, string initialEmail)
    {
        var result = await _userManager.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());
        return result ? (!string.IsNullOrEmpty(initialEmail) && email.ToLower() == initialEmail.ToLower() ? false : true) : false;
    }
    public async Task<long> GetUsersCountAsync()
    {
        return await _userManager.Users.CountAsync(x => x.Status != ApplicationUserStatusEnum.SuperAdmin);
    }

    public async Task ChanagePasswordAsync(long id, string currentPassword, string newPassword)
    {

        var user = await GetByIdAsync(id);
        var checkPassword = await _userManager.CheckPasswordAsync(user, currentPassword);
        if (!checkPassword)
        {
            throw new Exception("Current Password Not Valid");
        }
        await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

    }
    public async Task<ApplicationUser> FindByNameAsync(string userName)
    {
        return await _userManager.FindByNameAsync(userName);
    }

    public async Task<bool> ResetPassword(long id, string encryptPassword)
    {

        var user = await this._userManager.FindByIdAsync(id.ToString());

        await _userManager.RemovePasswordAsync(user);
        var result = await _userManager.AddPasswordAsync(user, encryptPassword);

        if (result.Succeeded) return true;
        else return false;

        //var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        //IdentityResult passwordChangeResult = await _userManager.ResetPasswordAsync(user, System.Web.HttpUtility.UrlDecode(resetToken), encryptPassword);
        //if (passwordChangeResult.Succeeded)
        //{
        //    return true;
        //}
        //else return false;
    }
}
