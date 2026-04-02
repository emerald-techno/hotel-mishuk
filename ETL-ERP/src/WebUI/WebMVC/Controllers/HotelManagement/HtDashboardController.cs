using Interface.Services.HotelManagement;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Controllers.Base;
using DU = Domain.Utility;

namespace WebMVC.Controllers.HotelManagement;

public class HtDashboardController : AppBaseController
{
    #region Config
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _iUnitWork;
    private readonly IRoomInfoService _iRoomService;
    private readonly IBookingServiceService _iBookingService;


    public HtDashboardController(ILogger<HomeController> logger, IUnitOfWork iUnitOfWork,
        IRoomInfoService iRoomService, IBookingServiceService iBookingService) : base(iUnitOfWork)
    {
        _logger = logger;
        _iUnitWork = iUnitOfWork;
        _iRoomService = iRoomService;
        _iBookingService = iBookingService;
    }
    #endregion

    public async Task<IActionResult> Index()
    {
        var model = await _iBookingService.GetDashBoardData();
        model.StrQueryDate = DateTime.Now.ToString("dd/MM/yyyy");
        return View(model);
    }

    #region JsonData

    [HttpGet]
    public async Task<ActionResult> GetRoomStatusByDate(string selectDate,int roomStatus = 0,int cleanStatus = 0)
    {
        try
        {
            var date = (DateTime)(!string.IsNullOrEmpty(selectDate) ? DU.Utility.ConvertStrToDate(selectDate) : DU.Utility.GetBdDateTimeNow());
            var rooms = await _iBookingService.GetRoomAvailabilityByDate(date, roomStatus: roomStatus, cleanStatus: cleanStatus);
            return Ok(rooms);
        }
        catch (Exception ex)
        {
            return Ok(SetError(ex.Message));
        }
    }

    #endregion
}
