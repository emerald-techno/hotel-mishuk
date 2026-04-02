using AutoMapper;
using Domain.Entities.Leave;
using Domain.Utility.Common;
using Domain.ViewModel.Leave.LeaveCf;
using Interface.Services.Hr;
using Interface.Services.Leave;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;

namespace WebMVC.Controllers.Leave
{
    public class LeaveCfController : AppBaseController
    {
        #region Config
        private readonly IUnitOfWork _iUnitOfWork;
        private readonly IMapper _iMapper;
        private readonly ILeaveCfService _iLeaveCfService;
        private readonly DropdownService _dropdownService;
        private readonly IEmployeeService _employeeService;
        public LeaveCfController(IUnitOfWork iUnitOfWork, IMapper iMapper, ILeaveCfService iLeaveCfService,
            DropdownService dropdownService, IEmployeeService employeeService) : base(iUnitOfWork)
        {
            _iUnitOfWork = iUnitOfWork;
            _iMapper = iMapper;
            _iLeaveCfService = iLeaveCfService;
            _dropdownService = dropdownService;
            _employeeService = employeeService;
        }
        #endregion

        #region Create
        [HttpGet]
        [Authorize(Permissions.CfLeave.Create)]
        public IActionResult Create()
        {
            var model = new LeaveCfVm();
            model.EmployeeLookUp = _dropdownService.GetEmployeeSelectListItems();
            model.LeaveTypeLookUp = _dropdownService.GetEmpLeaveTypeSelectListItems();
            model.LeaveYearLookUp = _dropdownService.GetYearSelectListItems();
            model.LeaveYear = DateTime.Now.Year;
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(LeaveCfVm modelVm)
        {
            modelVm.EmployeeLookUp = _dropdownService.GetEmployeeSelectListItems();
            modelVm.LeaveTypeLookUp = _dropdownService.GetEmpLeaveTypeSelectListItems();
            modelVm.LeaveYearLookUp = _dropdownService.GetYearSelectListItems();
            try
            {
                if (!ModelState.IsValid)
                {
                    SaveFailedMsg("Information Is Not Correct");
                    return View("Create", modelVm);
                }
                _iLeaveCfService.CurrentUserId = UserId;
                var model = _iMapper.Map<LeaveCf>(modelVm);
                model.ActionById = UserId;
                model.ActionDate = Domain.Utility.Utility.GetBdDateTimeNow();

                var isAdded = _iLeaveCfService.Add(model);

                if (!isAdded)
                {
                    SaveFailedMsg();
                    return View("Create", modelVm);
                }
                SaveSuccessMsg();
                return RedirectToAction("Create");
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
        [Authorize(Permissions.LeaveTypes.ListView)]
        public IActionResult Search()
        {
            var vm = new LeaveCfSearchVm();
            vm.EmployeeLookUp = _dropdownService.GetEmployeeSelectListItems();
            vm.LeaveTypeLookUp = _dropdownService.GetEmpLeaveTypeSelectListItems();
            vm.LeaveYearLookUp = _dropdownService.GetYearSelectListItems();
            return View(vm);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
        public async Task<IActionResult>
            Search(DataTablePagination<LeaveCfSearchVm, LeaveCfSearchVm> searchVm = null)
        {
            if (searchVm == null) searchVm = new DataTablePagination<LeaveCfSearchVm, LeaveCfSearchVm>();
            if (searchVm?.SearchModel == null) searchVm.SearchModel = new LeaveCfSearchVm();
            var dataTable = await _iLeaveCfService.SearchAsync(searchVm);
            return dataTable == null ? NotFound() : Ok(dataTable);
        }
        #endregion

        #region GetEmployeeJsonData
        public async Task<IActionResult> GetEmployeeJsonDataById(long id)
        {
            try
            {
                if (id == 0) return BadRequest();
                var model = await _employeeService.GetEmployeeDataAsync(id);
                if (model == null) return NotFound();
                return Ok(model);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion
    }
}
