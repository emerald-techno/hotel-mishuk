using AutoMapper;
using Domain.Entities.Leave;
using Domain.Enums.AppEnums;
using Domain.Utility.Common;
using Domain.ViewModel.Leave.DptLeaveReviewer;
using Interface.Services.Admin;
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
    public class DptLeaveReviewerController : AppBaseController
    {
        #region Config

        private readonly IUnitOfWork _iUnitOfWork;
        private readonly IMapper _iMapper;
        private readonly IDptLeaveReviewerService _iService;
        private readonly DropdownService _dropdownService;
        private readonly IDepartmentService _iDepartmentService;
        private readonly IHttpContextAccessor _ihttpContextAccessor;

        public DptLeaveReviewerController(IUnitOfWork iUnitOfWork, IMapper iMapper, IDptLeaveReviewerService iService,
            DropdownService dropdownService, IDepartmentService iDepartmentService, IHttpContextAccessor ihttpContextAccessor) : base(iUnitOfWork)
        {
            _iUnitOfWork = iUnitOfWork;
            _iMapper = iMapper;
            _iService = iService;
            _dropdownService = dropdownService;
            _iDepartmentService = iDepartmentService;
            _ihttpContextAccessor = ihttpContextAccessor;
        }

        #endregion

        #region Create
        [HttpGet]
        [Authorize(Permissions.DptLeaveReviewers.Create)]
        public IActionResult Create()
        {
            var model = new DptLeaveReviewerSearchVm();
            model.DepartmentLookUp = _dropdownService.GetNonAcademicDepartmentSelectListItems();
            model.ReviewerLookUp = _dropdownService.GetUserSelectListItems();
            model.AltReviewerLookUp = _dropdownService.GetUserSelectListItems();
            model.SLNoLookUp = _dropdownService.GetEmployeeLeaveReviewerSLNoListItems();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DptLeaveReviewerVm modelVm)
        {

            try
            {
                var model = _iMapper.Map<DptLeaveReviewer>(modelVm);
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
        Search(DataTablePagination<DptLeaveReviewerSearchVm, DptLeaveReviewerSearchVm> searchVm = null)
        {
            if (searchVm == null) searchVm = new DataTablePagination<DptLeaveReviewerSearchVm, DptLeaveReviewerSearchVm>();
            if (searchVm?.SearchModel == null) searchVm.SearchModel = new DptLeaveReviewerSearchVm();
            var dataTable = await _iService.SearchAsync(searchVm);
            return dataTable == null ? NotFound() : Ok(dataTable);
        }

        #endregion

        #region Delete

        [HttpGet]
        [Authorize(Permissions.DptLeaveReviewers.Delete)]
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
