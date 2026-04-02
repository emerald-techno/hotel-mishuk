using AutoMapper;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.Transaction;
using Interface.Services.Accounts;
using Interface.Services.Inventory;
using Interface.Services.Notification;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using Utility.Export;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using DU = Domain.Utility;

namespace WebMVC.Controllers.Inventory;

public class ReceiveController : AppBaseController
{
    #region Config

    private readonly IUnitOfWork _iUnitWork;
    private readonly ITranService _iService;
    private readonly DropdownService _dropdownService;
    private readonly IMapper _iMapper;
    private IHttpContextAccessor _iHttpContextAccessor;
    private readonly INtfNotificationMsgService _iNtfMsgService;
    private readonly IAccLedgerService _iAccLedgerService;

    public ReceiveController(IUnitOfWork iUnitOfWork,
                            ITranService iService,
                            DropdownService dropdownService,
                            IMapper iMapper,
                            IHttpContextAccessor iHttpContextAccessor,
                            INtfNotificationMsgService iNtfMsgService,
                            IAccLedgerService iAccLedgerService) : base(iUnitOfWork)
    {
        _iUnitWork = iUnitOfWork;
        _iService = iService;
        _dropdownService = dropdownService;
        _iMapper = iMapper;
        _iHttpContextAccessor = iHttpContextAccessor;
        _iNtfMsgService = iNtfMsgService;
        _iAccLedgerService = iAccLedgerService;
    }

    #endregion

    #region Create

