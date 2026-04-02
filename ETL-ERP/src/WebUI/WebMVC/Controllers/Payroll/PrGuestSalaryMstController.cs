using AutoMapper;
using Domain.Entities.Payroll;
using Domain.Utility.Common;
using Domain.ViewModel.Payroll.PrGuestSalary;
using Interface.Services.Payroll;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using Utility.Export;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using DU = Domain.Utility;

namespace WebMVC.Controllers.Payroll;

public class PrGuestSalaryMstController : AppBaseController
{
    #region Config

    private readonly IUnitOfWork _iUnitOfWork;
    private readonly IPrGuestSalaryMstService _iService;
    private readonly IMapper _iMapper;
    private readonly DropdownService _dropdownService;
    private readonly IHttpContextAccessor _ihttpContextAccessor;

    public PrGuestSalaryMstController(IUnitOfWork iUnitOfWork, IMapper iMapper, DropdownService dropdownService, IPrGuestSalaryMstService iService, IHttpContextAccessor ihttpContextAccessor) : base(iUnitOfWork)
    {
        _iUnitOfWork = iUnitOfWork;
        _iMapper = iMapper;
        _dropdownService = dropdownService;
        _iService = iService;
        _ihttpContextAccessor = ihttpContextAccessor;
    }
    #endregion

    #region Create

    [HttpGet]
    [Authorize(Permissions.PrGuestSalaryMsts.Create)]
    public ActionResult Create()
    {
        var model = new PrGuestSalaryMstVm();
        model.EmployeeLookup = _dropdownService.GetGuestEmployeeSelectListItems();
        model.YearLookup = _dropdownService.GetYearSelectListItems();
        model.MonthLookup = _dropdownService.GetMonthSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> Create(PrGuestSalaryMstVm modelVm)
    {
        modelVm.EmployeeLookup = _dropdownService.GetGuestEmployeeSelectListItems();
        modelVm.YearLookup = _dropdownService.GetYearSelectListItems();
        modelVm.MonthLookup = _dropdownService.GetMonthSelectListItems();
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return View("Report", modelVm);
            }


            _iService.CurrentUserId = UserId;
            var model = _iMapper.Map<PrGuestSalaryMst>(modelVm);
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
            return View("Create", modelVm);
        }
    }

    #endregion

    #region Search

    [HttpGet]
    [Authorize(Permissions.PrGuestSalaryMsts.ListView)]
    public IActionResult Search()
    {
        var vm = new PrGuestSalaryMstSearchVm();
        return View(vm);
    }


    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<PrGuestSalaryMstSearchVm, PrGuestSalaryMstSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<PrGuestSalaryMstSearchVm, PrGuestSalaryMstSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new PrGuestSalaryMstSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion

    #region Details

    [Authorize(Permissions.PrGuestSalaryMsts.DetailsView)]
    public async Task<ActionResult> Details(long id)
    {
        try
        {
            var model = await _iService.GetGuestSalaryMstByIdAsync(id);
            model.EmployeeLookup = _dropdownService.GetGuestEmployeeSelectListItems();
            model.YearLookup = _dropdownService.GetYearSelectListItems();
            model.MonthLookup = _dropdownService.GetMonthSelectListItems();
            return View(model);
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("_404");
        }
    }

    #endregion

    #region ApproveGuestSalary

    [Authorize(Permissions.PrGuestSalaryMsts.Approve)]

    public async Task<ActionResult> ApproveGuestSalaryMst(long id)
    {
        var data = _iService.GetById(id);
        if (data == null)
        {
            FailedMsg("No Data Found !!");
            return RedirectToAction("Details", new { id });
        }
        data.IsApproved = true;
        data.ApprovedById = UserId;

        var result = await _iService.UpdateAsync(data);
        if (!result)
        {
            FailedMsg("Failed Approve Guest Salary");
            return RedirectToAction("Details", new { id });
        }

        SaveSuccessMsg("Successfully Approve Guest Salary");
        return RedirectToAction("Details", new { id });
    }

    #endregion

    #region GuestSalaryReportPrint
    public async Task<ActionResult> GuestSalaryReportPrint(long id)
    {

        var model = await _iService.GetGuestSalaryMstByIdAsync(id);

        string html = await _iService.GuestSalaryDetailHtml(id);

        ExportToPDF export = new ExportToPDF(_ihttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = "Guest Salary Report";
        reportTitle.ReportTitle = "";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");

        string reportName = "Salary Sheet_" + DateTime.Now.ToString("dd/MM/yyyy");


        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 50, top: 80, left: 10, right: 10, isLandScape: false), "application/pdf");

    }
    #endregion
}
