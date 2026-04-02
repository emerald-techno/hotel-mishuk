using AutoMapper;
using Domain.Entities.Attendance;
using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.Attendance.DutyShift;
using Domain.ViewModel.HotelManagement.BedType;
using Interface.Services.Attendance;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using DU = Domain.Utility;

namespace WebMVC.Controllers.Attendance;

public class DutyShiftController : AppBaseController
{
    #region Config
    private readonly IUnitOfWork _iUnitWork;
    private readonly IDutyShiftService _iService;
    private readonly DropdownService _dropdownService;
    private readonly IMapper _iMapper;

    public DutyShiftController(IUnitOfWork iUnitOfWork,
                            IDutyShiftService iService,
                            DropdownService dropdownService,
                            IMapper iMapper) : base(iUnitOfWork)
    {
        _iUnitWork = iUnitOfWork;
        _iService = iService;
        _dropdownService = dropdownService;
        _iMapper = iMapper;
    }
    #endregion

    #region Create
    [Authorize(Permissions.DutyShifts.Create)]
    public IActionResult Create()
    {
        var model = new DutyShiftVm();
        model.ShiftTypeLookUp = _dropdownService.ShiftTypeSelectListItems();
        model.PayTypeLookUp = _dropdownService.PayTypeSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(DutyShiftVm modelVm)
    {
        modelVm.ShiftTypeLookUp = _dropdownService.ShiftTypeSelectListItems();
        modelVm.PayTypeLookUp = _dropdownService.PayTypeSelectListItems();
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return View("Create", modelVm);
            }
            _iService.CurrentUserId = UserId;
            var model = _iMapper.Map<DutyShift>(modelVm);
            model.ActionById = UserId;
            model.ActionDate = DU.Utility.GetBdDateTimeNow();

            var isAdded = await _iService.AddAsync(model);
            if (!isAdded)
            {
                SaveFailedMsg();
                return View("Create", modelVm);
            }
            SaveSuccessMsg();

            return RedirectToAction("Create");
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("Create", modelVm);
        }
    }
    #endregion

    #region Edit
    [HttpGet]
    [Authorize(Permissions.DutyShifts.Edit)]
    public IActionResult Edit(long id)
    {
        try
        {
            var data = _iService.GetById(id);
            if (data == null)
            {
                return NotFoundMsg();
            }
            var model = _iMapper.Map<DutyShiftVm>(data);
            model.ShiftTypeLookUp = _dropdownService.ShiftTypeSelectListItems();
            model.PayTypeLookUp = _dropdownService.PayTypeSelectListItems();
            return View(model);
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("_404");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(DutyShiftVm modelVm)
    {
        modelVm.ShiftTypeLookUp = _dropdownService.ShiftTypeSelectListItems();
        modelVm.PayTypeLookUp = _dropdownService.PayTypeSelectListItems();
        try
        {
            if (modelVm.Id <= 0 || !ModelState.IsValid)
                return View(modelVm);

            var model = _iMapper.Map<DutyShift>(modelVm);
            model.ActionById = UserId;
            model.ActionDate = DU.Utility.GetBdDateTimeNow();
            model.UpdatedById = UserId;
            model.UpdateDate = DU.Utility.GetBdDateTimeNow();
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
    [Authorize(Permissions.DutyShifts.Delete)]
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
        var isRemove = await _iService.UpdateAsync(data);
        return RedirectToAction("Search");
    }
    #endregion

    #region Search
    [HttpGet]
    [Authorize(Permissions.DutyShifts.ListView)]
    public IActionResult Search()
    {
        var vm = new DutyShiftSearchVm();
        return View(vm);
    }


    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<DutyShiftSearchVm, DutyShiftSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<DutyShiftSearchVm, DutyShiftSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new DutyShiftSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion
}