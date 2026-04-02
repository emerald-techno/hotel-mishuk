using AutoMapper;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory;
using Domain.ViewModel.Report;
using Interface.Services.Accounts;
using Interface.Services.Admin;
using Interface.Services.Inventory;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using Utility.Export;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;

namespace WebMVC.Controllers.Inventory;

public class InventoryReportController : AppBaseController
{
    #region Config
    private readonly IMapper _iMapper;
    private readonly IInventoryReportService _iService;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly DropdownService _dropdownService;
    private readonly CacheStoreService _cacheStoreService;
    private readonly IHttpContextAccessor _iHttpContextAccessor;
    private readonly IDepartmentService _iDepartmentService;
    private readonly IAccLedgerService _iAccLedgerService;

    public InventoryReportController(IInventoryReportService iService,
        IMapper iMapper, IUnitOfWork iUnitOfWork,
        DropdownService dropdownService,
        CacheStoreService cacheStoreService, IHttpContextAccessor iHttpContextAccessor,
        IDepartmentService iDepartmentService,
        IAccLedgerService iAccLedgerService) :
        base(iUnitOfWork, "InventoryReport")
    {
        _iService = iService;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
        _dropdownService = dropdownService;
        _cacheStoreService = cacheStoreService;
        _iHttpContextAccessor = iHttpContextAccessor;
        _iDepartmentService = iDepartmentService;
        _iAccLedgerService = iAccLedgerService;
    }

    #endregion

    #region StockReport

