using AutoMapper;
using Domain.Utility.Common;
using Domain.ViewModel.Accounting.AccTranDtl;
using Domain.ViewModel.Accounting.AccTranMst;
using Domain.ViewModel.Admin.FinancialYear;
using Interface.Services.Accounts;
using Interface.Services.Admin;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using Utility.CachingUtility;
using Utility.Export;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using DU = Domain.Utility;

namespace WebMVC.Controllers.Accounts;

public class AccTranMstController : AppBaseController
{
    #region Config

    private readonly IUnitOfWork _iUnitWork;
    private readonly IMapper _iMapper;
    private readonly IAccTranMstService _iService;
    private readonly IAccTranDtlService _iAccTranDtlService;
    private readonly ISetFincYearService _iFincYearService;
    private readonly ISetCurrencyService _iSetCurrencyService;
    private readonly IAccHeadService _iAccHeadService;
    private readonly DropdownService _dropdownService;
    private readonly IWebHostEnvironment _iWebHostEnvironment;
    private readonly IHttpContextAccessor _ihttpContextAccessor;
    private readonly IAccLedgerService _iAccLedgerService;

    public AccTranMstController(IAccTranMstService iService,
                            IMapper iMapper,
                            IUnitOfWork iUnitOfWork,
                            DropdownService dropdownService,
                            ISetFincYearService iFincYearService,
                            ISetCurrencyService iSetCurrencyService,
                            IAccHeadService iAccHeadService,
                            IWebHostEnvironment iWebHostEnvironment,
                            IAccTranDtlService iAccTranDtlService,
                            IHttpContextAccessor ihttpContextAccessor,
                            IAccLedgerService iAccLedgerService) : base(iUnitOfWork)
    {
        _iService = iService;
        _iMapper = iMapper;
        _iUnitWork = iUnitOfWork;
        _dropdownService = dropdownService;
        _iFincYearService = iFincYearService;
        _iSetCurrencyService = iSetCurrencyService;
        _iAccHeadService = iAccHeadService;
        _iWebHostEnvironment = iWebHostEnvironment;
        _ihttpContextAccessor = ihttpContextAccessor;
        _iAccTranDtlService = iAccTranDtlService;
        _iAccLedgerService = iAccLedgerService;
    }

    #endregion

    #region JournalCreate

