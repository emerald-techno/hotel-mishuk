using AutoMapper;
using Domain.Entities.Notification;
using Domain.Utility.Common;
using Domain.ViewModel.Notification;
using Interface.Services.Notification;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;

namespace WebMVC.Controllers.Notification;

public class NtfUserController : AppBaseController
{
    #region Config
    private readonly IUnitOfWork _iUnitWork;
    private readonly INtfUserSettingsService _iService;
    private readonly IMapper _iMapper;
    private readonly DropdownService _dropdownService;

    public NtfUserController(IUnitOfWork iUnitOfWork,
                            INtfUserSettingsService iService,
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

    [HttpGet]
    public IActionResult Create()
    {
        var model = new NtfUserSettingVm();
        model.UserLookUp = _dropdownService.GetUserSelectListItems();
        model.EventLookUp = _dropdownService.GetNtfEventSelectListItems();
        return View(model);
    }

    [HttpPost]
    public IActionResult Create(NtfUserSettingVm modelVm)
    {
        modelVm.UserLookUp = _dropdownService.GetUserSelectListItems();
        modelVm.EventLookUp = _dropdownService.GetNtfEventSelectListItems();

        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information Is Not Correct");
                return View("Create", modelVm);
            }
            _iService.CurrentUserId = UserId;
            var model = _iMapper.Map<NtfUserSettings>(modelVm);
            model.SettingDate = Domain.Utility.Utility.GetBdDateTimeNow();

            var isAdded = _iService.Add(model);
            if (!isAdded)
            {
                SaveFailedMsg();
                return View("Create", modelVm);
            }

            SaveSuccessMsg();
            return View();
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("Create", modelVm);
        }
    }
    #endregion

    #region Search
    [HttpGet]
    public IActionResult Search()
    {
        var vm = new NtfUserSettingSearchVm();
        return View(vm);
    }


    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<NtfUserSettingSearchVm, NtfUserSettingSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<NtfUserSettingSearchVm, NtfUserSettingSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new NtfUserSettingSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion

    #region Edit
    [HttpGet]
    public IActionResult Edit(long id)
    {
        try
        {
            var data = _iService.GetById(id);
            if (data == null)
            {
                return NotFoundMsg();
            }
            var model = _iMapper.Map<NtfUserSettingVm>(data);
            return View(model);
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("_404");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(NtfUserSettingVm modelVm)
    {
        try
        {
            if (modelVm.Id <= 0 || !ModelState.IsValid) return View(modelVm);
            var model = _iMapper.Map<NtfUserSettings>(modelVm);
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
    public async Task<IActionResult> Delete(long id)
    {
        var data = await _iService.GetByIdAsync(id);
        if (data == null)
        {
            DeleteFailedMsg();
            return NotFound();
        }
        _iService.Remove(data);
        var isRemove = _iUnitWork.Complete();

        if (!isRemove)
        {
            DeleteFailedMsg();
        }
        DeleteSuccessMsg("User Notification Removed");
        return RedirectToAction("Search");
    }
    #endregion
}
