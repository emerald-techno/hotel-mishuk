using AutoMapper;
using Domain.ViewModel.HotelManagement.OnlineBooking;
using Domain.ViewModel.HotelManagement.RoomCategory;
using Domain.ViewModel.Website;
using Interface.Repository.HotelManagement;
using Interface.Services.HotelManagement;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.X509;
using System.Diagnostics;
using Utility.Export;
using WebPublicMVC.Models;
using DU = Domain.Utility;

namespace WebPublicMVC.Controllers;

public class HomeController : Controller
{
    #region Config
    private readonly ILogger<HomeController> _logger;
    private readonly IRoomCategoryService _iRoomCategoryService;
    private readonly IOnlineBookingService _iOnlineBookingService;
    private readonly IOnlineBookingRepository _iOnlineBookingRepository;
    private readonly IMapper _iMapper;
    private IHttpContextAccessor _iHttpContextAccessor;

    public HomeController(ILogger<HomeController> logger,
        IRoomCategoryService iRoomCategoryService,
        IMapper iMapper,
        IOnlineBookingService iOnlineBookingService,
        IOnlineBookingRepository iOnlineBookingRepository,
        IHttpContextAccessor iHttpContextAccessor)
    {
        _logger = logger;
        _iRoomCategoryService = iRoomCategoryService;
        _iMapper = iMapper;
        _iOnlineBookingService = iOnlineBookingService;
        _iHttpContextAccessor = iHttpContextAccessor;
        _iOnlineBookingRepository = iOnlineBookingRepository;
    }
    #endregion 

    #region Index
    public async Task<IActionResult> Index()
    {
        var model = new RoomPageVm();
        var categories = await _iRoomCategoryService.RoomCategoryPublicData();

        model.RoomCategories = categories;
        return View(model);
    }
    #endregion

