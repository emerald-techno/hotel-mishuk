using AutoMapper;
using Domain.Utility.Common;
using Domain.ViewModel.Attendance.MonthlyAttSheet;
using Interface.Services.Attendance;
using Interface.Services.Payroll;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using Utility.Export;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;

namespace WebMVC.Controllers.Payroll;

public class MonthlyAttSheetController : AppBaseController
{
    #region Config

    private readonly IMapper _iMapper;
    private readonly IMonthlyAttSheetMstService _iService;
    private readonly IEmpAttendanceService _iEmpAttendanceService;
    private readonly IUnitOfWork _iUnitWork;
    private readonly DropdownService _dropdownService;
    private readonly IWebHostEnvironment _iWebHostEnvironment;
    private readonly IHttpContextAccessor _ihttpContextAccessor;

    public MonthlyAttSheetController(IMonthlyAttSheetMstService iService, IEmpAttendanceService iEmpAttendanceService, IMapper iMapper,
                            IUnitOfWork iUnitOfWork,
                            DropdownService dropdownService, IHttpContextAccessor ihttpContextAccessor) : base(iUnitOfWork)
    {
        _iService = iService;
        _iEmpAttendanceService = iEmpAttendanceService;
        _iMapper = iMapper;
        _iUnitWork = iUnitOfWork;
        _dropdownService = dropdownService;
        _ihttpContextAccessor = ihttpContextAccessor;
    }

    #endregion

    #region Create

    [HttpGet]
    [Authorize(Permissions.MonthlyAttendanceSheetMsts.ReportView)]
    public ActionResult Report()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> CreateReport(MonthlyAttSheetMstVm modelVm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return View("Report", modelVm);
            }

            _iService.CurrentUserId = UserId;

            var isAdded = await _iService.SheetAdd(modelVm);

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
    [Authorize(Permissions.MonthlyAttendanceSheetMsts.ListView)]
    public IActionResult Search()
    {
        var vm = new MonthlyAttSheetSearchVm();
        return View(vm);
    }


    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<MonthlyAttSheetSearchVm, MonthlyAttSheetSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<MonthlyAttSheetSearchVm, MonthlyAttSheetSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new MonthlyAttSheetSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion

    #region Details

    [Authorize(Permissions.MonthlyAttendanceSheetMsts.DetailsView)]

    public async Task<ActionResult> Details(long id)
    {
        try
        {
            var model = await _iService.GetSheetByIdAsync(id);
            return View(model);
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("_404");
        }
    }

    #endregion

    #region IsSheetExist

    public IActionResult IsSheetExist(short year, short month)
    {
        if (year > 1999 && month > 0)
        {
            var exist = _iService.GetFirstOrDefault(c => c.Year == year && c.Month == month);
            if (exist != null) return Ok(true);
            return Ok(false);
        }
        return BadRequest();
    }

    #endregion

    #region MonthlyAttendanceSheetReport

    [HttpPost]
    public async Task<PartialViewResult> GetMonthlyAttSheetReportPartial(MonthlyAttSheetMstVm modelVm)
    {
        var model = await _iEmpAttendanceService.GetMonthlyAttendanceData(modelVm);
        return model != null ? PartialView("PartialView/_MonthlyAttSheetReport", model) : PartialView("_404");
    }

    #endregion

    #region MonthlyManualAttendanceSheetReport

    [HttpPost]
    public async Task<PartialViewResult> GetMonthlyManualAttSheetReportPartial(MonthlyAttSheetMstVm modelVm)
    {
        var model = await _iEmpAttendanceService.GetMonthlyManualAttendanceData(modelVm);
        return model != null ? PartialView("PartialView/_MonthlyManualAttSheetReport", model) : PartialView("_404");
    }

    #endregion

    #region MonthlyAttendanceSheetPrint

    //public async Task<ActionResult> PrintMonthlyAttSheet(short year, short month)
    //{
    //    var modelVm = new MonthlyAttSheetMstVm();
    //    modelVm.Year = year;
    //    modelVm.Month = month;
    //    var model = await _iEmpAttendanceService.GetMonthlyAttendanceData(modelVm);
    //    MonthlyAttReport rpt = new MonthlyAttReport(_iWebHostEnvironment);
    //    return File(rpt.Report(model), "application/pdf");
    //}

    #endregion

    #region MonthlyAttendanceReportPrint
    public async Task<ActionResult> MonthlyAttendanceReportPrint(long id)
    {
        var model = await _iService.GetSheetByIdAsync(id);

        string html = await _iService.MonthlyAttendanceReportHtml(id);

        ExportToPDF export = new ExportToPDF(_ihttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Monthly Attendance Sheet";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");

        string reportName = "Monthly Attendance Sheet_" + DateTime.Now.ToString("dd/MM/yyyy");


        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 50, top: 80, left: 10, right: 10, isLandScape: true), "application/pdf");

    }
    #endregion
}
