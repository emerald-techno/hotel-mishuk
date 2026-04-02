using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Enums.AppEnums;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.Billing;
using Domain.ViewModel.HotelManagement.Booking;
using Domain.ViewModel.HotelManagement.FD_PaymentTranReport;
using Domain.ViewModel.HotelManagement.HotelReport;
using Domain.ViewModel.HotelManagement.RoomBooking;
using Interface.Services.HotelManagement;
using Interface.Services.Notification;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using Utility.Export;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using DU = Domain.Utility;

namespace WebMVC.Controllers.HotelManagement;

public class BookingServiceController : AppBaseController
{
    #region Config
    private readonly IUnitOfWork _iUnitWork;
    private readonly IBookingServiceService _iService;
    private readonly IMapper _iMapper;
    private readonly DropdownService _iDropdownService;
    private readonly IDateBreakfastService _iDateBreakfastService;
    private readonly IHttpContextAccessor _iHttpContextAccessor;
    private readonly IAppEmailSender _iAppSmsSender;
    private readonly INtfNotificationMsgService _iNtfNotificationMsgService;
    private readonly IBillService _iBillService;

    public BookingServiceController(IUnitOfWork iUnitOfWork,
                            IBookingServiceService iService,
                            IMapper iMapper,
                            DropdownService dropdownService,
                            IHttpContextAccessor iHttpContextAccessor,
                            IDateBreakfastService iDateBreakfastService,
                            IAppEmailSender iAppSmsSender,
                            IBillService iBillService,
                            INtfNotificationMsgService iNtfNotificationMsgService) : base(iUnitOfWork)
    {
        _iUnitWork = iUnitOfWork;
        _iService = iService;
        _iMapper = iMapper;
        _iDropdownService = dropdownService;
        _iHttpContextAccessor = iHttpContextAccessor;
        _iDateBreakfastService = iDateBreakfastService;
        _iAppSmsSender = iAppSmsSender;
        _iBillService = iBillService;
        _iNtfNotificationMsgService = iNtfNotificationMsgService;
    }
    #endregion

