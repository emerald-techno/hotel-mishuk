using AutoMapper;
using Domain.Entities.Leave;
using Domain.Utility.Common;
using Domain.ViewModel.Leave.LeaveType;
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
    public class LeaveTypeController : AppBaseController
    {
        #region Config
        private readonly IUnitOfWork _iUnitOfWork;
        private readonly IMapper _iMapper;
        private readonly ILeaveTypeService _iLeaveTypeService;
        private readonly DropdownService _dropdownService;
        public LeaveTypeController(IUnitOfWork iUnitOfWork, IMapper iMapper, ILeaveTypeService iLeaveTypeService, DropdownService dropdownService) : base(iUnitOfWork)
        {
            _iUnitOfWork = iUnitOfWork;
            _iMapper = iMapper;
            _iLeaveTypeService = iLeaveTypeService;
            _dropdownService = dropdownService;
        }
        #endregion

        #region Create
        [HttpGet]
        [Authorize(Permissions.LeaveTypes.Create)]
        public IActionResult Create()
        {
            var model = new LeaveTypeVm();
            model.GenderLookUp = _dropdownService.GetGenderSelectListItemsForLeaveTypes();
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(LeaveTypeVm modelVm)
        {
            modelVm.GenderLookUp = _dropdownService.GetGenderSelectListItemsForLeaveTypes();
            try
            {
                if (!ModelState.IsValid)
                {
                    SaveFailedMsg("Information Is Not Correct");
                    return View("Create", modelVm);
                }
                _iLeaveTypeService.CurrentUserId = UserId;
                var model = _iMapper.Map<LeaveType>(modelVm);
                model.ActionById = UserId;
                model.ActionDate = DU.Utility.GetBdDateTimeNow();

                var isAdded = _iLeaveTypeService.Add(model);

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
            var vm = new LeaveTypeSearchVm();
            return View(vm);
        }


        [HttpPost]
        [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
        public async Task<IActionResult>
            Search(DataTablePagination<LeaveTypeSearchVm, LeaveTypeSearchVm> searchVm = null)
        {
            if (searchVm == null) searchVm = new DataTablePagination<LeaveTypeSearchVm, LeaveTypeSearchVm>();
            if (searchVm?.SearchModel == null) searchVm.SearchModel = new LeaveTypeSearchVm();
            var dataTable = await _iLeaveTypeService.SearchAsync(searchVm);
            return dataTable == null ? NotFound() : Ok(dataTable);
        }
        #endregion

        #region Edit
        [HttpGet]
        [Authorize(Permissions.LeaveTypes.Edit)]
        public IActionResult Edit(long id)
        {
            try
            {
                var data = _iLeaveTypeService.GetById(id);
                if (data == null)
                {
                    return NotFoundMsg();
                }
                var model = _iMapper.Map<LeaveTypeVm>(data);
                model.GenderLookUp = _dropdownService.GetGenderSelectListItemsForLeaveTypes();
                return View(model);
            }
            catch (Exception e)
            {
                ExceptionMsg(e.Message);
                return View("_404");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(LeaveTypeVm modelVm)
        {

            modelVm.GenderLookUp = _dropdownService.GetGenderSelectListItemsForLeaveTypes();
            try
            {
                if (modelVm.Id <= 0 || !ModelState.IsValid) return View(modelVm);
                var model = _iMapper.Map<LeaveType>(modelVm);

                model.UpdatedById = UserId;
                model.UpdateDate = DU.Utility.GetBdDateTimeNow();
                var isAdded = await _iLeaveTypeService.UpdateAsync(model);
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
        [Authorize(Permissions.LeaveTypes.Delete)]
        public async Task<IActionResult> Delete(long id)
        {
            var data = await _iLeaveTypeService.GetByIdAsync(id);
            if (data == null)
            {
                DeleteFailedMsg();
                return NotFound();
            }
            data.IsDeleted = true;
            DeleteSuccessMsg();
            var isRemove = _iLeaveTypeService.Update(data);
            return RedirectToAction("Search");
        }
        #endregion
    }
}
