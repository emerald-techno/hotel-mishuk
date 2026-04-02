using AutoMapper;
using Domain.Entities.Payroll;
using Domain.Utility.Common;
using Domain.ViewModel.Payroll.PrSalaryMst;
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

public class PrSalaryMstController : AppBaseController
{
    #region Config

    private readonly IUnitOfWork _iUnitWork;
    private readonly IMapper _iMapper;
    private readonly IPrSalaryMstService _iService;
    private readonly IPrSalaryPartService _iPrSalaryPartService;
    private readonly IPrSalaryDtlService _iPrSalaryDtlService;
    private readonly DropdownService _dropdownService;
    private readonly IHttpContextAccessor _ihttpContextAccessor;

    public PrSalaryMstController(IPrSalaryMstService iService,
                            IPrSalaryPartService iPrSalaryPartService,
                            IMapper iMapper,
                            IUnitOfWork iUnitOfWork,
                            IPrSalaryDtlService iPrSalaryDtlService,
                            IHttpContextAccessor ihttpContextAccessor,
    DropdownService dropdownService) : base(iUnitOfWork)
    {
        _iService = iService;
        _iPrSalaryPartService = iPrSalaryPartService;
        _iMapper = iMapper;
        _iUnitWork = iUnitOfWork;
        _dropdownService = dropdownService;
        _iPrSalaryDtlService = iPrSalaryDtlService;
        _ihttpContextAccessor = ihttpContextAccessor;
    }

    #endregion

    #region Payroll

    [HttpGet]
    [Authorize(Permissions.PrSalaryMsts.Payroll)]
    public ActionResult Payroll()
    {
        return View();
    }

    [HttpPost]
    public async Task<PartialViewResult> GetPayrollPartial(PrSalaryMstVm modelVm)
    {
        try
        {
            var model = await _iService.CalculatePayroll(modelVm);
            return model != null ? PartialView("PartialView/_EmployeePayroll", model) : PartialView("_404");
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return PartialView("_404");
        }
    }

    #endregion

    #region SetPayroll

    [Authorize(Permissions.PrSalaryMsts.SetPayroll)]
    public async Task<IActionResult> IsPayrollExist(short year, short month)
    {
        if (year > 1999 && month > 0)
        {
            var existPayroll = await _iService.GetFirstOrDefaultAsync(c => c.Year == year && c.Month == month);
            if (existPayroll != null) return Ok(true);
            return Ok(false);
        }
        return BadRequest();
    }

    [HttpPost]
    [Authorize(Permissions.PrSalaryMsts.SetPayroll)]
    public async Task<ActionResult> SetPayroll(PrSalaryMstVm modelVm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return RedirectToAction("Payroll");
            }

            if (modelVm.Year > 1999 && modelVm.Month > 0)
            {
                var exist = _iService.GetFirstOrDefault(c => c.Year == modelVm.Year && c.Month == modelVm.Month, d => d.PrSalaryDtls);
                if (exist != null && exist.PrSalaryDtls.Count > 0) _iPrSalaryDtlService.RemoveRange(exist.PrSalaryDtls);
                if (exist != null) _iService.Remove(exist);
            }

            _iService.CurrentUserId = UserId;
            var model = _iMapper.Map<PrSalaryMst>(modelVm);

            model.SalaryDate = (DateTime)(!string.IsNullOrEmpty(modelVm.SalaryDateStr) ? DU.Utility.ConvertStrToDate(modelVm.SalaryDateStr) : modelVm.SalaryDate);
            model.TotalEmployee = (short)model.PrSalaryDtls.Count();
            model.TotalSalary = model.PrSalaryDtls.Sum(c => c.NetSalary);
            model.ActionById = UserId;
            model.ActionDate = DU.Utility.GetBdDateTimeNow();

            if (model.PrSalaryDtls.Count > 0)
            {
                foreach (var item in model.PrSalaryDtls)
                {
                    item.ActionById = UserId;
                    item.ActionDate = DU.Utility.GetBdDateTimeNow();
                }
            }

            var isAdded = await _iService.AddAsync(model);

            if (!isAdded)
            {
                SaveFailedMsg();
                return RedirectToAction("Payroll");
            }

