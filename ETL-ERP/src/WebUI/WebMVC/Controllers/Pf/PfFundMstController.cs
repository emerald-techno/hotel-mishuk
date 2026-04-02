using AutoMapper;
using Domain.Utility.Common;
using Domain.ViewModel.Pf.PfFund;
using Domain.ViewModel.Report;
using Interface.Services.Pf;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using Utility.Export;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;

namespace WebMVC.Controllers.Pf
{
    public class PfFundMstController : AppBaseController
    {
        #region Config

        private readonly IUnitOfWork _iUnitWork;
        private readonly IPfFundMstService _iService;
        private readonly IMapper _iMapper;
        private readonly DropdownService _dropdownService;
        private readonly IHttpContextAccessor _ihttpContextAccessor;

        public PfFundMstController(IUnitOfWork iUnitOfWork,
                                IPfFundMstService iService,
                                IMapper iMapper,
                                DropdownService dropdownService,
                                IHttpContextAccessor ihttpContextAccessor) : base(iUnitOfWork)
        {
            _iUnitWork = iUnitOfWork;
            _iService = iService;
            _iMapper = iMapper;
            _dropdownService = dropdownService;
            _ihttpContextAccessor = ihttpContextAccessor;
        }

        #endregion

        #region Create

        [HttpGet]
        [Authorize(Permissions.PfFundMsts.ReportGenerate)]
        public ActionResult Report()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreatePfReport(PfFundMstVm modelVm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    SaveFailedMsg("Information is not correct");
                    return View("Report", modelVm);
                }

                _iService.CurrentUserId = UserId;

                var isAdded = await _iService.PfReportAdd(modelVm);

                if (!isAdded)
                {
                    SaveFailedMsg();
                    return View("Report", modelVm);
                }

