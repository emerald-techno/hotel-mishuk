using AutoMapper;
using Domain.Entities.Identity;
using Domain.Utility.Common;
using Domain.ViewModel.Role;
using Interface.Services;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using WebMVC.Models.UserRole;

namespace WebMVC.Controllers;

[Authorize]
public class RoleController : AppBaseController
{
    private readonly ILogger<RoleController> _logger;
    private readonly IUnitOfWork _iUnitWork;
    private readonly DropdownService _dropdownService;
    private readonly IMapper _iMapper;
    private readonly IApplicationRoleService _roleService;

    public RoleController(ILogger<RoleController> logger, IUnitOfWork iUnitOfWork, DropdownService dropdownService, IMapper iMapper, IApplicationRoleService roleService) : base(iUnitOfWork)
    {
        _logger = logger;
        _iUnitWork = iUnitOfWork;
        _dropdownService = dropdownService;
        _iMapper = iMapper;
        _roleService = roleService;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Authorize(Permissions.UserRoles.CreateOrEdit)]
    public async Task<IActionResult> AddOrEdit(long? id)
    {
        var model = new SaveUserRoleModel();
        model.ViewPermission.LoadItems();
        model.ViewPermission.ResetRolePermissions();

        #region for edit
        if (id != null && id > 0)
        {
            var role = await _roleService.GetByIdAsync(id ?? 0);
            _iMapper.Map(role.ApplicationRole, model);
            model.ViewPermission.LoadRolePermissions(role.Permissions);
        }
        #endregion

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> AddOrEdit(SaveUserRoleModel model)
    {
        ModelState.Remove("Id");

        if (ModelState.IsValid)
        {
            var role = _iMapper.Map<ApplicationRole>(model);

            var result = model.Id > 0 ? await _roleService.UpdateAsync(role, model.ViewPermission.PreparePermissions()) : await _roleService.AddAsync(role, model.ViewPermission.PreparePermissions());

            if (result.Result.Succeeded)
            {
                TempData["SuccessNotify"] = "Role has been successfully saved";
                return RedirectToAction("Search");
            }
        }

        //TempData["ErrorNotify"] = "Role could not be saved";
        return View(model);
    }

    [HttpGet]
    [Authorize(Permissions.UserRoles.ListView)]
    public IActionResult Search()
    {
        var vm = new RoleSearchVm();
        return View(vm);
    }

    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult> Search(DataTablePagination<RoleSearchVm, RoleSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<RoleSearchVm, RoleSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new RoleSearchVm();
        var dataTable = await _roleService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    public async Task<IActionResult> IsExistsName(string Name, string InitialName)
    {
        var isExists = await _roleService.IsExistsNameAsync(Name, InitialName);
        return Json(!isExists);
    }
}
