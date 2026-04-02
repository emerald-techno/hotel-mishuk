using AutoMapper;
using Domain.Entities.HouseKeeping;
using Domain.Enums.AppEnums;
using Domain.Utility.Common;
using Domain.ViewModel.HouseKeeping.Dashboard;
using Domain.ViewModel.HouseKeeping.TaskName;
using Interface.Repository.HotelManagement;
using Interface.Repository.HouseKeeping;
using Interface.Services.HouseKeeping;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.HouseKeeping;

public class TaskNameService : BaseService<HkTaskName>, ITaskNameService
{
    #region Config
    private ITaskNameRepository _iRepository;
    private IBookingServiceRepository _iBookingServiceRepository;
    private IBookingRoomRepository _iBookingRoomRepository;
    private IRoomInfoRepository _iRoomInfoRepository;
    private IRoomAssignRepository _iRoomAssignRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;

    public TaskNameService(ITaskNameRepository iRepository, IBookingServiceRepository iBookingServiceRepository, IBookingRoomRepository iBookingRoomRepository, IRoomInfoRepository iRoomInfoRepository, IRoomAssignRepository iRoomAssignRepository, IMapper iMapper, IUnitOfWork iUnitOfWork) : base(iRepository, iUnitOfWork)
    {
        _iRepository = iRepository;
        _iBookingServiceRepository = iBookingServiceRepository;
        _iBookingRoomRepository = iBookingRoomRepository;
        _iRoomInfoRepository = iRoomInfoRepository;
        _iRoomAssignRepository = iRoomAssignRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
    }

    #endregion

    public async Task<DataTablePagination<HkTaskNameSearchVm, HkTaskNameSearchVm>> SearchAsync(DataTablePagination<HkTaskNameSearchVm, HkTaskNameSearchVm> model)
    {
        var dataList = await _iRepository.SearchAsync(model);
        return dataList;
    }

    public async Task<HkDashboard> GetDashboardData()
    {
        var model = await _iRepository.GetHkDashboardData();
        return model;
    }

