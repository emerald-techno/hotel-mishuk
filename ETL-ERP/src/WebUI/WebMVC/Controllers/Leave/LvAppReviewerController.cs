using AutoMapper;
using Domain.Enums.AppEnums;
using Domain.Utility.Common;
using Domain.ViewModel.Leave.LvAppReviewer;
using Interface.Services.Hr;
using Interface.Services.Leave;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;

namespace WebMVC.Controllers.Leave
{
    public class LvAppReviewerController : AppBaseController
    {
        #region Config

        private readonly IUnitOfWork _iUnitOfWork;
        private readonly IMapper _iMapper;
        private readonly ILvAppReviewerService _iService;
        private readonly DropdownService _dropdownService;
        private readonly IEmployeeService _iEmployeeService;
        private readonly IHttpContextAccessor _ihttpContextAccessor;

        public LvAppReviewerController(IUnitOfWork iUnitOfWork, IMapper iMapper, ILvAppReviewerService iService, DropdownService dropdownService, IEmployeeService iEmployeeService, IHttpContextAccessor ihttpContextAccessor) : base(iUnitOfWork)
        {
            _iUnitOfWork = iUnitOfWork;
            _iMapper = iMapper;
            _iService = iService;
            _dropdownService = dropdownService;
            _iEmployeeService = iEmployeeService;
            _ihttpContextAccessor = ihttpContextAccessor;
        }

        #endregion

        #region ReviewApplication

        [HttpPost]
        public async Task<ActionResult> ReviewApp(LvAppReviewerVm vm)
        {
            try
            {
                _iService.CurrentUserId = UserId;
                vm.Status = (short)LvAppReviewerStatusEnum.Review;
                var result = await _iService.LeaveAppReview(vm);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        #endregion

        #region ApproveApplication

        [HttpPost]
        public async Task<ActionResult> ApproveApp(LvAppReviewerVm vm)
        {
            try
            {
                _iService.CurrentUserId = UserId;
                vm.Status = (short)LvAppReviewerStatusEnum.Approve;
                var result = await _iService.LeaveAppReview(vm);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        #endregion

        #region RejectPayroll

        public async Task<ActionResult> RejectApp(LvAppReviewerVm vm)
        {
            try
            {
                _iService.CurrentUserId = UserId;
                vm.Status = (short)LvAppReviewerStatusEnum.Reject;
                var result = await _iService.LeaveAppReject(vm);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region Search

        [HttpGet]
        public IActionResult Search()
        {
            var vm = new LvAppReviewerSearchVm();
            return View(vm);
        }

        [HttpGet]
        public IActionResult MyReviewApps()
        {
            var vm = new LvAppReviewerSearchVm();
            vm.ReviewerId = UserId;
            return View(vm);
        }


        [HttpPost]
        [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
        public async Task<IActionResult>
            Search(DataTablePagination<LvAppReviewerSearchVm, LvAppReviewerSearchVm> searchVm = null)
        {
            if (searchVm == null) searchVm = new DataTablePagination<LvAppReviewerSearchVm, LvAppReviewerSearchVm>();
            if (searchVm?.SearchModel == null) searchVm.SearchModel = new LvAppReviewerSearchVm();
            var dataTable = await _iService.SearchAsync(searchVm);
            return dataTable == null ? NotFound() : Ok(dataTable);
        }

        #endregion
    }
}
