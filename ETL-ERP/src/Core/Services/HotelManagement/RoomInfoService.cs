using System.Transactions;
using AutoMapper;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Spreadsheet;
using Domain.Entities.HotelManagement;
using Domain.Entities.HouseKeeping;
using Domain.Enums.AppEnums;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.RoomInfo;
using Interface.Repository.HotelManagement;
using Interface.Repository.HouseKeeping;
using Interface.Services;
using Interface.Services.HotelManagement;
using Interface.UnitOfWork;
using Microsoft.Extensions.Logging;
using Services.Base;
using DU = Domain.Utility;

namespace Services.HotelManagement;

public class RoomInfoService : BaseService<HtRoomInfo>, IRoomInfoService
{
    #region Config
    private IRoomInfoRepository _iRepository;
    private IRoomFacilityMapRepository _iFacilityMapRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly IBookingServiceRepository _iBookingServiceRepository;
    private readonly IBookingRoomRepository _iBookingRoomRepository;
    private readonly IRoomStatusHistoryRepository _iRoomStatusHistoryRepository;
    private readonly IRoomAssignRepository _iRoomAssignRepository;
    private readonly ITaskAssignRepository _iTaskAssignRepository;
    private readonly ILogger<RoomInfoService> _iRoomLogger;
    private readonly IApplicationUserService _iApplicationUserService;

    public RoomInfoService(IRoomInfoRepository iRepository,
        IRoomFacilityMapRepository iFacilityMapRepository,
        IMapper iMapper,
        IUnitOfWork iUnitOfWork,
        IBookingServiceRepository iBookingServiceRepository,
        IBookingRoomRepository iBookingRoomRepository,
        IRoomStatusHistoryRepository iRoomStatusHistoryRepository,
        IRoomAssignRepository iRoomAssignRepository,
        ITaskAssignRepository iTaskAssignRepository,
        ILogger<RoomInfoService> iRoomLogger,
        IApplicationUserService iApplicationUserService) : base(iRepository, iUnitOfWork)
    {
        _iRepository = iRepository;
        _iFacilityMapRepository = iFacilityMapRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
        _iBookingServiceRepository = iBookingServiceRepository;
        _iBookingRoomRepository = iBookingRoomRepository;
        _iRoomStatusHistoryRepository = iRoomStatusHistoryRepository;
        _iRoomAssignRepository = iRoomAssignRepository;
        _iTaskAssignRepository = iTaskAssignRepository;
        _iRoomLogger = iRoomLogger;
        _iApplicationUserService = iApplicationUserService;
    }

    #endregion

    #region RoomAdd