    [Authorize(Permissions.InventoryReport.View)]
    public ActionResult StockReport()
    {
        var model = new StockVm();
        model.CategoryLookUp = _dropdownService.GetItemCategorySelectListItems();
        model.ItemLookUp = _dropdownService.GetItemSelectListItems();
        model.StockStatusLookUp = _dropdownService.GetStockStatusSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> StockReport(StockVm model)
    {
        var data = await _iService.GetStockReportHtml(model);
        return Ok(data);
    }

    public async Task<ActionResult> StockReportPrint(long? categoryId, long? itemId, int? stockStatus)
    {
        var model = new StockVm();
        model.CategoryId = categoryId ?? 0;
        model.ItemId = itemId ?? 0;
        model.StockStatus = stockStatus ?? 0;

        string html = await _iService.GetStockReportHtml(model);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Stock Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Stock Report";

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 120, top: 80, left: 18, right: 18, isLandScape: false), "application/pdf");
    }

    #endregion

    #region IssueReport

    public ActionResult IssueReport()
    {
        var model = new IssueReportVM();
        model.CategoryLookUp = _dropdownService.GetItemCategorySelectListItems();
        model.ItemLookUp = _dropdownService.GetItemSelectListItems();
        model.DepartmentLookUp = _dropdownService.GetDepartmentSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> IssueReport(IssueReportVM model)
    {
        var data = await _iService.GetIssueReportHtml(model);
        return Ok(data);
    }

    public async Task<ActionResult> IssueReportPrint(long? categoryId, long? itemId, long? departmentId, string strFromDate = null, string strToDate = null)
    {
        var model = new IssueReportVM();
        model.CategoryId = categoryId ?? 0;
        model.ItemId = itemId ?? 0;
        model.DepartmentId = departmentId ?? 0;
        model.StrFromDate = strFromDate;
        model.StrToDate = strToDate;

        string html = await _iService.GetIssueReportHtml(model);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Issue Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Issue Report";

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 120, top: 80, left: 18, right: 18, isLandScape: false), "application/pdf");
    }

    #endregion

    #region DepartmentStockReport

    public ActionResult DepartmentStockReport()
    {
        var model = new DepartmentStockVm();
        model.DepartmentLookUp = _dropdownService.GetDepartmentSelectListItems();
        model.CategoryLookUp = _dropdownService.GetItemCategorySelectListItems();
        model.ItemLookUp = _dropdownService.GetItemSelectListItems();
        model.StockStatusLookUp = _dropdownService.GetStockStatusSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> DepartmentStockReport(DepartmentStockVm model)
    {
        var data = await _iService.GetDepartmentWiseStockReportHtml(model);
        return Ok(data);
    }

    public async Task<ActionResult> DepartmentStockReportPrint(long? departmentId, long? itemId, long? categoryId, int? stockStatus)
    {
        var model = new DepartmentStockVm();
        model.DepartmentId = departmentId ?? 0;
        model.ItemId = itemId ?? 0;
        model.CategoryId = categoryId ?? 0;
        model.StockStatus = stockStatus ?? 0;

        string html = await _iService.GetDepartmentWiseStockReportHtml(model);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Department Stock Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Department Stock Report";

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 120, top: 80, left: 18, right: 18, isLandScape: false), "application/pdf");
    }

    #endregion

    #region HkDepartmentStockReport

    public async Task<ActionResult> HkDepartmentStockReport()
    {
        try
        {
            var model = new DepartmentStockVm();

            var housekeeperDpt = await _iDepartmentService.GetFirstOrDefaultAsync(x => x.Code == DepartmentCode.HouseKeeper);
            if (housekeeperDpt == null)
                throw new Exception("HouserKeeper Department Entry Not Found..!!");

            model.DepartmentId = housekeeperDpt.Id;
            model.ItemLookUp = _dropdownService.GetItemSelectListItems();
            return View(model);
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("_404");
        }

    }

    #endregion

    #region ResturantStockReport

    public async Task<ActionResult> ResturantStockReport()
    {
        try
        {
            var model = new DepartmentStockVm();

            var resturantDpt = await _iDepartmentService.GetFirstOrDefaultAsync(x => x.Code == DepartmentCode.Resturant);
            if (resturantDpt == null)
                throw new Exception("Resturant Department Entry Not Found..!!");

            model.DepartmentId = resturantDpt.Id;
            model.ItemLookUp = _dropdownService.GetItemSelectListItems();
            model.StockStatusLookUp = _dropdownService.GetStockStatusSelectListItems();
            model.CategoryLookUp = _dropdownService.GetItemCategorySelectListItems();
            return View(model);
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("_404");
        }

    }

    #endregion

    #region ItemCurrentStockByDpt

    [HttpPost]
    public async Task<IActionResult> GetItemCurrentStockByDptId(long dptId, long itemId)
    {
        try
        {
            var stockValue = await _iService.GetItemStockByDptId(dptId, itemId);
            if (stockValue == null)
                return Ok(new { Stock = 0 });
            return Ok(stockValue);
        }
        catch (Exception e)
        {
            return Ok(SetError(e.Message));
        }
    }

    #endregion

    #region StockRegisterReport

    [Authorize(Permissions.InventoryReport.View)]
    public ActionResult StockRegisterReport()
    {
        var model = new StockRegisterVm();
        var today = DateTime.Today;
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        model.StrFromDate = monthStart.ToString("dd/MM/yyyy");
        model.StrToDate = monthEnd.ToString("dd/MM/yyyy");
        model.CategoryLookUp = _dropdownService.GetItemCategorySelectListItems();
        model.ItemLookUp = _dropdownService.GetItemSelectListItems();
        model.StockStatusLookUp = _dropdownService.GetStockStatusSelectListItems();

        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> StockRegisterReport(StockRegisterVm model)
    {
        var data = await _iService.GetStockRegisterReportHtml(model);
        return Ok(data);
    }

    public async Task<ActionResult> StockRegisterReportPrint(long? categoryId, long? itemId, int? stockStatus, string strFromDate = null, string strToDate = null)
    {
        var model = new StockRegisterVm();
        model.CategoryId = categoryId ?? 0;
        model.ItemId = itemId ?? 0;
        model.StockStatus = stockStatus ?? 0;
        model.StrFromDate = strFromDate;
        model.StrToDate = strToDate;

        string html = await _iService.GetStockRegisterReportHtml(model);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Stock Register Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Stock Register Report";

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 120, top: 80, left: 18, right: 18, isLandScape: true), "application/pdf");
    }

    #endregion

    #region HkConsumeReport

    public async Task<ActionResult> HkConsumeReport()
    {
        try
        {
            var model = new ConsumeReportVm();

            var housekeeperDpt = await _iDepartmentService.GetFirstOrDefaultAsync(x => x.Code == DepartmentCode.HouseKeeper);
            if (housekeeperDpt == null)
                throw new Exception("HouserKeeper Department Entry Not Found..!!");

            model.DepartmentId = housekeeperDpt.Id;
            var today = DateTime.Today;
            var monthStart = new DateTime(today.Year, today.Month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            model.StrFromDate = monthStart.ToString("dd/MM/yyyy");
            model.StrToDate = monthEnd.ToString("dd/MM/yyyy");
            model.CategoryLookUp = _dropdownService.GetItemCategorySelectListItems();
            model.ItemLookUp = _dropdownService.GetItemSelectListItems();
            return View(model);
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("_404");
        }

    }

    #endregion

    #region DepartmentConsumeReport

    public ActionResult DepartmentConsumeReport()
    {
        var model = new ConsumeReportVm();
        var today = DateTime.Today;
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        model.StrFromDate = monthStart.ToString("dd/MM/yyyy");
        model.StrToDate = monthEnd.ToString("dd/MM/yyyy");
        model.CategoryLookUp = _dropdownService.GetItemCategorySelectListItems();
        model.ItemLookUp = _dropdownService.GetItemSelectListItems();
        model.DepartmentLookUp = _dropdownService.GetDepartmentSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> DepartmentConsumeReport(ConsumeReportVm model)
    {
        try
        {
            var data = await _iService.GetDepartmentWiseConsumeReportHtml(model);
            return Ok(data);
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    public async Task<ActionResult> DepartmentConsumeReportPrint(string strFromDate, string strToDate,long categoryId, long? departmentId, long? itemId)
    {
        var model = new ConsumeReportVm();
        model.DepartmentId = departmentId ?? 0;
        model.ItemId = itemId ?? 0;
        model.StrFromDate = strFromDate;
        model.StrToDate = strToDate;
       
        string html = await _iService.GetDepartmentWiseConsumeReportHtml(model);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Department Consume Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Department Consume Report";

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 120, top: 80, left: 18, right: 18, isLandScape: false), "application/pdf");
    }

    #endregion

    #region OrderDueReport

    public ActionResult OrderDueReport()
    {
        var model = new InvOrderDueReportVm();
        var today = DateTime.Today;
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        model.StrFromDate = monthStart.ToString("dd/MM/yyyy");
        model.StrToDate = monthEnd.ToString("dd/MM/yyyy");
        model.OrderLookUp = _dropdownService.GetOrderSelectListItems();
        model.SupplierLookUp = _dropdownService.GetSupplierSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> OrderDueReport(InvOrderDueReportVm model)
    {
        var data = await _iService.GetOrderDueReportHtml(model);
        return Ok(data);
    }

    public async Task<ActionResult> OrderDueReportPrint(string fromDate, string toDate, long? orderId, long? supplierId)
    {
        var model = new InvOrderDueReportVm();
        model.StrFromDate = fromDate;
        model.StrToDate = toDate;
        model.OrderId = orderId ?? 0;
        model.SupplierId = supplierId ?? 0;

        string html = await _iService.GetOrderDueReportHtml(model);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Order Due Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Order Due Report";

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 120, top: 80, left: 18, right: 18, isLandScape: false), "application/pdf");
    }

    #endregion

    #region HkDepartmentStockRegisterReport

    public async Task<ActionResult> HkDepartmentStockRegisterReport()
    {
        try
        {
            var model = new StockRegisterVm();

            var housekeeperDpt = await _iDepartmentService.GetFirstOrDefaultAsync(x => x.Code == DepartmentCode.HouseKeeper);
            if (housekeeperDpt == null)
                throw new Exception("HouserKeeper Department Entry Not Found..!!");

            model.DepartmentId = housekeeperDpt.Id;
            model.ItemLookUp = _dropdownService.GetItemSelectListItems();
            model.StockStatusLookUp = _dropdownService.GetStockStatusSelectListItems();
            return View(model);
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("_404");
        }

    }

    #endregion

    #region DepartmentStockRegisterReport

    [Authorize(Permissions.InventoryReport.View)]
    public ActionResult DepartmentStockRegisterReport()
    {
        var model = new StockRegisterVm();
        var today = DateTime.Today;
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        model.StrFromDate = monthStart.ToString("dd/MM/yyyy");
        model.StrToDate = monthEnd.ToString("dd/MM/yyyy");
        model.DepartmentLookUp = _dropdownService.GetDepartmentSelectListItems();
        model.CategoryLookUp = _dropdownService.GetItemCategorySelectListItems();
        model.ItemLookUp = _dropdownService.GetItemSelectListItems();
        model.StockStatusLookUp = _dropdownService.GetStockStatusSelectListItems();

        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> DepartmentStockRegisterReport(StockRegisterVm model)
    {
        //var data = await _iService.GetDepartmentStockRegisterReportHtml(model);
        var data = await _iService.GetDepartmentWiseStockReportWithAmountHtml(model);
        return Ok(data);
    }

    public async Task<ActionResult> DepartmentStockRegisterReportPrint(long departmentId, long? categoryId, long? itemId, int? stockStatus, string fromDate, string toDate)
    {
        var model = new StockRegisterVm();
        model.DepartmentId = departmentId;
        model.CategoryId = categoryId ?? 0;
        model.ItemId = itemId ?? 0;
        model.StrFromDate = fromDate;
        model.StrToDate = toDate;

        string html = await _iService.GetDepartmentWiseStockReportWithAmountHtml(model);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Department Stock Register Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Department Stock Register Report";

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 120, top: 80, left: 18, right: 18, isLandScape: true), "application/pdf");
    }


    #endregion

    #region ReceiveReport

    [Authorize(Permissions.InventoryReport.View)]
    public async Task<ActionResult> ReceiveReport()
    {
        var model = new ReceiveReportVM();
        model.CategoryLookUp = _dropdownService.GetItemCategorySelectListItems();
        model.ItemLookUp = _dropdownService.GetItemSelectListItems();
        model.LedgerLookUp = await _iAccLedgerService.GetMishukLedgerSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> ReceiveReport(ReceiveReportVM model)
    {
        var data = await _iService.GetReceiveReportHtml(model);
        return Ok(data);
    }

    public async Task<ActionResult> ReceiveReportPrint(long? categoryId, long? itemId, long? departmentId, string strFromDate = null, string strToDate = null, long? ledgerId = 0)
    {
        var model = new ReceiveReportVM();
        model.CategoryId = categoryId ?? 0;
        model.ItemId = itemId ?? 0;
        model.DepartmentId = departmentId ?? 0;
        model.LedgerId = ledgerId ?? 0;
        model.StrFromDate = strFromDate;
        model.StrToDate = strToDate;

        string html = await _iService.GetReceiveReportHtml(model);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Receive Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Receive Report";

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 80, top: 80, left: 18, right: 18, isLandScape: true), "application/pdf");
    }

    #endregion

    #region ItemConsumptionReport

    [Authorize(Permissions.InventoryReport.View)]
    public async Task<ActionResult> ItemConsumptionReport()
    {
        var model = new ItemConsumptionReportVm();
        //model.CategoryLookUp = _dropdownService.GetItemCategorySelectListItems();
        //model.ItemLookUp = _dropdownService.GetItemSelectListItems();
        //model.LedgerLookUp = await _iAccLedgerService.GetMishukLedgerSelectListItems();
        model.StrFromDate = DateTime.Today.Date.ToString("dd/MM/yyyy");
        model.StrToDate = DateTime.Today.AddDays(1).Date.ToString("dd/MM/yyyy");
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> ItemConsumptionReport(ItemConsumptionReportVm model)
    {
        var data = await _iService.ItemConsumptionReportHtml(model);
        return Ok(data);
    }

    public async Task<ActionResult> ItemConsumptionReportPrint(string strFromDate = null, string strToDate = null)
    {
        var model = new ItemConsumptionReportVm();

        model.StrToDate = strToDate;

        string html = await _iService.ItemConsumptionReportHtml(model);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Food Sales & Raw Material Usage Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Food Sales & Raw Material Usage Report (Ingradient Wise)";

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 80, top: 80, left: 18, right: 18, isLandScape: true), "application/pdf");
    }

    #endregion
}
