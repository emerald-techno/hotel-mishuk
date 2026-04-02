using AutoMapper;
using Domain.Utility.Common;
using Domain.ViewModel.Attendance.ShiftManagement;
using Interface.Services.Attendance;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;

namespace WebMVC.Controllers.Attendance;

public class ShiftManagementController : AppBaseController
{
    #region Config
    private readonly IUnitOfWork _iUnitWork;
    private readonly IShiftManagementService _iService;
    private readonly DropdownService _dropdownService;
    private readonly IMapper _iMapper;

    public ShiftManagementController(IUnitOfWork iUnitOfWork,
                            IShiftManagementService iService,
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
    [Authorize(Permissions.ShiftManagements.Create)]
    public IActionResult Create()
    {
        var model = new ShiftManagementVm();
        model.PermanentShiftLookUp = _dropdownService.GetPermanentShiftSelectListItems();
        model.DutyShiftLookUp = _dropdownService.GetDutyShiftSelectListItems();
        model.EmployeeLookUp = _dropdownService.GetEmployeeSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ShiftManagementVm modelVm)
    {
        modelVm.PermanentShiftLookUp = _dropdownService.GetPermanentShiftSelectListItems();
        modelVm.DutyShiftLookUp = _dropdownService.GetDutyShiftSelectListItems();
        modelVm.EmployeeLookUp = _dropdownService.GetEmployeeSelectListItems();

        try
        {
            //if (!ModelState.IsValid)
            //{
            //    SaveFailedMsg("Information is not correct");
            //    return View("Create", modelVm);
            //}
            _iService.CurrentUserId = UserId;
            var isAdded = await _iService.AddOrUpdate(modelVm);
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

    #region EmployeeShift
    [Authorize(Permissions.ShiftManagements.Create)]
    public IActionResult EmployeeShift()
    {
        var model = new ShiftManagementVm();
        model.PermanentShiftLookUp = _dropdownService.GetPermanentShiftSelectListItems();
        model.DutyShiftLookUp = _dropdownService.GetDutyShiftSelectListItems();
        model.EmployeeLookUp = _dropdownService.GetEmployeeSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EmployeeShift(ShiftManagementVm modelVm)
    {
        modelVm.PermanentShiftLookUp = _dropdownService.GetPermanentShiftSelectListItems();
        modelVm.DutyShiftLookUp = _dropdownService.GetDutyShiftSelectListItems();
        modelVm.EmployeeLookUp = _dropdownService.GetEmployeeSelectListItems();

        try
        {
            _iService.CurrentUserId = UserId;
            var isAdded = await _iService.AddOrUpdate(modelVm);
            if (!isAdded)
            {
                SaveFailedMsg();
                return View("EmployeeShift", modelVm);
            }

            SaveSuccessMsg();
            return RedirectToAction("EmployeeShift");
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("EmployeeShift", modelVm);
        }
    }
    #endregion

    #region Search

    [HttpGet]
    [Authorize(Permissions.ShiftManagements.ListView)]
    public IActionResult Search()
    {
        var vm = new ShiftManagementSearchVm();
        vm.PermanentShiftLookUp = _dropdownService.GetPermanentShiftSelectListItems();
        vm.DutyShiftLookUp = _dropdownService.GetDutyShiftSelectListItems();
        vm.EmployeeLookUp = _dropdownService.GetEmployeeSelectListItems();
        return View(vm);
    }

    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<ShiftManagementSearchVm, ShiftManagementSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<ShiftManagementSearchVm, ShiftManagementSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new ShiftManagementSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion

    #region JsonData

    public async Task<IActionResult> GetExistShift(short year, short month, long? employeeId)
    {
        if (year > 1999 && month > 0)
        {
            var existData = await _iService.GetShiftByMonthOfYearAsync(year, month, employeeId);
            if (existData != null)
                return Ok(existData);
            return Ok(null);
        }
        return BadRequest("Month And Year Information Is Not Correct...!");
    }

    #endregion
}