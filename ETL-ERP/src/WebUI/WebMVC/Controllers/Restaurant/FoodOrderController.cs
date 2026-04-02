using AutoMapper;
using Domain.Enums.AppEnums;
using Domain.Utility.Common;
using Domain.ViewModel.Report;
using Domain.ViewModel.Restaurant.FoodOrder;
using Domain.ViewModel.Restaurant.OrderPayment;
using Interface.Services.HotelManagement;
using Interface.Services.Restaurant;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using Utility.Export;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using DU = Domain.Utility;

namespace WebMVC.Controllers.Restaurant;

public class FoodOrderController : AppBaseController
{
    #region Config
    private readonly IUnitOfWork _iUnitWork;
    private readonly IFoodOrderService _iService;
    private readonly IBookingServiceService _iBookingService;
    private readonly IRoomDayAuditService _iRoomDayAuditService;
    private readonly IMapper _iMapper;
    private readonly DropdownService _iDropdownService;
    private IHttpContextAccessor _iHttpContextAccessor;

    public FoodOrderController(IUnitOfWork iUnitOfWork,
                            IFoodOrderService iService,
                            IRoomDayAuditService iRoomDayAuditService,
                            IMapper iMapper,
                            DropdownService dropdownService,
                            IHttpContextAccessor iHttpContextAccessor,
                            IBookingServiceService iBookingService) : base(iUnitOfWork)
    {
        _iUnitWork = iUnitOfWork;
        _iService = iService;
        _iRoomDayAuditService = iRoomDayAuditService;
        _iMapper = iMapper;
        _iDropdownService = dropdownService;
        _iHttpContextAccessor = iHttpContextAccessor;
        _iBookingService = iBookingService;
    }
    #endregion

