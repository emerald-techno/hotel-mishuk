using AutoMapper;
using Domain.Enums.AppEnums;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.BillPayment;
using Domain.ViewModel.Inventory.Order;
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

public class OrderController : AppBaseController
{
    #region Config

    private readonly IMapper _iMapper;
    private readonly IOrderService _iService;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly DropdownService _dropdownService;
    private readonly CacheStoreService _cacheStoreService;
    private IHttpContextAccessor _iHttpContextAccessor;
    private readonly INtfNotificationMsgService _iNtfNotificationMsgService;

    public OrderController(IUnitOfWork iUnitOfWork,
        IOrderService iService,
        IMapper iMapper,
        DropdownService dropdownService,
        CacheStoreService cacheStoreService,
        IHttpContextAccessor iHttpContextAccessor,
        INtfNotificationMsgService iNtfNotificationMsgService) : base(iUnitOfWork)
    {
        _iUnitOfWork = iUnitOfWork;
        _iService = iService;
        _dropdownService = dropdownService;
        _iMapper = iMapper;
        _iHttpContextAccessor = iHttpContextAccessor;
        _iNtfNotificationMsgService = iNtfNotificationMsgService;
    }

    #endregion

    #region Create

    [HttpGet]
    [Authorize(Permissions.Order.Create)]
    public async Task<IActionResult> Create()
    {
        var model = new OrderVm();
        model.OrderDateStr = DateTime.Now.ToString("dd/MM/yyyy");
        model.OrderNo = await _iService.GetOrderCode(); // It will be system generated
        model.SupplierLookUp = _dropdownService.GetSupplierSelectListItems();
        //model.RequsitionLookUp = _dropdownService.GetApproveRequsitionNotIssuedSeletListItems();
        model.RequsitionLookUp = _dropdownService.GetOrderRequsitionSeletListItems();
        model.ItemLookUp = _dropdownService.GetItemSelectListItems();
        model.UnitLookUp = _dropdownService.GetUnitSelectListItems();
        model.OrderType = "I";
        return View(model);
    }

