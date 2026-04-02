using AutoMapper;
using Domain.Enums.AppEnums;
using Domain.Utility;
using Interface.Services.HotelManagement;
using Interface.Services.HouseKeeping;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using DU = Domain.Utility;

namespace WebMVC.Controllers.HouseKeeping
{
    public class HkDashboardController : AppBaseController
    {
        #region Config
        private readonly IUnitOfWork _iUnitWork;
        private readonly IMapper _iMapper;
        private readonly DropdownService _iDropdownService;
        private readonly ITaskNameService _taskNameService;
        private readonly IBookingServiceService _iBookingService;


        public HkDashboardController(IUnitOfWork iUnitOfWork,
                                IMapper iMapper,
                                DropdownService iDropdownService,
                                ITaskNameService taskNameService,
                                IBookingServiceService iBookingService) : base(iUnitOfWork)
        {
            _iUnitWork = iUnitOfWork;
            _iMapper = iMapper;
            _iDropdownService = iDropdownService;
            _taskNameService = taskNameService;
            _iBookingService = iBookingService;
        }
        #endregion

        public async Task<IActionResult> Index()
        {
            var data = await _taskNameService.GetDashboardData();
            return View(data);
        }

        #region New Dashboard
        [HttpGet]
        public async Task<IActionResult> HKDashBoard()
        {
            var model = await _iBookingService.GetDashBoardData();

            model.StrQueryDate = DateTime.Now.ToString("dd/MM/yyyy");
            model.EmployeeLookUp = _iDropdownService.GetEmployeeSelectListItems();
            model.AvailabilityStatusLookUp = _iDropdownService.GetRoomAvailabilityStatusForHKSelectListItems();
            model.RoomCategoryLookUp = _iDropdownService.GetRoomCategorySelectListItems();
            model.CleanStatusLookUp = _iDropdownService.GetHKCleanStatusSelectListItems();

            var cleanStatusLookUp = _iDropdownService.GetHKCleanStatusSelectListItems()
                                                     .Where(x => x.Value != CleaningStatusEnum.O.ToInt32ToString() && 
                                                                 x.Value != CleaningStatusEnum.CO.ToInt32ToString() && 
                                                                 x.Value != CleaningStatusEnum.OOO.ToInt32ToString() && 
                                                                 x.Value != CleaningStatusEnum.VD.ToInt32ToString());
            model.CleanStatusLookUpForUpdate = cleanStatusLookUp;

            return View(model);
        }

        public async Task<ActionResult> HKDashBoardData(string selectDate, string roomNo = "", int roomStatus = -1, int cleanStatus = -1, int categoryId = 0, int houseKeeperId = 0)
        {
            try
            {
                var date = (DateTime)(!string.IsNullOrEmpty(selectDate) ? DU.Utility.ConvertStrToDate(selectDate) : DU.Utility.GetBdDateTimeNow());
                var rooms = await _taskNameService.GetRoomAvailabilityByDateV2(date, roomNo, roomStatus: roomStatus, cleanStatus: cleanStatus, categoryId: categoryId, houseKeeperId: houseKeeperId);
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                return Ok(SetError(ex.Message));
            }
        }
        #endregion

        #region New DashboardV2
        [HttpGet]
        public async Task<IActionResult> HKDashBoardV2()
        {
            var model = await _iBookingService.GetDashBoardData();

            model.StrQueryDate = DateTime.Now.ToString("dd/MM/yyyy");
            model.EmployeeLookUp = _iDropdownService.GetEmployeeSelectListItems();
            model.AvailabilityStatusLookUp = _iDropdownService.GetRoomAvailabilityStatusForHKSelectListItems();
            model.RoomCategoryLookUp = _iDropdownService.GetRoomCategorySelectListItems();
            model.CleanStatusLookUp = _iDropdownService.GetHKCleanStatusSelectListItems();

            var cleanStatusLookUp = _iDropdownService.GetHKCleanStatusSelectListItems()
                                                     .Where(x => x.Value != CleaningStatusEnum.O.ToInt32ToString() &&
                                                                 x.Value != CleaningStatusEnum.CO.ToInt32ToString() &&
                                                                 x.Value != CleaningStatusEnum.OOO.ToInt32ToString() &&
                                                                 x.Value != CleaningStatusEnum.VD.ToInt32ToString());
            model.CleanStatusLookUpForUpdate = cleanStatusLookUp;

            return View(model);
        }

        public async Task<ActionResult> HKDashBoardDataV2(string selectDate, string roomNo = "", int roomStatus = -1, int cleanStatus = -1, int categoryId = 0, int houseKeeperId = 0)
        {
            try
            {
                var date = (DateTime)(!string.IsNullOrEmpty(selectDate) ? DU.Utility.ConvertStrToDate(selectDate) : DU.Utility.GetBdDateTimeNow());
                var rooms = await _taskNameService.GetRoomAvailabilityByDateV2(date, roomNo, roomStatus: roomStatus, cleanStatus: cleanStatus, categoryId: categoryId, houseKeeperId: houseKeeperId);
                return Ok(rooms);
            }
            catch (Exception ex)
            {
                return Ok(SetError(ex.Message));
            }
        }
        #endregion
    }
}