                SaveSuccessMsg();
                return RedirectToAction("Report");
            }
            catch (Exception e)
            {
                ExceptionMsg(e.Message);
                return View("Report", modelVm);
            }
        }

        #endregion

        #region Search

        [HttpGet]
        [Authorize(Permissions.PfFundMsts.ListView)]
        public IActionResult Search()
        {
            var vm = new PfFundMstSearchVm();
            return View(vm);
        }


        [HttpPost]
        [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
        public async Task<IActionResult>
            Search(DataTablePagination<PfFundMstSearchVm, PfFundMstSearchVm> searchVm = null)
        {
            if (searchVm == null) searchVm = new DataTablePagination<PfFundMstSearchVm, PfFundMstSearchVm>();
            if (searchVm?.SearchModel == null) searchVm.SearchModel = new PfFundMstSearchVm();
            var dataTable = await _iService.SearchAsync(searchVm);
            return dataTable == null ? NotFound() : Ok(dataTable);
        }

        #endregion

        #region EmployeeSummary

        [HttpGet]
        [Authorize(Permissions.PfFundMsts.ListView)]
        public IActionResult EmployeeSummary()
        {
            var vm = new EmployeePfSummaryReportVm();
            return View(vm);
        }


        [HttpPost]
        [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
        public async Task<IActionResult>
            EmployeeSummarySearch(DataTablePagination<EmployeePfSummaryReportVm, EmployeePfSummaryReportVm> searchVm = null)
        {
            if (searchVm == null) searchVm = new DataTablePagination<EmployeePfSummaryReportVm, EmployeePfSummaryReportVm>();
            if (searchVm?.SearchModel == null) searchVm.SearchModel = new EmployeePfSummaryReportVm();
            var dataTable = await _iService.SearchDtlAsync(searchVm);
            return dataTable == null ? NotFound() : Ok(dataTable);
        }

        #endregion

        #region Details

        [Authorize(Permissions.PfFundMsts.DetailsView)]

        public async Task<ActionResult> Details(long id)
        {
            try
            {
                var model = await _iService.GetPfByIdAsync(id);
                return View(model);
            }
            catch (Exception e)
            {
                ExceptionMsg(e.Message);
                return View("_404");
            }
        }

        public async Task<ActionResult> SummaryDetails(long employeeId)
        {
            try
            {
                var model = await _iService.GetEmployeePfInfo(employeeId);
                return View(model);
            }
            catch (Exception e)
            {
                ExceptionMsg(e.Message);
                return View("_404");
            }
        }

        #endregion

        #region IsPfExist

        public IActionResult IsPfExist(short year, short month)
        {
            if (year > 1999 && month > 0)
            {
                DateTime firstDate = new DateTime(year, month, 1);
                DateTime lastDate = firstDate.AddMonths(1).AddSeconds(-1);

                var formDate = firstDate;
                var toDate = lastDate;

                var exist = _iService.GetFirstOrDefault(c => firstDate.Date == c.FundFromDate.Date && lastDate.Date == c.FundToDate.Date);
                if (exist != null) return Ok(true);
                return Ok(false);
            }

            return BadRequest();
        }

        #endregion

        #region GetPfFundReport

        [HttpPost]
        public async Task<PartialViewResult> GetPfFundReportPartial(PfFundMstVm modelVm)
        {
            var model = await _iService.GetEmpPfFundData(modelVm);
            model.FundType = PfReportType.Monthly;
            return model != null ? PartialView("PartialView/_PfFundReport", model) : PartialView("_404");
        }

        #endregion

        #region EmployeePFListReportPrint
        public async Task<ActionResult> EmployeePFListReportPrint()
        {

            string html = await _iService.EmployeePFListReportHtml();

            ExportToPDF export = new ExportToPDF(_ihttpContextAccessor);
            ExportDataTitle reportTitle = new ExportDataTitle();
            reportTitle.Header = PrintInfo.CompanyName;
            reportTitle.AddressOne = PrintInfo.CompanyAddress;
            reportTitle.ReportTitle = "Employee Provident Fund List Report";
            reportTitle.SignatureList = new List<string>();
            reportTitle.SignatureList.Add("Sign One");
            reportTitle.SignatureList.Add("Sign Two");

            string reportName = "Employee Provident Fund List_" + DateTime.Now.ToString("dd/MM/yyyy");


            return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 50, top: 80, left: 10, right: 10, isLandScape: true), "application/pdf");

        }
        #endregion

        #region EmployeePFDetailsReportPrint
        public async Task<ActionResult> EmployeePFDetailsReportPrint(long id)
        {
            string html = await _iService.EmployeePFDetailsReportHtml(id);

            ExportToPDF export = new ExportToPDF(_ihttpContextAccessor);
            ExportDataTitle reportTitle = new ExportDataTitle();
            reportTitle.Header = PrintInfo.CompanyName;
            reportTitle.AddressOne = PrintInfo.CompanyAddress;
            reportTitle.ReportTitle = "Employee Provident Fund Report Details";
            reportTitle.SignatureList = new List<string>();
            reportTitle.SignatureList.Add("Sign One");
            reportTitle.SignatureList.Add("Sign Two");

            string reportName = "Employee Provident Fund Details" + DateTime.Now.ToString("dd/MM/yyyy");


            return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 50, top: 80, left: 10, right: 10, isLandScape: false), "application/pdf");

        }
        #endregion

        #region PF Schedule Report
        public ActionResult PfScheduleReport()
        {
            var model = new PfFundMstVm();
            model.FormDateStr = DateTime.Today.ToString("dd/MM/yyyy");
            model.ToDateStr = DateTime.Today.ToString("dd/MM/yyyy");
            model.DepatmentLookup = _dropdownService.GetNonAcademicDepartmentSelectListItems();
            model.DesignationLookup = _dropdownService.GetDesignationSelectListItems();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> PfScheduleReport(PfFundMstVm model)
        {
            var data = await _iService.GetPfScheduleReportHtml(model);
            return Ok(data);
        }

        public async Task<ActionResult> PfScheduleReportPrint(string fromDate, string toDate)
        {
            var model = new PfFundMstVm();
            model.FormDateStr = fromDate;
            model.ToDateStr = toDate;

            string html = await _iService.GetPfScheduleReportHtml(model);

            ExportToPDF export = new ExportToPDF(_ihttpContextAccessor);
            ExportDataTitle reportTitle = new ExportDataTitle();
            reportTitle.Header = PrintInfo.CompanyName;
            reportTitle.AddressOne = PrintInfo.CompanyAddress;
            reportTitle.ReportTitle = "PF Schedule Report";
            reportTitle.SignatureList = new List<string>();
            reportTitle.SignatureList.Add("Sign One");
            reportTitle.SignatureList.Add("Sign Two");

            string reportName = "PfScheduleReport_" + DateTime.Today.ToString("dd_mm_yyy");


            return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 50, top: 80, left: 10, right: 10, isLandScape: true), "application/pdf");
        }
        #endregion
    }
}
