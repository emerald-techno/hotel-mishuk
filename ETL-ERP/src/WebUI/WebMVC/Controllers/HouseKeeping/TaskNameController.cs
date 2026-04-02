using AutoMapper;
using Domain.Entities.HouseKeeping;
using Domain.Utility.Common;
using Domain.ViewModel.HouseKeeping.TaskName;
using Interface.Services.HouseKeeping;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;

namespace WebMVC.Controllers.HouseKeeping;

public class TaskNameController : AppBaseController
{
    #region Config
    private readonly IUnitOfWork _iUnitWork;
    private readonly ITaskNameService _iService;
    private readonly IMapper _iMapper;
    private readonly DropdownService _dropdownService;

    public TaskNameController(IUnitOfWork iUnitOfWork,
                            ITaskNameService iService,
                            IMapper iMapper,
                            DropdownService dropdownService) : base(iUnitOfWork)
    {
        _iUnitWork = iUnitOfWork;
        _iService = iService;
        _iMapper = iMapper;
        _dropdownService = dropdownService;
    }
    #endregion

    #region Create
    //[Authorize(Permissions.TaskNames.Create)]
    [Authorize(Permissions.Module.HouseKeepingModule)]
    public IActionResult Create()
    {
        var model = new HkTaskNameVm();
        model.TaskTypeLookUp = _dropdownService.GetTaskTypeSelectListItems();
        return View(model);
    }

    [HttpPost]
    public Task<IActionResult> Create(HkTaskNameVm modelVm)
    {
        modelVm.TaskTypeLookUp = _dropdownService.GetTaskTypeSelectListItems();
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return Task.FromResult<IActionResult>(View("Create", modelVm));
            }
            _iService.CurrentUserId = UserId;
            var model = _iMapper.Map<HkTaskName>(modelVm);
            model.ActionById = UserId;
            model.ActionDate = Domain.Utility.Utility.GetBdDateTimeNow();

            var isAdded = _iService.Add(model);
            if (!isAdded)
            {
                SaveFailedMsg();
                return Task.FromResult<IActionResult>(View("Create", modelVm));
            }
            SaveSuccessMsg();

            return Task.FromResult<IActionResult>(RedirectToAction("Search"));
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return Task.FromResult<IActionResult>(View("Create", modelVm));
        }
    }
    #endregion

    #region Edit
    [HttpGet]
    //[Authorize(Permissions.TaskNames.Edit)]
    [Authorize(Permissions.Module.HouseKeepingModule)]
    public IActionResult Edit(long id)
    {
        try
        {
            var data = _iService.GetById(id);
            if (data == null)
            {
                return NotFoundMsg();
            }
            var model = _iMapper.Map<HkTaskNameVm>(data);
            model.TaskTypeLookUp = _dropdownService.GetTaskTypeSelectListItems();
            return View(model);
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("_404");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(HkTaskNameVm modelVm)
    {
        modelVm.TaskTypeLookUp = _dropdownService.GetTaskTypeSelectListItems();

        try
        {
            if (modelVm.Id <= 0 || !ModelState.IsValid)
                return View(modelVm);

            var model = _iMapper.Map<HkTaskName>(modelVm);
            model.ActionById = UserId;
            model.ActionDate = Domain.Utility.Utility.GetBdDateTimeNow();
            model.UpdatedById = UserId;
            model.UpdateDate = Domain.Utility.Utility.GetBdDateTimeNow();
            var isAdded = await _iService.UpdateAsync(model);
            if (!isAdded)
            {
                UpdateFailedMsg();
                return View("Edit", modelVm);
            }
            UpdateSuccessMsg();

            return RedirectToAction("Search");
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View(modelVm);
        }
    }
    #endregion

    #region Delete
    [HttpGet]
    //[Authorize(Permissions.TaskNames.Delete)]
    [Authorize(Permissions.Module.HouseKeepingModule)]
    public async Task<IActionResult> Delete(long id)
    {
        var data = await _iService.GetByIdAsync(id);
        if (data == null)
        {
            DeleteFailedMsg();
            return NotFound();
        }
        data.IsDeleted = true;
        DeleteSuccessMsg();
        var isRemove = _iService.Update(data);
        return RedirectToAction("Search");
    }
    #endregion

    #region Search
    [HttpGet]
    //[Authorize(Permissions.TaskNames.ListView)]
    [Authorize(Permissions.Module.HouseKeepingModule)]
    public IActionResult Search()
    {
        var vm = new HkTaskNameSearchVm();
        return View(vm);
    }

    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<HkTaskNameSearchVm, HkTaskNameSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<HkTaskNameSearchVm, HkTaskNameSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new HkTaskNameSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion

}