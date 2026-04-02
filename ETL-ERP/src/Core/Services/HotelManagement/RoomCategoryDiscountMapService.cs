using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Utility;
using Interface.Repository.HotelManagement;
using Interface.Services.HotelManagement;
using Interface.UnitOfWork;
using Services.Base;
using System.Transactions;

namespace Services.HotelManagement;

public class RoomCategoryDiscountMapService : BaseService<HtRoomCategoryDiscountMap>, IRoomCategoryDiscountMapService
{
    #region Config
    private IRoomCategoryDiscountMapRepository _iRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly IRoomInfoRepository _iRoomInfoRepository;
    private readonly IBookingServiceRepository _iBookingServiceRepository;
    private readonly IBookingRoomRepository _iBookingRoomRepository;

    public RoomCategoryDiscountMapService(IRoomCategoryDiscountMapRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork,
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

    public async Task<bool> SubmitDiscountSetupDataAsync(List<HtRoomCategoryDiscountMap> discountSetupList)
    {
        if (discountSetupList == null || !(discountSetupList.Count > 0))
            return false;

        discountSetupList = discountSetupList.Where(x => x.DiscountAmount > 0).ToList();

        foreach (var item in discountSetupList)
        {
            item.ActionById = CurrentUserId;
            item.ActionDate = Utility.GetBdDateTimeNow();
            item.IsActive = true;
            item.IsDeleted = false;
        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await _iRepository.AddRangeAsync(discountSetupList);
        var isAdded = await _iUnitOfWork.CompleteAsync();

        if (!isAdded)
            return false;

        ts.Complete();
        return true;
    }

}
