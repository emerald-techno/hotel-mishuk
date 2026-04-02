using AutoMapper;
using DocumentFormat.OpenXml.Wordprocessing;
using Domain.Entities.HotelManagement;
using Domain.Enums.AppEnums;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.RoomInfo;
using Interface.Repository.HotelManagement;
using Interface.Repository.HouseKeeping;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;
using System.Collections.Immutable;

namespace Repository.HotelManagement;

public class RoomInfoRepository : BaseRepository<HtRoomInfo>, IRoomInfoRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private IRoomAssignRepository _iRoomAssignRepository;
    private readonly IMapper _iMapper;

    public RoomInfoRepository(ApplicationDbContext db, IRoomAssignRepository iRoomAssignRepository, IMapper iMapper) : base(db)
    {
        Db = db;
        _iRoomAssignRepository = iRoomAssignRepository;
        _iMapper = iMapper;
    }
    #endregion

    #region Search
    public async Task<DataTablePagination<HtRoomInfoSearchVm, HtRoomInfoSearchVm>> SearchAsync(DataTablePagination<HtRoomInfoSearchVm, HtRoomInfoSearchVm> vm)
    {
        var searchResult = Context.HtRoomInfos
            .Include(c => c.Floor)
            .Include(c => c.RoomCategory)
            .Include(c => c.BedType)
            .AsQueryable().Where(c => !c.IsDeleted);
        var model = vm.SearchModel;

        if (model == null) throw new Exception("Search room facility not found");

        if (!string.IsNullOrEmpty(vm.Search.Value))
        {
            var value = vm.Search.Value.Trim().ToLower();
            searchResult = searchResult.Where(c => c.RoomNo.ToLower().Contains(value));
        }

        if (model.RoomCategoryId > 0)
        {
            searchResult = searchResult.Where(c => c.RoomCategoryId == model.RoomCategoryId);
        }

        if (model.FloorId > 0)
        {
            searchResult = searchResult.Where(c => c.FloorId == model.FloorId);
        }

        if (model.BedTypeId > 0)
        {
            searchResult = searchResult.Where(c => c.BedTypeId == model.BedTypeId);
        }

        if (model.BookingStatus != null)
        {
            var bookingStatus = (BookingStatusEnum)model.BookingStatus;
            searchResult = searchResult.Where(c => c.BookingStatus == bookingStatus);
        }

        if (model.CleaningStatus != null)
        {
            var cleanStatus = (CleaningStatusEnum)model.CleaningStatus;
            searchResult = searchResult.Where(c => c.CleaningStatus == cleanStatus);
        }

        if (model.HouseKeeperId > 0)
        {
            var assignRoomIds = Context.HkRoomAssigns
                .Where(c => !c.IsDeleted && (c.HouseKeeperId == model.HouseKeeperId && (c.Status != RoomAssignEnum.Completed || c.Status != RoomAssignEnum.Cancel))
                || (c.HouseKeeperId != model.HouseKeeperId && (c.Status == RoomAssignEnum.Assigned || c.Status == RoomAssignEnum.Running)))
                .Select(c => c.RoomId).ToList();

            searchResult = searchResult.Where(c => !assignRoomIds.Contains(c.Id));
        }

        var totalRecords = await searchResult.CountAsync();
        if (totalRecords > 0)
        {
            vm.recordsTotal = totalRecords;
            vm.recordsFiltered = totalRecords;
            vm.draw = vm.LineDraw ?? 0;

            var data = await searchResult.OrderBy(c => c.RoomNo)
                                         .Skip(vm.Start)
                                         .Take(vm.Length)
                                         .ToListAsync();

            vm.data = _iMapper.Map<List<HtRoomInfoSearchVm>>(data);

            var sl = vm.Start;

            foreach (var searchDto in vm.data)
            {
                var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                searchDto.SerialNo = ++sl;
                searchDto.RoomCategoryName = filterData.RoomCategory.CategoryName;
                searchDto.FloorName = filterData.Floor.FloorName;
                searchDto.BedTypeName = filterData.BedType?.TypeName;
            }
        }
        return vm;
    }

    #endregion

    #region HouseKeeperViewSearch
    public async Task<DataTablePagination<HtRoomInfoSearchVm, HtRoomInfoSearchVm>> HouseKeeperViewSearchAsync(DataTablePagination<HtRoomInfoSearchVm, HtRoomInfoSearchVm> vm)
    {
        var searchResult = Context.HtRoomInfos
            .Include(c => c.Floor)
            .Include(c => c.RoomCategory)
            .AsQueryable()
            .Where(c => !c.IsDeleted);

        var assignHouseKeeperList = Context.HkRoomAssigns
            .Include(c => c.HouseKeeper)
            .Where(x => !x.IsDeleted).ToList();

        var model = vm.SearchModel;

        if (model == null) throw new Exception("Search room not found");

        if (!string.IsNullOrEmpty(vm.Search.Value))
        {
            var value = vm.Search.Value.Trim().ToLower();
            searchResult = searchResult.Where(c => c.RoomNo.ToLower().Contains(value));
        }

        if (model.RoomCategoryId > 0)
        {
            searchResult = searchResult.Where(c => c.RoomCategoryId == model.RoomCategoryId);
        }

        if (model.FloorId > 0)
        {
            searchResult = searchResult.Where(c => c.FloorId == model.FloorId);
        }

        if (model.BookingStatus != null)
        {
            var bookingStatus = (BookingStatusEnum)model.BookingStatus;
            searchResult = searchResult.Where(c => c.BookingStatus == bookingStatus);
        }

        if (model.CleaningStatus != null && model.CleaningStatus > -1)
        {
            var cleanStatus = (CleaningStatusEnum)model.CleaningStatus;
            searchResult = searchResult.Where(c => c.CleaningStatus == cleanStatus);
        }

        if (model.HouseKeeperAvailabilityStatus != null)
        {
            var availabilityStatus = (AvailabilityStatusEnum)model.HouseKeeperAvailabilityStatus;
            searchResult = searchResult.Where(c => c.HouseKeeperAvailabilityStatus == availabilityStatus);
        }

        if (model.HouseKeeperId > 0)
        {
            //var assignRoomIds = Context.HkRoomAssigns
            //    .Where(c => !c.IsDeleted && (c.HouseKeeperId == model.HouseKeeperId && (c.Status != RoomAssignEnum.Completed || c.Status != RoomAssignEnum.Cancel))
            //    || (c.HouseKeeperId != model.HouseKeeperId && (c.Status == RoomAssignEnum.Assigned || c.Status == RoomAssignEnum.Running)))
            //    .Select(c => c.RoomId).ToList();

            var assignRoomIds = Context.HkRoomAssigns
                .Where(c => !c.IsDeleted && (c.HouseKeeperId == model.HouseKeeperId)).Select(c => c.RoomId).ToList();

            searchResult = searchResult.Where(c => !assignRoomIds.Contains(c.Id));
        }

        var totalRecords = await searchResult.CountAsync();
        if (totalRecords > 0)
        {
            vm.recordsTotal = totalRecords;
            vm.recordsFiltered = totalRecords;
            vm.draw = vm.LineDraw ?? 0;

            var data = await searchResult.OrderBy(c => c.RoomNo)
                                         .Skip(vm.Start)
                                         .Take(vm.Length)
                                         .ToListAsync();

            vm.data = _iMapper.Map<List<HtRoomInfoSearchVm>>(data);

            var sl = vm.Start;

            foreach (var searchDto in vm.data)
            {
                var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                searchDto.SerialNo = ++sl;
                searchDto.RoomCategoryName = filterData.RoomCategory.CategoryName;
                searchDto.FloorName = filterData.Floor.FloorName;

                var assignHouseKeeper = assignHouseKeeperList.FirstOrDefault(x => x.RoomId == searchDto.Id);

                searchDto.HouseKeeperAssignId = assignHouseKeeper != null ? assignHouseKeeper.Id : null;
                searchDto.HouseKeeperName = assignHouseKeeper != null ? assignHouseKeeper.HouseKeeper.Name : "";
                searchDto.HouseKeeperStatus = assignHouseKeeper != null ? (int)assignHouseKeeper.Status : null;
            }
        }
        return vm;
    }

    #endregion

    #region HouseKeeperViewSearchAll

    public async Task<DataTablePagination<HtRoomInfoSearchVm, HtRoomInfoSearchVm>> HouseKeeperViewSearchAllAsync(DataTablePagination<HtRoomInfoSearchVm, HtRoomInfoSearchVm> vm)
    {
        var searchResult = Context.HtRoomInfos
            .Include(c => c.Floor)
            .Include(c => c.RoomCategory)
            .AsQueryable()
            .Where(c => !c.IsDeleted);

        var assignHouseKeeperList = Context.HkRoomAssigns
            .Include(c => c.HouseKeeper)
            .Where(x => !x.IsDeleted).ToList();

        var model = vm.SearchModel;

        if (model == null) throw new Exception("Search room not found");

        if (!string.IsNullOrEmpty(vm.Search.Value))
        {
            var value = vm.Search.Value.Trim().ToLower();
            searchResult = searchResult.Where(c => c.RoomNo.ToLower().Contains(value));
        }

        if (model.RoomCategoryId > 0)
        {
            searchResult = searchResult.Where(c => c.RoomCategoryId == model.RoomCategoryId);
        }

        if (model.FloorId > 0)
        {
            searchResult = searchResult.Where(c => c.FloorId == model.FloorId);
        }

        if (model.BookingStatus != null)
        {
            var bookingStatus = (BookingStatusEnum)model.BookingStatus;
            searchResult = searchResult.Where(c => c.BookingStatus == bookingStatus);
        }

        if (model.CleaningStatus != null)
        {
            var cleanStatus = (CleaningStatusEnum)model.CleaningStatus;
            searchResult = searchResult.Where(c => c.CleaningStatus == cleanStatus);
        }

        if (model.HouseKeeperAvailabilityStatus != null)
        {
            var availabilityStatus = (AvailabilityStatusEnum)model.HouseKeeperAvailabilityStatus;
            searchResult = searchResult.Where(c => c.HouseKeeperAvailabilityStatus == availabilityStatus);
        }

        if (model.HouseKeeperId > 0)
        {
            var assignRoomIds = Context.HkRoomAssigns
                .Where(c => !c.IsDeleted && (c.HouseKeeperId == model.HouseKeeperId)).Select(c => c.RoomId).ToList();

            searchResult = searchResult.Where(c => !assignRoomIds.Contains(c.Id));
        }

        var totalRecords = await searchResult.CountAsync();
        if (totalRecords > 0)
        {
            vm.recordsTotal = totalRecords;
            vm.recordsFiltered = totalRecords;
            vm.draw = vm.LineDraw ?? 0;

            var data = await searchResult.OrderBy(c => c.RoomNo)
                                         .ToListAsync();

            vm.data = _iMapper.Map<List<HtRoomInfoSearchVm>>(data);

            var sl = vm.Start;

            foreach (var searchDto in vm.data)
            {
                var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                searchDto.SerialNo = ++sl;
                searchDto.RoomCategoryName = filterData.RoomCategory.CategoryName;
                searchDto.FloorName = filterData.Floor.FloorName;

                var assignHouseKeeper = assignHouseKeeperList.FirstOrDefault(x => x.RoomId == searchDto.Id);

                searchDto.HouseKeeperAssignId = assignHouseKeeper != null ? assignHouseKeeper.Id : null;
                searchDto.HouseKeeperName = assignHouseKeeper != null ? assignHouseKeeper.HouseKeeper.Name : "";
                searchDto.HouseKeeperStatus = assignHouseKeeper != null ? (int)assignHouseKeeper.Status : null;
            }
        }
        return vm;
    }

    #endregion

    #region Dispose

    public void Dispose()
    {
        Context.Dispose();
    }

    #endregion

    public async Task<DayWiseRoomInfoVm> GetDayWiseRoomInfo(long roomId, DateTime selectedDate)
    {
        var allRooms = await Context.HtRoomInfos
            .Include(x => x.RoomCategory)
            .Include(x => x.Floor).ToListAsync();

        var roomInfo = allRooms.FirstOrDefault(x => x.Id == roomId && x.IsActive && !x.IsDeleted);

        if (roomInfo == null)
            throw new Exception("Room Information Not Found...!");

        var bookingServiceList = await Context.HtBookingServices.Where(x => (x.BookingStatus == BookingServiceStatusEnum.Booked || x.BookingStatus == BookingServiceStatusEnum.CheckIn)).AsNoTracking().ToListAsync();

        var bookingServiceIds = bookingServiceList.Select(x => x.Id).ToList();
        var bookingRoomList = await Context.HtBookingRooms
            .Include(b => b.Booking)
            .Where(x => bookingServiceIds.Contains(x.BookingId) && !x.IsDeleted)
            .AsNoTracking().ToListAsync();

        //bookingRoomList = bookingRoomList.Where(x => x.CheckInTime.Date <= selectedDate.Date && x.CheckOutTime.Date > selectedDate.Date).ToList();
        bookingRoomList = bookingRoomList.Where(x => x.CheckInTime.Date <= selectedDate.Date && x.CheckOutTime.Date >= selectedDate.Date && x.ActualCheckOutTime is null).ToList();
        
        var roomIds = allRooms.Select(x => x.Id).ToList();
        var assignList = await _iRoomAssignRepository.GetAsync(x => roomIds.Contains(x.RoomId), h => h.HouseKeeper);
        var assignedHouseKeeper = assignList?.FirstOrDefault(x => x.RoomId == roomInfo.Id && x.Status == RoomAssignEnum.Assigned)?.HouseKeeper;

        var model = new DayWiseRoomInfoVm();

        model.RoomId = roomInfo.Id;
        model.RoomNo = roomInfo.RoomNo;
        model.OOORemarks = roomInfo.OOORemakrs;
        model.CleaningStatus = (short)roomInfo.CleaningStatus;
        model.IsAc = roomInfo.IsAc;
        model.IsBelcony = roomInfo.IsBelcony;
        model.NumberOfBed = roomInfo.NumberOfBed;
        model.Person = roomInfo.Person;
        model.Rent = roomInfo.Rent;
        model.Vat = roomInfo.Vat;
        model.ServiceCharge = roomInfo.ServiceCharge;
        model.TotalRent = roomInfo.TotalRent;
        model.RoomCategoryId = roomInfo.RoomCategoryId;
        model.RoomCategoryName = roomInfo.RoomCategory.CategoryName;
        model.FloorId = roomInfo.FloorId;
        model.FloorName = roomInfo.Floor.FloorName;
        model.BookingStatus = 1; // Available
        model.AssignedHouseKeeperId = assignedHouseKeeper?.Id;

        //Commented By Tawkir: 18/02/2025: For same room booking & occupied issue.
        //var filterData = bookingRoomList.FirstOrDefault(x => x.RoomId == model.RoomId);
        var filterData = bookingRoomList.OrderBy(x => x.CheckInTime).FirstOrDefault(x => x.RoomId == model.RoomId);


        if (filterData != null)
        {
            //model.BookingStatus = (short)(filterData.Booking.BookingStatus == BookingServiceStatusEnum.Booked ? 2
            //    : filterData.Booking.BookingStatus == BookingServiceStatusEnum.CheckIn ? 3 : 1);
            var bill = await Context.HtBillings
           .Include(x => x.BillingDetails)
            .ThenInclude(x => x.Service)
           .FirstOrDefaultAsync(x => x.BookingId == filterData.BookingId);

            var extraBedServiceDetails = bill?.BillingDetails?.Where(x => x.Service.ServiceCode == HtServiceCode.ExtraBed && x.ServiceDate?.Date == DateTime.Today.Date).ToList();

            if (filterData.ActualCheckInTime != null)
            {
                model.BookingStatus = 3; // Occupied
            }
            else if (filterData.ActualCheckOutTime == null)
            {
                model.BookingStatus = 2; // Booked
            }

            model.CheckInDate = filterData.CheckInTime;
            model.CheckOutDate = filterData.CheckOutTime;

            var bookingGuest = await Context.HtBookingGuests
                .Include(g => g.Guest)
                .FirstOrDefaultAsync(x => x.BookingId == filterData.BookingId && x.IsMain);

            model.GuestName = $"{bookingGuest?.Guest?.Salutation} {bookingGuest?.Guest?.FirstName} {bookingGuest?.Guest?.LastName}";
            model.GuestMobile = bookingGuest?.Guest?.Mobile;
            model.BookingId = filterData.BookingId;
            model.ExtraBed = extraBedServiceDetails?.Sum(x=>x.Quantity);

            #region set room global status
            var bookingStatus = filterData.Booking.BookingStatus;

            if (bookingStatus == BookingServiceStatusEnum.Booked)
            {
                model.Status = 1;
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
                    if (roomInfo.CleaningStatus == CleaningStatusEnum.OOO)
                        model.Status = 4;
                    else if (roomInfo.CleaningStatus == CleaningStatusEnum.VD)
                        model.Status = 5;
                    else if (roomInfo.CleaningStatus == CleaningStatusEnum.VC)
                        model.Status = 3;
                }
                else if (filterData.ActualCheckInTime == null && filterData.ActualCheckOutTime == null)
                    model.Status = 1;
            }
            else if (bookingStatus == BookingServiceStatusEnum.CheckOut)
            {
                if (roomInfo.CleaningStatus == CleaningStatusEnum.OOO)
                    model.Status = 4;
                else if (roomInfo.CleaningStatus == CleaningStatusEnum.VD)
                    model.Status = 5;
                else if (roomInfo.CleaningStatus == CleaningStatusEnum.VC)
                    model.Status = 3;
            }
            #endregion
        }
        else
        {
            #region set global room status 
            if (roomInfo.CleaningStatus == CleaningStatusEnum.OOO)
                model.Status = 4;
            else if (roomInfo.CleaningStatus == CleaningStatusEnum.VD)
                model.Status = 5;
            else if (roomInfo.CleaningStatus == CleaningStatusEnum.VC)
                model.Status = 3;
            else
                model.Status = 3;
            #endregion
        }

        return model;
    }
}
