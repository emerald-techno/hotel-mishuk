using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Enums.AppEnums;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.BusinessDaySummary;
using Domain.ViewModel.HotelManagement.FD_PaymentTranReport;
using Domain.ViewModel.HotelManagement.HotelReport;
using Domain.ViewModel.HotelManagement.RoomDayAudit;
using Domain.ViewModel.Report;
using Interface.Services.HotelManagement;
using Interface.Services.Restaurant;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using Utility.Export;
using WebMVC.Controllers.Base;
using DU = Domain.Utility;

namespace WebMVC.Controllers.HotelManagement;

public class NightAuditController : AppBaseController
{
    #region Config
    private readonly IUnitOfWork _iUnitWork;
    private readonly IRoomDayAuditService _iRoomDayAuditService;
    private readonly IFoodOrderService _iFoodOrderService;
    private readonly IMapper _iMapper;
    private readonly DropdownService _dropdownService;
    private readonly IHttpContextAccessor _iHttpContextAccessor;

    public NightAuditController(IUnitOfWork iUnitOfWork,
                            IRoomDayAuditService iRoomDayAuditService,
                            IMapper iMapper,
                            DropdownService dropdownService,
                            IHttpContextAccessor iHttpContextAccessor,
                            IFoodOrderService iFoodOrderService) : base(iUnitOfWork)
    {
        _iUnitWork = iUnitOfWork;
        _iRoomDayAuditService = iRoomDayAuditService;
        _iMapper = iMapper;
        _dropdownService = dropdownService;
        _iHttpContextAccessor = iHttpContextAccessor;
        _iFoodOrderService = iFoodOrderService;
    }
    #endregion

    #region RoomAudit

    [HttpGet]
    public async Task<ActionResult> RoomAudit()
    {
        var model = new RoomDayAuditVm();
        var businessDay = await _iRoomDayAuditService.GetCurrentBusinessDay();
        model.BusinessDate = businessDay != null ? businessDay.BusinessDate : DateTime.Now;
        model.AuditedInfo = await _iRoomDayAuditService.IsAudited(model.BusinessDate);
        return View(model);
    }

    [HttpGet]
    public async Task<ActionResult> RoomAuditWithDate()
    {
        var model = new RoomDayAuditVm();
        var businessDay = await _iRoomDayAuditService.GetCurrentBusinessDay();
        model.BusinessDate = businessDay != null ? businessDay.BusinessDate : DateTime.Now;
        model.AuditedInfo = await _iRoomDayAuditService.IsAudited(model.BusinessDate);
        return View(model);
    }

    [HttpPost]
    public async Task<PartialViewResult> GetRoomAuditPartial(string businessDateStr)
    {
        try
        {
            var businessDate = (DateTime)(!string.IsNullOrEmpty(businessDateStr) ? DU.Utility.ConvertStrToDate(businessDateStr) : DateTime.Now);
            _iRoomDayAuditService.CurrentUserId = UserId;

            var model = await _iRoomDayAuditService.GenerateRoomDayAuditsForDateAsync(businessDate);
            return model != null ? PartialView("PartialView/_RoomAudit", model) : PartialView("_404");
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return PartialView("_404");
        }
    }
    #endregion

    #region MultiRoomAuditApproval
    public async Task<ActionResult> MultiRoomAuditApproval(List<long> ids)
    {
        try
        {
            if (ids == null && ids.Count == 0)
                return BadRequest();

            var updateList = new List<HtRoomDayAudit>();
            var auditList = await _iRoomDayAuditService.GetAsync(x => ids.Contains(x.Id));

            if (auditList != null && auditList.Count > 0)
            {
                foreach (var audit in auditList)
                {
                    audit.IsCharged = true;
                    audit.Status = AuditStayStatus.Stayed;
                    updateList.Add(audit);
                }
            }

            var result = await _iRoomDayAuditService.UpdateRangeAsync(updateList);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    #endregion

    #region Room Audit Print
    public async Task<ActionResult> RoomAuditPrint(string businessDateStr)
    {
        var businessDate = (DateTime)(!string.IsNullOrEmpty(businessDateStr) ? DU.Utility.ConvertStrToDate(businessDateStr) : DateTime.Now);

        string html = await _iRoomDayAuditService.RoomAuditHtml(businessDate);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Room Audit";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Room Audit";

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 60, top: 80, left: 18, right: 18, isLandScape: true, width: 1500), "application/pdf");

    }
    #endregion

    #region NightAuditSummary

    [HttpGet]
    public async Task<ActionResult> NightAuditSummary()
    {
        var model = new BusinessDaySummaryVm();
        var businessDay = await _iRoomDayAuditService.GetCurrentBusinessDay();
        model.AuditStatus = businessDay != null ? businessDay.AuditStatus : (short)0;
        model.BusinessDate = businessDay != null ? businessDay.BusinessDate : DateTime.Now;
        return View(model);
    }