            SaveSuccessMsg();
            return RedirectToAction("Payroll");
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return RedirectToAction("Payroll");
        }
    }

    #endregion

    #region Search

    [HttpGet]
    [Authorize(Permissions.PrSalaryMsts.ListView)]
    public IActionResult Search()
    {
        var vm = new PrSalaryMstSearchVm();
        return View(vm);
    }


    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<PrSalaryMstSearchVm, PrSalaryMstSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<PrSalaryMstSearchVm, PrSalaryMstSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new PrSalaryMstSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion

    #region Details

    [Authorize(Permissions.PrSalaryMsts.DetailsView)]
    public async Task<ActionResult> Details(long id)
    {
        try
        {
            var model = await _iService.GetPrSalaryByIdAsync(id);
            return View(model);
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("_404");
        }
    }

    #endregion

    #region ApprovePayroll

    [Authorize(Permissions.PrSalaryMsts.Approve)]

    public async Task<ActionResult> ApprovePayroll(long id, string remarks)
    {
        try
        {
            _iService.CurrentUserId = UserId;
            var result = await _iService.ApprovePrSalary(id, remarks);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }

    #endregion

    #region RejectPayroll

    [Authorize(Permissions.PrSalaryMsts.Approve)]

    public async Task<ActionResult> RejectPayroll(long id)
    {
        try
        {
            if (id == 0) throw new Exception("Data Not Found... !");
            var exist = _iService.GetFirstOrDefault(c => c.Id == id, d => d.PrSalaryDtls);
            if (exist != null && exist.PrSalaryDtls.Count > 0) _iPrSalaryDtlService.RemoveRange(exist.PrSalaryDtls);
            if (exist != null) _iService.Remove(exist);
            var result = await _iUnitWork.CompleteAsync();
            return View("Payroll");
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("Details", new { id });
        }

    }

    #endregion

    #region Payslip

    [Authorize(Permissions.PrSalaryMsts.Payslip)]

    public async Task<ActionResult> Payslip(long id)
    {
        try
        {
            var model = await _iPrSalaryDtlService.GeneratePayslip(id);
            return View(model);
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("_404");
        }
    }

    #endregion

    #region PayPayslip

    [Authorize(Permissions.PrSalaryMsts.PayMultiSalary)]

    public async Task<ActionResult> PayMultiSalary(List<long> ids)
    {
        try
        {
            if (ids == null && ids.Count == 0) return BadRequest();
            var updateList = new List<PrSalaryDtl>();

            if (ids != null && ids.Count > 0)
            {
                foreach (var id in ids)
                {
                    var data = _iPrSalaryDtlService.GetFirstOrDefault(c => c.Id == id, p => p.SalaryMst);

                    if (data == null)
                    {
                        return BadRequest("One or more data not found");
                    }

                    data.IsPaid = true;
                    data.PaidById = UserId;
                    data.PaidDate = DU.Utility.GetBdDateTimeNow();

                    updateList.Add(data);
                }
            }
            _iPrSalaryDtlService.CurrentUserId = UserId;
            var result = await _iPrSalaryDtlService.PayMultiSalary(updateList);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(Permissions.PrSalaryMsts.PayPayslip)]

    public async Task<ActionResult> PayPayslip(long id)
    {
        var data = _iPrSalaryDtlService.GetById(id);
        if (data == null)
        {
            FailedMsg("No Data Found !!");
            return RedirectToAction("Payslip", new { id });
        }
        data.IsPaid = true;
        data.PaidById = UserId;
        data.PaidDate = DU.Utility.GetBdDateTimeNow();

        var result = await _iPrSalaryDtlService.UpdateAsync(data);
        if (!result)
        {
            FailedMsg("Failed Paid Salary");
            return RedirectToAction("Payslip", new { id });
        }

        SaveSuccessMsg("Successfully Paid Salary");
        return RedirectToAction("Payslip", new { id });
    }

    #endregion

    #region PrintPayroll

    public async Task<ActionResult> PrintPayroll(long id)
    {
        string html = await _iService.PayrollHtml(id);

        ExportToPDF export = new ExportToPDF(_ihttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "SALARY OF HOTEL MISHUK STAFFS";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Generated By");
        reportTitle.SignatureList.Add("Accounts Manager");
        reportTitle.IsSignature = false;


        string reportName = "Salary Sheet_" + DateTime.Now.ToString("dd/MM/yyyy");


        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 50, top: 80, left: 10, right: 10, isLandScape: true), "application/pdf");

    }

    #endregion

    #region PrintBankSheet

    public async Task<ActionResult> PrintBankSheet(long id)
    {
        string html = await _iService.PayrollBankHtml(id);

        ExportToPDF export = new ExportToPDF(_ihttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "SALARY SHEET";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Generated By");
        reportTitle.SignatureList.Add("Accounts Manager");
        reportTitle.IsSignature = false;

        string reportName = "Salary Sheet_" + DateTime.Now.ToString("dd/MM/yyyy");

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 50, top: 80, left: 10, right: 10, isLandScape: false), "application/pdf");

    }

    #endregion

    #region PrintPayslip

    public async Task<ActionResult> PrintPayslip(long id)
    {
        string html = await _iPrSalaryDtlService.PayslipHtml(id);

        ExportToPDF export = new ExportToPDF(_ihttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = @$"Pay Slip";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Generated By");
        reportTitle.SignatureList.Add("Manager");
        reportTitle.SignatureList.Add("Employee");
        reportTitle.IsSignature = true;

        string reportName = "Payslip";


        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 50, top: 80, left: 10, right: 10, isLandScape: false), "application/pdf");

    }

    #endregion

    #region PayrollReport

    public ActionResult PayrollReport()
    {
        var model = new PrSalaryMstVm();

        var today = DateTime.Today;
        model.DepartmentLookup = _dropdownService.GetDepartmentSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> PayrollReport(PrSalaryMstVm model)
    {
        var data = await _iService.DepartmentWisePayrollHtml(model.Year, model.Month, model.DepartmentId);
        return Ok(data);
    }

    public async Task<ActionResult> PayrollReportPrint(int year, int month, long? departmentId)
    {
        string html = await _iService.DepartmentWisePayrollHtml(year, month, departmentId, true);

        ExportToPDF export = new ExportToPDF(_ihttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "SALARY OF HOTEL MISHUK STAFFS";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Manager F&B & HR");
        reportTitle.SignatureList.Add("Manager Accounts");
        reportTitle.SignatureList.Add("A.G.M");
        reportTitle.SignatureList.Add("General Manager");
        reportTitle.SignatureList.Add("Managing Director");
        reportTitle.IsSignature = true;

        string reportName = "Employee Payroll Report";

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 120, top: 80, left: 18, right: 18, isLandScape: true), "application/pdf");
    }

    #endregion
}