    [HttpGet]
    [Authorize(Permissions.Receive.Create)]
    public async Task<IActionResult> Create()
    {
        var vm = new TransactionVm();
        vm.TranType = TransType.Receive;
        vm.TranNo = await _iService.GetTransAutoCode(vm.TranType);
        vm.TranDateStr = DateTime.Now.ToString("dd/MM/yyyy");
        vm.SupplierLookUp = _dropdownService.GetSupplierSelectListItems();
        vm.OrderLookUp = _dropdownService.GetOrderForReceiveSelectListItems();
        vm.OrderReceiveStatusLookup = _dropdownService.GetReceiveStatusSelectListItems();
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Create(TransactionVm modelVm)
    {
        try
        {
            modelVm.SupplierLookUp = _dropdownService.GetSupplierSelectListItems();
            modelVm.OrderLookUp = _dropdownService.GetOrderForReceiveSelectListItems();
            modelVm.OrderReceiveStatusLookup = _dropdownService.GetReceiveStatusSelectListItems();

            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information Is Not Correct");
                return View("Create", modelVm);
            }
            _iService.CurrentUserId = UserId;
            var isAdded = await _iService.AddAsync(modelVm);

            if (!isAdded)
            {
                SaveFailedMsg();
                return View("Create", modelVm);
            }

            var actionUrl = Url.Action("Details", "Receive", new { id = modelVm.Id }, Request.Scheme);

            string ntfMsgHtml = @$"<a href='{actionUrl}' target='_blank'><b>{modelVm.TranNo}-({modelVm.TranDate.ToString("dd/MM/yyyy")}) Receive Items</b></a>";
            string emailMsg = $@"{modelVm.TranNo}-({modelVm.TranDate.ToString("dd/MM/yyyy")}) Receive Items";

            var ntfGenerated = await _iNtfMsgService.GenerateNtf(NotificationEventCode.ReceiveItemNtf, ntfMsgHtml, emailMsg);

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

    #region DirectCreate

    [Authorize(Permissions.Receive.Create)]
    public async Task<IActionResult> DirectCreate()
    {
        var model = new TransactionVm();
        model.TranType = TransType.Receive;
        model.TranNo = await _iService.GetTransAutoCode(TransType.Receive);
        model.SupplierLookUp = _dropdownService.GetSupplierSelectListItems();
        model.LedgerLookUp = await _iAccLedgerService.GetMishukLedgerSelectListItems();

        model.UnitLookUp = _dropdownService.GetUnitSelectListItems();
        model.ItemCategoryLookUp = _dropdownService.GetItemCategorySelectListItems();

        model.TranDateStr = DateTime.Now.ToString("dd/MM/yyyy");
        model.IsReceiveAutoPay = false;
        model.IsReqAuto = true;
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> DirectCreate(TransactionVm modelVm)
    {
        modelVm.SupplierLookUp = _dropdownService.GetSupplierSelectListItems();
        modelVm.LedgerLookUp = await _iAccLedgerService.GetMishukLedgerSelectListItems();
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return View("DirectCreate", modelVm);
            }

            _iService.CurrentUserId = UserId;
            var (isAdded, receiveId) = await _iService.DirectReceiveEntryAsync(modelVm);

            if (!isAdded)
            {
                SaveFailedMsg();
                return RedirectToAction("DirectCreate");
            }
            SaveSuccessMsg();
            return RedirectToAction("Details", new { id = receiveId });
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return RedirectToAction("DirectCreate");
        }
    }

    #endregion

    #region Search

    [HttpGet]
    [Authorize(Permissions.Receive.ListView)]
    public IActionResult Search()
    {
        var vm = new TransactionSearchVm()
        {
            SupplierLookUp = _dropdownService.GetSupplierSelectListItems(),
            OrderLookUp = _dropdownService.GetOrderSelectListItems(),
            TranType = TransType.Receive,
        };
        return View(vm);
    }

    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<TransactionSearchVm, TransactionSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<TransactionSearchVm, TransactionSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new TransactionSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion

    #region Detail
    [Authorize(Permissions.Receive.Detail)]
    public async Task<ActionResult> Details(long id)
    {
        try
        {
            var model = await _iService.GetTransInfoDataAsync(id);
            return View(model);
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("_404");
        }

    }

    #endregion

    #region ReceivePrint

    public async Task<ActionResult> ReceivePrint(long id)
    {
        string html = await _iService.GetReceiveByIdAsyncHtml(id);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "";
        reportTitle.IsSignature = true;
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Store Manager");
        reportTitle.SignatureList.Add("Authorized Signature");

        string reportName = "Receive" + DateTime.Today.ToString("dd_mm_yyy");

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 120, top: 80, left: 18, right: 18, isLandScape: false, withFooter: true), "application/pdf");
    }

    #endregion

    #region DirectReceiveReport

    public ActionResult DirectReceiveReport()
    {
        var model = new DirectReceiveReportVm();
        var today = DateTime.Today;
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        model.StrFromDate = monthStart.ToString("dd/MM/yyyy");
        model.StrToDate = monthEnd.ToString("dd/MM/yyyy");
        model.SupplierLookUp = _dropdownService.GetSupplierSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> DirectReceiveReport(DirectReceiveReportVm model)
    {
        var data = await _iService.DirectReceiveReportHtml(model);
        return Ok(data);
    }

    public async Task<ActionResult> DirectReceiveReportPrint(string fromDate, string toDate, long? supplierId)
    {
        var model = new DirectReceiveReportVm();
        model.StrFromDate = fromDate;
        model.StrToDate = toDate;
        model.SupplierId = supplierId ?? 0;

        string html = await _iService.DirectReceiveReportHtml(model);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Direct Receive Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Order Due Report";

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 120, top: 80, left: 18, right: 18, isLandScape: false), "application/pdf");
    }

    #endregion

    #region Edit
    public async Task<IActionResult> Edit(long id)
    {
        try
        {
            var modelVm = await _iService.GetTransInfoDataAsync(id);
            modelVm.TranDateStr = modelVm.TranDate.ToString("dd/MM/yyyy");
            modelVm.SupplierLookUp = _dropdownService.GetSupplierSelectListItems();
            modelVm.LedgerLookUp = await _iAccLedgerService.GetMishukLedgerSelectListItems();
            modelVm.ItemLookUp = _dropdownService.GetItemSelectListItems();
            modelVm.UnitLookUp = _dropdownService.GetUnitSelectListItems();
            modelVm.ItemCategoryLookUp = _dropdownService.GetItemCategorySelectListItems();
            return View(modelVm);
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("_404");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(TransactionVm modelVm)
    {
        modelVm.SupplierLookUp = _dropdownService.GetSupplierSelectListItems();
        modelVm.LedgerLookUp = await _iAccLedgerService.GetMishukLedgerSelectListItems();
        modelVm.ItemLookUp = _dropdownService.GetItemSelectListItems();
        modelVm.UnitLookUp = _dropdownService.GetUnitSelectListItems();
        modelVm.ItemCategoryLookUp = _dropdownService.GetItemCategorySelectListItems();

        try
        {
            if (!ModelState.IsValid)
            {
                return View(modelVm);
            }
            _iService.CurrentUserId = UserId;
            var isAdded = await _iService.AddOrUpdate(modelVm);
            return RedirectToAction("Details", new { id = modelVm.Id });
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View(modelVm);
        }
    }


    #endregion

    #region Json Data

    public async Task<IActionResult> GetById(long id)
    {
        try
        {
            var data = await _iService.GetTransInfoDataAsync(id);
            return Ok(data);
        }
        catch (Exception e)
        {
            return Ok(SetError(e.Message));
        }
    }

    #endregion
}
