using AutoMapper;
using Domain.Entities.HR;
using Domain.Utility.Common;
using Domain.Utility;
using Domain.ViewModel.HouseKeeping.TaskAssign;
using Interface.Services.HouseKeeping;
using Interface.Services.Hr;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;

namespace WebMVC.Controllers.HouseKeeping;

public class TaskAssignController : AppBaseController
{
    #region Config
    private readonly IUnitOfWork _iUnitWork;
    private readonly ITaskAssignService _iService;
    private readonly IMapper _iMapper;
    private readonly DropdownService _dropdownService;
    private readonly IEmployeeService _iEmployeeService;

    public TaskAssignController(IUnitOfWork iUnitOfWork,
                            ITaskAssignService iService,
                            IMapper iMapper,
                            DropdownService dropdownService,
                            IEmployeeService iEmployeeService) : base(iUnitOfWork)
    {
        _iUnitWork = iUnitOfWork;
        _iService = iService;
        _iMapper = iMapper;
        _dropdownService = dropdownService;
        _iEmployeeService = iEmployeeService;
    }
    #endregion

    #region AssignTask

    //[Authorize(Permissions.TaskAssigns.Create)]
    [Authorize(Permissions.Module.HouseKeepingModule)]
    public IActionResult AssignTask()
    {
        var model = new TaskAssignSaveVm();
        model.EmployeeLookUp = _dropdownService.GetEmployeeSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AssignTask(TaskAssignSaveVm modelVm)
    {
        try
        {
            _iService.CurrentUserId = UserId;
            var isAdded = await _iService.AddAssignTask(modelVm);
            return Ok(isAdded);
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                message = "Entry Failed..!!",
                errors = e.Message
            });
        }
    }

    #endregion

    #region Search

    [HttpGet]
    //[Authorize(Permissions.TaskAssigns.ListView)]
    [Authorize(Permissions.Module.HouseKeepingModule)]
    public IActionResult Search()
    {
        var vm = new TaskAssignSearchVm();
        return View(vm);
    }


    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<TaskAssignSearchVm, TaskAssignSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<TaskAssignSearchVm, TaskAssignSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new TaskAssignSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion

    #region MyTask 

    [HttpGet]
    //[Authorize(Permissions.TaskAssigns.ListView)]
    [Authorize(Permissions.Module.HouseKeepingModule)]
    public IActionResult MyTask()
    {
        try
        {
            var vm = new TaskAssignSearchVm();
            var employee = GetEmployeeInfo();
            if (employee == null)
                throw new Exception("No House Keeper Found...!");
            vm.HouseKeeperId = employee.Id;

            return View(vm);
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return RedirectToAction("Index", "Home");
        }

    }

    #endregion

    #region JsonData

    [HttpGet]
    public async Task<ActionResult> GetTaskByAssignId(long id)
    {
        try
        {
            var tasks = await _iService.GetTaskByAssignId(id);
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            return Ok(SetError(ex.Message));
        }
    }

    #endregion

    private Employee GetEmployeeInfo()
    {
        var employee = _iEmployeeService.GetFirstOrDefault(c => c.UserId == UserId);
        return employee;
    }


    #region Approve Task
    [HttpGet]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public async Task<IActionResult> ApproveTask(long taskId, string auditorRemarks)
    {
        var data = await _iService.GetByIdAsync(taskId);
        if (data == null)
        {
            DeleteFailedMsg();
            return NotFound();
        }

        data.AuditorRemarks = auditorRemarks;
        data.AuditorId = UserId;
        data.AuditDate = DateTime.Now;

        DeleteSuccessMsg();
        var isRemove = _iService.Update(data);
        return RedirectToAction("Search");
    }
    #endregion
}
