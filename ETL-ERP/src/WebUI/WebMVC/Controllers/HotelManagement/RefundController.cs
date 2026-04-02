using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Enums.AppEnums;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.AdvanceRefund;
using Domain.ViewModel.HotelManagement.RefundReport;
using Domain.ViewModel.Report;
using Interface.Services.HotelManagement;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using Utility.Export;
using WebMVC.Controllers.Base;
using DU = Domain.Utility;

namespace WebMVC.Controllers.HotelManagement;

public class RefundController : AppBaseController
{
    #region Config
    private readonly IUnitOfWork _iUnitWork;
    private readonly IAdvanceRefundService _iService;
    private readonly IMapper _iMapper;
    private readonly DropdownService _dropdownService;
    private IHttpContextAccessor _iHttpContextAccessor;

    public RefundController(IUnitOfWork iUnitOfWork,
                            IAdvanceRefundService iService,
                            IMapper iMapper,
                            DropdownService dropdownService,
                            IHttpContextAccessor iHttpContextAccessor) : base(iUnitOfWork)
    {
        _iUnitWork = iUnitOfWork;
        _iService = iService;
        _iMapper = iMapper;
        _dropdownService = dropdownService;
        _iHttpContextAccessor = iHttpContextAccessor;
    }
    #endregion

    #region AdvanceRefund

    public IActionResult AdvanceRefund()
    {
        var model = new AdvanceRefundVm();
        model.BookingServiceLookUp = _dropdownService.GetRefundCancelBookingSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AdvanceRefund(AdvanceRefundVm modelVm)
    {
        modelVm.BookingServiceLookUp = _dropdownService.GetRefundCancelBookingSelectListItems();

        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return View("AdvanceRefund", modelVm);
            }
            _iService.CurrentUserId = UserId;
            var model = _iMapper.Map<HtAdvanceRefund>(modelVm);
            model.RefundDate = DU.Utility.GetBdDateTimeNow();
            model.RefundMode = PayModeEnum.Cash;
            model.ActionById = UserId;
            model.ActionDate = DU.Utility.GetBdDateTimeNow();

            var isAdded = await _iService.AddAsync(model);
            if (!isAdded)
            {
                SaveFailedMsg();
                return View("AdvanceRefund", modelVm);
            }
            SaveSuccessMsg();

            return RedirectToAction("AdvanceRefund");
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("AdvanceRefund", modelVm);
        }
    }

    #endregion

    #region HtRefund

    public ActionResult HtRefundReport()
    {
        var model = new RefundReportVm();

        var today = DateTime.Today;
        //model.StrFromDate = today.ToString("dd/MM/yyyy");
        model.StrFromDate = new DateTime(today.Year, today.Month, 1).ToString("dd/MM/yyyy");
        model.StrToDate = today.ToString("dd/MM/yyyy");
        model.GuestLookUp = _dropdownService.GetGuestSelectListItems();
        model.CompayLookUp = _dropdownService.GetClientCompanySelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> HtRefundReport(RefundReportVm model)
    {
        var data = await _iService.HtRefundReportHtml(model);
        return Ok(data);
    }

    public async Task<ActionResult> HtRefundReportPrint(string fromDate, string toDate, long? guestId, long? companyId)
    {
        var model = new RefundReportVm();
        model.StrFromDate = fromDate;
        model.StrToDate = toDate;
        model.GuestId = guestId;
        model.CompanyId = companyId;

        string html = await _iService.HtRefundReportHtml(model);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.IsHeaderLogo = true;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Statement of Refund";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Restraurant Monthly Sales Statement";

        return File(export.ExportContentToPdfWithLogo(html, reportName, reportTitle: reportTitle, bottom: 90, top: 110, left: 18, right: 18, isLarge: true, isLandScape: true, companyImage: true), "application/pdf");
    }

    #endregion

    #region JsonData

    [HttpGet]
    public async Task<ActionResult> GetCancelBookingInfoBookingId(long bookingId)
    {
        try
        {
            var bookingInfo = await _iService.PrepareAdvanceRefund(bookingId);
            return Ok(bookingInfo);
        }
        catch (Exception ex)
        {
            return Ok(SetError(ex.Message));
        }
    }

    #endregion
}
