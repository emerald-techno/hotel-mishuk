using AutoMapper;
using Domain.Entities.HR;
using Domain.Utility.Common;
using Domain.ViewModel.Leave.EmpLeaveApplication;
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
    public class EmpLeaveAppController : AppBaseController
    {
        #region Config

        private readonly IUnitOfWork _iUnitOfWork;
        private readonly IMapper _iMapper;
        private readonly IEmpLeaveApplicationService _iService;
        private readonly DropdownService _dropdownService;
        private readonly IEmployeeService _iEmployeeService;
        private readonly IHttpContextAccessor _ihttpContextAccessor;

        public EmpLeaveAppController(IUnitOfWork iUnitOfWork, IMapper iMapper, IEmpLeaveApplicationService iService,
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

        #region GeneralApply

        public ActionResult GeneralApply()
        {
            var model = new EmpLeaveApplicationVm();
            model.EmployeeLookUp = _dropdownService.GetEmployeeSelectListItems();
            model.LeaveTypeLookUp = _dropdownService.GetEmpLeaveTypeSelectListItems();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> GeneralApply(EmpLeaveApplicationVm modelVm)
        {
            modelVm.LeaveTypeLookUp = _dropdownService.GetEmpLeaveTypeSelectListItems();
            modelVm.EmployeeLookUp = _dropdownService.GetEmployeeSelectListItems();

            try
            {
                if (!ModelState.IsValid)
                {
                    SaveFailedMsg("Information Is Not Correct");
                    return View("GeneralApply", modelVm);
                };

                _iService.CurrentUserId = UserId;
                var isAdded = await _iService.GeneralLeaveApplyAsync(modelVm);

                if (!isAdded)
                {
                    SaveFailedMsg();
                    return View("GeneralApply", modelVm);
                }

                SaveSuccessMsg();

                return RedirectToAction("GeneralApply");
            }
            catch (Exception e)
            {
                ExceptionMsg(e.Message);
                return View("GeneralApply", modelVm);
            }
        }

        #endregion

        #region Apply

        public ActionResult Apply()
        {
            var model = new EmpLeaveApplicationVm();
            model.LeaveTypeLookUp = _dropdownService.GetEmpLeaveTypeSelectListItems();

            return View(model);
        }

        [HttpPost]
        public async Task<PartialViewResult> ApplyPreview(EmpLeaveApplicationVm modelVm)
        {
            try
            {
                var employee = GetEmployeeInfo();
                if (employee == null) throw new Exception("No Employee Found...!");
                modelVm.EmployeeId = employee.Id;

                modelVm.FromDate = (DateTime)(!string.IsNullOrEmpty(modelVm.FromDateStr) ? DU.Utility.ConvertStrToDate(modelVm.FromDateStr) : modelVm.FromDate);
                modelVm.ToDate = (DateTime)(!string.IsNullOrEmpty(modelVm.ToDateStr) ? DU.Utility.ConvertStrToDate(modelVm.ToDateStr) : modelVm.ToDate);

                var model = await _iService.GetLeaveAppPreviewData(modelVm);
                return model != null ? PartialView("PartialView/_EmpLeaveAppPreview", model) : PartialView("_404");
            }
            catch (Exception ex)
            {
                ExceptionMsg(ex.Message);
                return PartialView("_404");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Apply(EmpLeaveApplicationVm modelVm)
        {

            modelVm.LeaveTypeLookUp = _dropdownService.GetEmpLeaveTypeSelectListItems();

            try
            {
                if (!ModelState.IsValid)
                {
                    SaveFailedMsg("Information Is Not Correct");
                    return View("Apply", modelVm);
                };

                _iService.CurrentUserId = UserId;
                var isAdded = await _iService.LeaveApplyAsync(modelVm);

                if (!isAdded)
                {
                    SaveFailedMsg();
                    return View("Apply", modelVm);
                }

                SaveSuccessMsg();

                return RedirectToAction("Apply");
            }
            catch (Exception e)
            {
                ExceptionMsg(e.Message);
                return View("Apply", modelVm);
            }
        }

        #endregion

        #region Search

        [HttpGet]
        public IActionResult Search()
        {
            var vm = new EmpLeaveApplicationSearchVm();
            return View(vm);
        }

        [HttpGet]
        public IActionResult EmpAppSearch()
        {
            var vm = new EmpLeaveApplicationSearchVm();

            var employee = GetEmployeeInfo();
            if (employee == null) throw new Exception("No Employee Found...!");
            vm.EmployeeId = employee.Id;

            return View(vm);
        }


        [HttpPost]
        [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
        public async Task<IActionResult>
            Search(DataTablePagination<EmpLeaveApplicationSearchVm, EmpLeaveApplicationSearchVm> searchVm = null)
        {
            if (searchVm == null) searchVm = new DataTablePagination<EmpLeaveApplicationSearchVm, EmpLeaveApplicationSearchVm>();
            if (searchVm?.SearchModel == null) searchVm.SearchModel = new EmpLeaveApplicationSearchVm();
            var dataTable = await _iService.SearchAsync(searchVm);
            return dataTable == null ? NotFound() : Ok(dataTable);
        }

        #endregion

        #region Detail

        public async Task<ActionResult> Details(long id)
        {
            try
            {
                _iService.CurrentUserId = UserId;
                var model = await _iService.GetLeaveAppDataById(id);
                model.FromDateStr = model.FromDate.ToString("dd/MM/yyyy");
                model.ToDateStr = model.ToDate.ToString("dd/MM/yyyy");
                return View(model);
            }
            catch (Exception e)
            {
                ExceptionMsg(e.Message);
                return View("_404");
            }
        }

        #endregion

        #region CancelLeave

        public async Task<ActionResult> CancelLeave(long id)
        {
            try
            {
                var isCancel = await _iService.CancelLeaveApp(id);
                if (isCancel)
                {
                    SaveSuccessMsg("Leave Cancel Success...!");
                }

                return RedirectToAction("Details", new { id = id });
            }
            catch (Exception e)
            {
                ExceptionMsg(e.Message);
                return RedirectToAction("Details", new { id = id });
            }
        }

        #endregion

        #region LeaveReport
        [Authorize(Permissions.EmpLeaveApplications.ReportView)]
        public ActionResult LeaveReport()
        {
            var model = new EmpLeaveReportVm();
            model.DepatmentLookup = _dropdownService.GetNonAcademicDepartmentSelectListItems();
            model.DesignationLookup = _dropdownService.GetDesignationSelectListItems();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> LeaveReport(EmpLeaveReportVm model)
        {
            var data = await _iService.GetLeaveReportData(model);
            return Ok(data);
        }

        #endregion

        private Employee GetEmployeeInfo()
        {
            var employee = _iEmployeeService.GetFirstOrDefault(c => c.UserId == UserId);
            return employee;
        }
    }
}