    public async Task<bool> RoomAddAsync(HtRoomInfoVm vm)
    {
        var roomModel = _iMapper.Map<HtRoomInfo>(vm);

        roomModel.ActionById = CurrentUserId;
        roomModel.ActionDate = DU.Utility.GetBdDateTimeNow();
        roomModel.IsActive = true;
        var vatAmount = DU.Utility.PercentCalculation(roomModel.Vat, roomModel.Rent);
        roomModel.TotalRent = roomModel.Rent + roomModel.ServiceCharge + vatAmount;

        var roomFacilityList = new List<HtRoomFacilityMap>();

        if (vm.RoomFacilityMaps.Count > 0)
        {
            foreach (var item in vm.RoomFacilityMaps)
            {
                var model = new HtRoomFacilityMap();
                model.RoomCategoryId = roomModel.RoomCategoryId;
                model.ActionDate = DU.Utility.GetBdDateTimeNow();
                model.ActionById = CurrentUserId;
                model.FacilityId = item.FacilityId;

                roomFacilityList.Add(model);
            }
        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await _iRepository.AddAsync(roomModel);
        var isAdded = await _iUnitOfWork.CompleteAsync();

        if (roomFacilityList != null && roomFacilityList.Count > 0)
        {
            roomFacilityList.ForEach(x => x.RoomId = roomModel.Id);

            _iFacilityMapRepository.AddRange(roomFacilityList);
            var isFacilityAdded = await _iUnitOfWork.CompleteAsync();

            if (!isAdded && !isFacilityAdded) return false;
        }

        if (!isAdded) return false;
        ts.Complete();
        return true;
    }

    #endregion

    #region UpdateRooms

    public async Task<bool> RoomUpdateAsync(HtRoomInfoVm vm)
    {
        if (vm == null && !(vm.Id > 0))
            throw new Exception("Room Information Is Not Correct...!!");

        var roomInfo = await _iRepository.GetFirstOrDefaultAsync(x => x.Id == vm.Id && !x.IsDeleted);

        if (roomInfo == null)
            throw new Exception("Room Information Not Found...!!");

        // Map updated fields from vm to roomInfo
        _iMapper.Map(vm, roomInfo);

        roomInfo.UpdatedById = CurrentUserId;
        roomInfo.UpdateDate = Utility.GetBdDateTimeNow();

        var roomExistFacilityList = await _iFacilityMapRepository.GetAsync(x => x.RoomId == roomInfo.Id && !x.IsDeleted);
        var roomFacilityList = new List<HtRoomFacilityMap>();

        if (vm.RoomFacilityMaps?.Count > 0)
        {
            foreach (var item in vm.RoomFacilityMaps)
            {
                var model = new HtRoomFacilityMap();
                model.RoomId = roomInfo.Id;
                model.RoomCategoryId = roomInfo.RoomCategoryId;
                model.ActionDate = DU.Utility.GetBdDateTimeNow();
                model.ActionById = CurrentUserId;
                model.FacilityId = item.FacilityId;

                var existFacility = roomExistFacilityList.FirstOrDefault(x => x.FacilityId == model.FacilityId);

                if (existFacility == null)
                {
                    roomFacilityList.Add(model);
                }
            }
        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await _iRepository.UpdateAsync(roomInfo);
        var isAdded = await _iUnitOfWork.CompleteAsync();

        if (roomFacilityList != null && roomFacilityList.Count > 0)
        {
            _iFacilityMapRepository.AddRange(roomFacilityList);
            var isFacilityAdded = await _iUnitOfWork.CompleteAsync();

            if (!isAdded && !isFacilityAdded) return false;
        }

        if (!isAdded) return false;
        ts.Complete();
        return true;
    }

    #endregion

    public async Task<DataTablePagination<HtRoomInfoSearchVm, HtRoomInfoSearchVm>> SearchAsync(DataTablePagination<HtRoomInfoSearchVm, HtRoomInfoSearchVm> model)
    {
        var dataList = await _iRepository.SearchAsync(model);
        return dataList;
    }

    public async Task<DataTablePagination<HtRoomInfoSearchVm, HtRoomInfoSearchVm>> HouseKeeperViewSearchAsync(DataTablePagination<HtRoomInfoSearchVm, HtRoomInfoSearchVm> model)
    {
        var dataList = await _iRepository.HouseKeeperViewSearchAsync(model);
        return dataList;
    }

    public async Task<dynamic> GetAvailableRoomByCategoryId(long categoryId, DateTime checkInDate, DateTime checkOutDate)
    {
        //var bookingServiceList = await _iBookingServiceRepository.GetAsync(x => (x.CheckInTime.Date <= checkInDate.Date && x.CheckOutTime.Date > checkInDate.Date)
        //|| (x.CheckInTime.Date < checkOutDate.Date && x.CheckOutTime.Date >= checkOutDate.Date));

        var bookingServiceList = await _iBookingServiceRepository.GetAsync(x => (checkInDate.Date >= x.CheckInTime.Date && checkInDate.Date < x.CheckOutTime.Date)
        || (checkOutDate.Date > x.CheckInTime.Date && checkOutDate.Date <= x.CheckOutTime.Date)
        || (checkInDate.Date <= x.CheckInTime.Date && x.CheckOutTime.Date <= checkOutDate.Date));

        bookingServiceList = bookingServiceList.Where(x => x.BookingStatus == BookingServiceStatusEnum.Booked || x.BookingStatus == BookingServiceStatusEnum.CheckIn).ToList();

        var bookingServiceIds = bookingServiceList.Select(x => x.Id).ToList();
        var bookingRoomList = await _iBookingRoomRepository.GetAsync(x => bookingServiceIds.Contains(x.BookingId) && !x.IsDeleted);

        //bookingRoomList = bookingRoomList.Where(x => (x.CheckInTime.Date <= checkInDate.Date && x.CheckOutTime.Date > checkInDate.Date)
        //|| (x.CheckInTime.Date <= checkOutDate.Date && x.CheckOutTime.Date > checkOutDate.Date)
        //|| (checkInDate.Date <= x.CheckInTime.Date && x.CheckOutTime.Date <= checkOutDate.Date) && x.ActualCheckOutTime is null).ToList();

        bookingRoomList = bookingRoomList.Where(x => (x.CheckInTime.Date <= checkInDate.Date && x.CheckOutTime.Date > checkInDate.Date)
        || (x.CheckInTime.Date < checkOutDate.Date && x.CheckOutTime.Date > checkOutDate.Date)
        || (checkInDate.Date <= x.CheckInTime.Date && x.CheckOutTime.Date <= checkOutDate.Date) && x.ActualCheckOutTime is null).ToList();

        var bookingRoomIds = bookingRoomList.Select(x => x.RoomId).ToList();

        //var availableRoomList = (await _iRepository.GetAsync(x => x.RoomCategoryId == categoryId && !x.IsDeleted && x.IsActive && !bookingRoomIds.Contains(x.Id)
        //&& x.HouseKeeperAvailabilityStatus == AvailabilityStatusEnum.Available
        //&& x.IsActive && !x.IsDeleted)).Select(c => new
        //{
        //    c.Id,
        //    Name = $"{c.RoomNo}{(c.IsAc ? "(AC)" : "")}{(c.IsBelcony ? "(Belcony)" : "")}"
        //}).ToList();

        var availableRoomList = (await _iRepository.GetAsync(x => x.RoomCategoryId == categoryId && !x.IsDeleted && x.IsActive && !bookingRoomIds.Contains(x.Id) && x.IsActive && !x.IsDeleted)).OrderBy(o => o.RoomNo).Select(c => new
        {
            c.Id,
            Name = $"{c.RoomNo}{(c.IsAc ? "(AC)" : "")}{(c.IsBelcony ? "(Belcony)" : "")}"
        }).ToList();

        return availableRoomList;
    }

    public async Task<dynamic> GetAvailableRoomByDateRange(DateTime checkInDate, DateTime checkOutDate)
    {
        //var bookingServiceList = await _iBookingServiceRepository.GetAsync(x => (x.CheckInTime.Date <= checkInDate.Date && x.CheckOutTime.Date > checkInDate.Date)
        //|| (x.CheckInTime.Date <= checkOutDate.Date && x.CheckOutTime.Date > checkOutDate.Date));

        var bookingServiceList = await _iBookingServiceRepository.GetAsync(x => (checkInDate.Date >= x.CheckInTime.Date && checkInDate.Date < x.CheckOutTime.Date)
        || (checkOutDate.Date > x.CheckInTime.Date && checkOutDate.Date <= x.CheckOutTime.Date)
        || (checkInDate.Date <= x.CheckInTime.Date && x.CheckOutTime.Date <= checkOutDate.Date));

        bookingServiceList = bookingServiceList.Where(x => x.BookingStatus == BookingServiceStatusEnum.Booked || x.BookingStatus == BookingServiceStatusEnum.CheckIn).ToList();

        var bookingServiceIds = bookingServiceList.Select(x => x.Id).ToList();
        var bookingRoomList = await _iBookingRoomRepository.GetAsync(x => bookingServiceIds.Contains(x.BookingId) && !x.IsDeleted);

        //bookingRoomList = bookingRoomList.Where(x => (x.CheckInTime.Date <= checkInDate.Date && x.CheckOutTime.Date > checkInDate.Date)
        //|| (x.CheckInTime.Date <= checkOutDate.Date && x.CheckOutTime.Date > checkOutDate.Date)
        //|| (checkInDate.Date <= x.CheckInTime.Date && x.CheckOutTime.Date <= checkOutDate.Date)).ToList();

        bookingRoomList = bookingRoomList.Where(x => (x.CheckInTime.Date <= checkInDate.Date && x.CheckOutTime.Date > checkInDate.Date)
        || (x.CheckInTime.Date < checkOutDate.Date && x.CheckOutTime.Date > checkOutDate.Date)
        || (checkInDate.Date <= x.CheckInTime.Date && x.CheckOutTime.Date <= checkOutDate.Date)).ToList();

        var bookingRoomIds = bookingRoomList.Select(x => x.RoomId).ToList();

        //var availableRoomList = (await _iRepository.GetAsync(x => !x.IsDeleted && x.IsActive && !bookingRoomIds.Contains(x.Id)
        //&& x.HouseKeeperAvailabilityStatus == AvailabilityStatusEnum.Available, c => c.RoomCategory)).Select(c => new
        //{
        //    c.Id,
        //    c.RoomNo,
        //    c.Rent,
        //    c.ServiceCharge,
        //    c.TotalRent,
        //    CategoryId = c.RoomCategoryId,
        //    CategoryName = c.RoomCategory.CategoryName,
        //}).ToList().OrderBy(o => o.RoomNo);

        var availableRoomList = (await _iRepository.GetAsync(x => !x.IsDeleted && x.IsActive && !bookingRoomIds.Contains(x.Id), c => c.RoomCategory)).Select(c => new
        {
            c.Id,
            c.RoomNo,
            c.Rent,
            c.ServiceCharge,
            c.TotalRent,
            CategoryId = c.RoomCategoryId,
            CategoryName = c.RoomCategory.CategoryName,
        }).ToList().OrderBy(o => o.RoomNo);

        return availableRoomList;
    }

    public async Task<DayWiseRoomInfoVm> GetDayWiseRoomInfo(long roomId, DateTime selectedDate)
    {
        var result = await _iRepository.GetDayWiseRoomInfo(roomId, selectedDate);
        return result;
    }

    #region CheckRoomIsAvaliable

    public async Task<bool> CheckRoomIsAvaliable(long roomId, DateTime checkInDate, DateTime checkOutDate, long? sameBooKId = null)
    {
        //var bookingServiceList = await _iBookingServiceRepository.GetAsync(x => (x.CheckInTime.Date <= checkInDate.Date && x.CheckOutTime.Date > checkInDate.Date)
        //|| (x.CheckInTime.Date <= checkOutDate.Date && x.CheckOutTime.Date > checkOutDate.Date));
        if (roomId == 0)
            return false;

        var bookingServiceList = await _iBookingServiceRepository.GetAsync(x => (checkInDate.Date >= x.CheckInTime.Date && checkInDate.Date < x.CheckOutTime.Date)
        || (checkOutDate.Date > x.CheckInTime.Date && checkOutDate.Date <= x.CheckOutTime.Date)
        || (checkInDate.Date <= x.CheckInTime.Date && x.CheckOutTime.Date <= checkOutDate.Date));

        bookingServiceList = bookingServiceList.Where(x => x.BookingStatus == BookingServiceStatusEnum.Booked || x.BookingStatus == BookingServiceStatusEnum.CheckIn).ToList();

        if (sameBooKId != null && sameBooKId > 0)
        {
            bookingServiceList = bookingServiceList.Where(x => x.Id != sameBooKId).ToList();
        }

        var bookingServiceIds = bookingServiceList.Select(x => x.Id).ToList();
        var bookingRoomList = await _iBookingRoomRepository.GetAsync(x => bookingServiceIds.Contains(x.BookingId) && !x.IsDeleted);

        //bookingRoomList = bookingRoomList.Where(x => (x.CheckInTime.Date <= checkInDate.Date && x.CheckOutTime.Date > checkInDate.Date)
        //|| (x.CheckInTime.Date <= checkOutDate.Date && x.CheckOutTime.Date > checkOutDate.Date)
        //|| (checkInDate.Date <= x.CheckInTime.Date && x.CheckOutTime.Date <= checkOutDate.Date)).ToList();

        bookingRoomList = bookingRoomList.Where(x => (x.CheckInTime.Date <= checkInDate.Date && x.CheckOutTime.Date > checkInDate.Date)
        || (x.CheckInTime.Date < checkOutDate.Date && x.CheckOutTime.Date > checkOutDate.Date)
        || (checkInDate.Date <= x.CheckInTime.Date && x.CheckOutTime.Date <= checkOutDate.Date)).ToList();

        var bookingRoomIds = bookingRoomList.Select(x => x.RoomId).ToList();

        //var availableRoomList = (await _iRepository.GetAsync(x => !x.IsDeleted && x.IsActive && !bookingRoomIds.Contains(x.Id)
        //&& x.HouseKeeperAvailabilityStatus == AvailabilityStatusEnum.Available, c => c.RoomCategory)).ToList();

        var availableRoomList = (await _iRepository.GetAsync(x => !x.IsDeleted && x.IsActive && !bookingRoomIds.Contains(x.Id), c => c.RoomCategory)).ToList();

        return availableRoomList.Any(x => x.Id == roomId);
    }

    #endregion

    #region HkRoomViewPrintHtml
    public async Task<string> HkRoomViewPrintHtml(DataTablePagination<HtRoomInfoSearchVm, HtRoomInfoSearchVm> model)
    {
        var fullHtml = "";
        var dataTable = await _iRepository.HouseKeeperViewSearchAllAsync(model);
        var data = dataTable.data;

        fullHtml += "<table class='table table-bordered' style='width:100%;text-align:center;repeat-header:yes;'>";

        fullHtml += "<thead>";
        fullHtml += "<tr>";
        fullHtml += "<th style='width:5%'>Sl.</th>";
        fullHtml += "<th style='width:10%'>Room No</th>";
        fullHtml += "<th style='width:30%'>Category</th>";
        fullHtml += "<th style='width:15%'>Availability</th>";
        fullHtml += "<th style='width:15%'>Status</th>";
        fullHtml += "<th style='width:25%'>HouseKeeper</th>";
        fullHtml += "</tr>";
        fullHtml += "</thead>";

        fullHtml += "<tbody>";
        if (data.Count > 0)
        {
            foreach (var (v, i) in data.GetItemWithIndex())
            {
                fullHtml += "<tr>";

                fullHtml += $@"<td>{i + 1}</td>";
                fullHtml += $@"<td class='text-center'>{v.RoomNo}</td>";
                fullHtml += $@"<td class='text-start'>{v.RoomCategoryName}</td>";
                fullHtml += $@"<td class='text-center'>{v.AvailabilityStatusText}</td>";
                fullHtml += $@"<td class='text-center'>{v.CleaningStatusText}</td>";
                fullHtml += $@"<td class='text-center'>{v.HouseKeeperName}</td>";

                fullHtml += "</tr>";
            }
        }
        fullHtml += "</tbody>";
        fullHtml += "</table>";

        return fullHtml;
    }

    #endregion

    #region CleanStatusUpdate

    public async Task<bool> RoomCleanStatusUpdate(long roomId, int cleanStatus)
    {
        var cleaningStatus = (CleaningStatusEnum)cleanStatus;
        var roomInfo = await _iRepository.GetByIdAsync(roomId);
        if (roomInfo == null)
            throw new Exception("Room Information Not Found...!!");

        roomInfo.CleaningStatus = cleaningStatus;
        roomInfo.HouseKeeperAvailabilityStatus = cleaningStatus == CleaningStatusEnum.VC ? AvailabilityStatusEnum.Available : roomInfo.HouseKeeperAvailabilityStatus;


        var roomHstList = await _iRoomStatusHistoryRepository.GetAsync(x => x.RoomId == roomId && x.IsCurrent && !x.IsDeleted);

        if (roomHstList.Count > 0)
            roomHstList.ForEach(x => x.IsCurrent = false);

        var roomStatusHst = new HtRoomStatusHistory();
        roomStatusHst.RoomId = roomId;
        roomStatusHst.RoomStatusById = CurrentUserId;
        roomStatusHst.StatusDate = DateTime.Now;
        roomStatusHst.IsCurrent = true;
        roomStatusHst.ActionDate = DU.Utility.GetBdDateTimeNow();
        roomStatusHst.ActionById = CurrentUserId;

        var userInfo = await _iApplicationUserService.GetByIdAsync(CurrentUserId);

        if (roomInfo.CleaningStatus == CleaningStatusEnum.VD)
        {
            roomStatusHst.RoomStatus = RoomStatusHstEnum.VD;

            _iRoomLogger.LogInformation($"Room Is Make V & D By {userInfo?.FullName} Force-Fully. " +
                $"{DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss tt")}");
        }
        else if (roomInfo.CleaningStatus == CleaningStatusEnum.OOO)
        {
            roomStatusHst.RoomStatus = RoomStatusHstEnum.OutOfOrder;

            _iRoomLogger.LogInformation($"Room Is Make OOO By {userInfo?.FullName} Force-Fully. " +
                $"{DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss tt")}");
        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        if (roomInfo.CleaningStatus == CleaningStatusEnum.VD || roomInfo.CleaningStatus == CleaningStatusEnum.OOO)
        {
            if (roomHstList.Count > 0)
            {
                _iRoomStatusHistoryRepository.UpdateRange(roomHstList);
            }

            await _iRoomStatusHistoryRepository.AddAsync(roomStatusHst);
        }

        await _iRepository.UpdateAsync(roomInfo);
        var isUpdate = await _iUnitOfWork.CompleteAsync();

        if (!isUpdate) return false;
        ts.Complete();
        return true;
    }

    #endregion

    #region RoomAvailabilityStatusUpdate

    public async Task<bool> RoomAvailabilityStatusUpdate(long roomId, int availabilityStatus)
    {
        var roomInfo = await _iRepository.GetByIdAsync(roomId);
        if (roomInfo == null)
            throw new Exception("Room Information Not Found...!!");

        roomInfo.HouseKeeperAvailabilityStatus = (AvailabilityStatusEnum)availabilityStatus;

        var roomHstList = await _iRoomStatusHistoryRepository.GetAsync(x => x.RoomId == roomId && x.IsCurrent && !x.IsDeleted);

        if (roomHstList.Count > 0)
            roomHstList.ForEach(x => x.IsCurrent = false);

        var roomStatusHst = new HtRoomStatusHistory();
        roomStatusHst.RoomId = roomId;
        roomStatusHst.RoomStatusById = CurrentUserId;
        roomStatusHst.StatusDate = DateTime.Now;
        roomStatusHst.IsCurrent = true;
        roomStatusHst.ActionDate = DU.Utility.GetBdDateTimeNow();
        roomStatusHst.ActionById = CurrentUserId;

        var userInfo = await _iApplicationUserService.GetByIdAsync(CurrentUserId);

        if (roomInfo.HouseKeeperAvailabilityStatus == AvailabilityStatusEnum.Available)
        {
            roomStatusHst.RoomStatus = RoomStatusHstEnum.Available;

            _iRoomLogger.LogInformation($"Room Is Make Available By {userInfo?.FullName} Force-Fully. " +
                $"{DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss tt")}");
        }
        else if (roomInfo.HouseKeeperAvailabilityStatus == AvailabilityStatusEnum.OutOfOrder)
        {
            roomStatusHst.RoomStatus = RoomStatusHstEnum.OutOfOrder;

            _iRoomLogger.LogInformation($"Room Is Make Out Of Order By {userInfo?.FullName} Force-Fully. " +
                $"{DateTime.Now.ToString("dd/MMM/yyyy hh:mm:ss tt")}");
        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        if (roomInfo.CleaningStatus == CleaningStatusEnum.VD || roomInfo.CleaningStatus == CleaningStatusEnum.OOO)
        {
            if (roomHstList.Count > 0)
            {
                _iRoomStatusHistoryRepository.UpdateRange(roomHstList);
            }

            await _iRoomStatusHistoryRepository.AddAsync(roomStatusHst);
        }

        await _iRepository.UpdateAsync(roomInfo);
        var isUpdate = await _iUnitOfWork.CompleteAsync();

        if (!isUpdate) return false;
        ts.Complete();
        return true;
    }

    #endregion

    #region GetAvailableRooms

    public async Task<dynamic> GetAvailableRooms(DateTime checkInDate, DateTime checkOutDate, long? categoryId)
    {
        var bookingServiceList = await _iBookingServiceRepository.GetAsync(x => (checkInDate.Date >= x.CheckInTime.Date && checkInDate.Date < x.CheckOutTime.Date)
        || (checkOutDate.Date > x.CheckInTime.Date && checkOutDate.Date <= x.CheckOutTime.Date)
        || (checkInDate.Date <= x.CheckInTime.Date && x.CheckOutTime.Date <= checkOutDate.Date));

        bookingServiceList = bookingServiceList.Where(x => x.BookingStatus == BookingServiceStatusEnum.Booked || x.BookingStatus == BookingServiceStatusEnum.CheckIn).ToList();

        var bookingServiceIds = bookingServiceList.Select(x => x.Id).ToList();
        var bookingRoomList = await _iBookingRoomRepository.GetAsync(x => bookingServiceIds.Contains(x.BookingId) && !x.IsDeleted);

        bookingRoomList = bookingRoomList.Where(x => (x.CheckInTime.Date <= checkInDate.Date && x.CheckOutTime.Date > checkInDate.Date)
        || (x.CheckInTime.Date < checkOutDate.Date && x.CheckOutTime.Date > checkOutDate.Date)
        || (checkInDate.Date <= x.CheckInTime.Date && x.CheckOutTime.Date <= checkOutDate.Date)).ToList();

        var bookingRoomIds = bookingRoomList.Select(x => x.RoomId).ToList();

        var availableRoomList = (await _iRepository.GetAsync(x => !x.IsDeleted && x.IsActive && !bookingRoomIds.Contains(x.Id), c => c.RoomCategory)).Select(c => new
        {
            c.Id,
            c.RoomNo,
            c.Rent,
            c.ServiceCharge,
            c.TotalRent,
            CategoryId = c.RoomCategoryId,
            CategoryName = c.RoomCategory.CategoryName,
        }).ToList().OrderBy(o => o.RoomNo);

        if (categoryId > 0)
        {
            availableRoomList = (await _iRepository.GetAsync(x => x.RoomCategoryId == categoryId && !x.IsDeleted && x.IsActive && !bookingRoomIds.Contains(x.Id), c => c.RoomCategory)).Select(c => new
            {
                c.Id,
                c.RoomNo,
                c.Rent,
                c.ServiceCharge,
                c.TotalRent,
                CategoryId = c.RoomCategoryId,
                CategoryName = c.RoomCategory.CategoryName,
            }).ToList().OrderBy(o => o.RoomNo);
        }

        return availableRoomList;
    }

    #endregion

    #region MultipleRoomStatusUpdate
    public async Task<bool> MultipleRoomStatusUpdate(List<long> roomIds, int? cleanStatus)
    {
        if (roomIds == null || roomIds.Count == 0)
            throw new Exception("No Room Data Found");

        var historiesToUpdate = new List<HtRoomStatusHistory>();
        var historiesToAdd = new List<HtRoomStatusHistory>();
        var roomsToUpdate = new List<HtRoomInfo>();
        var assignRoomUpdateList = new List<HkRoomAssign>();
        var updateableTaskList = new List<HkTaskAssign>();

        var roomList = await _iRepository.GetAsync(x => roomIds.Contains(x.Id) && !x.IsDeleted);
        var roomHistoryList = await _iRoomStatusHistoryRepository.GetAsync(x => roomIds.Contains(x.RoomId) && x.IsCurrent && !x.IsDeleted);

        var assignRoomList = await _iRoomAssignRepository.GetAsync(x => roomIds.Contains(x.RoomId) && x.Status == RoomAssignEnum.Assigned && !x.IsDeleted);
        var assignIds = assignRoomList.Select(x => x.Id).ToList();

        var assignTaskList = await _iTaskAssignRepository.GetAsync(x => assignIds.Contains(x.AssignId) && x.CompleteDate == null && !x.IsDeleted, o => o.Task);

        foreach (var roomId in roomIds)
        {
            var room = roomList.FirstOrDefault(x => x.Id == roomId) ?? throw new Exception($"Room information not found..!!");
            var roomHistories = roomHistoryList.Where(x => x.RoomId == roomId).ToList();
            var current = _iMapper.Map<List<HtRoomStatusHistory>>(roomHistories);

            if (current != null && current.Count > 0)
            {
                current.ForEach(h => h.IsCurrent = false);
                historiesToUpdate.AddRange(current);
            }

            var newHistory = new HtRoomStatusHistory
            {
                RoomId = roomId,
                RoomStatusById = CurrentUserId,
                StatusDate = DateTime.Now,
                IsCurrent = true,
                ActionDate = DU.Utility.GetBdDateTimeNow(),
                ActionById = CurrentUserId
            };

            var shouldAdd = false;

            if (cleanStatus != null)
            {
                room.CleaningStatus = (CleaningStatusEnum)cleanStatus.Value;
                room.HouseKeeperAvailabilityStatus = room.CleaningStatus == CleaningStatusEnum.VC ? AvailabilityStatusEnum.Available : room.HouseKeeperAvailabilityStatus;

                if (room.CleaningStatus == CleaningStatusEnum.VD)
                {
                    newHistory.RoomStatus = RoomStatusHstEnum.VD; shouldAdd = true;
                }
                else if (room.CleaningStatus == CleaningStatusEnum.VC)
                {
                    newHistory.RoomStatus = RoomStatusHstEnum.Available;
                    shouldAdd = true;
                    #region Assigned Room
                    var assignRoom = assignRoomList?.FirstOrDefault(x => x.RoomId == room.Id);
                    if (assignRoom != null)
                    {
                        assignRoom.Status = RoomAssignEnum.Completed;
                        assignRoomUpdateList.Add(assignRoom);
                    }

                    #endregion

                    #region Assigned Task
                    var assignedTask = assignTaskList?.FirstOrDefault(x => x.AssignId == assignRoom.Id && x.Task.Name == HKTaskName.RoomCleaning);

                    if (assignedTask != null)
                    {
                        assignedTask.CompleteDate = DU.Utility.GetBdDateTimeNow();
                        assignedTask.CompleteRemarks = $@"Task Completed On {DU.Utility.GetBdDateTimeNow()}";
                        assignedTask.UpdatedById = CurrentUserId;
                        assignedTask.UpdateDate = DU.Utility.GetBdDateTimeNow();

                        updateableTaskList.Add(assignedTask);
                    }
                    #endregion
                }
            }

            if (shouldAdd) historiesToAdd.Add(newHistory);

            room.UpdatedById = CurrentUserId;
            roomsToUpdate.Add(room);
        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        if (historiesToUpdate.Any())
            _iRoomStatusHistoryRepository.UpdateRange(historiesToUpdate);

        if (historiesToAdd.Any())
            await _iRoomStatusHistoryRepository.AddRangeAsync(historiesToAdd);

        if (roomsToUpdate.Any())
            await _iRepository.UpdateRangeAsync(roomsToUpdate);

        if (assignRoomUpdateList.Any())
            await _iRoomAssignRepository.UpdateRangeAsync(assignRoomUpdateList);

        if (updateableTaskList.Any())
            await _iTaskAssignRepository.UpdateRangeAsync(updateableTaskList);

        var ok = await _iUnitOfWork.CompleteAsync();
        if (!ok) return false;

        ts.Complete();
        return true;
    }
    #endregion

    #region MultipleRoomMakeOOO
    public async Task<bool> MultipleRoomMakeOOO(List<long> roomIds, string remarks)
    {
        if (roomIds == null || roomIds.Count == 0)
            throw new Exception("No Room Data Found");

        //var historiesToUpdate = new List<HtRoomStatusHistory>();
        //var historiesToAdd = new List<HtRoomStatusHistory>();
        var roomsToUpdate = new List<HtRoomInfo>();

        var roomList = await _iRepository.GetAsync(x => roomIds.Contains(x.Id) && !x.IsDeleted);
        //var roomHistoryList = await _iRoomStatusHistoryRepository.GetAsync(x => roomIds.Contains(x.RoomId) && x.IsCurrent && !x.IsDeleted);
        var assignRoomList = await _iRoomAssignRepository.GetAsync(x => roomIds.Contains(x.RoomId) && x.Status == RoomAssignEnum.Assigned && !x.IsDeleted);

        foreach (var roomId in roomIds)
        {
            var room = roomList.FirstOrDefault(x => x.Id == roomId) ?? throw new Exception($"Room information not found..!!");
            // var roomHistories = roomHistoryList.Where(x => x.RoomId == roomId).ToList();
            //var current = _iMapper.Map<List<HtRoomStatusHistory>>(roomHistories);

            //if (current != null && current.Count > 0)
            //{
            //    current.ForEach(h => h.IsCurrent = false);
            //    historiesToUpdate.AddRange(current);
            //}

            //var newHistory = new HtRoomStatusHistory
            //{
            //    RoomId = roomId,
            //    RoomStatusById = CurrentUserId,
            //    StatusDate = DateTime.Now,
            //    IsCurrent = true,
            //    ActionDate = DU.Utility.GetBdDateTimeNow(),
            //    ActionById = CurrentUserId
            //};

            //var shouldAdd = false;

            if (room != null)
            {
                room.HouseKeeperAvailabilityStatus = AvailabilityStatusEnum.OutOfOrder;
                room.CleaningStatus = CleaningStatusEnum.OOO;
                room.OOORemakrs = remarks;
                room.OOODate = Utility.GetBdDateTimeNow();
                room.UpdatedById = CurrentUserId;

                //newHistory.RoomStatus = RoomStatusHstEnum.OutOfOrder;

            }

            //if (shouldAdd) historiesToAdd.Add(newHistory);
            roomsToUpdate.Add(room);
        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        //await _iRoomStatusHistoryRepository.AddRangeAsync(historiesToAdd);

        if (roomsToUpdate.Any())
            await _iRepository.UpdateRangeAsync(roomsToUpdate);

        var ok = await _iUnitOfWork.CompleteAsync();
        if (!ok) return false;

        ts.Complete();
        return true;

    }
    #endregion

    #region MultipleRoomMakeAvailable
    public async Task<bool> MultipleRoomMakeAvailable(List<long> roomIds)
    {
        if (roomIds == null || roomIds.Count == 0)
            throw new Exception("No Room Data Found");

        var roomsToUpdate = new List<HtRoomInfo>();
        var assignRoomUpdateList = new List<HkRoomAssign>();

        var roomList = await _iRepository.GetAsync(x => roomIds.Contains(x.Id) && !x.IsDeleted);
        if (!(roomList.Count > 0))
        {
            throw new Exception("No Rooms Found");
        }
        var assignRoomList = await _iRoomAssignRepository.GetAsync(x => roomIds.Contains(x.RoomId) && x.Status == RoomAssignEnum.Assigned && !x.IsDeleted);

        foreach (var roomId in roomIds)
        {
            var room = roomList.FirstOrDefault(x => x.Id == roomId) ?? throw new Exception($"Room information not found..!!");

            room.CleaningStatus = CleaningStatusEnum.VC;
            room.HouseKeeperAvailabilityStatus = AvailabilityStatusEnum.Available;
            room.OOODate = null;
            room.OOORemakrs = "";
            room.UpdatedById = CurrentUserId;

            var assignRoom = assignRoomList?.FirstOrDefault(x => x.RoomId == room.Id);
            if (assignRoom != null)
            {
                assignRoom.Status = RoomAssignEnum.Completed;
                assignRoomUpdateList.Add(assignRoom);
            }
            roomsToUpdate.Add(room);
        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        if (roomsToUpdate.Any())
            await _iRepository.UpdateRangeAsync(roomsToUpdate);

        if (assignRoomUpdateList.Any())
            await _iRoomAssignRepository.UpdateRangeAsync(assignRoomUpdateList);

        var ok = await _iUnitOfWork.CompleteAsync();
        if (!ok) return false;

        ts.Complete();
        return true;

    }
    #endregion
}