    #region Booking
    //[Authorize(Permissions.BookingServices.Create)]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public async Task<IActionResult> Booking()
    {
        var model = new HtBookingServiceVm();
        model.PayModeLookUp = _iDropdownService.GetPayModeSelectListItems();
        model.ComplementaryLookUp = _iDropdownService.GetComplementaryListItems();
        model.OnlineBookingLookUp = _iDropdownService.GetApprovedOnlineBookingSelectListItems();
        model.CompanyLookUp = _iDropdownService.GetClientCompanySelectListItems();
        model.CountryWithCodeLookUp = _iDropdownService.GetCountrySelectListItemsWithCode();
        model.DistrictLookUp = _iDropdownService.GetDistrictSelectListItems();
        model.GenderLookUp = _iDropdownService.GetGenderSelectListItems();

        model.PaymentVm = new HtBookingPaymentVm();
        model.PaymentVm.TransactionNo = await _iService.GetHtPaymentAutoCode();

        model.BookingDateStr = DateTime.Now.ToString("dd/MM/yyyy");
        model.PaidDateStr = DateTime.Now.ToString("dd/MM/yyyy");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Booking(HtBookingServiceVm modelVm)
    {
        modelVm.PayModeLookUp = _iDropdownService.GetPayModeSelectListItems();
        modelVm.OnlineBookingLookUp = _iDropdownService.GetApprovedOnlineBookingSelectListItems();

        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return View("Booking", modelVm);
            }
            _iService.CurrentUserId = UserId;

            var (isAdded, bookingId) = await _iService.RoomBookingEntry(modelVm);
            if (!isAdded)
            {
                SaveFailedMsg();
                return View("Booking", modelVm);
            }

            SaveSuccessMsg();
            return RedirectToAction("Details", new { id = bookingId });
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("Booking", modelVm);
        }
    }
    #endregion

    #region UpdateBooking
    [Authorize(Permissions.Module.HotelManagementModule)]
    public async Task<IActionResult> UpdateBooking(long id)
    {
        var model = await _iService.GetBookingInfoById(id);
        model.PayModeLookUp = _iDropdownService.GetPayModeSelectListItems();

        if (model.BookingStatus != BookingServiceStatusEnum.Booked)
            return RedirectToAction("Search");

        model.ComplementaryLookUp = _iDropdownService.GetComplementaryListItems();
        model.CheckInTimeStr = model.CheckInTime.ToString("dd/MM/yyyy");
        model.CheckOutTimeStr = model.CheckOutTime.ToString("dd/MM/yyyy");

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateBooking(HtBookingServiceVm modelVm)
    {
        modelVm.PayModeLookUp = _iDropdownService.GetPayModeSelectListItems();
        modelVm.ComplementaryLookUp = _iDropdownService.GetComplementaryListItems();
        modelVm.CheckInTimeStr = modelVm.CheckInTime.ToString("dd/MM/yyyy");
        modelVm.CheckOutTimeStr = modelVm.CheckOutTime.ToString("dd/MM/yyyy");
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return RedirectToAction("UpdateBooking", new { id = modelVm.Id });
            }
            _iService.CurrentUserId = UserId;

            var isAdded = await _iService.UpdateBooking(modelVm);
            if (!isAdded)
            {
                SaveFailedMsg();
                return RedirectToAction("UpdateBooking", new { id = modelVm.Id });
            }

            SaveSuccessMsg();
            return RedirectToAction("Details", new { id = modelVm.Id });
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return RedirectToAction("UpdateBooking", new { id = modelVm.Id });
        }
    }

    #endregion

    #region Search
    [HttpGet]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public IActionResult Search()
    {
        var vm = new BookingServiceSearchVm();
        vm.BookingServiceStatusLookUp = _iDropdownService.GetBookingServiceStatusSelectListItems();
        vm.PaymentStatusLookUp = _iDropdownService.GetPayemntStatusSelectListItems();
        vm.OnlineBookingLookUp = _iDropdownService.GetApprovedOnlineBookingSelectListItems();
        //vm.FormDateStr = DateTime.Today.AddDays(-2).ToString("dd/MM/yyyy");
        //vm.ToDateStr = DateTime.Today.ToString("dd/MM/yyyy");
        return View(vm);
    }


    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<BookingServiceSearchVm, BookingServiceSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<BookingServiceSearchVm, BookingServiceSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new BookingServiceSearchVm();
        searchVm.SearchModel.BookingType = BookingType.Room;
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion

    #region CheckIn
    //[Authorize(Permissions.BookingServices.CheckIn)]
    [Authorize(Permissions.Module.HotelManagementModule)]

    public async Task<IActionResult> CheckIn(long id)
    {
        var model = await _iService.GetBookingInfoById(id);
        model.PayModeLookUp = _iDropdownService.GetPayModeSelectListItems();
        model.CountryWithCodeLookUp = _iDropdownService.GetCountrySelectListItemsWithCode();
        model.GenderLookUp = _iDropdownService.GetGenderSelectListItems();
        model.DistrictLookUp = _iDropdownService.GetDistrictSelectListItems();
        model.CompanyLookUp = _iDropdownService.GetClientCompanySelectListItems();
        model.PaidDateStr = DateTime.Now.ToString("dd/MM/yyyy");
        model.CheckInTimeStr = model.CheckInTime.ToString("dd/MM/yyyy");
        model.CheckOutTimeStr = model.CheckOutTime.ToString("dd/MM/yyyy");

        model.PaymentVm = new HtBookingPaymentVm();
        model.PaymentVm.TransactionNo = await _iService.GetHtPaymentAutoCode();

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> CheckIn(HtBookingServiceVm modelVm)
    {
        modelVm.PayModeLookUp = _iDropdownService.GetPayModeSelectListItems();
        modelVm.CountryWithCodeLookUp = _iDropdownService.GetCountrySelectListItemsWithCode();
        modelVm.GenderLookUp = _iDropdownService.GetGenderSelectListItems();
        modelVm.DistrictLookUp = _iDropdownService.GetDistrictSelectListItems();
        modelVm.CompanyLookUp = _iDropdownService.GetClientCompanySelectListItems();

        modelVm.CheckInTimeStr = modelVm.CheckInTime.ToString("dd/MM/yyyy");
        modelVm.CheckOutTimeStr = modelVm.CheckOutTime.ToString("dd/MM/yyyy");
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return RedirectToAction("CheckIn", new { id = modelVm.Id });
            }
            _iService.CurrentUserId = UserId;

            var isAdded = await _iService.RoomCheckIn(modelVm);
            if (!isAdded)
            {
                SaveFailedMsg();
                return RedirectToAction("CheckIn", new { id = modelVm.Id });
            }

            _iBillService.CurrentUserId = UserId;
            var isBillGenerated = await _iBillService.GenerateInitialBill(modelVm.Id);

            if (isAdded && modelVm.VDRooms.Count > 0)
            {
                SaveWarningMsg($"The following room(s): {string.Join(',', modelVm.VDRooms)} could not be checked in due to cleaning status. Please review their availability or cleaning status.");
                return RedirectToAction("Details", new { id = modelVm.Id });
            }

            SaveSuccessMsg();
            return RedirectToAction("Details", new { id = modelVm.Id });
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return RedirectToAction("CheckIn", new { id = modelVm.Id });
        }
    }

    #endregion

    #region UpdateCheckIn
    [Authorize(Permissions.Module.HotelManagementModule)]
    public async Task<IActionResult> UpdateCheckIn(long id)
    {
        var model = await _iService.GetBookingInfoById(id);
        model.PayModeLookUp = _iDropdownService.GetPayModeSelectListItems();
        if (model.BookingStatus != BookingServiceStatusEnum.CheckIn)
            return RedirectToAction("Search");
        model.ComplementaryLookUp = _iDropdownService.GetComplementaryListItems();
        model.CheckInTimeStr = model.CheckInTime.ToString("dd/MM/yyyy");
        model.CheckOutTimeStr = model.CheckOutTime.ToString("dd/MM/yyyy");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateCheckIn(HtBookingServiceVm modelVm)
    {
        modelVm.PayModeLookUp = _iDropdownService.GetPayModeSelectListItems();
        modelVm.ComplementaryLookUp = _iDropdownService.GetComplementaryListItems();
        modelVm.CheckOutTimeStr = modelVm.CheckOutTime.ToString("dd/MM/yyyy");
        modelVm.CheckInTimeStr = modelVm.CheckInTime.ToString("dd/MM/yyyy");

        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return RedirectToAction("UpdateCheckIn", new { id = modelVm.Id });
            }
            _iService.CurrentUserId = UserId;

            var isAdded = await _iService.RoomCheckInUpdate(modelVm);

            if (!isAdded)
            {
                SaveFailedMsg();
                return RedirectToAction("UpdateCheckIn", new { id = modelVm.Id });
            }
            if (isAdded && modelVm.VDRooms.Count > 0)
            {
                SaveWarningMsg($"The following room(s): {string.Join(',', modelVm.VDRooms)} could not be checked in due to their current status. Please review their availability or cleaning status.");
                return RedirectToAction("Details", new { id = modelVm.Id });
            }
            if (isAdded && modelVm.RoomsWithExtraService.Count > 0)
            {
                SaveWarningMsg($"The following room(s): {string.Join(',', modelVm.RoomsWithExtraService)} could not be removed because these room(s) contains extra service.");
                return RedirectToAction("Details", new { id = modelVm.Id });
            }

            SaveSuccessMsg();
            return RedirectToAction("Details", new { id = modelVm.Id });
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return RedirectToAction("UpdateCheckIn", new { id = modelVm.Id });
        }
    }

    #endregion

    #region Details_Old
    //[Authorize(Permissions.Module.HotelManagementModule)]
    //public async Task<IActionResult> Details(long id)
    //{
    //    var model = await _iService.GetBookingInfoById(id);
    //    model.PaidDateStr = DateTime.Now.ToString("dd/MM/yyyy");
    //    model.PayModeLookUp = _iDropdownService.GetPayModeSelectListItems();
    //    return View(model);
    //}
    #endregion

    #region Details

    [Authorize(Permissions.Module.HotelManagementModule)]
    public async Task<IActionResult> Details(long id)
    {
        _iService.CurrentUserId = UserId;
        var model = await _iService.GetBookingInfoById(id);
        model.PaidDateStr = DateTime.Now.ToString("dd/MM/yyyy");
        model.PayModeLookUp = _iDropdownService.GetPayModeSelectListItems();
        model.ComplementaryLookUp = _iDropdownService.GetComplementaryListItems();

        if (model.BookingBill == null)
            model.BookingBill = new BillingVm();

        model.BookingBill.ServiceLookUp = _iDropdownService.GetExtraServiceSelectListItems(true, true);
        model.BookingBill.BookingRoomLookUp = await _iService.GetBookingOccupiedRoom(model.Id);

        model.PaymentVm = new HtBookingPaymentVm();
        model.PaymentVm.TransactionNo = model.BookingStatus != BookingServiceStatusEnum.CheckOut ? await _iService.GetHtPaymentAutoCode() : "";
        return View(model);
    }
    #endregion

    #region CheckOut
    //[Authorize(Permissions.BookingServices.CheckOut)]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public async Task<IActionResult> CheckOut(long id)
    {
        var model = await _iService.GetBookingInfoById(id);
        if (model.BookingStatus != BookingServiceStatusEnum.CheckIn)
        {
            FailedMsg("Booking Is Not Check-In Yet...!!");
            return RedirectToAction("Search");
        }
        return View(model);
    }


    [HttpPost]
    public async Task<IActionResult> CheckOut(HtBookingServiceVm modelVm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return View("CheckOut", modelVm);
            }
            _iService.CurrentUserId = UserId;

            // change if auto refund remove..::Tawkir
            modelVm.IsRefund = true;
            var (isCheckOut, billId) = await _iService.RoomCheckOut(modelVm);
            if (!isCheckOut)
            {
                SaveFailedMsg();
                return RedirectToAction("CheckOut", new { id = modelVm.Id });
            }

            SaveSuccessMsg();
            return RedirectToAction("Details", "Bill", new { id = billId });
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return RedirectToAction("CheckOut", new { id = modelVm.Id });
        }
    }

    #endregion

    #region BookingPayment
    //[Authorize(Permissions.BookingServices.Payment)]
    [Authorize(Permissions.Module.HotelManagementModule)]
    [HttpPost]
    public async Task<IActionResult> PaymentEntry(BookingPaymentDto modelVm)
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
            var isAdded = await _iService.PaymentEntry(modelVm);
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

    #region JsonData

    [HttpGet]
    public async Task<ActionResult> GetBookedRoomByBookingId(long id)
    {
        try
        {
            var rooms = await _iService.GetBookedRoomListByBookingId(id);
            return Ok(rooms);
        }
        catch (Exception ex)
        {
            return Ok(SetError(ex.Message));
        }
    }

    [HttpGet]
    public async Task<ActionResult> GetBookedHallByBookingId(long id)
    {
        try
        {
            var halls = await _iService.GetBookedHallListByBookingId(id);
            return Ok(halls);
        }
        catch (Exception ex)
        {
            return Ok(SetError(ex.Message));
        }
    }

    [HttpGet]
    public async Task<ActionResult> GetBookingGuestByBookingId(long id)
    {
        try
        {
            var guestList = await _iService.GetBookingGuestListByBookingId(id);
            return Ok(guestList);
        }
        catch (Exception ex)
        {
            return Ok(SetError(ex.Message));
        }
    }

    public async Task<IActionResult> GetHallByDate(string bookingDateStr, int shift)
    {
        var bookingDate = (DateTime)(!string.IsNullOrEmpty(bookingDateStr) ? DU.Utility.ConvertStrToDate(bookingDateStr) : DateTime.Now);
        var data = await _iService.GetDynamicAvailableHallByDateAndShift(bookingDate, shift);
        return Ok(data);
    }

    [HttpPost]
    public IActionResult GetHallStatusJsonData()
    {
        var dataList = _iDropdownService.GetHallShiftDynamicData();
        return Ok(dataList);
    }

    #endregion

    #region BookingServiceReport

    public ActionResult BookingServiceReport()
    {
        var model = new BookingServiceReportVm();

        var today = DateTime.Today;
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        model.StrFromDate = monthStart.ToString("dd/MM/yyyy");
        model.StrToDate = monthEnd.ToString("dd/MM/yyyy");
        model.RoomCategoryLookUp = _iDropdownService.GetRoomCategorySelectListItems();
        model.RoomLookUp = _iDropdownService.GetRoomSelectListItems();
        model.FloorLookUp = _iDropdownService.GetFloorSelectListItems();
        model.BookingReportStatusLookUp = _iDropdownService.GetBookingReportStatusSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> BookingServiceReport(BookingServiceReportVm model)
    {
        var data = await _iService.GetBookingServiceReportHtml(model);
        return Ok(data);
    }

    public async Task<ActionResult> BookingServiceReportPrint(string fromDate, string toDate, long? categoryId, long? roomId, long? floorId, short? bookingStatus)
    {
        var model = new BookingServiceReportVm();
        model.StrFromDate = fromDate;
        model.StrToDate = toDate;
        model.RoomCategoryId = categoryId ?? 0;
        model.RoomId = roomId ?? 0;
        model.FloorId = floorId ?? 0;
        model.BookingStatus = bookingStatus ?? 0;

        string html = await _iService.GetBookingServiceReportHtml(model, true);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        //reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.IsHeaderLogo = true;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Statement of Room Booking";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Room Booking Statement";

        //return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 120, top: 80, left: 18, right: 18, isLandScape: false), "application/pdf");
        return File(export.ExportContentToPdfWithLogo(html, reportName, reportTitle: reportTitle, bottom: 90, top: 110, left: 18, right: 18, isLarge: true, isLandScape: true, companyImage: true), "application/pdf");
    }

    #endregion

    #region TodayArrivalReport

    public ActionResult TodayArrival()
    {
        var model = new BookingArrivalVm();
        var today = DateTime.Today;
        model.StrQueryDate = today.ToString("dd/MM/yyyy");
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> TodayArrivalReport(BookingArrivalVm model)
    {
        var data = await _iService.GetArrivalReportHtml(model);
        return Ok(data);
    }

    public async Task<ActionResult> TodayArrivalReportPrint(string queryDate)
    {
        var model = new BookingArrivalVm();
        model.StrQueryDate = queryDate;

        string html = await _iService.GetArrivalReportHtml(model, true);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        //reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.IsHeaderLogo = true;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Today Arrival List";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Arrival List";

        return File(export.ExportContentToPdfWithLogo(html, reportName, reportTitle: reportTitle, bottom: 90, top: 110, left: 18, right: 18, isLarge: true, isLandScape: false, companyImage: true), "application/pdf");
    }

    #endregion

    #region ExpectedCheckOutReport

    public ActionResult TodayCheckOut()
    {
        var model = new BookingArrivalVm
        {
            StrFromDate = DateTime.Today.Date.ToString("yyyy-MM-dd"),
            StrToDate = DateTime.Today.Date.ToString("yyyy-MM-dd")
        };
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> TodayCheckOutReport(string strFromDate, string strToDate)
    {
        var vm = new BookingArrivalVm
        {
            StrFromDate = strFromDate,
            StrToDate = strToDate
        };

        var data = await _iService.GetTodayCheckOutReportHtml(vm);
        return Ok(data);
    }

    public async Task<ActionResult> TodayCheckOutReportPrint(string strFromDate, string strToDate)
    {
        var vm = new BookingArrivalVm
        {
            StrFromDate = strFromDate ?? DateTime.Today.Date.ToString("yyyy-MM-dd"),
            StrToDate = strToDate ?? DateTime.Today.Date.ToString("yyyy-MM-dd")
        };

        string html = await _iService.GetTodayCheckOutReportHtml(vm, true);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.IsHeaderLogo = true;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Expected Departure List";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Checkout List";

        return File(export.ExportContentToPdfWithLogo(html, reportName, reportTitle: reportTitle, bottom: 90, top: 110, left: 18, right: 18, isLarge: true, isLandScape: true, companyImage: true), "application/pdf");
    }

    #endregion

    #region CheckOutReport

    public ActionResult CheckOutReport()
    {
        var model = new DepartureReportVm
        {
            StrFromDate = DateTime.Today.Date.ToString("yyyy-MM-dd"),
            StrToDate = DateTime.Today.Date.ToString("yyyy-MM-dd")
        };

        return View(model);
    }


    [HttpPost]
    public async Task<ActionResult> CheckOutReport(string strFromDate, string strToDate)
    {
        try
        {
            //vm.StrFromDate = DateTime.Today.Date.ToString("yyyy-MM-dd");
            //vm.StrToDate = DateTime.Today.Date.ToString("yyyy-MM-dd");

            var vm = new DepartureReportVm
            {
                StrFromDate = strFromDate,
                StrToDate = strToDate
            };
            var data = await _iService.GetCheckOutReportHtml(vm);
            return Ok(data);

        }
        catch (Exception ex)
        {

            throw new Exception(ex.Message);
        }
    }

    public async Task<ActionResult> CheckOutReportPrint(string strFromDate, string strToDate)
    {
        var vm = new DepartureReportVm
        {
            StrFromDate = strFromDate ?? DateTime.Today.Date.ToString("yyyy-MM-dd"),
            StrToDate = strToDate ?? DateTime.Today.Date.ToString("yyyy-MM-dd")
        };

        string html = await _iService.GetCheckOutReportHtml(vm, true);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        //reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.IsHeaderLogo = true;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Check-Out List";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Checkout List";

        return File(export.ExportContentToPdfWithLogo(html, reportName, reportTitle: reportTitle, bottom: 90, top: 110, left: 18, right: 18, isLarge: true, isLandScape: true), "application/pdf");
    }

    #endregion

    #region InHouseGuestReport

    public ActionResult InHouseGuest()
    {
        var model = new RoomWiseGuestVm();
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> InHouseGuestReport()
    {
        var data = await _iService.InHouseGuestReportHtml();
        return Ok(data);
    }

    public async Task<ActionResult> InHouseGuestReportPrint()
    {
        string html = await _iService.InHouseGuestReportHtml(true);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        //reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.IsHeaderLogo = true;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Daily In House Guest List";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "In House Guest List";

        return File(export.ExportContentToPdfWithLogo(html, reportName, reportTitle: reportTitle, bottom: 90, top: 110, left: 18, right: 18, isLandScape: true, isLarge: true, companyImage: true), "application/pdf");
    }

    #endregion

    #region DailySalesReport

    public ActionResult RoomDailySales()
    {
        var model = new RoomDailySalesReportVm();
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> RoomDailySalesReport()
    {
        var data = await _iService.RoomDailySalesReportHtml();
        return Ok(data);
    }

    public async Task<ActionResult> RoomDailySalesReportPrint()
    {
        string html = await _iService.RoomDailySalesReportHtml(true);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.IsHeaderLogo = true;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Daily Sales Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Daily Sales Report";

        //return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 120, top: 80, left: 18, right: 18, isLandScape: false), "application/pdf");
        return File(export.ExportContentToPdfWithLogo(html, reportName, reportTitle: reportTitle, bottom: 90, top: 110, left: 18, right: 18, isLandScape: true, isLarge: true, companyImage: true), "application/pdf");
    }

    #endregion

    #region No Show Booking
    [HttpGet]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public async Task<IActionResult> BookingNoShow(long id)
    {
        try
        {
            _iService.CurrentUserId = UserId;

            var (isNoShow, billId) = await _iService.BookingNoShow(id);
            if (!isNoShow)
            {
                SaveFailedMsg();
                return RedirectToAction("Details", new { id = id });
            }

            SaveSuccessMsg();
            return RedirectToAction("Details", "Bill", new { id = billId });
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return RedirectToAction("Details", new { id = id });
        }
    }
    #endregion

    #region BookingDailySalesReport

    public ActionResult BookingDailySalesReport()
    {
        var model = new BookingDailySalesReportVm();

        var today = DateTime.Today;
        model.StrQueryDate = today.ToString("dd/MM/yyyy");
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> BookingDailySalesReport(BookingDailySalesReportVm model)
    {
        var data = await _iService.GetBookingDailySalesReportHtml(model);
        return Ok(data);
    }

    public async Task<ActionResult> BookingDailySalesReportPrint(string queryDate)
    {
        var model = new BookingDailySalesReportVm();
        model.StrQueryDate = queryDate;


        string html = await _iService.GetBookingDailySalesReportHtml(model, true);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.IsHeaderLogo = true;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Statement of Room Booking";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Room Booking Statement";

        //return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 120, top: 80, left: 20, right: 20, isLandScape: true, isLarge: true, width: 1100, height: 595), "application/pdf");
        return File(export.ExportContentToPdfWithLogo(html, reportName, reportTitle: reportTitle, bottom: 70, top: 110, left: 18, right: 18, isLandScape: true, isLegalPage: true, companyImage: true), "application/pdf");
    }

    #endregion

    #region RoomAvailibility

    public ActionResult RoomAvailibility()
    {
        var model = new RoomAvailabilityVm();
        model.CategoryLookup = _iDropdownService.GetRoomCategorySelectListItems();
        return View(model);
    }

    #endregion

    #region Hall Booking

    #region HallBookingEntry

    [Authorize(Permissions.Module.HotelManagementModule)]
    public IActionResult HallBooking()
    {
        var model = new HtBookingServiceVm();
        model.PayModeLookUp = _iDropdownService.GetPayModeSelectListItems();
        model.HallBookingShiftLookUp = _iDropdownService.GetHallShiftSelectListItems();
        model.BookingDateStr = DateTime.Now.ToString("dd/MM/yyyy");
        model.PaidDateStr = DateTime.Now.ToString("dd/MM/yyyy");
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> HallBooking(HtBookingServiceVm modelVm)
    {
        modelVm.PayModeLookUp = _iDropdownService.GetPayModeSelectListItems();
        modelVm.HallBookingShiftLookUp = _iDropdownService.GetHallShiftSelectListItems();

        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return View("HallBooking", modelVm);
            }
            _iService.CurrentUserId = UserId;

            var isAdded = await _iService.HallBookingEntry(modelVm);
            if (!isAdded)
            {
                SaveFailedMsg();
                return View("HallBooking", modelVm);
            }

            SaveSuccessMsg();
            return RedirectToAction("HallBooking");
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("HallBooking", modelVm);
        }
    }

    #endregion

    #region HallDetails
    [Authorize(Permissions.Module.HotelManagementModule)]
    public async Task<IActionResult> HallDetails(long id)
    {
        var model = await _iService.GetHallBookingInfoById(id);
        model.PaidDateStr = DateTime.Now.ToString("dd/MM/yyyy");
        model.PayModeLookUp = _iDropdownService.GetPayModeSelectListItems();
        return View(model);
    }
    #endregion

    #region UpdateHallBooking

    [Authorize(Permissions.Module.HotelManagementModule)]
    public async Task<IActionResult> UpdateHallBooking(long id)
    {
        var model = await _iService.GetHallBookingInfoById(id);

        if (model.BookingStatus != BookingServiceStatusEnum.Booked)
            return RedirectToAction("Search");

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateHallBooking(HtBookingServiceVm modelVm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return RedirectToAction("UpdateHallBooking", new { id = modelVm.Id });
            }
            _iService.CurrentUserId = UserId;

            var isAdded = await _iService.HallBookingUpdate(modelVm);
            if (!isAdded)
            {
                SaveFailedMsg();
                return RedirectToAction("UpdateHallBooking", new { id = modelVm.Id });
            }

            SaveSuccessMsg();
            return RedirectToAction("HallDetails", new { id = modelVm.Id });
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return RedirectToAction("UpdateHallBooking", new { id = modelVm.Id });
        }
    }

    #endregion

    #region HallBookingComplete

    [HttpPost]
    public async Task<IActionResult> CompleteHallBooking(HtBookingServiceVm modelVm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return RedirectToAction("HallDetails", new { id = modelVm.Id });
            }
            _iService.CurrentUserId = UserId;

            // change if auto refund remove..::Tawkir
            modelVm.IsRefund = true;

            var (isCheckOut, billId) = await _iService.HallBookingComplete(modelVm);
            if (!isCheckOut)
            {
                SaveFailedMsg();
                return RedirectToAction("HallDetails", new { id = modelVm.Id });
            }

            SaveSuccessMsg();
            return RedirectToAction("Details", "Bill", new { id = billId });
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return RedirectToAction("HallDetails", new { id = modelVm.Id });
        }
    }

    #endregion

    #region HallSearch
    [HttpGet]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public IActionResult HallSearch()
    {
        var vm = new BookingServiceSearchVm();
        vm.BookingServiceStatusLookUp = _iDropdownService.GetBookingServiceStatusSelectListItems();
        vm.PaymentStatusLookUp = _iDropdownService.GetPayemntStatusSelectListItems();
        vm.OnlineBookingLookUp = _iDropdownService.GetApprovedOnlineBookingSelectListItems();
        return View(vm);
    }


    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        HallSearch(DataTablePagination<BookingServiceSearchVm, BookingServiceSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<BookingServiceSearchVm, BookingServiceSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new BookingServiceSearchVm();
        searchVm.SearchModel.BookingType = BookingType.Hall;
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion

    #region HallAvailableReport

    public ActionResult HallAvailable()
    {
        var model = new HallAvaliableReportVm();
        model.HallLookUp = _iDropdownService.GetHallSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> HallAvailableReport(HallAvaliableReportVm model)
    {
        var data = await _iService.GetHallAvailableReportHtml(model);
        return Ok(data);
    }

    public async Task<ActionResult> HallAvailableReportPrint(long hallId, string fromDateStr, string toDateStr)
    {
        var model = new HallAvaliableReportVm();
        model.HallId = hallId;
        model.FromDateStr = fromDateStr;
        model.ToDateStr = toDateStr;

        string html = await _iService.GetHallAvailableReportHtml(model, true);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Hall Availibility By Date Range";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Arrival List";

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 120, top: 80, left: 18, right: 18, isLandScape: false), "application/pdf");
    }

    #endregion

    #region HallJsonData

    public async Task<IActionResult> GetHallByDateRange(string fromDateStr, string toDateStr, int shift)
    {
        var fromDate = (DateTime)(!string.IsNullOrEmpty(fromDateStr) ? DU.Utility.ConvertStrToDate(fromDateStr) : DateTime.Now);
        var toDate = (DateTime)(!string.IsNullOrEmpty(toDateStr) ? DU.Utility.ConvertStrToDate(toDateStr) : DateTime.Now);

        var data = await _iService.GetDynamicAvailableHallByDateRange(fromDate, toDate, shift);
        return Ok(data);
    }

    #endregion

    #endregion

    #region DailyInHouseGuestReport

    public ActionResult DailyInHouseGuest()
    {
        var model = new DailyInHouseGuestVm();
        model.StrReportDate = DateTime.Today.ToString("dd/MM/yyyy");
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> DailyInHouseGuestReport(DailyInHouseGuestVm vm)
    {
        var data = await _iService.DailyInHouseGuestReportHtml(vm);
        return Ok(data);
    }

    public async Task<ActionResult> DailyInHouseGuestReportPrint(string strReportDate)
    {
        var model = new DailyInHouseGuestVm();
        model.StrReportDate = strReportDate;
        model.IsPirnt = true;
        string html = await _iService.DailyInHouseGuestReportHtml(model);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.IsHeaderLogo = false;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Daily In House Guest List";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Daily In House Guest List";

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 50, top: 80, left: 18, right: 18), "application/pdf");
    }

    #endregion

    #region ChangeCbf
    [Authorize(Permissions.Module.HotelManagementModule)]
    [HttpPost]
    public async Task<IActionResult> ChangeCbf(DateBreakfastDto modelVm)
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

            if (string.IsNullOrEmpty(modelVm.BreakfastDateStr))
                return BadRequest(new
                {
                    message = "Date Is Not Correct...!",
                });

            var model = new HtDateBreakfast();

            model.BreakfastDate = (DateTime)(!string.IsNullOrEmpty(modelVm.BreakfastDateStr) ? DU.Utility.ConvertStrToDate(modelVm.BreakfastDateStr) : DU.Utility.GetBdDateTimeNow());
            model.BreakfastAmount = modelVm.BreakfastAmount;
            model.ActionDate = DU.Utility.GetBdDateTimeNow();
            model.ActionById = UserId;

            var isAdded = await _iDateBreakfastService.AddAsync(model);
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

    #region GuestDueReport

    public ActionResult GuestDueReport()
    {
        var model = new GuestDueReportVm();

        var today = DateTime.Today;
        model.StrQueryDate = "05/11/2024";// today.ToString("dd/MM/yyyy");
        model.GuestLookUp = _iDropdownService.GetGuestSelectListItems();
        model.CompanyLookUp = _iDropdownService.GetClientCompanySelectListItems();
        model.GroupLookUp = _iDropdownService.GetGuestDueReportGroupByList();
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> GuestDueReport(GuestDueReportVm model)
    {
        model.StrQueryDate = "05/11/2024";
        //model.ReporetGroupBy = "C"; //G=GUEST WISE , C= COMPANY WISE
        var data = await _iService.GuestDueReportHtml(model);
        return Ok(data);
    }

    public async Task<ActionResult> GuestDueReportPrint(long GuestId = 0, long companyId = 0, string strFromDate = null, string strToDate = null, string groupBy = "")
    {
        var model = new GuestDueReportVm();
        model.GuestId = GuestId;
        model.CompanyId = companyId;
        model.StrFromDate = strFromDate;
        model.StrToDate = strToDate;
        model.ReporetGroupBy = groupBy;
        model.StrQueryDate = "05/11/2024";

        string html = await _iService.GuestDueReportHtml(model, true);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.IsHeaderLogo = true;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Guest Due Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Guest Due Report";

        return File(export.ExportContentToPdfWithLogo(html, reportName, reportTitle: reportTitle, bottom: 90, top: 110, left: 18, right: 18, isLandScape: true, isLarge: true, companyImage: true), "application/pdf");
    }

    #endregion

    #region RoomOccupancyReport

    public ActionResult RoomOccupancyReport()
    {
        var model = new RoomOccupancyReportVm();
        var today = DateTime.Today;
        model.FromDateStr = "01/" + today.Month.ToString() + "/" + today.Year.ToString();
        model.ToDateStr = today.ToString("dd/MM/yyyy");
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> RoomOccupancyReport(RoomOccupancyReportVm model)
    {
        var data = await _iService.RoomOccupancyReportHtml(model);
        return Ok(data);
    }

    public async Task<ActionResult> RoomOccupancyReportPrint(string fromDateStr, string toDateStr)
    {
        var model = new RoomOccupancyReportVm();
        model.FromDateStr = fromDateStr;
        model.ToDateStr = toDateStr;

        string html = await _iService.RoomOccupancyReportHtml(model);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.IsHeaderLogo = true;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Room Occupancy Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Room Occupancy Report";

        //return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 120, top: 80, left: 18, right: 18, isLandScape: false), "application/pdf");
        return File(export.ExportContentToPdfWithLogo(html, reportName, reportTitle: reportTitle, bottom: 90, top: 110, left: 18, right: 18, isLarge: true, isLandScape: true, companyImage: true), "application/pdf");
    }

    #endregion

    #region AdvanceReport

    public ActionResult AdvanceReport()
    {
        var model = new AdvanceReportVm();

        var today = DateTime.Today;
        model.GuestLookUp = _iDropdownService.GetGuestSelectListItems();
        model.CompanyLookUp = _iDropdownService.GetClientCompanySelectListItems();
        model.AdvanceTypeLookUp = _iDropdownService.GetAdvanceTypeSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> AdvanceReport(AdvanceReportVm model)
    {
        var data = await _iService.AdvanceReportHtml(model);
        return Ok(data);
    }

    public async Task<ActionResult> AdvanceReportPrint(long GuestId = 0, long companyId = 0, string qType = "")
    {
        var model = new AdvanceReportVm();
        model.GuestId = GuestId;
        model.CompanyId = companyId;
        model.QType = qType;

        string html = await _iService.AdvanceReportHtml(model);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.IsHeaderLogo = true;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Advance Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Advance Report";

        //return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 120, top: 80, left: 18, right: 18, isLandScape: true), "application/pdf");
        return File(export.ExportContentToPdfWithLogo(html, reportName, reportTitle: reportTitle, bottom: 90, top: 110, left: 18, right: 18, isLandScape: true, isLarge: true, companyImage: true), "application/pdf");
    }

    #endregion

    #region BookingBillDetailPrint

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

        return File(export.ExportBillContentToPdfWithLogo(html, reportName, companyImage: true, reportTitle: reportTitle, bottom: 110, top: 90, left: 21, right: 21), "application/pdf");
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

    #region DeleteBooking
    [HttpGet]
    public async Task<IActionResult> BookingDelete(long bookingId)
    {
        try
        {
            if (bookingId > 0)
            {
                _iService.CurrentUserId = UserId;
                var isDelete = await _iService.BookingRemoveAsync(bookingId);

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

    #region ReservationBill

    public async Task<ActionResult> ReservationPrint(long bookingId)
    {
        string html = await _iService.ReservationBillHtml(bookingId);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = "";
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.AddressTwo = "Hotline: 01715-946471, Email: hotelmishuk@gmail.com";
        reportTitle.ReportTitle = "";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Prepared By");

        reportTitle.IsSignature = true;
        reportTitle.IsFooterChange = true;
        reportTitle.IsHeaderLogo = true;

        string reportName = "Reservation Bill";

        return File(export.ExportBillContentToPdfWithLogo(html, reportName, companyImage: true, reportTitle: reportTitle, bottom: 100, top: 95, left: 35, right: 35), "application/pdf");
    }

    #endregion

    #region ReservationCardPrint

    public async Task<ActionResult> ReservationCardPrint(long bookingId)
    {
        string html = await _iService.ReservationCardHtml(bookingId);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = "";
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.AddressTwo = "Hotline: 01715-946471, Email: hotelmishuk@gmail.com";
        //reportTitle.ReportTitle = "";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Guest Signature");
        reportTitle.SignatureList.Add("Authorized Signature");

        reportTitle.IsSignature = true;
        reportTitle.IsFooterChange = true;
        reportTitle.IsHeaderLogo = true;

        string reportName = "Reservation Card";

        return File(export.ExportBillContentToPdfWithLogo(html, reportName, companyImage: true, reportTitle: reportTitle, bottom: 100, top: 95, left: 35, right: 35), "application/pdf");
    }

    #endregion

    #region EmptyReservationCardPrint

    public async Task<ActionResult> EmptyReservationCardPrint()
    {
        string html = await _iService.ReservationCardHtmlEmpty();

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = "";
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.AddressTwo = "Hotline: 01715-946471, Email: hotelmishuk@gmail.com";
        //reportTitle.ReportTitle = "";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Guest Signature");
        reportTitle.SignatureList.Add("Authorized Signature");

        reportTitle.IsSignature = true;
        reportTitle.IsFooterChange = true;
        reportTitle.IsHeaderLogo = true;

        string reportName = "Reservation Card";

        return File(export.ExportBillContentToPdfWithLogo(html, reportName, companyImage: true, reportTitle: reportTitle, bottom: 100, top: 95, left: 35, right: 35), "application/pdf");
    }

    #endregion

    #region CategoryWiseRoomAvailableReport

    public ActionResult CategoryWiseRoomAvailableReport()
    {
        var model = new CategoryWiseRoomAvailableReportVm();
        var today = DateTime.Today;
        model.StartDateStr = today.ToString("dd/MM/yyyy");
        model.EndDateStr = today.AddMonths(1).ToString("dd/MM/yyyy");
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> CategoryWiseRoomAvailableReport(CategoryWiseRoomAvailableReportVm model)
    {
        var data = await _iService.CategoryWiseRoomAvailableReportHtml(model);
        return Ok(data);
    }

    public async Task<ActionResult> CategoryWiseRoomAvailableReportPrint(string startDateStr, string endDateStr)
    {
        var model = new CategoryWiseRoomAvailableReportVm();
        model.StartDateStr = startDateStr;
        model.EndDateStr = endDateStr;
        string html = await _iService.CategoryWiseRoomAvailableReportHtml(model, true);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.IsHeaderLogo = true;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Hotel Forecast Room Position";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Hotel Forecast Room Position";

        return File(export.ExportContentToPdfWithLogo(html, reportName, reportTitle: reportTitle, bottom: 90, top: 110, left: 18, right: 18, isLandScape: true, isLarge: true, companyImage: true), "application/pdf");
    }

    #endregion

    #region PaymentTransactionReport

    public ActionResult PaymentTransactionReport()
    {
        var model = new PaymentTransactionReportVm
        {
            StrFromDate = DateTime.Today.Date.ToString("yyyy-MM-dd"),
            StrToDate = DateTime.Today.Date.ToString("yyyy-MM-dd"),
            PayModeLookUp = _iDropdownService.GetPayModeSelectListItems()
        };

        return View(model);
    }


    [HttpPost]
    public async Task<ActionResult> PaymentTransactionReport(PaymentTransactionReportVm model)
    {
        try
        {
            var data = await _iService.GetPaymentTransactionReportHtml(model);
            return Ok(data);

        }
        catch (Exception ex)
        {

            throw new Exception(ex.Message);
        }
    }

    public async Task<ActionResult> PaymentTransactionReportPrint(string strFromDate, string strToDate, int? payMode, bool showOnlyDue)
    {
        var vm = new PaymentTransactionReportVm
        {
            StrFromDate = strFromDate ?? DateTime.Today.Date.ToString("yyyy-MM-dd"),
            StrToDate = strToDate ?? DateTime.Today.Date.ToString("yyyy-MM-dd"),
            PayMode = payMode,
            ShowOnlyDue = showOnlyDue
        };

        string html = await _iService.GetPaymentTransactionReportHtml(vm, true);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        //reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.IsHeaderLogo = true;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Transaction Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Transaction Report";

        return File(export.ExportContentToPdfWithLogo(html, reportName, reportTitle: reportTitle, bottom: 90, top: 110, left: 18, right: 18, isLarge: true, isLandScape: true), "application/pdf");
    }

    #endregion

    #region MoneyreceiptPrint

    public async Task<ActionResult> MoneyreceiptPrint(long id)
    {
        string html = await _iService.MoneyReceiptHtmlAsync(id);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        //reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.IsHeaderLogo = true;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Money Rceipt";

        return File(export.ExportContentToPdfWithLogo(html, reportName, reportTitle: reportTitle, width: 600, bottom: 20, top: 90, left: 18, right: 18, isLarge: true, isLandScape: true), "application/pdf");
    }

    #endregion

    #region Cancel Booking
    [HttpGet]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public async Task<IActionResult> Cancel(long id, string cancelReason)
    {
        var data = await _iService.GetByIdAsync(id);
        if (data == null)
        {
            DeleteFailedMsg();
            return NotFound();
        }

        data.BookingStatus = BookingServiceStatusEnum.Canceled;
        data.CancelBy = UserId;
        data.CancelReason = cancelReason;
        data.CancelDate = DateTime.Now;

        DeleteSuccessMsg();
        var isRemove = _iService.Update(data);
        return RedirectToAction("Search");
    }
    #endregion

    #region CancelReport

    public ActionResult CancelReport()
    {
        var today = DateTime.Today;
        var startDate = new DateTime(DateTime.Now.Year, 1, 1);
        var model = new CancelReportVm
        {
            ToDateStr = today.ToString("dd/MM/yyyy"),
            FromDateStr = startDate.ToString("dd/MM/yyyy"),
            BookingFilter = "",
            GuestFilter = ""

        };
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> CancelReport(CancelReportVm model)
    {
        try
        {
            var data = await _iService.CancelReportHtml(model);
            return Ok(data);

        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<ActionResult> CancelReportPrint(string fromDateStr, string toDateStr, string bookingNo, string guestName)
    {
        var model = new CancelReportVm();
        model.ToDateStr = toDateStr;
        model.FromDateStr = fromDateStr;
        model.BookingFilter = bookingNo;
        model.GuestFilter = guestName;

        bool isPrint = true;
        string html = await _iService.CancelReportHtml(model, isPrint);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Cancel Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Cancel Report";

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 120, top: 80, left: 18, right: 18, isLandScape: true, width: 1500), "application/pdf");
    }

    #endregion

    #region ReviceCancelBooking
    [Authorize(Permissions.Module.HotelManagementModule)]
    public async Task<IActionResult> ReviceCancelBooking(long id)
    {
        var model = await _iService.GetBookingInfoById(id);
        model.PayModeLookUp = _iDropdownService.GetPayModeSelectListItems();

        if (model.BookingStatus != BookingServiceStatusEnum.Canceled)
            return RedirectToAction("Search");

        model.ComplementaryLookUp = _iDropdownService.GetComplementaryListItems();
        model.CheckInTimeStr = model.CheckInTime.ToString("dd/MM/yyyy");
        model.CheckOutTimeStr = model.CheckOutTime.ToString("dd/MM/yyyy");

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> ReviceCancelBooking(HtBookingServiceVm modelVm)
    {
        modelVm.PayModeLookUp = _iDropdownService.GetPayModeSelectListItems();
        modelVm.ComplementaryLookUp = _iDropdownService.GetComplementaryListItems();
        modelVm.CheckInTimeStr = modelVm.CheckInTime.ToString("dd/MM/yyyy");
        modelVm.CheckOutTimeStr = modelVm.CheckOutTime.ToString("dd/MM/yyyy");
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return RedirectToAction("UpdateBooking", new { id = modelVm.Id });
            }
            _iService.CurrentUserId = UserId;

            var isAdded = await _iService.ReviceCancelBooking(modelVm);
            if (!isAdded)
            {
                SaveFailedMsg();
                return RedirectToAction("ReviceCancelBooking", new { id = modelVm.Id });
            }

            SaveSuccessMsg();
            return RedirectToAction("Details", new { id = modelVm.Id });
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return RedirectToAction("ReviceCancelBooking", new { id = modelVm.Id });
        }
    }

    #endregion

    #region ExtraServiceReport

    public ActionResult ExtraServiceReport()
    {
        var model = new ExtraServiceReportVm
        {
            StrFromDate = DateTime.Today.Date.ToString("yyyy-MM-dd"),
            StrToDate = DateTime.Today.Date.ToString("yyyy-MM-dd"),
            ServiceLookUp = _iDropdownService.GetServiceSelectListItems()
        };

        return View(model);
    }


    [HttpPost]
    public async Task<ActionResult> ExtraServiceReport(string strFromDate, string strToDate,string serviceCode, long? serviceId)
    {
        try
        {
            //vm.StrFromDate = DateTime.Today.Date.ToString("yyyy-MM-dd");
            //vm.StrToDate = DateTime.Today.Date.ToString("yyyy-MM-dd");

            var vm = new ExtraServiceReportVm
            {
                StrFromDate = strFromDate,
                StrToDate = strToDate,
                ServiceId = serviceId,
                ServiceCode = serviceCode
            };
            var data = await _iService.ExtraServiceReportHtml(vm);
            return Ok(data);

        }
        catch (Exception ex)
        {

            throw new Exception(ex.Message);
        }
    }

    public async Task<ActionResult> ExtraServiceReportPrint(string strFromDate, string strToDate, long? serviceId)
    {
        var vm = new ExtraServiceReportVm
        {
            StrFromDate = strFromDate ?? DateTime.Today.Date.ToString("yyyy-MM-dd"),
            StrToDate = strToDate ?? DateTime.Today.Date.ToString("yyyy-MM-dd"),
            ServiceId = serviceId
        };

        string html = await _iService.ExtraServiceReportHtml(vm, true);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.IsHeaderLogo = true;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Extra Service Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Extra Service List";

        return File(export.ExportContentToPdfWithLogo(html, reportName, reportTitle: reportTitle, bottom: 90, top: 120, left: 18, right: 18, isLarge: true, isLandScape: true), "application/pdf");
    }

    #endregion

    #region GetExtraServiceCount
    [HttpPost]
    public async Task<ActionResult> GetExtraServiceCount(string strFromDate, string strToDate, string serviceCode, long? serviceId)
    {
        try
        {
            //vm.StrFromDate = DateTime.Today.Date.ToString("yyyy-MM-dd");
            //vm.StrToDate = DateTime.Today.Date.ToString("yyyy-MM-dd");

            var vm = new ExtraServiceReportVm
            {
                StrFromDate = strFromDate,
                StrToDate = strToDate,
                ServiceId = serviceId,
                ServiceCode = serviceCode
            };
            var data = await _iService.TodayExtraServiceReportCount(vm);
            return Ok(data);

        }
        catch (Exception ex)
        {

            throw new Exception(ex.Message);
        }
    }
    #endregion

    #region BookingHallReport

    public ActionResult BookingHallReport()
    {
        var model = new BookingHallReportVm
        {
            StrFromDate = DateTime.Today.Date.ToString("yyyy-MM-dd"),
            StrToDate = DateTime.Today.Date.ToString("yyyy-MM-dd")
        };

        return View(model);
    }


    [HttpPost]
    public async Task<ActionResult> BookingHallReport(BookingHallReportVm model)
    {
        try
        {
            var data = await _iService.BookingHallReportHtml(model);
            return Ok(data);

        }
        catch (Exception ex)
        {

            throw new Exception(ex.Message);
        }
    }

    public async Task<ActionResult> BookingHallReportPrint(string strFromDate, string strToDate)
    {
        var vm = new BookingHallReportVm
        {
            StrFromDate = strFromDate ?? DateTime.Today.Date.ToString("yyyy-MM-dd"),
            StrToDate = strToDate ?? DateTime.Today.Date.ToString("yyyy-MM-dd")
        };

        string html = await _iService.BookingHallReportHtml(vm, true);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.IsHeaderLogo = true;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Booking Hall Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Booking Hall Report";

        return File(export.ExportContentToPdfWithLogo(html, reportName, reportTitle: reportTitle, bottom: 90, top: 120, left: 18, right: 18, isLarge: true, isLandScape: true), "application/pdf");
    }
    #endregion

    #region ReservationRepot

    public ActionResult ReservationReport()
    {
        var today = DateTime.Today;

        var model = new ReservationReportVm
        {
            ToDateStr = today.ToString("dd/MM/yyyy"),
            FromDateStr = today.ToString("dd/MM/yyyy"),
            BookingFilter = "",
            GuestFilter = ""

        };
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> ReservationReport(ReservationReportVm model)
    {
        try
        {
            var data = await _iService.ReservationReportHtml(model);
            return Ok(data);

        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<ActionResult> ReservationReportPrint(string fromDateStr, string toDateStr, string bookingNo, string guestName)
    {
        var model = new ReservationReportVm();
        model.ToDateStr = toDateStr;
        model.FromDateStr = fromDateStr;
        model.BookingFilter = bookingNo;
        model.GuestFilter = guestName;

        bool isPrint = true;
        string html = await _iService.ReservationReportHtml(model, isPrint);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        //reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.IsHeaderLogo = true;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Reservation Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Reservation Report";

        return File(export.ExportContentToPdfWithLogo(html, reportName, reportTitle: reportTitle, bottom: 90, top: 110, left: 18, right: 18, isLarge: true, isLandScape: true, companyImage: true), "application/pdf");
    }

    #endregion

    #region ConfirmBooking

    [HttpGet]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public async Task<IActionResult> ConfirmBooking(long id)
    {
        var data = await _iService.GetByIdAsync(id);
        if (data == null)
        {
            ExceptionMsg("Booking Confirmation Failed...!!");
            return RedirectToAction("Details", new { id = id });
        }
        data.BookingConfirmStatus = BookingConfirmEnum.Confirm;
        UpdateSuccessMsg("Booking Confirmation Done..!!");
        var isConfirm = _iService.Update(data);
        return RedirectToAction("Details", new { id = id });
    }
    #endregion

    #region RemoveComplementary
    [HttpPost]
    public async Task<IActionResult> RemoveComplementary(long bookingRoomId)
    {
        try
        {
            if (bookingRoomId > 0)
            {
                _iService.CurrentUserId = UserId;
                var isDelete = await _iService.RemoveComplementary(bookingRoomId);

                return Ok(isDelete);
            }

            return BadRequest("BookingRoomId Not Found..!!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }
    #endregion

    #region ComplementaryEntry
    [HttpPost]
    public async Task<IActionResult> ComplementaryEntryAsync(long bookingRoomId, long complementaryId)
    {
        try
        {
            if (bookingRoomId > 0 && complementaryId > 0)
            {
                _iService.CurrentUserId = UserId;
                var isAdded = await _iService.ComplementaryEntryAsync(bookingRoomId, complementaryId);

                return Ok(isAdded);
            }

            return BadRequest("Information is not correct..!!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }
    #endregion

    #region RoomChangeReport

    public ActionResult RoomChangeReport()
    {
        var model = new RoomChangeReportVm
        {
            StrFromDate = DateTime.Today.Date.ToString("yyyy-MM-dd"),
            StrToDate = DateTime.Today.Date.ToString("yyyy-MM-dd")
        };

        return View(model);
    }


    [HttpPost]
    public async Task<ActionResult> RoomChangeReport(string strFromDate, string strToDate)
    {
        try
        {
            //vm.StrFromDate = DateTime.Today.Date.ToString("yyyy-MM-dd");
            //vm.StrToDate = DateTime.Today.Date.ToString("yyyy-MM-dd");

            var vm = new RoomChangeReportVm
            {
                StrFromDate = strFromDate,
                StrToDate = strToDate
            };
            var data = await _iService.GetRoomChangeReportHtml(vm);
            return Ok(data);

        }
        catch (Exception ex)
        {

            throw new Exception(ex.Message);
        }
    }

    public async Task<ActionResult> RoomChangeReportPrint(string strFromDate, string strToDate)
    {
        var vm = new RoomChangeReportVm
        {
            StrFromDate = strFromDate ?? DateTime.Today.Date.ToString("yyyy-MM-dd"),
            StrToDate = strToDate ?? DateTime.Today.Date.ToString("yyyy-MM-dd")
        };

        string html = await _iService.GetRoomChangeReportHtml(vm, true);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        //reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.IsHeaderLogo = true;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Room Change Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Room Change List";

        return File(export.ExportContentToPdfWithLogo(html, reportName, reportTitle: reportTitle, bottom: 90, top: 110, left: 18, right: 18, isLarge: true, isLandScape: true, companyImage: true), "application/pdf");
    }

    #endregion

    #region DailyInHouseGuestLedgerReport

    public ActionResult DailyInHouseGuestLedger()
    {
        var model = new InHouseGuestLedgerReportVm();
        model.StrQueryDate = DateTime.Today.ToString("dd/MM/yyyy");
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> DailyInHouseGuestLedgerReport(InHouseGuestLedgerReportVm vm)
    {
        var data = await _iService.InHouseGuestLedgerReportHtml(vm);
        return Ok(data);
    }

    public async Task<ActionResult> DailyInHouseGuestLedgerReportPrint(string strReportDate)
    {
        var model = new InHouseGuestLedgerReportVm();
        model.StrQueryDate = strReportDate;
        model.IsPirnt = true;
        string html = await _iService.InHouseGuestLedgerReportHtml(model);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        //reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.IsHeaderLogo = true;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Daily In House Guest Ledger Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Daily In House Guest Ledger Report";

        return File(export.ExportContentToPdfWithLogo(html, reportName, reportTitle: reportTitle, bottom: 90, top: 110, left: 18, right: 18, isLarge: true, isLandScape: true, companyImage: true), "application/pdf");

    }

    #endregion
}
