using Domain.ViewModel.Website;
using Interface.Services.HotelManagement;
using Interface.Services.Notification;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebMVC.Controllers.Base;
using WebMVC.Models;


namespace WebMVC.Controllers;

[Authorize]
public class HomeController : AppBaseController
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _iUnitWork;
    private readonly IRoomInfoService _iRoomService;
    private readonly IBookingServiceService _iBookingService;
    private readonly INtfNotificationMsgService _iNtfNotificationMsgService;
    private readonly IOnlineBookingService _iOnlineBookingService;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork iUnitOfWork,
        IRoomInfoService iRoomService, IBookingServiceService iBookingService,
        INtfNotificationMsgService iNtfNotificationMsgService,
        IOnlineBookingService iOnlineBookingService) : base(iUnitOfWork)
    {
        _logger = logger;
        _iUnitWork = iUnitOfWork;
        _iRoomService = iRoomService;
        _iBookingService = iBookingService;
        _iNtfNotificationMsgService = iNtfNotificationMsgService;
        _iOnlineBookingService = iOnlineBookingService;
    }

    public async Task<IActionResult> Index()
    {
        var model = await _iBookingService.GetDashBoardData();
        model.StrQueryDate = DateTime.Now.ToString("dd/MM/yyyy");
        return View(model);
    }

    public IActionResult Restaurant()
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

    #region Json Data

    public async Task<IActionResult> GetUserNotification()
    {
        try
        {
            var data = await _iNtfNotificationMsgService.GetUserNotification(UserId);
            return Ok(data);
        }
        catch (Exception e)
        {
            return Ok(SetError(e.Message));
        }
    }

    public async Task<IActionResult> GetPendingOnlineBooking()
    {
        try
        {
            var data = await _iOnlineBookingService.GetUpcomingPendingBooking();
            return Ok(data);
        }
        catch (Exception e)
        {
            return Ok(SetError(e.Message));
        }
    }

    #endregion
}