    public async Task<IActionResult> Create(OrderVm modelVm)
    {
        modelVm.SupplierLookUp = _dropdownService.GetSupplierSelectListItems();
        //modelVm.RequsitionLookUp = _dropdownService.GetApproveRequsitionNotIssuedSeletListItems();
        modelVm.RequsitionLookUp = _dropdownService.GetOrderRequsitionSeletListItems();
        modelVm.ItemLookUp = _dropdownService.GetItemSelectListItems();
        modelVm.UnitLookUp = _dropdownService.GetUnitSelectListItems();

        try
        {
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
    [Authorize(Permissions.Order.ListView)]
    public IActionResult Search()
    {
        var vm = new OrderSearchVm()
        {
            SupplierLookUp = _dropdownService.GetSupplierSelectListItems(),
            RequstionLookup = _dropdownService.GetRequisitionSelectListItems()
        };
        return View(vm);
    }

    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<OrderSearchVm, OrderSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<OrderSearchVm, OrderSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new OrderSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }
    #endregion

    #region Detail
    [Authorize(Permissions.Order.Detail)]
    public async Task<ActionResult> Details(long id)
    {
        try
        {
            var model = await _iService.GetOrderDataAsync(id);
            var vm = _iMapper.Map<OrderVm>(model);
            return View(vm);
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("_404");
        }
    }

    #endregion

    #region JSON Data

    [HttpPost]
    public IActionResult GetOrderJsonData(long reqId)
    {
        var dataList = _dropdownService.GetOrderDynamicData(reqId);
        return Ok(dataList);
    }

    public async Task<IActionResult> GetById(long id)
    {
        try
        {
            var data = await _iService.GetOrderDataAsync(id);
            return Ok(data);
        }
        catch (Exception e)
        {
            return Ok(SetError(e.Message));
        }
    }

    public async Task<IActionResult> OrderItemsForReceive(long orderId)
    {
        try
        {
            var data = await _iService.GetOrderItemForReceive(orderId);
            return Ok(data);
        }
        catch (Exception e)
        {
            return BadRequest(SetError(e.Message));
        }
    }

    public async Task<IActionResult> OrderBill(long orderId)
    {
        try
        {
            var data = await _iService.GetBillDataAsync(orderId);
            return Ok(data);
        }
        catch (Exception e)
        {
            return BadRequest(SetError(e.Message));
        }
    }

    [HttpPost]
    public IActionResult GetOrderBySupplier(long supplierId)
    {
        var dataList = _iService.GetOrderDynamicDataBySupplier(supplierId);
        return Ok(dataList);
    }

    [HttpPost]
    public IActionResult GetReceiveOrderBySupplier(long supplierId)
    {
        var dataList = _iService.GetReceivedOrderDynamicDataBySupplier(supplierId);
        return Ok(dataList);
    }


    #endregion

    #region Approval

    [HttpGet]
    public IActionResult ApprovalSearch()
    {
        var vm = new OrderSearchVm()
        {
            SupplierLookUp = _dropdownService.GetSupplierSelectListItems(),
            RequstionLookup = _dropdownService.GetRequisitionSelectListItems()
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> SubmitReview(long orderId)
    {
        try
        {
            var order = _iService.GetById(orderId);
            if (order == null)
            {
                SaveFailedMsg("Order Not Found");
                return RedirectToAction("Search");
            }

            order.Status = (short)OrderStatusEnum.REVIEW;

            var isUpdate = _iService.Update(order);
            if (!isUpdate)
            {
                SaveFailedMsg("Can't Submit For Review");
            }

            var actionUrl = Url.Action("ReviewOrder", "Order", new { id = order.Id }, Request.Scheme);
            string ntfMsgHtml = @$"<a href='{actionUrl}' target='_blank'><b>Order {order.OrderNo}-({order.OrderDate.ToString("dd/MM/yyyy")}) Submited For Review..</b></a>";
            string emailMsg = $@"Order {order.OrderNo}-({order.OrderDate.ToString("dd/MM/yyyy")}) Submited For Review..";

            var ntfGenerated = await _iNtfNotificationMsgService.GenerateNtf(NotificationEventCode.OrderReviewNtf, ntfMsgHtml, emailMsg);

            SaveSuccessMsg("Order has been submitted for Review");
            return RedirectToAction("Search");
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return RedirectToAction("Search");
        }
    }

    [HttpGet]
    public async Task<IActionResult> ReviewOrder(long id)
    {
        try
        {
            var modelVm = await _iService.GetOrderDataAsync(id);
            modelVm.OrderDateStr = DU.Utility.ConvertDateToStr(modelVm.OrderDate);

            return View(modelVm);
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("_404");
        }
    }

    [HttpPost]
    public async Task<IActionResult> ApproveOrder(OrderApprovalVm modelVm)
    {
        modelVm.Status = (short)OrderStatusEnum.APPROVED;
        _iService.CurrentUserId = UserId;
        var actionUrl = Url.Action("Details", "Order", new { id = modelVm.Id }, Request.Scheme);
        var isUpdated = await _iService.ReviewUpdate(modelVm, actionUrl);
        return Ok(isUpdated);
    }

    [HttpPost]
    public IActionResult RejectOrder(long orderId)
    {
        var order = _iService.GetById(orderId);

        if (order == null) return Ok(false);

        order.Status = (short)OrderStatusEnum.REJECTED;

        var isUpdated = _iService.Update(order);

        return Ok(isUpdated);
    }


    #endregion

    #region PayBill
    [Authorize(Permissions.Order.PayBill)]
    public IActionResult OrderPayBill()
    {
        var model = new InventoryBillPaymentVm();
        model.PayModeLookUp = _dropdownService.GetPayModeSelectListItems();
        model.OrderLookUp = _dropdownService.GetReceiveOrderSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> OrderPayBill(InventoryBillPaymentVm modelVm)
    {
        modelVm.PayModeLookUp = _dropdownService.GetPayModeSelectListItems();
        modelVm.OrderLookUp = _dropdownService.GetReceiveOrderSelectListItems();

        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return RedirectToAction("OrderPayBill");
            }
            _iService.CurrentUserId = UserId;

            var isAdded = await _iService.OrderPayBill(modelVm);
            if (!isAdded)
            {
                SaveFailedMsg();
                return RedirectToAction("OrderPayBill");
            }

            SaveSuccessMsg();
            return RedirectToAction("OrderPayBill");
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return RedirectToAction("OrderPayBill");
        }
    }

    #endregion

    #region OrderPrint

    public async Task<ActionResult> OrderPrint(long id)
    {
        string html = await _iService.GetOrderByIdAsyncHtml(id);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "";
        reportTitle.IsSignature = true;
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Store Manager");
        reportTitle.SignatureList.Add("Authorized Signature");

        string reportName = "Purchase Order" + DateTime.Today.ToString("dd_mm_yyy");

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 120, top: 80, left: 18, right: 18, isLandScape: false, withFooter: true), "application/pdf");
    }

    #endregion
}
