using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Enums.AppEnums;
using Domain.Utility.Common;
using Domain.ViewModel.Restaurant.Customer;
using Interface.Repository.Common;
using Interface.Repository.HotelManagement;
using Interface.Repository.Restaurant;
using Interface.Services.Restaurant;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Restaurant;

public class CustomerService : BaseService<RsCustomer>, ICustomerService
{
    #region Config
    private ICustomerRepository _iRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly IAutoCodeRepository _iAutoCodeRepository;

    private readonly IBookingServiceRepository _iBookingServiceRepository;
    private readonly IBookingRoomRepository _iBookingRoomRepository;
    private readonly IBookingGuestRepository _iBookingGuestRepository;
    private readonly IRoomInfoRepository _iRoomInfoRepository;

    public CustomerService(ICustomerRepository iRepository,
        IMapper iMapper,
        IUnitOfWork iUnitOfWork,
        IAutoCodeRepository iAutoCodeRepository,
        IRoomInfoRepository iRoomInfoRepository,
        IBookingServiceRepository iBookingServiceRepository,
        IBookingRoomRepository iBookingRoomRepository,
        IBookingGuestRepository iBookingGuestRepository) : base(iRepository, iUnitOfWork)
    {
        _iRepository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
        _iAutoCodeRepository = iAutoCodeRepository;
        _iRoomInfoRepository = iRoomInfoRepository;
        _iBookingServiceRepository = iBookingServiceRepository;
        _iBookingRoomRepository = iBookingRoomRepository;
        _iBookingGuestRepository = iBookingGuestRepository;
    }

    #endregion

    #region CustomerEntry
    public async Task<bool> CustomerEntry(CustomerVm vm)
    {
        if (vm == null)
            throw new Exception("Customer info not found...!");

        var existingCustomerInfo = _iRepository.GetSingleOrDefault(d => d.Mobile == vm.Mobile);
        if (existingCustomerInfo != null)
            throw new Exception("Customer mobile number already exists");

        var customerModel = _iMapper.Map<RsCustomer>(vm);

        customerModel.CustomerCode = await GetCustomerCode();
        customerModel.ActionById = CurrentUserId;
        customerModel.ActionDate = Domain.Utility.Utility.GetBdDateTimeNow();

        await _iRepository.AddAsync(customerModel);
        var isAdded = await _iUnitOfWork.CompleteAsync();
        if (!isAdded) return false;
        return true;
    }
    #endregion

    #region Search

    public async Task<DataTablePagination<CustomerSearchVm, CustomerSearchVm>> SearchAsync(DataTablePagination<CustomerSearchVm, CustomerSearchVm> model)
    {
        var dataList = await _iRepository.SearchAsync(model);
        return dataList;
    }

    #endregion

    #region GetGuestCode

    public async Task<string> GetCustomerCode()
    {
        var data = await _iAutoCodeRepository.GetMaxAutoCode(TableEnum.RsCustomers.ToString(), "CustomerCode", "CUS", 6);
        return data;
    }

    #endregion

    #region CurrentGuestByRoomId

    public async Task<RsCustomer?> GetCurrentGuestCustomerByRoomId(long roomId)
    {
        if (roomId == 0)
            return null;

        var roomInfo = _iRoomInfoRepository.GetFirstOrDefault(x => x.Id == roomId && x.IsActive && !x.IsDeleted);
        if (roomInfo == null)
            return null;

        var bookingList = await _iBookingServiceRepository.GetAsync(x => x.BookingStatus == BookingServiceStatusEnum.CheckIn
        && x.BookingStatus != BookingServiceStatusEnum.CheckOut && !x.IsDeleted);
        var bookingIds = bookingList.Select(x => x.Id);

        var occupiedRoomList = await _iBookingRoomRepository.GetAsync(c => bookingIds.Contains(c.BookingId) && !c.IsDeleted);

        var filterData = occupiedRoomList.FirstOrDefault(x => x.RoomId == roomId && x.ActualCheckInTime != null && x.ActualCheckOutTime == null && !x.IsDeleted);
        if (filterData == null)
            return null;

        var roomGuestInfo = await _iBookingGuestRepository.GetFirstOrDefaultAsync(x => x.BookingId == filterData.BookingId && x.IsMain && !x.IsDeleted);
        if (roomGuestInfo == null)
            return null;

        var customerInfo = await _iRepository.GetFirstOrDefaultAsync(x => x.GuestId == roomGuestInfo.GuestId && !x.IsDeleted);
        if (customerInfo == null)
            return null;

        return customerInfo;
    }

    #endregion
}