    [HttpGet]
    [Authorize(Permissions.AccTranMsts.JournalCreate)]
    public async Task<IActionResult> JournalCreate()
    {
        var model = new AccTranMstVm();
        model.VcDateStr = DateTime.Now.ToString("dd/MM/yyyy");
        model.VcNo = await _iService.GetVoucherAutoCode(VoucherTypeCode.JournalVoucher, DateTime.Now);
        model.CurrencyId = _iSetCurrencyService.GetFirstOrDefault(c => !c.IsDeleted).Id;
        model.CurrencyLookup = _dropdownService.GetCurrencySelectListItems();
        model.FinYearLookup = _dropdownService.GetFinYearSelectListItems();
        model.LedgerLookup = _dropdownService.GetAccLedgerSelectListItems();
        model.AccountLookup = await _iAccLedgerService.GetMishukLedgerSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> JournalCreate(AccTranMstVm modelVm)
    {
        modelVm.LedgerLookup = _dropdownService.GetAccLedgerSelectListItems();
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information Is Not Correct");
                return View("JournalCreate", modelVm);
            }
            _iService.CurrentUserId = UserId;

            var isAddedId = await _iService.AddAccTran(modelVm);
            if (isAddedId == 0)
            {
                SaveFailedMsg();
                return View("JournalCreate", modelVm);
            }
            SaveSuccessMsg();

            return RedirectToAction("JournalDetails", new { id = isAddedId });
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("JournalCreate", modelVm);
        }
    }

    #endregion

    #region AccOpeningCreate

    [HttpGet]
    [Authorize(Permissions.AccTranMsts.AccOpeningCreate)]
    public async Task<IActionResult> AccOpeningCreate()
    {
        var model = new AccTranMstVm();
        model.VcDateStr = DateTime.Now.ToString("dd/MM/yyyy");
        model.VcNo = await _iService.GetVoucherAutoCode(VoucherTypeCode.OpeningVoucher, DateTime.Now);
        model.VcType = VoucherType.OpeningVoucher;
        model.SubVacType = VoucherType.OpeningVoucher;
        model.CurrencyId = _iSetCurrencyService.GetFirstOrDefault(c => !c.IsDeleted).Id;
        model.CurrencyLookup = _dropdownService.GetCurrencySelectListItems();
        model.FinYearLookup = _dropdownService.GetFinYearSelectListItems();
        model.LedgerLookup = _dropdownService.GetAccLedgerSelectListItems();
        model.AccountLookup = await _iAccLedgerService.GetMishukLedgerSelectListItems();
        return View(model);
    }

    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
               SearchOpening(DataTablePagination<AccTranDtlSearchVm, AccTranDtlSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<AccTranDtlSearchVm, AccTranDtlSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new AccTranDtlSearchVm();
        var dataTable = await _iAccTranDtlService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    [HttpPost]
    public async Task<IActionResult> AccOpeningCreate(AccTranMstVm modelVm)
    {
        modelVm.LedgerLookup = _dropdownService.GetAccLedgerSelectListItems();
        modelVm.CurrencyLookup = _dropdownService.GetCurrencySelectListItems();
        modelVm.FinYearLookup = _dropdownService.GetFinYearSelectListItems();
        modelVm.AccountLookup = await _iAccLedgerService.GetMishukLedgerSelectListItems();

        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information Is Not Correct");
                return View("AccOpeningCreate", modelVm);
            }
            _iService.CurrentUserId = UserId;

            long isAddOrUpdate = 0;

            var finYear = await _iFincYearService.GetFirstOrDefaultAsync(x => x.Id == modelVm.FinYearId && !x.IsDeleted);
            if (finYear == null)
            {
                SaveFailedMsg("Financial year not found...!!");
                return View("AccOpeningCreate", modelVm);
            }

            if (finYear.IsActive == false)
            {
                SaveFailedMsg("Financial year is lock...!!");
                return View("AccOpeningCreate", modelVm);
            }

            if (modelVm.IsOpenignUpdate)
            {
                isAddOrUpdate = await _iService.UpdateOpeningTransaction(modelVm);
            }
            else
            {
                isAddOrUpdate = await _iService.AddAccTran(modelVm);
            }

            if (isAddOrUpdate == 0)
            {
                SaveFailedMsg();
                return View("AccOpeningCreate", modelVm);
            }
            SaveSuccessMsg();

            return RedirectToAction("AccOpeningCreate");
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("AccOpeningCreate", modelVm);
        }
    }

    #endregion

    #region BankDebitVoucher

    [HttpGet]
    [Authorize(Permissions.AccTranMsts.BankDebitVoucher)]
    public async Task<IActionResult> BankDebitVoucher()
    {
        var model = new AccTranMstVm();
        model.VcDateStr = DateTime.Now.ToString("dd/MM/yyyy");
        model.VcNo = await _iService.GetVoucherAutoCode(VoucherTypeCode.BankDebitVoucher, DateTime.Now);
        model.CurrencyId = _iSetCurrencyService.GetFirstOrDefault(c => !c.IsDeleted).Id;
        model.CurrencyLookup = _dropdownService.GetCurrencySelectListItems();
        model.FinYearLookup = _dropdownService.GetFinYearSelectListItems();
        model.LedgerLookup = _dropdownService.GetAccLedgerSelectListItems();
        model.AccountLookup = await _iAccLedgerService.GetMishukLedgerSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> BankDebitVoucher(AccTranMstVm modelVm)
    {
        modelVm.LedgerLookup = _dropdownService.GetAccLedgerSelectListItems();
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information Is Not Correct");
                return View("BankDebitVoucher", modelVm);
            }
            _iService.CurrentUserId = UserId;
            var isAdded = await _iService.AddAccTran(modelVm);
            if (isAdded == 0)
            {
                SaveFailedMsg();
                return View("BankDebitVoucher", modelVm);
            }
            SaveSuccessMsg();

            return RedirectToAction("BankDebitVoucher");
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("BankDebitVoucher", modelVm);
        }
    }
    #endregion

    #region BankCreditVoucher

    [HttpGet]
    [Authorize(Permissions.AccTranMsts.BankCreditVoucher)]
    public async Task<IActionResult> BankCreditVoucher()
    {
        var model = new AccTranMstVm();
        model.VcDateStr = DateTime.Now.ToString("dd/MM/yyyy");
        model.VcNo = await _iService.GetVoucherAutoCode(VoucherTypeCode.BankCreditVoucher, DateTime.Now);
        model.CurrencyId = _iSetCurrencyService.GetFirstOrDefault(c => !c.IsDeleted).Id;
        model.CurrencyLookup = _dropdownService.GetCurrencySelectListItems();
        model.FinYearLookup = _dropdownService.GetFinYearSelectListItems();
        model.LedgerLookup = _dropdownService.GetAccLedgerSelectListItems();
        model.AccountLookup = await _iAccLedgerService.GetMishukLedgerSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> BankCreditVoucher(AccTranMstVm modelVm)
    {
        modelVm.LedgerLookup = _dropdownService.GetAccLedgerSelectListItems();
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information Is Not Correct");
                return View("BankCreditVoucher", modelVm);
            }
            _iService.CurrentUserId = UserId;
            var isAdded = await _iService.AddAccTran(modelVm);
            if (isAdded == 0)
            {
                SaveFailedMsg();
                return View("BankCreditVoucher", modelVm);
            }
            SaveSuccessMsg();

            return RedirectToAction("BankCreditVoucher");
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("BankCreditVoucher", modelVm);
        }
    }
    #endregion

    #region CashDebitVoucher

    [HttpGet]
    [Authorize(Permissions.AccTranMsts.CashDebitVoucher)]
    public async Task<IActionResult> CashDebitVoucher()
    {
        var model = new AccTranMstVm();
        model.VcDateStr = DateTime.Now.ToString("dd/MM/yyyy");
        model.VcNo = await _iService.GetVoucherAutoCode(VoucherTypeCode.CashDebitVoucher, DateTime.Now);
        model.CurrencyId = _iSetCurrencyService.GetFirstOrDefault(c => !c.IsDeleted).Id;
        model.CurrencyLookup = _dropdownService.GetCurrencySelectListItems();
        model.FinYearLookup = _dropdownService.GetFinYearSelectListItems();
        model.LedgerLookup = _dropdownService.GetAccLedgerSelectListItems();
        model.AccountLookup = await _iAccLedgerService.GetMishukLedgerSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> CashDebitVoucher(AccTranMstVm modelVm)
    {
        modelVm.LedgerLookup = _dropdownService.GetAccLedgerSelectListItems();
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information Is Not Correct");
                return View("CashDebitVoucher", modelVm);
            }
            _iService.CurrentUserId = UserId;
            var isAdded = await _iService.AddAccTran(modelVm);
            if (isAdded == 0)
            {
                SaveFailedMsg();
                return View("CashDebitVoucher", modelVm);
            }
            SaveSuccessMsg();

            return RedirectToAction("CashDebitVoucher");
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("CashDebitVoucher", modelVm);
        }
    }
    #endregion

    #region CashCreditVoucher

    [HttpGet]
    [Authorize(Permissions.AccTranMsts.CashCreditVoucher)]
    public async Task<IActionResult> CashCreditVoucher()
    {
        var model = new AccTranMstVm();
        model.VcDateStr = DateTime.Now.ToString("dd/MM/yyyy");
        model.VcNo = await _iService.GetVoucherAutoCode(VoucherTypeCode.CashCreditVoucher, DateTime.Now);
        model.CurrencyId = _iSetCurrencyService.GetFirstOrDefault(c => !c.IsDeleted).Id;
        model.CurrencyLookup = _dropdownService.GetCurrencySelectListItems();
        model.FinYearLookup = _dropdownService.GetFinYearSelectListItems();
        model.LedgerLookup = _dropdownService.GetAccLedgerSelectListItems();
        model.AccountLookup = await _iAccLedgerService.GetMishukLedgerSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> CashCreditVoucher(AccTranMstVm modelVm)
    {
        modelVm.LedgerLookup = _dropdownService.GetAccLedgerSelectListItems();
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information Is Not Correct");
                return View("CashCreditVoucher", modelVm);
            }
            _iService.CurrentUserId = UserId;
            var isAdded = await _iService.AddAccTran(modelVm);
            if (isAdded == 0)
            {
                SaveFailedMsg();
                return View("CashCreditVoucher", modelVm);
            }
            SaveSuccessMsg();

            return RedirectToAction("CashCreditVoucher");
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("CashCreditVoucher", modelVm);
        }
    }
    #endregion

    #region Search

    [HttpGet]
    [Authorize(Permissions.AccTranMsts.ListView)]
    public IActionResult VoucherSearch()
    {
        var vm = new AccTranMstSearchVm();
        vm.FormDateStr = DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy");
        vm.ToDateStr = DateTime.Today.ToString("dd/MM/yyyy");

        vm.VcTypeLookUp = _dropdownService.GetVcTypeSelectListItems();
        return View(vm);
    }

    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<AccTranMstSearchVm, AccTranMstSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<AccTranMstSearchVm, AccTranMstSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new AccTranMstSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion

    #region Details

    [Authorize(Permissions.AccTranMsts.DetailsView)]
    public async Task<ActionResult> JournalDetails(long id)
    {
        try
        {
            var model = await _iService.GetJournalDataAsync(id);
            model.NoteTypeLookup = _dropdownService.GetVcNoteTypeSelectListItem();
            return View(model);
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("_404");
        }
    }

    #endregion

    #region LedgerReport

    [Authorize(Permissions.AccTranMsts.ReportView)]

    public async Task<ActionResult> LedgerReport()
    {
        var finYar = await _iFincYearService.GetFincYearByDate(DateTime.Now);
        var model = new AccReportVm();
        model.LedgerLookup = _dropdownService.GetAccLedgerSelectListItems();
        model.FincYearLookup = _dropdownService.GetFinYearSelectListItems();
        model.AccountLookup = await _iAccLedgerService.GetMishukLedgerSelectListItems();
        model.StrFromDate = finYar.YearStartDate.ToString("dd/MM/yyyy");
        model.StrToDate = finYar.YearEndDate.ToString("dd/MM/yyyy");
        model.FinYearId = finYar.Id;
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> LedgerReport(AccReportVm model)
    {
        var data = await _iService.GetLedgerReportHtml(model, false);
        return Ok(data);
    }

    public async Task<ActionResult> LedgerReportPrint(string fromDate, string toDate, long ledgerId, long finYearId, long mishukLedgerId)
    {
        var model = new AccReportVm();
        model.StrFromDate = fromDate;
        model.StrToDate = toDate;
        model.LedgerId = ledgerId;
        model.FinYearId = finYearId;
        model.MishukLedgerId = mishukLedgerId;

        string html = await _iService.GetLedgerReportHtml(model, true);

        ExportToPDF export = new ExportToPDF(_ihttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Ledger Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");

        string reportName = "Ladger_Report";


        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 50, top: 80, left: 10, right: 10, isLandScape: false), "application/pdf");
    }

    #endregion

    #region TrialBalance

    [Authorize(Permissions.AccTranMsts.ReportView)]

    public async Task<ActionResult> TrialBalance()
    {
        var model = new AccReportVm();
        var finYear = await _iFincYearService.GetFincYearByDate(DateTime.Today);
        model.FincYearLookup = _dropdownService.GetFinYearSelectListItems();
        model.StrFromDate = finYear.YearStartDate.ToString("dd/MM/yyyy");
        model.StrToDate = finYear.YearEndDate.ToString("dd/MM/yyyy");
        model.AccountLookup = await _iAccLedgerService.GetMishukLedgerSelectListItems();
        model.FinYearId = finYear.Id;
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> TrialBalance(AccReportVm model)
    {
        var data = await _iService.GetTrialBalanceHtml(model);
        return Ok(data);
    }

    public async Task<ActionResult> TrialBalancePrint(string fromDate, string toDate, long finYearId, long accountId = 0)
    {
        var model = new AccReportVm();
        model.StrFromDate = fromDate;
        model.StrToDate = toDate;
        model.FinYearId = finYearId;
        model.MishukLedgerId = accountId;

        string html = await _iService.GetTrialBalanceHtml(model);

        ExportToPDF export = new ExportToPDF(_ihttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Trial Balance";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = true;

        string reportName = "Trial_Balance";

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 50, top: 80, left: 10, right: 10, isLandScape: false), "application/pdf");
    }

    #endregion

    #region JsonData

    public async Task<IActionResult> GetFincYearByDate(string dateStr)
    {
        try
        {
            IFormatProvider culture = new CultureInfo("bn-BD", true);
            var convertedDate = DateTime.Parse(dateStr, culture, DateTimeStyles.AssumeLocal);

            var result = await _iFincYearService.GetFincYearByDate(convertedDate);
            return Ok(result);

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }

    public async Task<IActionResult> GetFincYearById(long fincYearId)
    {
        try
        {
            var result = await _iFincYearService.GetFinancialYearById(fincYearId);
            var model = _iMapper.Map<SetFincYearVm>(result);
            model.YearStartDateStr = result.YearStartDate.ToString("dd/MM/yyyy");
            model.YearEndDateStr = result.YearEndDate.ToString("dd/MM/yyyy");
            return Ok(model);

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }

    public async Task<IActionResult> GetOpenignDataByYear(long finYearId)
    {
        try
        {
            var result = await _iService.GetOpeningDataAsync(finYearId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }
    public async Task<IActionResult> GetClosingDataByYear(long finYearId)
    {
        try
        {
            var result = await _iService.GetClosingDataAsync(finYearId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }

    public async Task<IActionResult> GetQuickVoucherById(long vcId)
    {
        try
        {
            var result = await _iService.GetQuickVoucherByVcId(vcId);
            return Ok(result);

        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }

    #endregion

    #region voucherPrint
    public async Task<ActionResult> VoucherPrint(long id)
    {
        string html = await _iService.GetVoucherDetaliHtml(id);

        ExportToPDF export = new ExportToPDF(_ihttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");

        string reportName = "VoucherDetail";


        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 50, top: 80, left: 10, right: 10, isLandScape: false), "application/pdf");
    }
    #endregion

    #region ReceiptAndPayments

    [Authorize(Permissions.AccTranMsts.ReportView)]

    public async Task<ActionResult> ReceiptAndPayments()
    {
        var model = new AccReportVm();
        model.FincYear = await _iFincYearService.GetFincYearByDate(DateTime.Today);
        model.StrFromDate = model.FincYear.YearStartDate.ToString("dd/MM/yyyy");
        model.StrToDate = model.FincYear.YearEndDate.ToString("dd/MM/yyyy");
        model.AccountLookup = await _iAccLedgerService.GetMishukLedgerSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> ReceiptAndPayments(AccReportVm model)
    {
        var data = await _iService.GetReceiptAndPaymentsReportHtml(model);
        return Ok(data);
    }

    public async Task<ActionResult> ReceiptAndPaymentsPrint(string fromDate, string toDate, long? mishukLedgerId)
    {
        var model = new AccReportVm();
        model.StrFromDate = fromDate;
        model.StrToDate = toDate;
        model.MishukLedgerId = mishukLedgerId;

        string html = await _iService.GetReceiptAndPaymentsReportHtml(model);

        ExportToPDF export = new ExportToPDF(_ihttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Statement of Receipts & Payments";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "ReceiptsAndPayments";

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 120, top: 80, left: 18, right: 18, isLandScape: false), "application/pdf");
    }

    #endregion

    #region ProfitOrLossStatement

    [Authorize(Permissions.AccTranMsts.ReportView)]

    public async Task<ActionResult> ProfitOrLossStatement()
    {
        var model = new AccReportVm();
        model.FincYear = await _iFincYearService.GetFincYearByDate(DateTime.Today);
        model.StrFromDate = model.FincYear.YearStartDate.ToString("dd/MM/yyyy");
        model.StrToDate = model.FincYear.YearEndDate.ToString("dd/MM/yyyy");
        model.AccountLookup = await _iAccLedgerService.GetMishukLedgerSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> ProfitOrLossStatement(AccReportVm model)
    {
        var data = await _iService.GetProfitOrLossStatementHtml(model);
        return Ok(data);
    }

    public async Task<ActionResult> ProfitOrLossStatementPrint(string fromDate, string toDate, long mishukLedgerId)
    {
        var model = new AccReportVm();
        model.StrFromDate = fromDate;
        model.StrToDate = toDate;
        model.MishukLedgerId = mishukLedgerId;

        string html = await _iService.GetProfitOrLossStatementHtml(model);

        ExportToPDF export = new ExportToPDF(_ihttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Profit or Loss Statement";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "ProfitOrLossStatement";

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 120, top: 80, left: 18, right: 18, isLandScape: false), "application/pdf");
    }

    #endregion

    #region QuickEntry

    [HttpGet]
    [Authorize(Permissions.AccTranMsts.JournalCreate)]
    public async Task<IActionResult> QuickVoucherEntry(string drCr = "C", string strDate = "")
    {
        var model = new QuickVoucherVm();
        model.VcDateStr = string.IsNullOrEmpty(strDate) ? DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy") : strDate;
        model.VcNo = await _iService.GetVoucherAutoCode(VoucherTypeCode.JournalVoucher, DateTime.Now.AddDays(-1));
        model.FinYearLookup = _dropdownService.GetFinYearSelectListItems();
        model.LedgerLookup = _dropdownService.GetAccLedgerSelectListItems();
        model.DrCrLookup = _dropdownService.GetDrCrSelectListItems();
        model.DrCr = drCr;

        var finYearId = (await _iFincYearService.GetFincYearByDate(DateTime.Now.AddDays(-1).Date)).Id;
        var misukLedger = await _iAccLedgerService.GetFirstOrDefaultAsync(x => x.LedgerCode == AccLadgerCode.HotelMisuk);
        if (misukLedger == null)
            throw new Exception("Ladger For Mishuk Not Found...!!");

        model.CurrentFincYearId = finYearId;
        model.MishukLedgerId = misukLedger.Id;

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> QuickVoucherEntry(QuickVoucherVm modelVm)
    {
        modelVm.LedgerLookup = _dropdownService.GetAccLedgerSelectListItems();
        modelVm.DrCrLookup = _dropdownService.GetDrCrSelectListItems();

        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information Is Not Correct");
                return View("QuickVoucherEntry", modelVm);
            }
            _iService.CurrentUserId = UserId;
            var isAdded = await _iService.AddQuickVc(modelVm);
            if (isAdded == 0)
            {
                SaveFailedMsg();
                return View("QuickVoucherEntry", modelVm);
            }
            //SaveSuccessMsg();
            return RedirectToAction("QuickVoucherEntry", "AccTranMst", new { drCr = modelVm.DrCr, strDate = modelVm.VcDateStr });
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("QuickVoucherEntry", modelVm);
        }
    }

    [HttpPost]
    public async Task<ActionResult> QuickLedgerReport(AccReportVm model)
    {
        var finYearDate = DU.Utility.ConvertStrToDate(model.StrFromDate) ?? DateTime.Now;
        model.FinYearId = (await _iFincYearService.GetFincYearByDate(finYearDate.Date)).Id;
        var data = await _iService.GetQuickLedgerReportHtml(model, false);
        return Ok(data);
    }

    public async Task<ActionResult> QuickLedgerReportPrint(string fromDate, string toDate, long ledgerId, long finYearId)
    {
        var model = new AccReportVm();
        model.StrFromDate = fromDate;
        model.StrToDate = toDate;
        model.LedgerId = ledgerId;
        //model.FinYearId = finYearId;
        var finYearDate = DU.Utility.ConvertStrToDate(model.StrFromDate) ?? DateTime.Now;
        model.FinYearId = (await _iFincYearService.GetFincYearByDate(finYearDate.Date)).Id;

        string html = await _iService.GetQuickLedgerReportHtml(model, true);

        ExportToPDF export = new ExportToPDF(_ihttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Mishuk Ledger Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");

        string reportName = "Ladger_Report";


        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 50, top: 80, left: 10, right: 10, isLandScape: false), "application/pdf");
    }

    #endregion

    #region QuickUpdate

    [HttpPost]
    public async Task<IActionResult> QuickUpdate(QuickVoucherVm modelVm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    message = "Validation failed",
                    errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                )
                });
            };

            _iService.CurrentUserId = UserId;

            var isUpdate = await _iService.UpdateQuickVc(modelVm);
            return Ok(isUpdate);
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                message = "Update Failed..!!",
                errors = e.Message
            });
        }
    }

    #endregion

    #region DeleteVoucher
    [HttpGet]
    public async Task<IActionResult> DeleteVoucher(long voucherId)
    {
        try
        {
            if (voucherId > 0)
            {
                _iService.CurrentUserId = UserId;
                var isDelete = await _iService.VoucherRemoveAsync(voucherId);

                return Ok(isDelete);
            }

            return BadRequest();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }
    #endregion

    #region Restaurent
    [HttpGet]
    [Authorize(Permissions.AccTranMsts.JournalCreate)]
    public async Task<IActionResult> QuickVoucherEntryForRestaurant(string drCr = "C", string strDate = "")
    {
        var model = new QuickVoucherVm();
        model.VcDateStr = string.IsNullOrEmpty(strDate) ? DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy") : strDate;
        model.VcNo = await _iService.GetVoucherAutoCode(VoucherTypeCode.JournalVoucher, DateTime.Now.AddDays(-1));
        model.FinYearLookup = _dropdownService.GetFinYearSelectListItems();
        model.LedgerLookup = _dropdownService.GetAccLedgerSelectListItems();
        model.DrCrLookup = _dropdownService.GetDrCrSelectListItems();
        model.DrCr = drCr;

        var finYearId = (await _iFincYearService.GetFincYearByDate(DateTime.Now.AddDays(-1).Date)).Id;
        var defaultLeger = await _iAccLedgerService.GetFirstOrDefaultAsync(x => x.LedgerCode == AccLadgerCode.RestaurantLedger);
        if (defaultLeger == null)
            throw new Exception("Ladger For Restaurant Not Found...!!");

        model.CurrentFincYearId = finYearId;
        model.MishukLedgerId = defaultLeger.Id;

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> QuickVoucherEntryForRestaurant(QuickVoucherVm modelVm)
    {
        modelVm.LedgerLookup = _dropdownService.GetAccLedgerSelectListItems();
        modelVm.DrCrLookup = _dropdownService.GetDrCrSelectListItems();

        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information Is Not Correct");
                return View("QuickVoucherEntryForRestaurant", modelVm);
            }
            _iService.CurrentUserId = UserId;
            var isAdded = await _iService.AddQuickVc(modelVm);
            if (isAdded == 0)
            {
                SaveFailedMsg();
                return View("QuickVoucherEntryForRestaurant", modelVm);
            }
            //SaveSuccessMsg();
            return RedirectToAction("QuickVoucherEntryForRestaurant", "AccTranMst", new { drCr = modelVm.DrCr, strDate = modelVm.VcDateStr });
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("QuickVoucherEntryForRestaurant", modelVm);
        }
    }
    #endregion

    #region AmariResort
    [HttpGet]
    [Authorize(Permissions.AccTranMsts.JournalCreate)]
    public async Task<IActionResult> QuickVoucherEntryForAmariResort(string drCr = "C", string strDate = "")
    {
        var model = new QuickVoucherVm();
        model.VcDateStr = string.IsNullOrEmpty(strDate) ? DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy") : strDate;
        model.VcNo = await _iService.GetVoucherAutoCode(VoucherTypeCode.JournalVoucher, DateTime.Now.AddDays(-1));
        model.FinYearLookup = _dropdownService.GetFinYearSelectListItems();
        model.LedgerLookup = _dropdownService.GetAccLedgerSelectListItems();
        model.DrCrLookup = _dropdownService.GetDrCrSelectListItems();
        model.DrCr = drCr;

        var finYearId = (await _iFincYearService.GetFincYearByDate(DateTime.Now.AddDays(-1).Date)).Id;
        var defaultLeger = await _iAccLedgerService.GetFirstOrDefaultAsync(x => x.LedgerCode == AccLadgerCode.AmariResortLedger);
        if (defaultLeger == null)
            throw new Exception("Ladger For Amari Resosrt Not Found...!!");

        model.CurrentFincYearId = finYearId;
        model.MishukLedgerId = defaultLeger.Id;

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> QuickVoucherEntryForAmariResort(QuickVoucherVm modelVm)
    {
        modelVm.LedgerLookup = _dropdownService.GetAccLedgerSelectListItems();
        modelVm.DrCrLookup = _dropdownService.GetDrCrSelectListItems();

        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information Is Not Correct");
                return View("QuickVoucherEntryForAmariResort", modelVm);
            }
            _iService.CurrentUserId = UserId;
            var isAdded = await _iService.AddQuickVc(modelVm);
            if (isAdded == 0)
            {
                SaveFailedMsg();
                return View("QuickVoucherEntryForAmariResort", modelVm);
            }
            //SaveSuccessMsg();

            return RedirectToAction("QuickVoucherEntryForAmariResort", "AccTranMst", new { drCr = modelVm.DrCr, strDate = modelVm.VcDateStr });
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("QuickVoucherEntryForAmariResort", modelVm);
        }
    }
    #endregion

    #region StaffKitchen
    [HttpGet]
    [Authorize(Permissions.AccTranMsts.JournalCreate)]
    public async Task<IActionResult> QuickVoucherEntryForKitchen(string drCr = "C", string strDate = "")
    {
        var model = new QuickVoucherVm();
        model.VcDateStr = string.IsNullOrEmpty(strDate) ? DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy") : strDate;
        model.VcNo = await _iService.GetVoucherAutoCode(VoucherTypeCode.JournalVoucher, DateTime.Now.AddDays(-1));
        model.FinYearLookup = _dropdownService.GetFinYearSelectListItems();
        model.LedgerLookup = _dropdownService.GetAccLedgerSelectListItems();
        model.DrCrLookup = _dropdownService.GetDrCrSelectListItems();
        model.DrCr = drCr;

        var finYearId = (await _iFincYearService.GetFincYearByDate(DateTime.Now.AddDays(-1).Date)).Id;
        var defaultLeger = await _iAccLedgerService.GetFirstOrDefaultAsync(x => x.LedgerCode == AccLadgerCode.StaffKitchenLedger);
        if (defaultLeger == null)
            throw new Exception("Ladger For Staff Kitchen Resosrt Not Found...!!");

        model.CurrentFincYearId = finYearId;
        model.MishukLedgerId = defaultLeger.Id;

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> QuickVoucherEntryForKitchen(QuickVoucherVm modelVm)
    {
        modelVm.LedgerLookup = _dropdownService.GetAccLedgerSelectListItems();
        modelVm.DrCrLookup = _dropdownService.GetDrCrSelectListItems();

        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information Is Not Correct");
                return View("QuickVoucherEntryForKitchen", modelVm);
            }
            _iService.CurrentUserId = UserId;
            var isAdded = await _iService.AddQuickVc(modelVm);
            if (isAdded == 0)
            {
                SaveFailedMsg();
                return View("QuickVoucherEntryForKitchen", modelVm);
            }
            //SaveSuccessMsg();

            return RedirectToAction("QuickVoucherEntryForKitchen", "AccTranMst", new { drCr = modelVm.DrCr, strDate = modelVm.VcDateStr });
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("QuickVoucherEntryForKitchen", modelVm);
        }
    }
    #endregion

    #region HeadWiseBalance

    [Authorize(Permissions.AccTranMsts.ReportView)]

    public async Task<ActionResult> HeadWiseBalance()
    {
        var model = new AccReportVm();
        var finYear = await _iFincYearService.GetFincYearByDate(DateTime.Today);
        model.FincYearLookup = _dropdownService.GetFinYearSelectListItems();
        model.StrFromDate = finYear.YearStartDate.ToString("dd/MM/yyyy");
        model.StrToDate = finYear.YearEndDate.ToString("dd/MM/yyyy");
        model.AccountLookup = await _iAccLedgerService.GetMishukLedgerSelectListItems();
        model.HeadLookup = _dropdownService.GetAccHeadSelectListItems();
        model.FinYearId = finYear.Id;
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> HeadWiseBalance(AccReportVm model)
    {
        var data = await _iService.GetHeadWiseBalanceHtml(model);
        return Ok(data);
    }

    public async Task<ActionResult> HeadWiseBalancePrint(string fromDate, string toDate, long finYearId, long accountId = 0, long headId = 0)
    {
        var model = new AccReportVm();
        model.StrFromDate = fromDate;
        model.StrToDate = toDate;
        model.HeadId = headId;
        model.FinYearId = finYearId;
        model.MishukLedgerId = accountId;

        string html = await _iService.GetHeadWiseBalanceHtml(model);

        ExportToPDF export = new ExportToPDF(_ihttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Head Wise Balance Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = true;

        string reportName = "Head_Wise_Balance_Report";

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 50, top: 90, left: 10, right: 10, isLandScape: false), "application/pdf");
    }

    #endregion
}
