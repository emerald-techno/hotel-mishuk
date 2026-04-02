using AutoMapper;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.Billing;
using Domain.ViewModel.Restaurant.FoodOrder;
using Interface.Services.HotelManagement;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using Utility.Export;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;

namespace WebMVC.Controllers.HotelManagement;

public class BillController : AppBaseController
{
    #region Config
    private readonly IUnitOfWork _iUnitWork;
    private readonly IBillService _iService;
    private readonly IBookingServiceService _iBookingServiceService;
    private readonly IMapper _iMapper;
    private readonly DropdownService _dropdownService;
    private IHttpContextAccessor _iHttpContextAccessor;

    public BillController(IUnitOfWork iUnitOfWork,
                            IBillService iService,
                            IMapper iMapper,
                            DropdownService dropdownService,
                            IBookingServiceService iBookingServiceService,
                            IHttpContextAccessor iHttpContextAccessor) : base(iUnitOfWork)
    {
        _iUnitWork = iUnitOfWork;
        _iService = iService;
        _iMapper = iMapper;
        _dropdownService = dropdownService;
        _iBookingServiceService = iBookingServiceService;
        _iHttpContextAccessor = iHttpContextAccessor;
    }
    #endregion

    #region Details
    //[Authorize(Permissions.Bills.Details)]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public async Task<ActionResult> Details(long id)
    {
        try
        {

            var model = await _iService.GetBillByIdAsync(id);
            model.PayModeLookUp = _dropdownService.GetPayModeSelectListItems();
            model.ServiceLookUp = _dropdownService.GetServiceSelectListItems();
            model.PaidDateStr = DateTime.Now.ToString("dd/MM/yyyy");

            model.ServiceLookUp = _dropdownService.GetExtraServiceSelectListItems(true, true);
            model.BookingRoomLookUp = await _iBookingServiceService.GetBookingOccupiedRoom(model.BookingId);
            //model.TransactionNo =  await _iBookingServiceService.GetHtPaymentAutoCode();
            return View(model);
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("_404");
        }
    }

    #endregion

    #region Search
    [HttpGet]
    //[Authorize(Permissions.Bills.ListView)]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public IActionResult Search()
    {
        var vm = new BillingSearchVm();
        vm.FormDateStr = DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy");
        vm.ToDateStr = DateTime.Today.ToString("dd/MM/yyyy");
        vm.BillStatusLookUp = _dropdownService.GetBillStatusSelectListItems();
        return View(vm);
    }

    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult> Search(DataTablePagination<BillingSearchVm, BillingSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<BillingSearchVm, BillingSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new BillingSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }
    #endregion

    #region PayBill

    [HttpPost]
    //[Authorize(Permissions.Bills.PayBills)]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public async Task<ActionResult> PayBill(PayBillVm modelVm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return RedirectToAction("Details", new { id = modelVm.BillId });
            }
            _iService.CurrentUserId = UserId;

            var isAdded = await _iService.PayBill(modelVm);

            if (!isAdded)
            {
                SaveFailedMsg();
                return RedirectToAction("Details", new { id = modelVm.BillId });
            }

            SaveSuccessMsg();
            return RedirectToAction("Details", new { id = modelVm.BillId });
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return RedirectToAction("Details", new { id = modelVm.BillId });
        }
    }

    #endregion

    #region BillPrint

    public async Task<ActionResult> BillPrint(long id)
    {
        string html = await _iService.GetBillByIdAsyncHtml(id);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "";
        reportTitle.IsSignature = true;
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Authorized Signature");
        reportTitle.SignatureList.Add("Guest Signature");

        string reportName = "Bill_" + DateTime.Today.ToString("dd_mm_yyy");

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 120, top: 80, left: 18, right: 18, isLandScape: false, withFooter: true), "application/pdf");
    }

    #endregion

    #region BillServiceAdd

    [HttpPost]
    public async Task<IActionResult> BillServiceAdd(SaveBillingDetailVm modelVm)
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
            }

            _iService.CurrentUserId = UserId;
            var isAdded = await _iService.BillDetailAddAsync(modelVm);
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

    #region BillDetail
    //[Authorize(Permissions.Bills.Details)]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public async Task<ActionResult> BillDetail(long id)
    {
        try
        {
            var model = await _iService.GetBillByIdAsync(id);
            //var model = new BillingVm();
            //model.Id = id;
            model.PayModeLookUp = _dropdownService.GetPayModeSelectListItems();
            model.ServiceLookUp = _dropdownService.GetServiceSelectListItems();
            model.PaidDateStr = DateTime.Now.ToString("dd/MM/yyyy");
            return View(model);
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("_404");
        }
    }
    [HttpPost]
    public async Task<ActionResult> GetBillingDetailById(long id)
    {
        var data = await _iService.GetBillDetailHtmlById(id);

        return Ok(data);
    }

    public async Task<ActionResult> BillDetailPrint(long id)
    {
        var html = await _iService.GetBillDetailHtmlById(id);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = "GUEST INVOICE";
        reportTitle.AddressOne = "";
        reportTitle.ReportTitle = "";
        reportTitle.IsSignature = true;
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Guest Signature");
        reportTitle.SignatureList.Add("Authorized Signature");
        reportTitle.IsHeaderLogo = true;
        string reportName = "Invoice";

        //return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 120, top: 80, left: 20, right: 20, isLandScape: true,isLarge:true), "application/pdf");
        //return File(export.ExportBillContentToPdf(html, reportName, companyImage: true, reportTitle: reportTitle, bottom: 110, top: 90, left: 21, right: 21), "application/pdf");
        return File(export.ExportBillContentToPdfWithLogo(html, reportName, companyImage: true, reportTitle: reportTitle, bottom: 110, top: 90, left: 21, right: 21), "application/pdf");
    }
    #endregion

    #region MakeBillComplimentary
    [HttpPost]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public async Task<IActionResult> MakeBillComplimentary(MakeComplimentaryVm modelVm)
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
            }
            ;

            _iService.CurrentUserId = UserId;
            var isAdded = await _iService.MakeComplimentary(modelVm);
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

    #region DiscountUpdate

    [HttpPost]
    public async Task<IActionResult> DiscountUpdate(BillDiscountUpdateDto dto)
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
            }
            ;

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

    #region BillClose

    [HttpPost]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public async Task<IActionResult> BillClose(long billId)
    {
        try
        {
            _iService.CurrentUserId = UserId;
            var isClose = await _iService.FullPaymentBillClose(billId);
            return Ok(isClose);
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                message = "Bill Close Failed..!!",
                errors = e.Message
            });
        }
    }

    #endregion

    #region DeleteBillService
    [HttpGet]
    public async Task<IActionResult> BillServiceDelete(long id)
    {
        try
        {
            if (id > 0)
            {
                _iService.CurrentUserId = UserId;
                var isDelete = await _iService.BillServiceRemoveAsync(id);
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
}