    [HttpPost]
    public async Task<PartialViewResult> GetNightAuditSummaryPartial(string businessDateStr)
    {
        try
        {
            var businessDate = (DateTime)(!string.IsNullOrEmpty(businessDateStr) ? DU.Utility.ConvertStrToDate(businessDateStr) : DateTime.Now);

            var model = await _iRoomDayAuditService.GetNightAuditSummary(businessDate);
            return model != null ? PartialView("PartialView/_NightAuditSummaryPartial", model) : PartialView("_404");
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return PartialView("_404");
        }
    }

    [HttpPost]
    public async Task<ActionResult> CloseBusinessDay(string businessDateStr)
    {
        try
        {
            var businessDate = (DateTime)(!string.IsNullOrEmpty(businessDateStr) ? DU.Utility.ConvertStrToDate(businessDateStr) : DateTime.Now);
            _iRoomDayAuditService.CurrentUserId = UserId;
            var isClose = await _iRoomDayAuditService.CloseBusinessDay(businessDate);
            return Ok(isClose);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    #endregion

    #region FrontDesk Payment Audit
    [HttpPost]
    public async Task<ActionResult> GetPaymentTransectionAuditPartial(string businessDateStr)
    {
        try
        {
            var model = await _iRoomDayAuditService.GetPaymentTransactionForDateAsync(new PaymentTransactionReportVm { StrFromDate = businessDateStr, StrToDate = businessDateStr });
            return model != null ? PartialView("PartialView/_PaymentTransectionAudit", model) : PartialView("_404");
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return PartialView("_404");
        }
    }
    #endregion

    #region FoPaymentAuditApproval
    public async Task<ActionResult> FoPaymentAuditApproval(List<long> ids)
    {
        try
        {
            if (ids == null && ids.Count == 0)
                return BadRequest();

            var result = await _iRoomDayAuditService.ApproveFoPaymentsAsync(ids, UserId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    #endregion

    #region Restaurant Audit
    [HttpPost]
    public async Task<ActionResult> GeRestaurantAuditPartial(string businessDateStr)
    {
        try
        {
            var model = await _iRoomDayAuditService.GetRestaurantAuditDataForDateAsync(new RsDailySalesReportVm { StrFromDate = businessDateStr, StrToDate = businessDateStr });
            return model != null ? PartialView("PartialView/_RestaurantAudit", model) : PartialView("_404");
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return PartialView("_404");
        }
    }
    #endregion

    #region MultiRsOrderAuditApproval
    public async Task<ActionResult> MultiRsOrderAuditApproval(List<long> ids)
    {
        try
        {
            if (ids == null && ids.Count == 0)
                return BadRequest();

            var updateList = new List<RsFoodOrder>();
            var auditList = await _iFoodOrderService.GetAsync(x => ids.Contains(x.Id));

            if (auditList != null && auditList.Count > 0)
            {
                foreach (var audit in auditList)
                {
                    audit.AuditById = UserId;
                    audit.AuditDate = DU.Utility.GetBdDateTimeNow();
                    audit.AuditRemarks = "Auto Audit Approve";
                    updateList.Add(audit);
                }
            }

            var result = await _iFoodOrderService.UpdateRangeAsync(updateList);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    #endregion

    #region Restaurant Payment Audit
    [HttpPost]
    public async Task<ActionResult> GeRestaurantPaymentAuditPartial(string businessDateStr)
    {
        try
        {
            var model = await _iRoomDayAuditService.GetRestaurentPaymentAuditDataForDateAsync(new RsTransectionReportVm { StrFromDate = businessDateStr, StrToDate = businessDateStr });
            return model != null ? PartialView("PartialView/_RestaurantPaymentAudit", model) : PartialView("_404");
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return PartialView("_404");
        }
    }
    #endregion

    #region RsPaymentAuditApproval
    public async Task<ActionResult> RsPaymentAuditApproval(List<long> ids)
    {
        try
        {
            if (ids == null && ids.Count == 0)
                return BadRequest();

            var result = await _iRoomDayAuditService.ApproveRsPaymentsAsync(ids, UserId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    #endregion

    #region Service Audit Partial
    [HttpPost]
    public async Task<ActionResult> GetServiceAuditPartial(string businessDateStr)
    {
        try
        {
            var model = await _iRoomDayAuditService.GetServiceAuditDataForDateAsync(new ExtraServiceReportVm { StrFromDate = businessDateStr, StrToDate = businessDateStr });
            return model != null ? PartialView("PartialView/_ServiceAudit", model) : PartialView("_404");
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return PartialView("_404");
        }
    }
    #endregion

    #region HallAuditPartial
    [HttpPost]
    public async Task<ActionResult> GetHallAuditPartial(string businessDateStr)
    {
        try
        {
            var model = await _iRoomDayAuditService.GetHallAuditDataForDateAsync(new BookingHallReportVm { StrFromDate = businessDateStr, StrToDate = businessDateStr });
            return model != null ? PartialView("PartialView/_HallAudit", model) : PartialView("_404");
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return PartialView("_404");
        }
    }
    #endregion

    #region MultiHallAuditApproval
    public async Task<ActionResult> MultiHallAuditApproval(List<long> ids)
    {
        try
        {
            if (ids == null && ids.Count == 0)
                return BadRequest();

            var result = await _iRoomDayAuditService.ApproveHallBookingAsync(ids, UserId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    #endregion

    #region MultiExtraServiceAuditApproval
    public async Task<ActionResult> MultiExtraServiceAuditApproval(List<long> ids)
    {
        try
        {
            if (ids == null && ids.Count == 0)
                return BadRequest();

            var result = await _iRoomDayAuditService.ApproveExtraServiceAsync(ids, UserId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    #endregion

    #region AuditSummaryReport

    public async Task<ActionResult> AuditSummary()
    {
        var model = new BusinessDaySummaryVm();
        var businessDay = await _iRoomDayAuditService.GetCurrentBusinessDay();
        model.AuditStatus = businessDay != null ? businessDay.AuditStatus : (short)0;
        model.BusinessDate = businessDay != null ? businessDay.BusinessDate : DateTime.Now;
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> NightAuditSummaryReport(string businessDateStr)
    {
        try
        {
            var businessDate = (DateTime)(!string.IsNullOrEmpty(businessDateStr) ? DU.Utility.ConvertStrToDate(businessDateStr) : DateTime.Now);
            var dataHtml = await _iRoomDayAuditService.GetAuditSummaryHtml(businessDate);
            return Ok(dataHtml);

        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<ActionResult> NightAuditSummaryReportPrint(string businessDateStr)
    {
        var businessDate = (DateTime)(!string.IsNullOrEmpty(businessDateStr) ? DU.Utility.ConvertStrToDate(businessDateStr) : DateTime.Now);
        string html = await _iRoomDayAuditService.GetAuditSummaryHtml(businessDate);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.IsHeaderLogo = false;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Night Audit Summary";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Night_Audit_Summary";

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 50, top: 80, left: 18, right: 18), "application/pdf");
    }

    #endregion

    #region Room Audit Reports
    [HttpPost]
    public async Task<IActionResult> GetRoomAuditHtml(string businessDateStr)
    {
        try
        {
            var businessDate = (DateTime)(!string.IsNullOrEmpty(businessDateStr) ? DU.Utility.ConvertStrToDate(businessDateStr) : DateTime.Now);
            var html = await _iRoomDayAuditService.GenerateRoomAuditHtmlAsync(businessDate);

            return Ok(html);
        }
        catch (Exception ex)
        {
            return BadRequest("Error generating room audit HTML: " + ex.Message);
        }
    }
    public async Task<ActionResult> GetRoomAuditHtmlPrint(string businessDateStr)
    {
        var businessDate = (DateTime)(!string.IsNullOrEmpty(businessDateStr) ? DU.Utility.ConvertStrToDate(businessDateStr) : DateTime.Now);

        string html = await _iRoomDayAuditService.GenerateRoomAuditHtmlAsync(businessDate, true);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);

        ExportDataTitle reportTitle = new ExportDataTitle();

        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Room Audit Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Room_Audit_Report";

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 60, top: 80, left: 18, right: 18, isLandScape: true, width: 1500), "application/pdf");
    }


    #endregion

    #region FO Payment Transection Report
    [HttpPost]
    public async Task<IActionResult> GetPaymentTransactionHtml(string businessDateStr)
    {
        try
        {
            var html = await _iRoomDayAuditService.GeneratePaymentTransactionHtmlAsync(businessDateStr);
            return Ok(html);
        }
        catch (Exception ex)
        {
            return BadRequest("Error generating payment transaction HTML: " + ex.Message);
        }
    }

    public async Task<ActionResult> PaymentTransactionAuditPrint(string businessDateStr)
    {
        string html = await _iRoomDayAuditService.GeneratePaymentTransactionHtmlAsync(businessDateStr, true);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);

        ExportDataTitle reportTitle = new ExportDataTitle();

        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Payment Transaction Audit";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Payment_Transaction_Audit";

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 60, top: 80, left: 18, right: 18, isLandScape: true, width: 1500), "application/pdf");
    }
    #endregion

    #region RsOrder Audit Report
    [HttpPost]
    public async Task<IActionResult> GetRestaurantAuditHtml(string businessDateStr)
    {
        try
        {
            var html = await _iRoomDayAuditService.GenerateRestaurantAuditHtmlAsync(businessDateStr);
            return Ok(html);
        }
        catch (Exception ex)
        {
            return BadRequest("Error generating restaurant audit HTML: " + ex.Message);
        }
    }

    public async Task<ActionResult> RestaurantAuditPrint(string businessDateStr)
    {

        string html = await _iRoomDayAuditService.GenerateRestaurantAuditHtmlAsync(businessDateStr, true);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);

        ExportDataTitle reportTitle = new ExportDataTitle();

        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Restaurant Audit Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;
        string reportName = "Restaurant_Audit_Report";

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 60, top: 80, left: 18, right: 18, isLandScape: true, width: 1500), "application/pdf");
    }

    #endregion

    #region RS Payment Transection Report
    [HttpPost]
    public async Task<IActionResult> GetRestaurantPaymentAuditHtml(string businessDateStr)
    {
        try
        {
            var html = await _iRoomDayAuditService.GenerateRestaurantPaymentAuditHtmlAsync(businessDateStr);
            return Ok(html);
        }
        catch (Exception ex)
        {
            return BadRequest("Error generating restaurent payment transaction HTML: " + ex.Message);
        }
    }

    public async Task<ActionResult> RestaurantPaymentAuditPrint(string businessDateStr)
    {
        string html = await _iRoomDayAuditService.GenerateRestaurantPaymentAuditHtmlAsync(businessDateStr, true);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);

        ExportDataTitle reportTitle = new ExportDataTitle();

        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Restaurant Audit Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Restaurant_Payment_Audit";

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 60, top: 80, left: 18, right: 18, isLandScape: true, width: 1500), "application/pdf");
    }
    #endregion

    #region Service Audit Report
    [HttpPost]
    public async Task<IActionResult> GetServiceAuditHtml(string businessDateStr)
    {
        try
        {
            var html = await _iRoomDayAuditService.GenerateServiceAuditHtmlAsync(businessDateStr);
            return Ok(html);
        }
        catch (Exception ex)
        {
            return BadRequest("Error generating service audit HTML: " + ex.Message);
        }
    }

    public async Task<ActionResult> ServiceAuditPrint(string businessDateStr)
    {
        string html = await _iRoomDayAuditService.GenerateServiceAuditHtmlAsync(businessDateStr, true);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);

        ExportDataTitle reportTitle = new ExportDataTitle();

        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Service Audit Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Service_Audit_Report";

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 60, top: 80, left: 18, right: 18, isLandScape: true, width: 1500), "application/pdf");
    }

    #endregion

    #region GetHallAuditHtml
    [HttpPost]
    public async Task<IActionResult> GetHallAuditHtml(string businessDateStr)
    {
        try
        {
            var html = await _iRoomDayAuditService.GenerateHallAuditHtmlAsync(businessDateStr);
            return Ok(html);
        }
        catch (Exception ex)
        {
            return BadRequest("Error generating hall audit HTML: " + ex.Message);
        }
    }

    public async Task<ActionResult> HallAuditPrint(string businessDateStr)
    {
        string html = await _iRoomDayAuditService.GenerateHallAuditHtmlAsync(businessDateStr, true);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);

        ExportDataTitle reportTitle = new ExportDataTitle();

        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Hall Audit Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Hall_Audit_Report";

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 60, top: 80, left: 18, right: 18, isLandScape: true, width: 1500), "application/pdf");
    }

    #endregion

    #region RoomReAudit

    [HttpPost]
    public async Task<IActionResult> RoomReAudit(string businessDateStr)
    {
        try
        {
            var businessDate = (DateTime)(!string.IsNullOrEmpty(businessDateStr) ? DU.Utility.ConvertStrToDate(businessDateStr) : DateTime.Now);

            _iRoomDayAuditService.CurrentUserId = UserId;
            var count = await _iRoomDayAuditService.RoomReAuditAsync(businessDate);

            return Ok(new
            {
                success = true,
                reAuditedRooms = count
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    #endregion

    #region JsonData

    [HttpGet]
    public async Task<ActionResult> GetAuditInfoByDate(string businessDateStr)
    {
        try
        {
            var businessDate = (DateTime)(!string.IsNullOrEmpty(businessDateStr) ? DU.Utility.ConvertStrToDate(businessDateStr) : DateTime.Now);

            var auditInfos = await _iRoomDayAuditService.IsAudited(businessDate);
            return Ok(auditInfos);
        }
        catch (Exception ex)
        {
            return Ok(SetError(ex.Message));
        }
    }

    #endregion
}