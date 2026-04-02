using AutoMapper;
using Domain.Enums.AppEnums;
using Domain.Utility.Common;
using Domain.ViewModel.Pf.EmpLoan;
using Interface.Services.Pf;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using Utility.Export;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using DU = Domain.Utility;

namespace WebMVC.Controllers.Pf;

public class EmpLoanMstController : AppBaseController
{
    #region Config

    private readonly IUnitOfWork _iUnitOfWork;
    private readonly IMapper _iMapper;
    private readonly IEmpLoanMstService _iService;
    private readonly IEmpLoanDtlService _iDtlService;
    private readonly DropdownService _dropdownService;
    private readonly IHttpContextAccessor _ihttpContextAccessor;
    public EmpLoanMstController(IUnitOfWork iUnitOfWork, IMapper iMapper, IEmpLoanMstService iService, DropdownService dropdownService, IEmpLoanDtlService iDtlService, IHttpContextAccessor ihttpContextAccessor) : base(iUnitOfWork)
    {
        _iUnitOfWork = iUnitOfWork;
        _iMapper = iMapper;
        _iService = iService;
        _dropdownService = dropdownService;
        _iDtlService = iDtlService;
        _ihttpContextAccessor = ihttpContextAccessor;
    }

    #endregion

    #region Create

    [HttpGet]
    [Authorize(Permissions.EmpLoanMsts.Create)]
    public ActionResult Create()
    {
        var model = new EmpLoanMstVm();
        model.EmployeeLookup = _dropdownService.GetEmployeeSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> Create(EmpLoanMstVm modelVm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return View("Report", modelVm);
            }

            _iService.CurrentUserId = UserId;
            var isAdded = await _iService.AddAsync(modelVm);

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
    [Authorize(Permissions.EmpLoanMsts.ListView)]
    public IActionResult Search()
    {
        var vm = new EmpLoanMstSearchVm();
        return View(vm);
    }


    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<EmpLoanMstSearchVm, EmpLoanMstSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<EmpLoanMstSearchVm, EmpLoanMstSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new EmpLoanMstSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion

    #region Details

    [Authorize(Permissions.EmpLoanMsts.DetailsView)]
    public async Task<ActionResult> Details(long id)
    {
        try
        {
            var data = await _iService.GetFirstOrDefaultAsync(c => c.Id == id, e => e.Employee, c => c.EmpLoanDtls);
            var model = _iMapper.Map<EmpLoanMstVm>(data);
            model.PaidDateStr = DateTime.Now.ToString("dd/MM/yyyy");
            return View(model);
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("_404");
        }
    }

    #endregion

    #region AdvanceLoan

    public async Task<ActionResult> AdvanceLoan(long id, double advAmount)
    {
        try
        {
            _iService.CurrentUserId = UserId;
            var result = await _iService.AdvanceLoanCalculation(id, advAmount);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }

    #endregion

    #region LoanHeld

    [Authorize(Permissions.EmpLoanMsts.Create)]
    public async Task<ActionResult> LoanHeld(long id)
    {
        var data = _iService.GetById(id);
        if (data == null)
        {
            FailedMsg("No Data Found !!");
            return RedirectToAction("Details", new { id });
        }
        data.Status = (short)LoanStatusEnum.Held;
        var result = await _iService.UpdateAsync(data);
        if (!result)
        {
            FailedMsg("Failed To Held Loan");
            return RedirectToAction("Details", new { id });
        }

        SaveSuccessMsg("Successfully Loan Held");
        return RedirectToAction("Details", new { id });
    }

    #endregion

    #region LoanStart

    [Authorize(Permissions.EmpLoanMsts.Create)]
    public async Task<ActionResult> LoanStart(long id)
    {
        var data = _iService.GetById(id);
        if (data == null)
        {
            FailedMsg("No Data Found !!");
            return RedirectToAction("Details", new { id });
        }
        data.Status = (short)LoanStatusEnum.Running;
        var result = await _iService.UpdateAsync(data);
        if (!result)
        {
            FailedMsg("Failed To Start Loan");
            return RedirectToAction("Details", new { id });
        }

        SaveSuccessMsg("Successfully Loan Started");
        return RedirectToAction("Details", new { id });
    }

    #endregion

    #region MarkAsPaid

    public async Task<ActionResult> MarkAsPaid(long id)
    {
        var data = _iDtlService.GetById(id);
        if (data == null)
        {
            FailedMsg("No Data Found !!");
            return RedirectToAction("Details", new { id = data.LoanId });
        }
        data.IsPaid = true;
        data.PaidDate = DateTime.Now;
        data.PaidSetById = UserId;
        var result = await _iDtlService.UpdateAsync(data);
        if (!result)
        {
            FailedMsg("Failed Mark Paid");
            return RedirectToAction("Details", new { id = data.LoanId });
        }

        SaveSuccessMsg("Successfully Mark As Paid");
        return RedirectToAction("Details", new { id = data.LoanId });
    }

    public async Task<ActionResult> PaidWithWaiver(InstalmentPaidVm vm)
    {
        try
        {
            _iService.CurrentUserId = UserId;
            var result = await _iService.LoanPaid(vm);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    #endregion

    #region EmpLoanDetailsReportPrint

    public async Task<ActionResult> EmpLoanDetailsReportPrint(long id)
    {
        string html = await _iService.EmpLoanDetailsReportHtml(id);

        ExportToPDF export = new ExportToPDF(_ihttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Employee Loan Details Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");

        string reportName = "Employee Loan Details_" + DateTime.Now.ToString("dd/MM/yyyy");


        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 50, top: 80, left: 10, right: 10, isLandScape: false), "application/pdf");

    }

    #endregion

    #region EmployeeLoanReport

    public ActionResult EmployeeLoanReport()
    {
        var model = new EmployeeLoanReportVm();
        model.StrQueryDate = DateTime.Today.ToString("dd/MM/yyyy");
        
        model.EmployeeLookUp = _dropdownService.GetEmployeeSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> EmployeeLoanReport(EmployeeLoanReportVm model)
    {
        model.EmployeeLookUp = _dropdownService.GetEmployeeSelectListItems();
        model.StrQueryDate = DateTime.Today.ToString("dd/MM/yyyy");
        var data = await _iService.EmployeeLoanReportHtml(model);
        return Ok(data);
    }

    public async Task<ActionResult> EmployeeLoanReportPrint(string strFromDate, string strToDate, long employeeId = 0)
    {
        var model = new EmployeeLoanReportVm();
        model.EmployeeId = employeeId;
        model.StrFromDate = strFromDate;
        model.StrToDate = strToDate;
        model.StrQueryDate = DateTime.Today.ToString("dd/MM/yyyy");

        string html = await _iService.EmployeeLoanReportHtml(model);

        ExportToPDF export = new ExportToPDF(_ihttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Employee Loan Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Employee Loan Report";

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 120, top: 80, left: 18, right: 18, isLandScape: true), "application/pdf");
    }

    #endregion
}