    #region GetRoomAvailabilityByDate
    public async Task<List<HKDashBoardDataVm>> GetRoomAvailabilityByDate(DateTime selectedDate, int roomStatus = 0, int cleanStatus = 0, int categoryId = 0)
    {
        var dataList = new List<HKDashBoardDataVm>();

        // All booking  
        var bookingList = await _iBookingServiceRepository.GetAsync(x => x.CheckInTime.Date <= selectedDate.Date && x.CheckOutTime.Date >= selectedDate.Date && !x.IsDeleted
        && x.BookingType == BookingType.Room && !x.IsDeleted);

        //remove cancel booking and no show booking
        bookingList = bookingList.Where(x => x.BookingStatus != BookingServiceStatusEnum.Canceled && x.BookingStatus != BookingServiceStatusEnum.NoShow).ToList();

        var bookingListIds = bookingList.Select(x => x.Id);
        var bookingRoomList = await _iBookingRoomRepository.GetAsync(c => bookingListIds.Contains(c.BookingId) && !c.IsDeleted, x => x.Booking, r => r.Room);

        bookingRoomList = bookingRoomList.Where(x => x.CheckInTime.Date <= selectedDate.Date && x.CheckOutTime.Date >= selectedDate.Date && x.ActualCheckOutTime is null).ToList();

        //Added By Tawkir: 18/02/2025: For same room booking & occupied issue.
        bookingRoomList = bookingRoomList.OrderBy(x => x.CheckInTime).ToList();

        var bookedRoomList = bookingRoomList.Select(x => x.Room.RoomNo).ToList();

        var totalRooms = await _iRoomInfoRepository.GetAsync(x => x.IsActive && !x.IsDeleted, c => c.RoomCategory);


        if (totalRooms.Count > 0)
        {
            foreach (var room in totalRooms)
            {
                var filterData = bookingRoomList.FirstOrDefault(x => x.RoomId == room.Id);

                var model = new HKDashBoardDataVm();
                model.RoomId = room.Id;
                model.RoomNo = room.RoomNo;
                model.CategoryName = room?.RoomCategory?.CategoryName;
                model.CategoryId = room?.RoomCategory?.Id;
                model.CleaningStatus = (int)room.CleaningStatus;

                /*
                 Status: 
                    1 = Booked
                    2 = Occupied
                    3 = Available
                    4 = Out Of Order
                    5 = Vacant & Dirty
                */

                if (filterData != null)
                {
                    var bookingStatus = filterData.Booking.BookingStatus;

                    if (bookingStatus == BookingServiceStatusEnum.Booked)
                    {
                        model.Status = 1;
                        if (filterData.ActualCheckInTime == null && filterData.ActualCheckOutTime == null && filterData.CheckInTime.Date == DateTime.Today.Date)
                        {
                            model.Status = 6;
                        }
                    }
                    else if (bookingStatus == BookingServiceStatusEnum.CheckIn)
                    {
                        if (filterData.ActualCheckInTime != null && filterData.ActualCheckOutTime == null)
                        {
                            model.Status = 2;

                            if (filterData.CheckOutTime.Date == selectedDate.Date)
                                model.IsTodayCheckout = true;
                            else
                                model.IsTodayCheckout = false;
                        }
                        else if (filterData.ActualCheckInTime != null && filterData.ActualCheckOutTime != null)
                        {
                            if (room.CleaningStatus == CleaningStatusEnum.OOO)
                            {
                                model.Status = 4;
                            }
                            else if (room.CleaningStatus == CleaningStatusEnum.VD)
                            {
                                model.Status = 5;
                            }
                            else if (room.CleaningStatus == CleaningStatusEnum.VC)
                            {
                                model.Status = 3;
                            }
                        }
                        else if (filterData.ActualCheckInTime == null && filterData.ActualCheckOutTime == null)
                        {
                            model.Status = 1;
                        }
                    }
                    else if (bookingStatus == BookingServiceStatusEnum.CheckOut)
                    {
                        if (room.CleaningStatus == CleaningStatusEnum.OOO)
                        {
                            model.Status = 4;
                        }
                        else if (room.CleaningStatus == CleaningStatusEnum.VD)
                        {
                            model.Status = 5;
                        }
                        else if (room.CleaningStatus == CleaningStatusEnum.VC)
                        {
                            model.Status = 3;
                        }
                    }
                }
                else
                {
                    if (room.CleaningStatus == CleaningStatusEnum.OOO)
                    {
                        model.Status = 4;
                    }
                    else if (room.CleaningStatus == CleaningStatusEnum.VD)
                    {
                        model.Status = 5;
                    }
                    //else if (room.HouseKeeperAvailabilityStatus == AvailabilityStatusEnum.Occupied)
                    //{
                    //    model.Status = 2;
                    //}
                    else if (room.CleaningStatus == CleaningStatusEnum.VC)
                    {
                        model.Status = 3;
                    }
                    else
                    {
                        model.Status = 3;
                    }
                }

                dataList.Add(model);
            }
        }
        if (categoryId > 0)
            dataList = dataList.Where(o => o.CategoryId == categoryId).ToList();

        if (roomStatus > 0 && cleanStatus > 0)
            dataList = dataList.Where(o => o.Status == roomStatus || o.CleaningStatus == cleanStatus).ToList();
        else if (roomStatus > 0)
            dataList = dataList.Where(o => o.Status == roomStatus).ToList();
        else if (cleanStatus > 0)
            dataList = dataList.Where(o => o.CleaningStatus == cleanStatus).ToList();

        return dataList.OrderBy(x => x.RoomNo).ToList();
    }
    #endregion

