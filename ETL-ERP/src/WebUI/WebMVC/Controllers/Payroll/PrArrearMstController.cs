using AutoMapper;
using Domain.Entities.Payroll;
using Domain.Utility.Common;
using Domain.ViewModel.Payroll.PrArrearMst;
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

public class PrArrearMstController : AppBaseController
{
    #region Config

    private readonly IUnitOfWork _iUnitWork;
    private readonly IPrArrearMstService _iService;
    private readonly IMapper _iMapper;
    private readonly DropdownService _dropdownService;
    private readonly IHttpContextAccessor _ihttpContextAccessor;

    public PrArrearMstController(IUnitOfWork iUnitOfWork, IMapper iMapper, DropdownService dropdownService, IPrArrearMstService iService, IHttpContextAccessor ihttpContextAccessor) : base(iUnitOfWork)
    {
        _iUnitWork = iUnitOfWork;
        _iMapper = iMapper;
        _dropdownService = dropdownService;
        _iService = iService;
        _ihttpContextAccessor = ihttpContextAccessor;
    }
    #endregion

    #region Create

    [HttpGet]
    [Authorize(Permissions.PrArrearMsts.Create)]
    public ActionResult Create()
    {
        var model = new PrArrearMstVm();
        model.EmployeeLookup = _dropdownService.GetEmployeeSelectListItems();
        model.YearLookup = _dropdownService.GetYearSelectListItems();
        model.MonthLookup = _dropdownService.GetMonthSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> Create(PrArrearMstVm modelVm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return View("Report", modelVm);
            }


            _iService.CurrentUserId = UserId;
            var model = _iMapper.Map<PrArrearMst>(modelVm);

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
    [Authorize(Permissions.PrArrearMsts.ListView)]
    public IActionResult Search()
    {
        var vm = new PrArrearMstSearchVm();
        return View(vm);
    }


    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<PrArrearMstSearchVm, PrArrearMstSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<PrArrearMstSearchVm, PrArrearMstSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new PrArrearMstSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion

    #region Details

    [Authorize(Permissions.PrArrearMsts.DetailsView)]

    public async Task<ActionResult> Details(long id)
    {
        try
        {
            var model = await _iService.GetPrArrearByIdAsync(id);
            return View(model);
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("_404");
        }
    }

    #endregion

    #region Delete

    [HttpGet]
    [Authorize(Permissions.PrArrearMsts.Delete)]
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
        return RedirectToAction("Search");
    }

    #endregion

    #region ApproveArrear

    [Authorize(Permissions.PrArrearMsts.Approve)]

    public async Task<ActionResult> ApproveArrear(long id)
    {
        var data = _iService.GetById(id);
        if (data == null)
        {
            FailedMsg("No Data Found !!");
            return RedirectToAction("Details", new { id });
        }
        data.IsApproved = true;
        data.ApprovedById = UserId;
        data.ApprovedDate = DU.Utility.GetBdDateTimeNow();

        var result = await _iService.UpdateAsync(data);
        if (!result)
        {
            FailedMsg("Failed Approve Arrear");
            return RedirectToAction("Details", new { id });
        }

        SaveSuccessMsg("Successfully Approve Arrear");
        return RedirectToAction("Details", new { id });
    }

    #endregion

    #region ArrearDetailsReportPrint

    public async Task<ActionResult> ArrearDetailsReportPrint(long id)
    {
        string html = await _iService.ArrearDetailsReportHtml(id);

        ExportToPDF export = new ExportToPDF(_ihttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Arrear Details Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");

        string reportName = "Arrear Details_" + DateTime.Now.ToString("dd/MM/yyyy");


        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 50, top: 80, left: 10, right: 10, isLandScape: false), "application/pdf");

    }

    #endregion
}
