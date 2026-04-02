using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Enums.AppEnums;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.RoomCategory;
using Domain.ViewModel.Website;
using Interface.Repository.HotelManagement;
using Interface.Services.HotelManagement;
using Interface.UnitOfWork;
using Services.Base;
using System.Collections.Generic;
using System.Transactions;

namespace Services.HotelManagement;

public class RoomCategoryService : BaseService<HtRoomCategory>, IRoomCategoryService
{
    #region Config
    private IRoomCategoryRepository _iRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly IRoomInfoRepository _iRoomInfoRepository;
    private readonly IBookingServiceRepository _iBookingServiceRepository;
    private readonly IBookingRoomRepository _iBookingRoomRepository;

    public RoomCategoryService(IRoomCategoryRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork,
        IRoomInfoRepository iRoomInfoRepository,
        IBookingServiceRepository iBookingServiceRepository,
        IBookingRoomRepository iBookingRoomRepository) : base(iRepository, iUnitOfWork)
    {
        _iRepository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
        _iRoomInfoRepository = iRoomInfoRepository;
        _iBookingServiceRepository = iBookingServiceRepository;
        _iBookingRoomRepository = iBookingRoomRepository;
    }

    #endregion

    public async Task<DataTablePagination<HtRoomCategorySearchVm, HtRoomCategorySearchVm>> SearchAsync(DataTablePagination<HtRoomCategorySearchVm, HtRoomCategorySearchVm> model)
    {
        var dataList = await _iRepository.SearchAsync(model);
        return dataList;
    }

    public async Task<List<HtRoomCategoryVm>> RoomCategoryPublicData()
    {
        var dataList = await _iRepository.GetRoomCategoryPublicData();
        return dataList;
    }

    public async Task<bool> RoomCategoryAddAsync(HtRoomCategoryVm vm)
    {
        var roomCategoryModel = _iMapper.Map<HtRoomCategory>(vm);

        roomCategoryModel.ActionById = CurrentUserId;
        roomCategoryModel.ActionDate = Utility.GetBdDateTimeNow();
        roomCategoryModel.IsActive = true;

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        await _iRepository.AddAsync(roomCategoryModel);
        var isAdded = await _iUnitOfWork.CompleteAsync();
        if (!isAdded) { return false; }
        ts.Complete();
        return true;
    }

    public async Task<List<HtRoomCategory>> GetRoomCategoryByRoomAvailibity(DateTime? arrivedDate, DateTime? depatureDate, short? roomCount = 1)
    {
        
        var arrivalDate = arrivedDate?.Date;
        var departureDate = depatureDate?.Date;

        var bookingServiceList = await _iBookingServiceRepository.GetAsync(x => (arrivalDate >= x.CheckInTime.Date && arrivalDate < x.CheckOutTime.Date)
        || (departureDate > x.CheckInTime.Date && departureDate <= x.CheckOutTime.Date)
        || (arrivalDate <= x.CheckInTime.Date && x.CheckOutTime.Date <= departureDate));

        bookingServiceList = bookingServiceList.Where(x => x.BookingStatus == BookingServiceStatusEnum.Booked || x.BookingStatus == BookingServiceStatusEnum.CheckIn).ToList();

        var bookingServiceIds = bookingServiceList.Select(x => x.Id).ToList();
        var bookingRoomList = await _iBookingRoomRepository.GetAsync(x => bookingServiceIds.Contains(x.BookingId) && !x.IsDeleted);

        bookingRoomList = bookingRoomList.Where(x => (x.CheckInTime.Date <= arrivalDate && x.CheckOutTime.Date > arrivalDate)
        || (x.CheckInTime.Date < departureDate && x.CheckOutTime.Date > departureDate)
        || (arrivalDate <= x.CheckInTime.Date && x.CheckOutTime.Date <= departureDate)).ToList();

        var bookingRoomIds = bookingRoomList.Select(x => x.RoomId).ToList();

        var availableRoomList = (await _iRoomInfoRepository.GetAsync(x => !x.IsDeleted && x.IsActive && !bookingRoomIds.Contains(x.Id), c => c.RoomCategory)).Select(c => new
        {
            c.Id,
            c.RoomNo,
            c.Rent,
            c.ServiceCharge,
            c.TotalRent,
            CategoryId = c.RoomCategoryId,
            CategoryName = c.RoomCategory.CategoryName,
        }).ToList();

        if (availableRoomList == null && availableRoomList.Count > 0 is false)
            return new List<HtRoomCategory>();

        var availableRoomCategoryIds = availableRoomList.Select(x => x.CategoryId).ToList();
        var availableCategoryList = await _iRepository.GetAsync(x => availableRoomCategoryIds.Contains(x.Id) && !x.IsDeleted && x.IsActive && x.IsWebSiteShow);

        return availableCategoryList.OrderByDescending(x => x.CategoryName).ToList();
    }
    //public async Task<List<HtRoomCategoryDiscountSetUpVm>> DiscountSetUP()
    //{
    //    var roomInfo = await _iRoomInfoRepository.GetAsync(
    //    x => x.IsActive && !x.IsDeleted,
    //    c => c.RoomCategory
    //);

    //    var model = roomInfo
    //        .Select(x => x.RoomCategory)
    //        .Distinct()
    //        .Select(cat => new HtRoomCategoryDiscountSetUpVm
    //        {
    //            Id = cat.Id,
    //            RoomCategoryId = cat.Id,
    //            RoomCategory = cat.CategoryName,
    //            ActualAmount = cat.Rent,
    //            CurrentOfferRate = cat.OfferRate > 0 ? (long)cat.OfferRate : cat.Rent,
    //            FromDate = DateTime.Today,
    //            ToDate = DateTime.Today.AddDays(7),
    //            FromDateStr = DateTime.Today.ToString("yyyy-MM-dd"),
    //            ToDateStr = DateTime.Today.AddDays(7).ToString("yyyy-MM-dd"),
    //            DiscountType = DiscountTypeEnum.Percent,
    //            DiscountAmount = 0
    //        }).ToList();

    //    return model;
    //}

    public async Task<List<HtRoomCategoryDiscountSetUpVm>> GetDiscountedCategories(DateTime fromDate, DateTime toDate, DiscountTypeEnum discountType, double discountAmount)
    {
        var data = await _iRepository.GetAsync(x => x.IsActive && !x.IsDeleted);

        var model = data.Select(item =>
        {
            double newRate = item.Rent;

            if (discountType == DiscountTypeEnum.Percent)
            {
                newRate = item.Rent - (item.Rent * discountAmount / 100);
            }
            else if (discountType == DiscountTypeEnum.Amount)
                newRate = item.Rent - discountAmount;

            if (newRate < 0) newRate = 0;

            return new HtRoomCategoryDiscountSetUpVm
            {
                RoomCategoryId = item.Id,
                RoomCategory = item.CategoryName,
                ActualAmount = item.Rent,
                NewOfferRate = newRate,
                DiscountAmount = discountAmount,
                DiscountType = discountType,
                FromDate = fromDate,
                ToDate = toDate,
                FromDateStr = fromDate.ToString("yyyy-MM-dd"),
                ToDateStr = toDate.ToString("yyyy-MM-dd"),
                isPercent = discountType == DiscountTypeEnum.Percent
            };
        }).ToList();

        return model;
    }



}
