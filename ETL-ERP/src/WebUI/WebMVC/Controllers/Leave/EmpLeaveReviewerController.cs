using AutoMapper;
using Domain.Entities.Leave;
using Domain.Enums.AppEnums;
using Domain.Utility.Common;
using Domain.ViewModel.Leave.EmpLeaveReviewer;
using Interface.Services.Hr;
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
    public class EmpLeaveReviewerController : AppBaseController
    {

        #region Config

        private readonly IUnitOfWork _iUnitOfWork;
        private readonly IMapper _iMapper;
        private readonly IEmpLeaveReviewerService _iService;
        private readonly DropdownService _dropdownService;
        private readonly IEmployeeService _iEmployeeService;
        private readonly IHttpContextAccessor _ihttpContextAccessor;

        public EmpLeaveReviewerController(IUnitOfWork iUnitOfWork, IMapper iMapper, IEmpLeaveReviewerService iService,
            DropdownService dropdownService, IEmployeeService iEmployeeService, IHttpContextAccessor ihttpContextAccessor) : base(iUnitOfWork)
        {
            _iUnitOfWork = iUnitOfWork;
            _iMapper = iMapper;
            _iService = iService;
            _dropdownService = dropdownService;
            _iEmployeeService = iEmployeeService;
            _ihttpContextAccessor = ihttpContextAccessor;
        }

        #endregion

        #region Create

        [HttpGet]
        [Authorize(Permissions.EmpLeaveReviewers.Create)]
        public async Task<IActionResult> Create()
        {
            var model = new EmpLeaveReviewerSearchVm();
            model.EmployeeLookUp = _dropdownService.GetEmployeeSelectListItems();
            model.ReviewerLookUp = _dropdownService.GetUserSelectListItems();
            model.AltReviewerLookUp = _dropdownService.GetUserSelectListItems();
            model.SLNoLookUp = _dropdownService.GetEmployeeLeaveReviewerSLNoListItems();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmpLeaveReviewerVm modelVm)
        {

            try
            {

                var model = _iMapper.Map<EmpLeaveReviewer>(modelVm);
                model.ReviewFor = (short)ReviewForEnum.Leave;
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
            catch (Exception e)
            {
                ExceptionMsg(e.Message);
                return View("Create");
            }
        }

        #endregion

        #region Search

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
        public async Task<IActionResult>
        Search(DataTablePagination<EmpLeaveReviewerSearchVm, EmpLeaveReviewerSearchVm> searchVm = null)
        {
            if (searchVm == null) searchVm = new DataTablePagination<EmpLeaveReviewerSearchVm, EmpLeaveReviewerSearchVm>();
            if (searchVm?.SearchModel == null) searchVm.SearchModel = new EmpLeaveReviewerSearchVm();
            var dataTable = await _iService.SearchAsync(searchVm);
            return dataTable == null ? NotFound() : Ok(dataTable);
        }

        #endregion

        #region Delete

        [HttpGet]
        [Authorize(Permissions.EmpLeaveReviewers.Delete)]
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
            return RedirectToAction("Create");
        }

        #endregion
    }
}