    #region Create
    //[Authorize(Permissions.FoodOrders.Create)]
    [Authorize(Permissions.Module.RestaurantModule)]
    public async Task<IActionResult> MakeOrder()
    {
        var model = new FoodOrderVm();
        model.OrderNo = await _iService.GetFoodOrderCode();
        model.OrderType = RsOrderTypeEnum.Order;
        model.OrderDateStr = DateTime.Now.ToString("dd/MM/yyyy");
        model.CustomerTypeLookUp = _iDropdownService.GetDefaultSelectListItem();
        model.CustomerLookUp = _iDropdownService.GetDefaultSelectListItem();
        model.RoomLookUp = await _iBookingService.GetOccupiedRoomSelectListItems();
        model.TableLookUp = _iDropdownService.GetTableSelectListItems();
        model.PayModeLookUp = _iDropdownService.GetPayModeSelectListItems();
        model.WaiterLookUp = _iDropdownService.GetWaiterSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> MakeOrder(FoodOrderVm modelVm)
    {
        modelVm.CustomerTypeLookUp = _iDropdownService.GetCustomerTypeSelectListItems();
        modelVm.CustomerLookUp = _iDropdownService.GetDefaultSelectListItem();
        modelVm.RoomLookUp = await _iBookingService.GetOccupiedRoomSelectListItems();
        modelVm.TableLookUp = _iDropdownService.GetTableSelectListItems();
        modelVm.PayModeLookUp = _iDropdownService.GetPayModeSelectListItems();
        modelVm.WaiterLookUp = _iDropdownService.GetWaiterSelectListItems();

        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return View("MakeOrder", modelVm);
            }
            _iService.CurrentUserId = UserId;
            var (isAdded, orderId) = await _iService.FoodOrderEntry(modelVm);
            if (!isAdded)
            {
                SaveFailedMsg();
                return View("MakeOrder", modelVm);
            }

            SaveSuccessMsg();
            return RedirectToAction("Details", new { id = orderId });
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("MakeOrder", modelVm);
        }
    }
    #endregion

    #region Reservation

    [Authorize(Permissions.Module.RestaurantModule)]
    public async Task<IActionResult> Reservation()
    {
        var model = new FoodOrderVm();
        model.OrderNo = await _iService.GetFoodOrderCode();
        model.OrderType = RsOrderTypeEnum.Reservation;
        model.ReservationDateStr = DateTime.Now.ToString("dd/MM/yyyy");

        model.CustomerTypeLookUp = _iDropdownService.GetDefaultSelectListItem();
        model.CustomerLookUp = _iDropdownService.GetDefaultSelectListItem();
        model.PayModeLookUp = _iDropdownService.GetPayModeSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Reservation(FoodOrderVm modelVm)
    {
        modelVm.CustomerTypeLookUp = _iDropdownService.GetCustomerTypeSelectListItems();
        modelVm.CustomerLookUp = _iDropdownService.GetDefaultSelectListItem();
        modelVm.PayModeLookUp = _iDropdownService.GetPayModeSelectListItems();

        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return View("Reservation", modelVm);
            }
            _iService.CurrentUserId = UserId;
            var (isAdded, orderId) = await _iService.FoodOrderEntry(modelVm);
            if (!isAdded)
            {
                SaveFailedMsg();
                return View("Reservation", modelVm);
            }

            SaveSuccessMsg();
            return RedirectToAction("Details", new { id = orderId });
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("Reservation", modelVm);
        }
    }

    #endregion

    #region Search
    [HttpGet]
    //[Authorize(Permissions.FoodOrders.ListView)]
    [Authorize(Permissions.Module.RestaurantModule)]
    public IActionResult Search()
    {
        var vm = new FoodOrderSearchVm();
        vm.FormDateStr = DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy");
        vm.ToDateStr = DateTime.Today.ToString("dd/MM/yyyy");
        vm.FoodOrderStatusLookUp = _iDropdownService.GetFoodOrderStatusSelectListItems();
        vm.OrderPaymentStatusLookUp = _iDropdownService.GetFoodOrderPaymentStatusSelectListItems();
        vm.OrderType = RsOrderTypeEnum.Order;
        return View(vm);
    }

    [HttpGet]
    [Authorize(Permissions.Module.RestaurantModule)]
    public IActionResult ReservationSearch()
    {
        var vm = new FoodOrderSearchVm();
        vm.FormDateStr = DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy");
        vm.ToDateStr = DateTime.Today.ToString("dd/MM/yyyy");
        vm.FoodOrderStatusLookUp = _iDropdownService.GetFoodOrderStatusSelectListItems();
        vm.OrderPaymentStatusLookUp = _iDropdownService.GetFoodOrderPaymentStatusSelectListItems();
        vm.OrderType = RsOrderTypeEnum.Reservation;
        return View(vm);
    }

    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult> Search(DataTablePagination<FoodOrderSearchVm, FoodOrderSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<FoodOrderSearchVm, FoodOrderSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new FoodOrderSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }
    #endregion

    #region Details
    //[Authorize(Permissions.FoodOrders.Details)]
    [Authorize(Permissions.Module.RestaurantModule)]
    public async Task<ActionResult> Details(long id)
    {
        try
        {
            var model = await _iService.GetFoodOrderByIdAsync(id);
            model.CustomerLookUp = _iDropdownService.GetRsCustomerSelectListItems(true, model.CustomerTypeCode);
            model.RoomLookUp = await _iBookingService.GetOccupiedRoomSelectListItems();
            model.TableLookUp = _iDropdownService.GetTableSelectListItems();
            model.PayModeLookUp = _iDropdownService.GetPayModeSelectListItems();
            model.FoodItemLookUp = _iDropdownService.GetFoodItemSelectListItems();
            model.CustomerTypeLookUp = _iDropdownService.GetCustomerTypeSelectListItems();
            model.CustomerTypeWithoutHotelLookUp = _iDropdownService.GetCustomerTypeSelectListItems(true, true);
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

    #region UpdateReservation

    public async Task<IActionResult> UpdateReservation(long id)
    {
        var model = await _iService.GetFoodOrderByIdAsync(id);
        model.PayModeLookUp = _iDropdownService.GetPayModeSelectListItems();
        model.FoodItemLookUp = _iDropdownService.GetFoodItemSelectListItems();
        model.OrderDateStr = model.OrderDate.ToString("dd/MM/yyyy");

        if (model.OrderType != RsOrderTypeEnum.Reservation)
            return RedirectToAction("Search");

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateReservation(FoodOrderVm modelVm)
    {
        modelVm.PayModeLookUp = _iDropdownService.GetPayModeSelectListItems();
        modelVm.FoodItemLookUp = _iDropdownService.GetFoodItemSelectListItems();

        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return RedirectToAction("UpdateReservation", new { id = modelVm.Id });
            }
            _iService.CurrentUserId = UserId;

            var isAdded = await _iService.UpdateOrder(modelVm);
            if (!isAdded)
            {
                SaveFailedMsg();
                return RedirectToAction("UpdateReservation", new { id = modelVm.Id });
            }

            SaveSuccessMsg();
            return RedirectToAction("Details", new { id = modelVm.Id });
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return RedirectToAction("UpdateReservation", new { id = modelVm.Id });
        }
    }

    #endregion

    #region RefundReservation

    public async Task<IActionResult> RefundReservation(long id)
    {
        var model = await _iService.GetFoodOrderByIdAsync(id);
        model.PayModeLookUp = _iDropdownService.GetPayModeSelectListItems();

        if (model.OrderType != RsOrderTypeEnum.Reservation)
            return RedirectToAction("Search");

        if (model.OrderStatus != RsOrderStatusEnum.Canceled)
        {
            FailedMsg("Reservation Is Not Canceled...!!");
            return RedirectToAction("Details", new { id = model.Id });
        }

        return View(model);
    }

    [HttpPost]
    [Authorize(Permissions.Module.RestaurantModule)]
    public async Task<ActionResult> RefundReservation(PayRsOrderVm modelVm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return RedirectToAction("RefundReservation", new { id = modelVm.OrderId });
            }
            _iService.CurrentUserId = UserId;

            var isAdded = await _iService.RefundReservation(modelVm);
            if (!isAdded)
            {
                SaveFailedMsg();
                return RedirectToAction("RefundReservation", new { id = modelVm.OrderId });
            }

            SaveSuccessMsg();
            return RedirectToAction("RefundReservation", new { id = modelVm.OrderId });
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return RedirectToAction("RefundReservation", new { id = modelVm.OrderId });
        }
    }

    #endregion

    #region OrderBillPrint
    //[Authorize(Permissions.FoodOrders.OrderBillPrint)]
    [Authorize(Permissions.Module.RestaurantModule)]
    public async Task<ActionResult> OrderBillPrint(long id)
    {
        var (html, pageHeight) = await _iService.GetOrderBillByIdAsyncHtml(id);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyRestaurantName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "";
        reportTitle.IsSignature = false;
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Manager");
        reportTitle.SignatureList.Add("");
        reportTitle.SignatureList.Add("Client");

        string reportName = "Food order details_" + DateTime.Today.ToString("dd_mm_yyy");

        //return File(export.ExportReceiptContentToPdf(html, reportName, reportTitle: reportTitle, isLandScape: false, withFooter: true), "application/pdf");
        return File(export.ExportPosBillContentLogoToPdf(html, reportName, reportTitle: reportTitle, companyImage: false, isLandScape: false, withFooter: true, pgHeight: pageHeight, left: 2, right: 10, top: 55), "application/pdf");
    }

    #endregion

    #region OrderKotPrint
    //[Authorize(Permissions.FoodOrders.OrderKotPrint)]
    [Authorize(Permissions.Module.RestaurantModule)]
    public async Task<ActionResult> OrderKotPrint(long id)
    {
        try
        {
            var (html, pageHeight) = await _iService.GenerateKot(id);

            ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
            ExportDataTitle reportTitle = new ExportDataTitle();
            reportTitle.Header = PrintInfo.CompanyRestaurantName;
            reportTitle.AddressOne = PrintInfo.CompanyAddress;
            reportTitle.ReportTitle = "";
            reportTitle.IsSignature = false;
            reportTitle.SignatureList = new List<string>();
            reportTitle.SignatureList.Add("Manager");
            reportTitle.SignatureList.Add("");
            reportTitle.SignatureList.Add("Client");

            string reportName = "Food order details_" + DateTime.Today.ToString("dd_mm_yyy");

            //return File(export.ExportReceiptContentToPdf(html, reportName, reportTitle: reportTitle, isLandScape: false, withFooter: true), "application/pdf");
            return File(export.ExportPosBillContentLogoToPdf(html, reportName, reportTitle: reportTitle, companyImage: false, isLandScape: false, withFooter: true, pgHeight: pageHeight, left: 2, right: 10, top: 55), "application/pdf");
        }
        catch (Exception ex)
        {
            FailedMsg(ex.Message);
            return RedirectToAction("Details", new { id = id });
        }

    }

    #endregion

    #region ReservationBillPrint
    [Authorize(Permissions.Module.RestaurantModule)]
    public async Task<ActionResult> ReservationBillPrint(long id)
    {
        var (html, pageHeight) = await _iService.GetReservationBillByIdAsyncHtml(id);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyRestaurantName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "";
        reportTitle.IsSignature = false;
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Manager");
        reportTitle.SignatureList.Add("");
        reportTitle.SignatureList.Add("Client");

        string reportName = "Reservation Bill_" + DateTime.Today.ToString("dd_mm_yyy");

        //return File(export.ExportReceiptContentToPdf(html, reportName, reportTitle: reportTitle, isLandScape: false, withFooter: true), "application/pdf");
        return File(export.ExportPosBillContentLogoToPdf(html, reportName, reportTitle: reportTitle, companyImage: false, isLandScape: false, withFooter: true, pgHeight: pageHeight, left: 2, right: 10, top: 55), "application/pdf");
    }

    #endregion

    #region FoodOrderStatusChange
    //[Authorize(Permissions.FoodOrders.FoodOrderStatusChange)]
    [Authorize(Permissions.Module.RestaurantModule)]
    public async Task<IActionResult> FoodOrderStatusChange(long orderId, int orderStatus)
    {
        try
        {
            _iService.CurrentUserId = UserId;
            var isUpdate = await _iService.FoodOrderStatusChangeAsync(orderId, orderStatus);
            return Ok(isUpdate);
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                message = "Change Failed..!!",
                errors = e.Message
            });
        }
    }

    #endregion

    #region PayOrder
    [HttpPost]
    //[Authorize(Permissions.FoodOrders.PayOrder)]
    [Authorize(Permissions.Module.RestaurantModule)]
    public async Task<ActionResult> PayOrder(PayRsOrderVm modelVm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return RedirectToAction("Details", new { id = modelVm.OrderId });
            }
            _iService.CurrentUserId = UserId;

            var isAdded = await _iService.PayFoodOrder(modelVm);
            if (!isAdded)
            {
                SaveFailedMsg();
                return RedirectToAction("Details", new { id = modelVm.OrderId });
            }

            SaveSuccessMsg();
            return RedirectToAction("Details", new { id = modelVm.OrderId });
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return RedirectToAction("Details", new { id = modelVm.OrderId });
        }
    }

    #endregion

    #region JsonData

    [HttpPost]
    public IActionResult GetCustomerTypeJsonData()
    {
        var dataList = _iDropdownService.GetCustomerTypeDynamicData();
        return Ok(dataList);
    }

    public async Task<IActionResult> GetFoodOrderById(long id)
    {
        var data = await _iService.GetFoodOrderByIdAsync(id);
        if (data == null)
            return BadRequest("No Item Found...!!");

        return Ok(data);
    }

    #endregion

    #region RsMonthlySalesReport

    public ActionResult RsMonthlySalesReport()
    {
        var model = new RsDailySalesReportVm();

        var today = DateTime.Today;
        //model.StrFromDate = today.ToString("dd/MM/yyyy");
        model.StrFromDate = new DateTime(today.Year, today.Month, 1).ToString("dd/MM/yyyy");
        model.StrToDate = today.ToString("dd/MM/yyyy");
        model.CustomerLookUp = _iDropdownService.GetRsCustomerSelectListItems();
        model.CustomerTypeLookUp = _iDropdownService.GetCustomerTypeSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> RsMonthlySalesReport(RsDailySalesReportVm model)
    {
        var data = await _iService.RsMonthlySalesReportHtml(model);
        return Ok(data);
    }

    public async Task<ActionResult> RsMonthlySalesReportPrint(string fromDate, string toDate, long customerId, long customerTypeId)
    {
        var model = new RsDailySalesReportVm();
        model.StrFromDate = fromDate;
        model.StrToDate = toDate;
        model.CustomerId = customerId;
        model.CustomerTypeId = customerTypeId;

        string html = await _iService.RsMonthlySalesReportHtml(model);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyRestaurantName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Statement of Order Monthly Sales";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Restraurant Monthly Sales Statement";

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 120, top: 80, left: 18, right: 18, isLandScape: true), "application/pdf");
    }

    #endregion

    #region RsCustomerWiseDueReport

    public ActionResult RsCustomerWiseDueReport()
    {
        var model = new RsCustomerWiseDueReportVm();

        var today = DateTime.Today;
        model.StrQueryDate = today.ToString("dd/MM/yyyy");
        model.CustomerLookUp = _iDropdownService.GetRsCustomerSelectListItems();
        model.CustomerTypeLookUp = _iDropdownService.GetCustomerTypeSelectListItems();
        model.GroupByList = _iDropdownService.GetDueReportGroupByList();
        model.ShowWithoutEmployee = true;
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> RsCustomerWiseDueReport(RsCustomerWiseDueReportVm model)
    {
        model.StrQueryDate = "01/Oct/2024";
        var data = await _iService.RsCustomerWiseDueReportHtml(model);
        return Ok(data);
    }

    //GROUP BY : O=ORDER WISE, C=CUSTOMER/CLIENT WISE
    public async Task<ActionResult> RsCustomerWiseDueReportPrint(long customerId = 0, string groupBy = "O", short customerTypeId = 0)
    {
        var model = new RsCustomerWiseDueReportVm();
        model.StrQueryDate = "01/Oct/2024";
        model.CustomerId = customerId;
        model.GroupBy = groupBy;
        model.CustomerTypeId = customerTypeId;

        string html = await _iService.RsCustomerWiseDueReportHtml(model);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyRestaurantName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Customer Wise Due Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Customer Wise Due Report";

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 120, top: 80, left: 18, right: 18, isLandScape: true), "application/pdf");
    }

    #endregion

    #region RsSalesReport

    public ActionResult RsSalesReport()
    {
        var model = new RsSalesReportVm();
        var today = DateTime.Today;
        model.StrFromDate = today.ToString("dd/MM/yyyy");
        model.StrToDate = today.ToString("dd/MM/yyyy");
        model.CategoryLookup = _iDropdownService.GetFoodCategorySelectListItems();
        model.ItemLookup = _iDropdownService.GetFoodItemSelectListItems();
        model.GroupByLookup = _iDropdownService.GetFoodOrderReportGroupByList();
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> RsSalesReport(RsSalesReportVm model)
    {
        var data = await _iService.RsSalesReportHtml(model);
        return Ok(data);
    }

    public async Task<ActionResult> RsSalesReportPrint(string strFromDate, string strToDate, int cateogryId = 0, int itemId = 0, string groupBy = "I")
    {
        var model = new RsSalesReportVm();
        model.StrFromDate = strFromDate;
        model.StrToDate = strToDate;
        model.CategoryId = cateogryId;
        model.ItemId = itemId;
        model.GroupBy = groupBy;

        string html = await _iService.RsSalesReportHtml(model);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyRestaurantName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Rastaurant Sales Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Rastaurant Sales Report";

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 120, top: 80, left: 18, right: 18, isLandScape: true), "application/pdf");
    }

    #endregion

    #region RsDailySalesSummary

    public ActionResult RsDailySalesSummary()
    {
        var model = new RsDailySalesSummaryVm();
        var today = DateTime.Today;
        model.StrQueryDate = today.ToString("dd/MM/yyyy");
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> RsDailySalesSummary(RsDailySalesSummaryVm model)
    {
        var data = await _iService.RsDailySalesSummaryReportHtml(model);
        return Ok(data);
    }

    public async Task<ActionResult> RsDailySalesSummaryPrint(string StrQueryDate)
    {
        var model = new RsDailySalesSummaryVm();
        model.StrQueryDate = StrQueryDate;

        string html = await _iService.RsDailySalesSummaryReportHtml(model);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyRestaurantName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Daily Sales Summary Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Cashier");
        reportTitle.SignatureList.Add("Manager F&B");
        reportTitle.SignatureList.Add("Assistant General Manager");
        reportTitle.SignatureList.Add("General Manager");
        reportTitle.IsSignature = true;

        string reportName = "Daily Sales Summary Report";

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 120, top: 80, left: 12, right: 12, isLandScape: true), "application/pdf");
    }

    #endregion

    #region FoodItemAdd

    [HttpPost]
    public async Task<IActionResult> FoodItemAdd(SaveFoodOrderItemDto dto)
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
            var isAdded = await _iService.FoodItemsAddAsync(dto);
            return Ok(isAdded);
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                message = "Entry Failed..!!",
                errors = e.Message
            });
        }
    }

    #endregion

    #region FoodItemAdd

    [HttpPost]
    public async Task<IActionResult> MultipleFoodItemAdd(List<SaveFoodOrderItemDto> dtos)
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
            var isAdded = await _iService.FoodMultipleItemsAddAsync(dtos);
            return Ok(isAdded);
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                message = "Entry Failed..!!",
                errors = e.Message
            });
        }
    }

    #endregion

    #region DeleteFoodItem
    [HttpGet]
    public async Task<IActionResult> DeleteFoodItem(long id)
    {
        try
        {
            if (id > 0)
            {
                _iService.CurrentUserId = UserId;
                var isDelete = await _iService.FoodItemRemoveAsync(id);
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

    #region DiscountUpdate

    [HttpPost]
    public async Task<IActionResult> DiscountUpdate(DiscountUpdateDto dto)
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
            var isAdded = await _iService.DiscountUpdateAsync(dto);
            return Ok(isAdded);
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                message = "Entry Failed..!!",
                errors = e.Message
            });
        }
    }

    #endregion 

    #region AssignRoom

    [HttpPost]
    public async Task<IActionResult> AssignRoom(AssignRoomVm dto)
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
            var isAdded = await _iService.RoomAssign(dto);
            return Ok(isAdded);
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                message = "Entry Failed..!!",
                errors = e.Message
            });
        }
    }

    #endregion

    #region SingleKotPrint
    [Authorize(Permissions.Module.RestaurantModule)]
    public async Task<ActionResult> SingleKotPrint(long id, string kotNo)
    {
        try
        {
            var (html, pageHeight) = await _iService.SingleKotHtml(id, kotNo);

            ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
            ExportDataTitle reportTitle = new ExportDataTitle();
            reportTitle.Header = PrintInfo.CompanyRestaurantName;
            reportTitle.AddressOne = PrintInfo.CompanyAddress;
            reportTitle.ReportTitle = "";
            reportTitle.IsSignature = false;
            reportTitle.SignatureList = new List<string>();
            reportTitle.SignatureList.Add("Manager");
            reportTitle.SignatureList.Add("");
            reportTitle.SignatureList.Add("Client");

            string reportName = "Food order details_" + DateTime.Today.ToString("dd_mm_yyy");

            return File(export.ExportPosBillContentLogoToPdf(html, reportName, reportTitle: reportTitle, companyImage: false, isLandScape: false, withFooter: true, pgHeight: pageHeight, left: 2, right: 10, top: 55), "application/pdf");
        }
        catch (Exception ex)
        {
            FailedMsg(ex.Message);
            return RedirectToAction("Details", new { id = id });
        }
    }

    #endregion

    #region SingleFoodItemServed
    [HttpGet]
    public async Task<IActionResult> SingleFoodItemServed(long id)
    {
        try
        {
            if (id > 0)
            {
                _iService.CurrentUserId = UserId;
                var isServed = await _iService.FoodItemServedAsync(id);
                return Ok(isServed);
            }
            return BadRequest();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }
    #endregion

    #region Customer Type Update

    [HttpPost]
    public async Task<IActionResult> CustomerTypeUpdate(CustomerTypeUpdateDto dto)
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
            var isAdded = await _iService.CusomerTypeUpdateAsync(dto);
            return Ok(isAdded);
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                message = "Entry Failed..!!",
                errors = e.Message
            });
        }
    }

    #endregion 

    #region Customer Name Update

    [HttpPost]
    public async Task<IActionResult> CustomerNameUpdate(CustomerNameUpdateDto dto)
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
            var isAdded = await _iService.CustomerNameUpdateAsync(dto);
            return Ok(isAdded);
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                message = "Entry Failed..!!",
                errors = e.Message
            });
        }
    }

    #endregion

    #region DeletePayment
    [HttpGet]
    public async Task<IActionResult> DeletePayment(long paymentId)
    {
        try
        {
            if (paymentId > 0)
            {
                _iService.CurrentUserId = UserId;
                var isDelete = await _iService.PaymentRemoveAsync(paymentId);

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

    #region DeleteOrder
    [HttpGet]
    public async Task<IActionResult> DeleteOrder(long orderId)
    {
        try
        {
            if (orderId > 0)
            {
                _iService.CurrentUserId = UserId;
                var isDelete = await _iService.OrderRemoveAsync(orderId);

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

    #region RsPaymentTransectionReport

    public ActionResult RsPaymentTransectionReport()
    {
        var model = new RsTransectionReportVm();

        var today = DateTime.Today;
        //model.StrFromDate = today.ToString("dd/MM/yyyy");
        model.StrFromDate = today.ToString("dd/MM/yyyy");
        model.StrToDate = today.ToString("dd/MM/yyyy");
        model.CustomerLookUp = _iDropdownService.GetRsCustomerSelectListItems();
        model.CustomerTypeLookUp = _iDropdownService.GetCustomerTypeSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> RsPaymentTransectionReport(RsTransectionReportVm model)
    {
        var data = await _iService.RsPaymentTransectionReportHtml(model);
        return Ok(data);
    }

    public async Task<ActionResult> RsPaymentTransectionReportPrint(string fromDate, string toDate, long customerId, long customerTypeId)
    {
        var model = new RsTransectionReportVm();
        model.StrFromDate = fromDate;
        model.StrToDate = toDate;
        model.CustomerId = customerId;
        model.CustomerTypeId = customerTypeId;

        string html = await _iService.RsPaymentTransectionReportHtml(model);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyRestaurantName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Statement of Restaurant Transections";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Restaurant Transections Statement";

        //return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 120, top: 80, left: 18, right: 18, isLandScape: true), "application/pdf");
        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 90, top: 80, left: 18, right: 18, isLarge: true, isLandScape: true), "application/pdf");
    }

    #endregion

}
