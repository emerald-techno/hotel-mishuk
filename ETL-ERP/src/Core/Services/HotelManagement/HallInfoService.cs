using AutoMapper;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using DocumentFormat.OpenXml.Spreadsheet;
using Domain.Entities.HotelManagement;
using Domain.Enums.AppEnums;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.RoomInfo;
using Interface.Repository.HotelManagement;
using Interface.Services.HotelManagement;
using Interface.UnitOfWork;
using Services.Base;
using System.Transactions;
using DU = Domain.Utility;

namespace Services.HotelManagement;

public class HallInfoService : BaseService<HtHallInfo>, IHallInfoService
{
    #region Config
    private IHallInfoRepository _iRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;

    private readonly IBookingServiceRepository _iBookingServiceRepository;
    private readonly IBookingHallRepository _iBookingHallRepository;


    public HallInfoService(IHallInfoRepository iRepository,
        IRoomFacilityMapRepository iFacilityMapRepository,
        IMapper iMapper,
        IUnitOfWork iUnitOfWork,
        IBookingServiceRepository iBookingServiceRepository,
        IBookingHallRepository iBookingHallRepository) : base(iRepository, iUnitOfWork)
    {
        _iRepository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
        _iBookingServiceRepository = iBookingServiceRepository;
        _iBookingHallRepository = iBookingHallRepository;
    }

    #endregion

    #region HallAdd

    public async Task<bool> HallAddAsync(HtHallInfoVm vm)
    {
        var hallModel = _iMapper.Map<HtHallInfo>(vm);

        hallModel.ActionById = CurrentUserId;
        hallModel.ActionDate = DU.Utility.GetBdDateTimeNow();
        hallModel.IsActive = true;
        var vatAmount = DU.Utility.PercentCalculation(hallModel.Vat, hallModel.Rent);
        hallModel.TotalRent = hallModel.Rent + hallModel.ServiceCharge + vatAmount;

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await _iRepository.AddAsync(hallModel);
        var isAdded = await _iUnitOfWork.CompleteAsync();

        if (!isAdded) return false;
        ts.Complete();
        return true;
    }

    #endregion

    public async Task<DataTablePagination<HtHallInfoSearchVm, HtHallInfoSearchVm>> SearchAsync(DataTablePagination<HtHallInfoSearchVm, HtHallInfoSearchVm> model)
    {
        var dataList = await _iRepository.SearchAsync(model);
        return dataList;
    }

    #region CheckRoomIsAvaliable

    public async Task<bool> CheckHallIsAvaliable(DateTime bookingDate, int hallShift, long hallId)
    {
        if (hallShift == 0 && hallId == 0)
            return false;

        var hallBookingList = await _iBookingServiceRepository.GetAsync(x => x.CheckInTime.Date <= bookingDate.Date && x.CheckOutTime.Date >= bookingDate.Date && x.BookingType == BookingType.Hall && !x.IsDeleted);
        var unavailableList = hallBookingList.Where(x => x.BookingStatus == BookingServiceStatusEnum.CheckIn || x.BookingStatus == BookingServiceStatusEnum.Booked).ToList();
        var unavailableListIds = unavailableList.Select(x => x.Id);

        var unavailableHallList = await _iBookingHallRepository.GetAsync(c => unavailableListIds.Contains(c.BookingId) && c.BookingDate.Date == bookingDate.Date && !c.IsDeleted);

        if (hallShift > 0 && hallShift != (int)HallBookingShiftEnum.Both)
        {
            unavailableHallList = unavailableHallList.Where(x => x.HallShift == (HallBookingShiftEnum)hallShift || x.HallShift == HallBookingShiftEnum.Both).ToList();
        }
        else if (hallShift > 0 && hallShift == (int)HallBookingShiftEnum.Both)
        {
            unavailableHallList = unavailableHallList.Where(x => (x.HallShift == HallBookingShiftEnum.DayShift || x.HallShift == HallBookingShiftEnum.NightShift || x.HallShift == HallBookingShiftEnum.Both)).ToList();
        }

        var unavailableHallIds = unavailableHallList.Select(x => x.HallId).ToList();

        var totalAvailableHallList = await _iRepository.GetAsync(x => !unavailableHallIds.Contains(x.Id));

        return totalAvailableHallList.Any(x => x.Id == hallId);
    }

    #endregion
}
