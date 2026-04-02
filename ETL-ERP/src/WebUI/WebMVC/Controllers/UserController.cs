using AutoMapper;
using Domain.Entities.Identity;
using Domain.Utility.Common;
using Domain.ViewModel.User;
using Domain.ViewModel.UserAccount;
using Interface.Services;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using WebMVC.Models.User;

namespace WebMVC.Controllers;

[Authorize]
public class UserController : AppBaseController
{
    private readonly ILogger<UserController> _logger;
    private readonly IUnitOfWork _iUnitWork;
    private readonly DropdownService _dropdownService;
    private readonly IMapper _iMapper;
    private readonly IApplicationUserService _userService;
    private readonly IApplicationRoleService _roleService;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public UserController(ILogger<UserController> logger, IUnitOfWork iUnitOfWork, DropdownService dropdownService, IMapper iMapper, IApplicationUserService userService, IApplicationRoleService roleService, SignInManager<ApplicationUser> signInManager) : base(iUnitOfWork)
    {
        _logger = logger;
        _iUnitWork = iUnitOfWork;
        _dropdownService = dropdownService;
        _iMapper = iMapper;
        _userService = userService;
        _roleService = roleService;
        _signInManager = signInManager;
    }

    [HttpGet]
    [Authorize(Permissions.Users.ListView)]
    public async Task<IActionResult> Search()
    {
        var vm = new UserSearchVm();
        return View(vm);
    }

    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult> Search(DataTablePagination<UserSearchVm, UserSearchVm> searchVm = null)
    {
        try
        {
            if (searchVm == null) searchVm = new DataTablePagination<UserSearchVm, UserSearchVm>();
            if (searchVm?.SearchModel == null) searchVm.SearchModel = new UserSearchVm();
            var dataTable = await _userService.SearchAsync(searchVm);
            return dataTable == null ? NotFound() : Ok(dataTable);
        }
        catch (Exception ex)
        {
            throw new Exception("Problem in operation", ex);
        }

    }

    [HttpGet]
    [Authorize(Permissions.Users.Create)]
    public IActionResult Add()
    {
        var model = new SaveUserModel();
        model.RoleLookUp = _dropdownService.GetRoleSelectListItems();
        model.EmployeeLookUp = _dropdownService.GetEmployeeSelectListItems();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(SaveUserModel model)
    {
        model.RoleLookUp = _dropdownService.GetRoleSelectListItems();

        try
        {
            if (ModelState.IsValid)
            {
                var user = _iMapper.Map<ApplicationUser>(model);

                var result = await _userService.AddAsync(user, model.UserRoleId, UserPassword.Password, model.EmployeeId);

                if (result.Result.Succeeded)
                {
                    return RedirectToAction("Search");
                }
            }

            return View(model);

        }
        catch (Exception ex)
        {
            throw new Exception("Problem in operation", ex);
            //return View(model);
        }

    }

    public async Task<IActionResult> IsExistsUserName(string UserName, string InitialUserName)
    {
        var isExists = await _userService.IsExistsUserNameAsync(UserName, InitialUserName);
        return Json(!isExists);
    }

    public async Task<IActionResult> IsExistsEmail(string Email, string InitialEmail)
    {
        var isExists = await _userService.IsExistsEmailAsync(Email, InitialEmail);
        return Json(!isExists);
    }

    #region ChangePassword

    public IActionResult ChangePassword(long id)
    {
        try
        {
            var model = new ChangePasswordModel();
            model.Id = id;
            return View(model);
        }
        catch (Exception ex)
        {
            throw;
        }

    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                if (model.NewPassword == model.CurrentPassword)
                {
                    throw new Exception("Current Password & New Password can not same");
                }
                else
                {
                    await _userService.ChanagePasswordAsync(model.Id, model.CurrentPassword, model.NewPassword);
                    await _signInManager.SignOutAsync();
                    return RedirectToAction("Login", "Account");
                }
            }
            return View(model);

        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View(model);
        }

    }

    #endregion
}