    #region Room
    public async Task<IActionResult> Room()
    {
        try
        {
            var model = new RoomPageVm();
            var categories = await _iRoomCategoryService.RoomCategoryPublicData();

            model.RoomCategories = categories.OrderBy(x=>x.TotalRent).ToList();
            return View(model);
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public IActionResult Room1()
    {
        return View();
    }

    #endregion

    #region Room Details
    public IActionResult RoomDetails(long id)
    {
        var model = new RoomDetailsPageVm();
        var room = _iRoomCategoryService.GetById(id);
        model.RoomCategory = room;
        return View(model);
    }

    #endregion

    #region SpecialOffer
    public IActionResult SpecialOffer()
    {
        return View();
    }
    #endregion

    #region Service
    public IActionResult Service()
    {
        return View();
    }
    #endregion

    #region Restaurant
    public IActionResult Restaurant()
    {
        return View();
    }
    #endregion

    #region Reservation
    public IActionResult Reservation(string arrivedDate, string depatureDate, long? categoryId)
    {
        var model = new RoomPageVm();

        if (!string.IsNullOrEmpty(arrivedDate) && !string.IsNullOrEmpty(depatureDate))
        {
            var checkInDate = (DateTime)(!string.IsNullOrEmpty(arrivedDate) ? DU.Utility.ConvertStrToDate(arrivedDate) : DU.Utility.GetBdDateTimeNow());
            var checkOutDate = (DateTime)(!string.IsNullOrEmpty(depatureDate) ? DU.Utility.ConvertStrToDate(depatureDate) : DU.Utility.GetBdDateTimeNow());

            model.StrFromDate = checkInDate.ToString("dd/MM/yyyy");
            model.StrToDate = checkOutDate.ToString("dd/MM/yyyy");
            model.RoomCategoryId = categoryId;
        }
        else
        {
            model.StrFromDate = DateTime.Now.ToString("dd/MM/yyyy");
            model.StrToDate = DateTime.Now.AddDays(1).ToString("dd/MM/yyyy");
        }

        return View(model);
    }
    #endregion

    #region ChooseRoom
    public async Task<IActionResult> ChooseRoom(DateTime? arrivedDate, DateTime? depatureDate, int? roomCount)
    {
        var model = new RoomPageVm();
        var categories = arrivedDate != null
            ? await _iRoomCategoryService.GetRoomCategoryByRoomAvailibity(arrivedDate, depatureDate)
            : _iRoomCategoryService.Get(x => x.IsActive && !x.IsDeleted);

        model.RoomCategories = _iMapper.Map<List<HtRoomCategoryVm>>(categories);

        if (arrivedDate != null && depatureDate != null)
        {
            model.StrFromDate = arrivedDate?.ToString("dd/MM/yyyy");
            model.StrToDate = depatureDate?.ToString("dd/MM/yyyy");
            model.RoomCount = roomCount;
        }

        return View(model);
    }
    #endregion

    #region MakeReservation
    public IActionResult MakeReservation(ICollection<OnlineBookingDetailVm> detailVms)
    {
        try
        {
            var model = new OnlineBookingVm();
            if (detailVms.Any())
            {
                foreach (var room in detailVms)
                {
                    room.CheckInDate = (DateTime)(!string.IsNullOrEmpty(room.CheckInDateStr) ? DU.Utility.ConvertStrToDate(room.CheckInDateStr) : DU.Utility.GetBdDateTimeNow());
                    room.CheckOutDate = (DateTime)(!string.IsNullOrEmpty(room.CheckOutDateStr) ? DU.Utility.ConvertStrToDate(room.CheckOutDateStr) : DU.Utility.GetBdDateTimeNow());
                }
            }

            model.RoomCount = detailVms.Sum(x => x.RoomCount);
            model.Adult = detailVms.Sum(x => x.Adult);
            model.NetRent = detailVms.Sum(x => x.TotalRent);

            var minCheckInDate = detailVms.Select(x => x.CheckInDate).Min();
            var maxCheckOutDate = detailVms.Select(x => x.CheckOutDate).Max();

            model.ArrivalDate = minCheckInDate;
            model.DepartureDate = maxCheckOutDate;
            model.OnlineBookingDetails = detailVms;

            return View(model);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }

    [HttpPost]
    public async Task<IActionResult> MakeReservationEntry(OnlineBookingVm modelVm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View("MakeReservation", new { detailVms = modelVm.OnlineBookingDetails });
            }

            var (isAdded, bookingId) = await _iOnlineBookingService.OnlineBookingEntryAsync(modelVm);

            if (!isAdded)
            {
                return View("MakeReservation", new { detailVms = modelVm.OnlineBookingDetails });
            }

            return RedirectToAction("CheckOut", new { bookingId });

        }
        catch (Exception e)
        {
            return RedirectToAction("MakeReservation", new { detailVms = modelVm.OnlineBookingDetails });
        }
    }

    #endregion 

    #region OnlineBookingBillPrint

    public async Task<ActionResult> OnlineBookingBillPrint(long id)
    {
        var html = await _iOnlineBookingService.GetOnlineBookingBillByIdAsyncHtml(id);

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

    #region CheckOut
    public async Task<IActionResult> CheckOut(long bookingId)
    {
        try
        {
            if (bookingId > 0 is false)
                return RedirectToAction("Index");

            var model = await _iOnlineBookingService.GetBookingBillByIdAsync(bookingId);

            return View(model);
        }

        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }
    #endregion

    #region Event
    public IActionResult Event()
    {
        return View();
    }
    #endregion

    #region EventDetails
    public IActionResult EventDetail()
    {
        return View();
    }
    #endregion

    public IActionResult Attraction()
    {
        return View();
    }
    public IActionResult Gallery()
    {
        return View();
    }
    public IActionResult About()
    {
        return View();
    }
    public IActionResult Blog()
    {
        return View();
    }
    public IActionResult Contact()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    #region JsonData

    [HttpGet]
    public async Task<ActionResult> GetAvailableRoomCategory(string arrivedDate, string depatureDate)
    {
        try
        {
            var checkInDate = (DateTime)(!string.IsNullOrEmpty(arrivedDate) ? DU.Utility.ConvertStrToDate(arrivedDate) : DU.Utility.GetBdDateTimeNow());
            var checkOutDate = (DateTime)(!string.IsNullOrEmpty(depatureDate) ? DU.Utility.ConvertStrToDate(depatureDate) : DU.Utility.GetBdDateTimeNow());


            var roomCategoryList = await _iRoomCategoryService.GetRoomCategoryByRoomAvailibity(checkInDate, checkOutDate);
            return Ok(roomCategoryList.OrderBy(x=>x.TotalRent));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    #endregion
}
