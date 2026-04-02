using AutoMapper;
using Domain.Entities.Leave;
using Domain.Utility.Common;
using Domain.ViewModel.Leave.LeaveSetup;
using Interface.Services.Leave;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using DU = Domain.Utility;

namespace WebMVC.Controllers.Leave
{
    public class LeaveSetupController : AppBaseController
    {
        #region Config
        private readonly IUnitOfWork _iUnitOfWork;
        private readonly IMapper _iMapper;
        private readonly ILeaveSetupService _iLeaveSetupService;
        private readonly DropdownService _dropdownService;
        private readonly IHttpContextAccessor _ihttpContextAccessor;
        private readonly ILeaveTypeService _iLeaveTypeService;
        public LeaveSetupController(IUnitOfWork iUnitOfWork, IMapper iMapper, ILeaveSetupService iLeaveSetupService, DropdownService dropdownService, IHttpContextAccessor ihttpContextAccessor, ILeaveTypeService iLeaveTypeService) : base(iUnitOfWork)
        {
            _iUnitOfWork = iUnitOfWork;
            _iMapper = iMapper;
            _iLeaveSetupService = iLeaveSetupService;
            _dropdownService = dropdownService;
            _ihttpContextAccessor = ihttpContextAccessor;
            _iLeaveTypeService = iLeaveTypeService;
        }
        #endregion

        #region Create
        [HttpGet]
        [Authorize(Permissions.LeaveSetups.Create)]
        public IActionResult Create()
        {
            var model = new LeaveSetupVm();
            model.LeaveLimitTypeLookUp = _dropdownService.GetLeaveLimitTypeSelectListItems();
            model.LeaveTypeLookUp = _dropdownService.GetEmpLeaveTypeSelectListItems();
            model.IsCarryForward = false;
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(LeaveSetupVm modelVm)
        {
            modelVm.LeaveLimitTypeLookUp = _dropdownService.GetLeaveLimitTypeSelectListItems();
            modelVm.LeaveTypeLookUp = _dropdownService.GetEmpLeaveTypeSelectListItems();

            try
            {
                if (!ModelState.IsValid)
                {
                    SaveFailedMsg("Information Is Not Correct");
                    return View("Create", modelVm);
                }

                _iLeaveSetupService.CurrentUserId = UserId;
                var model = _iMapper.Map<LeaveSetup>(modelVm);
                model.ActiveDate = DateTime.Now;
                model.ActionById = UserId;
                model.IsActive = true;
                model.ActionDate = DU.Utility.GetBdDateTimeNow();

                var isAdded = _iLeaveSetupService.Add(model);

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

        #region GetLeaveTypeJsonDataById
        public IActionResult GetLeaveTypeJsonDataById(long id)
        {
            try
            {
                if (id == 0) return BadRequest();
                var data = _iLeaveTypeService.GetById(id);
                if (data == null) return NotFound();
                return Ok(data);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        #endregion

        #region Search
        [HttpGet]
        [Authorize(Permissions.LeaveSetups.ListView)]
        public IActionResult Search()
        {
            var vm = new LeaveSetupSearchVm();
            return View(vm);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
        public async Task<IActionResult>
            Search(DataTablePagination<LeaveSetupSearchVm, LeaveSetupSearchVm> searchVm = null)
        {
            if (searchVm == null) searchVm = new DataTablePagination<LeaveSetupSearchVm, LeaveSetupSearchVm>();
            if (searchVm?.SearchModel == null) searchVm.SearchModel = new LeaveSetupSearchVm();
            var dataTable = await _iLeaveSetupService.SearchAsync(searchVm);
            return dataTable == null ? NotFound() : Ok(dataTable);
        }
        #endregion

        #region Edit
        [HttpGet]
        [Authorize(Permissions.LeaveSetups.Edit)]
        public IActionResult Edit(long id)
        {
            try
            {
                var data = _iLeaveSetupService.GetById(id);
                if (data == null)
                {
                    return NotFoundMsg();
                }
                var model = _iMapper.Map<LeaveSetupVm>(data);
                model.LeaveLimitTypeLookUp = _dropdownService.GetLeaveLimitTypeSelectListItems();
                model.LeaveTypeLookUp = _dropdownService.GetEmpLeaveTypeSelectListItems();
                return View(model);
            }
            catch (Exception e)
            {
                ExceptionMsg(e.Message);
                return View("_404");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(LeaveSetupVm modelVm)
        {
            modelVm.LeaveLimitTypeLookUp = _dropdownService.GetLeaveLimitTypeSelectListItems();
            modelVm.LeaveTypeLookUp = _dropdownService.GetEmpLeaveTypeSelectListItems();

            try
            {
                if (modelVm.Id <= 0 || !ModelState.IsValid) return View(modelVm);
                var model = _iMapper.Map<LeaveSetup>(modelVm);
                model.ActiveDate = DateTime.Now;
                model.IsActive = true;
                model.UpdatedById = UserId;
                model.UpdateDate = DU.Utility.GetBdDateTimeNow();
                var isAdded = await _iLeaveSetupService.UpdateAsync(model);
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
    }
}
