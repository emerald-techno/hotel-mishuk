using AutoMapper;
using Domain.Utility.Common;
using Domain.ViewModel.Attendance.EmpAttendence;
using Interface.Services.Attendance;
using Interface.Services.HotelManagement;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using Utility.Export;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;

namespace WebMVC.Controllers.Attendance;

[Authorize]
public class EmpAttendanceController : AppBaseController
{
    #region Config
    private readonly IUnitOfWork _iUnitWork;
    private readonly IEmpAttendanceService _iService;
    private readonly DropdownService _dropdownService;
    private readonly IMapper _iMapper;
    private readonly IWebHostEnvironment _iWebHostEnvironment;
    private readonly IHttpContextAccessor _iHttpContextAccessor;

    public EmpAttendanceController(IUnitOfWork iUnitOfWork,
                            IEmpAttendanceService iService,
                            DropdownService dropdownService,
                            IMapper iMapper,
                            IHttpContextAccessor iHttpContextAccessor,
                            IWebHostEnvironment iWebHostEnvironment
                            ) : base(iUnitOfWork)
    {
        _iUnitWork = iUnitOfWork;
        _iService = iService;
        _dropdownService = dropdownService;
        _iMapper = iMapper;
        _iWebHostEnvironment = iWebHostEnvironment;
        _iHttpContextAccessor = iHttpContextAccessor;
    }
    #endregion

    #region Create

    [HttpGet]
    [Authorize(Permissions.EmpAttendances.Create)]
    public IActionResult Create()
    {
        var model = new EmpAttendanceVm();
        model.AttendDateStr = DateTime.Now.ToString("dd/MM/yyyy");
        model.EmployeetLookUp = _dropdownService.GetEmployeeSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(EmpAttendanceVm modelVm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information Is Not Correct");
                return View("Create", modelVm);
            }
            _iService.CurrentUserId = UserId;
            var isAdded = await _iService.AddEmployeeInOutTime(modelVm);
            if (isAdded)
            {
                SaveSuccessMsg();
                return RedirectToAction("Create", modelVm);
            }
            else
            {
                SaveFailedMsg();
                return RedirectToAction("Create", modelVm);
            }
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return RedirectToAction("Create", modelVm);
        }
    }
    #endregion

    #region MonthlyAttendanceReport

    [HttpGet]
    [Authorize(Permissions.EmpAttendances.ReportView)]
    public ActionResult MonthlyAttendanceReport()
    {

        var model = new EmpAttendanceReportVm();
        model.Year = DateTime.Now.Year;
        model.Month = DateTime.Now.Month;
        model.YearLookUp = _dropdownService.GetYearSelectListItems();
        model.MonthLookUp = _dropdownService.GetMonthSelectListItems();
        model.EmployeeLookUp = _dropdownService.GetEmployeeSelectListItems();

        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> AttendanceReport(EmpAttendanceReportVm model)
    {
        var data = await _iService.GetReportData(model);
        return Ok(data);
    }

    #endregion

    #region EmployeeDailyAttendanceReport

    [HttpGet]
    [Authorize(Permissions.EmpAttendances.ReportView)]
    public ActionResult EmpDailyAttendanceReport()
    {
        var model = new EmpDailyAttendanceReportVm();
        model.AttendDateStr = DateTime.Now.ToString("dd/MM/yyyy");
        model.EmployeeLookUp = _dropdownService.GetEmployeeSelectListItems();

        return View(model);
    }

    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        EmployeeAttendanceDailyReport(DataTablePagination<EmpDailyAttendanceReportVm, EmpDailyAttendanceReportVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<EmpDailyAttendanceReportVm, EmpDailyAttendanceReportVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new EmpDailyAttendanceReportVm();
        var dataTable = await _iService.DailyReportAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion

    #region Print

    public async Task<ActionResult> PrintEmpJobCard(long empId, string formDateStr, string toDateStr, long departmentId, long designationId)
    {
        var searchVm = new DataTablePagination<EmpDailyAttendanceReportVm, EmpDailyAttendanceReportVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new EmpDailyAttendanceReportVm();
        searchVm.SearchModel.EmployeeId = empId;
        searchVm.SearchModel.FormDateStr = formDateStr;
        searchVm.SearchModel.ToDateStr = toDateStr;
        var html = await _iService.DailyReportHtml(searchVm);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Daily Attendance Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");

        string reportName = "Leave Report_" + DateTime.Now.ToString("dd/MM/yyyy");

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 50, top: 80, left: 10, right: 10, isLandScape: false), "application/pdf");
    }

    public async Task<ActionResult> PrintMonthlyAttReport(short year, short month, long? empId)
    {
        var searchVm = new EmpAttendanceReportVm();
        searchVm.Year = year;
        searchVm.Month = month;
        searchVm.EmployeeId = empId ?? 0;
        var html = await _iService.MonthlyAttReportHtml(searchVm);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Monthly Attendance Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");

        string reportName = "Leave Report_" + DateTime.Now.ToString("dd/MM/yyyy");

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 50, top: 80, left: 10, right: 10, isLandScape: false), "application/pdf");
    }

    #endregion

    #region Import

    public IActionResult Import()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Import(IFormFile importFile)
    {
        try
        {
            _iService.CurrentUserId = UserId;
            var isImported = await _iService.ImportAsync(importFile);
            return RedirectToAction("MonthlyAttendanceReport");
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return RedirectToAction("Import");
        }

    }

    #endregion

    #region TestEntry

    //public async Task<IActionResult> TestEntry()
    //{
    //    _iService.CurrentUserId = UserId;
    //    var result = await _iService.TestAttEntry();
    //    return RedirectToAction("MonthlyAttendanceReport");
    //}

    #endregion

    #region DayWiseAttendanceReport

    [HttpGet]
    [Authorize(Permissions.EmpAttendances.ReportView)]
    public ActionResult DayWiseAttendanceReport()
    {
        var model = new EmpDailyAttendanceReportVm();
        model.DesignationLookUp = _dropdownService.GetDesignationSelectListItems();
        model.DepartmentLookUp = _dropdownService.GetDepartmentSelectListItems();
        return View(model);
    }

    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        DayWiseAttendanceReport(DataTablePagination<EmpDailyAttendanceReportVm, EmpDailyAttendanceReportVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<EmpDailyAttendanceReportVm, EmpDailyAttendanceReportVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new EmpDailyAttendanceReportVm();
        searchVm.SearchModel.IsDayWise = true;
        var dataTable = await _iService.DayWiseReportAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion

    #region DayWiseAttendanceReportPrint

    public async Task<ActionResult> DayWiseAttendanceReportPrint(string searchDate, short designationId, short departmentId)
    {
        var searchVm = new DataTablePagination<EmpDailyAttendanceReportVm, EmpDailyAttendanceReportVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new EmpDailyAttendanceReportVm();
        searchVm.SearchModel.SearchDateStr = searchDate;
        searchVm.SearchModel.DesignationId = designationId;
        searchVm.SearchModel.DepartmentId = departmentId;

        var html = await _iService.DayWiseReportHtml(searchVm);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Day Wise Attendance Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");

        string reportName = "Leave Report_" + DateTime.Now.ToString("dd/MM/yyyy");

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 50, top: 80, left: 10, right: 10, isLandScape: false), "application/pdf");
    }

    #endregion
}