    #region GetRoomAvailabilityByDateV2
    public async Task<HKDashboardResponseVm> GetRoomAvailabilityByDateV2(DateTime selectedDate, string roomNo, int roomStatus = -1, int cleanStatus = -1, int categoryId = 0, int houseKeeperId = 0)
    {
        var dataList = new HKDashboardResponseVm();

        // All booking  
        var bookingList = await _iBookingServiceRepository.GetAsync(x => x.CheckInTime.Date <= selectedDate.Date && x.CheckOutTime.Date >= selectedDate.Date && !x.IsDeleted
        && x.BookingType == BookingType.Room && !x.IsDeleted);

        //remove cancel booking and no show booking
        bookingList = bookingList.Where(x => x.BookingStatus != BookingServiceStatusEnum.Canceled && x.BookingStatus != BookingServiceStatusEnum.NoShow).ToList();

        var bookingListIds = bookingList.Select(x => x.Id);
        var bookingRoomList = await _iBookingRoomRepository.GetAsync(c => bookingListIds.Contains(c.BookingId) && !c.IsDeleted, x => x.Booking, r => r.Room);

        bookingRoomList = bookingRoomList.Where(x => x.CheckInTime.Date <= selectedDate.Date && x.CheckOutTime.Date >= selectedDate.Date && x.ActualCheckOutTime is null).ToList();

        //Added By Tawkir: 18/02/2025: For same room booking & occupied issue.
        bookingRoomList = bookingRoomList.OrderBy(x => x.CheckInTime).ToList();

        var bookedRoomList = bookingRoomList.Select(x => x.Room.RoomNo).ToList();

        var totalRooms = await _iRoomInfoRepository.GetAsync(x => x.IsActive && !x.IsDeleted, c => c.RoomCategory);
        var roomIds = totalRooms.Select(x => x.Id).ToList();

        var assignList = await _iRoomAssignRepository.GetAsync(x => roomIds.Contains(x.RoomId), h => h.HouseKeeper);

        if (totalRooms.Count > 0)
        {
            foreach (var room in totalRooms)
            {
                var filterData = bookingRoomList.FirstOrDefault(x => x.RoomId == room.Id);

                var assignedHouseKeeper = assignList?.FirstOrDefault(x => x.RoomId == room.Id && x.Status == RoomAssignEnum.Assigned)?.HouseKeeper;

                var model = new HKDashBoardDataVm();
                model.RoomId = room.Id;
                model.RoomNo = room.RoomNo;
                model.CategoryName = room?.RoomCategory?.CategoryName;
                model.CategoryId = room?.RoomCategory?.Id;
                model.CleaningStatus = (int)room.CleaningStatus;
                model.AssignedHouseKeeperName = assignedHouseKeeper?.Name ?? "";
                model.AssignedHouseKeeperId = assignedHouseKeeper?.Id;
                model.HouseKeeperId = assignedHouseKeeper?.Id;


                /*
                 Status: 
                    0 = Available,
                    1 = Occupied,
                    2 = OutOfOrder,
                    3 = TodayCheckIn
                    4 = Expected C/Out
                    5 = Reserved,
                    6 = Already Checked Out,
                    7 = N/A
                */

                if (filterData != null)
                {
                    var bookingStatus = filterData.Booking.BookingStatus;

                    if (bookingStatus == BookingServiceStatusEnum.Booked)
                    {
                        model.Status = 5;

                        if (filterData.ActualCheckInTime == null && filterData.ActualCheckOutTime == null && filterData.CheckInTime.Date == DateTime.Today.Date)
                        {
                            model.Status = 3;
                        }
                    }
                    else if (bookingStatus == BookingServiceStatusEnum.CheckIn)
                    {
                        if (filterData.ActualCheckInTime != null && filterData.ActualCheckOutTime == null)
                        {
                            if (filterData.CheckOutTime.Date == selectedDate.Date)
                            {
                                model.Status = room.HouseKeeperAvailabilityStatus == AvailabilityStatusEnum.Occupied ? 4 : 7;
                                model.IsTodayCheckout = true;
                            }
                            else
                            {
                                model.Status = room.HouseKeeperAvailabilityStatus == AvailabilityStatusEnum.Occupied ? 1 : 7;
                                model.IsTodayCheckout = false;
                            }
                        }
                        else if (filterData.ActualCheckInTime != null && filterData.ActualCheckOutTime != null)
                        {
                            model.Status = room.HouseKeeperAvailabilityStatus == AvailabilityStatusEnum.CheckedOut ? 6 : 7;
                        }
                        else if (filterData.ActualCheckInTime == null && filterData.ActualCheckOutTime == null)
                        {
                            if (filterData.CheckInTime.Date == DateTime.Today.Date)
                            {
                                model.Status = 3;
                            }
                            else
                            {
                                model.Status = 1;
                            }
                        }
                    }
                    else if (bookingStatus == BookingServiceStatusEnum.CheckOut)
                    {
                        //model.Status = (int)room.HouseKeeperAvailabilityStatus;
                        model.Status = room.HouseKeeperAvailabilityStatus == AvailabilityStatusEnum.CheckedOut ? 6
                            : room.HouseKeeperAvailabilityStatus == AvailabilityStatusEnum.Available ? 0
                            : room.HouseKeeperAvailabilityStatus == AvailabilityStatusEnum.OutOfOrder ? 2 : 7;
                    }
                }
                else
                {
                    //model.Status = (int)room.HouseKeeperAvailabilityStatus;

                    model.Status = room.HouseKeeperAvailabilityStatus == AvailabilityStatusEnum.CheckedOut ? 6
                        : room.HouseKeeperAvailabilityStatus == AvailabilityStatusEnum.Available ? 0
                        : room.HouseKeeperAvailabilityStatus == AvailabilityStatusEnum.OutOfOrder ? 2 : 7;

                    //if (room.CleaningStatus == CleaningStatusEnum.OOO)
                    //{
                    //    model.Status = 4;
                    //}
                    //else if (room.CleaningStatus == CleaningStatusEnum.VD)
                    //{
                    //    model.Status = 5;
                    //}
                    //else if (room.CleaningStatus == CleaningStatusEnum.VC)
                    //{
                    //    model.Status = 3;
                    //}
                    //else
                    //{
                    //    model.Status = 3;
                    //}
                }

                dataList.Rooms.Add(model);
            }
        }


        dataList.AllRoomCount = dataList.Rooms.Count;
        dataList.AvailableCount = dataList.Rooms.Count(x => x.Status == 0);
        dataList.OccupiedCount = dataList.Rooms.Count(x => x.Status == 1);
        dataList.OOOCount = dataList.Rooms.Count(x => x.Status == 2);
        dataList.CInCount = dataList.Rooms.Count(x => x.Status == 3);
        dataList.COutCount = dataList.Rooms.Count(x => x.Status == 4);


        if (categoryId > 0)
            dataList.Rooms = dataList.Rooms.Where(o => o.CategoryId == categoryId).ToList();
        if (roomNo != null && roomNo.Length > 0)
            dataList.Rooms = dataList.Rooms.Where(o => o.RoomNo.Contains(roomNo)).ToList();
        if (houseKeeperId > 0)
            dataList.Rooms = dataList.Rooms.Where(o => o.HouseKeeperId == houseKeeperId).ToList();

        if (roomStatus > -1 && cleanStatus > -1)
            dataList.Rooms = dataList.Rooms.Where(o => o.Status == roomStatus && o.CleaningStatus == cleanStatus).ToList();
        else if (roomStatus > -1)
            dataList.Rooms = dataList.Rooms.Where(o => o.Status == roomStatus).ToList();
        else if (cleanStatus > -1)
            dataList.Rooms = dataList.Rooms.Where(o => o.CleaningStatus == cleanStatus).ToList();
        dataList.Rooms = dataList.Rooms.OrderBy(x => x.RoomNo).ToList();
        return dataList;
    }
    #endregion
}
