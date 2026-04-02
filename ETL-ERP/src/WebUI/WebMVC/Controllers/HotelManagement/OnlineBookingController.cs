using AutoMapper;
using Domain.Enums.AppEnums;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.Complementary;
using Domain.ViewModel.HotelManagement.OnlineBooking;
using Interface.Services.HotelManagement;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.HotelManagement;
using Utility.CachingUtility;
using Utility.Export;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;

namespace WebMVC.Controllers.HotelManagement;

public class OnlineBookingController : AppBaseController
{
    #region Config
    private readonly IUnitOfWork _iUnitWork;
    private readonly IOnlineBookingService _iService;
    private readonly IMapper _iMapper;
    private readonly DropdownService _dropdownService;
    private IHttpContextAccessor _iHttpContextAccessor;

    public OnlineBookingController(IUnitOfWork iUnitOfWork,
                            IOnlineBookingService iService,
                            IMapper iMapper,
                            IHttpContextAccessor iHttpContextAccessor,
                            DropdownService dropdownService) : base(iUnitOfWork)
    {
        _iUnitWork = iUnitOfWork;
        _iService = iService;
        _iMapper = iMapper;
        _iHttpContextAccessor = iHttpContextAccessor;
        _dropdownService = dropdownService;
    }
    #endregion

    #region Delete
    [HttpGet]
    //[Authorize(Permissions.OnlineBooking.Delete)]
    [Authorize(Permissions.Module.HotelManagementModule)]
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

    #region Approve
    [HttpGet]
    //[Authorize(Permissions.OnlineBooking.Approve)]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public async Task<IActionResult> Approve(long id)
    {
        var data = await _iService.GetByIdAsync(id);
        if (data == null)
        {
            DeleteFailedMsg();
            return NotFound();
        }
        data.Status = OnlineBookingStatusEnum.Approved;
        UpdateSuccessMsg("Approve success and SMS has been send..!!");
        var isRemove = _iService.Update(data);
        return RedirectToAction("Search");
    }
    #endregion

    #region Reject
    [HttpGet]
    //[Authorize(Permissions.OnlineBooking.Reject)]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public async Task<IActionResult> Reject(long id)
    {
        var data = await _iService.GetByIdAsync(id);
        if (data == null)
        {
            DeleteFailedMsg();
            return NotFound();
        }
        data.Status = OnlineBookingStatusEnum.Canceled;
        UpdateSuccessMsg("Booking has been rejected..!!");
        var isRemove = _iService.Update(data);
        return RedirectToAction("Search");
    }
    #endregion

    #region Search

    [HttpGet]
    //[Authorize(Permissions.OnlineBooking.ListView)]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public IActionResult Search()
    {
        var vm = new OnlineBookingSearchVm()
        {
            StatusLookUp = _dropdownService.GetApprovalStatusSelectListItems()
        };
        return View(vm);
    }


    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<OnlineBookingSearchVm, OnlineBookingSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<OnlineBookingSearchVm, OnlineBookingSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new OnlineBookingSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion

    #region Details
    [HttpGet]
    public async Task<IActionResult> Details(long id)
    {
        try
        {
            if (id > 0 is false)
                return RedirectToAction("Index");

            var model = await _iService.GetBookingBillByIdAsync(id);

            return View(model);
        }

        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }
    #endregion

    #region OnlineBookingBillPrint

    public async Task<ActionResult> OnlineBookingBillPrint(long id)
    {
        var html = await _iService.GetOnlineBookingBillByIdAsyncHtml(id);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = "";
        reportTitle.AddressOne = "";
        reportTitle.ReportTitle = "";
        reportTitle.IsSignature = false;
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Manager");
        reportTitle.SignatureList.Add("");
        reportTitle.SignatureList.Add("Client");

        string reportName = "Online Booking details_" + DateTime.Today.ToString("dd_mm_yyy");

        //return File(export.ExportReceiptContentToPdf(html, reportName, reportTitle: reportTitle, isLandScape: false, withFooter: true), "application/pdf");
        return File(export.ExportBillContentToPdfWithLogo(html, reportName, companyImage: true, reportTitle: reportTitle, bottom: 110, top: 80, left: 21, right: 21), "application/pdf");
    }
    #endregion

    #region JsonData

    public async Task<IActionResult> GetOnlineBookingById(long id)
    {
        var data = await _iService.GetBookingBillByIdAsync(id);
        if (data == null)
        {
            return BadRequest("Online Booking Not Found..!!!");
        }
        var model = _iMapper.Map<OnlineBookingVm>(data);
        return Ok(model);
    }

    #endregion
}
