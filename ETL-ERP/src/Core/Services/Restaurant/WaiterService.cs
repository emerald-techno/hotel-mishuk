using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.Restaurant.Waiter;
using Interface.Repository.Restaurant;
using Interface.Services.Restaurant;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Restaurant;

public class WaiterService : BaseService<RsWaiter>, IWaiterService
{
    #region Config
    private IWaiterRepository _iRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;

    public WaiterService(IWaiterRepository iRepository,
        IMapper iMapper,
        IUnitOfWork iUnitOfWork) : base(iRepository, iUnitOfWork)
    {
        _iRepository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
    }

    #endregion

    public async Task<DataTablePagination<WaiterSearchVm, WaiterSearchVm>> SearchAsync(DataTablePagination<WaiterSearchVm, WaiterSearchVm> model)
    {
        var dataList = await _iRepository.SearchAsync(model);
        return dataList;
    }

    public async Task<bool> WaiterAddAsync(WaiterVm vm)
    {
        var waiterModel = _iMapper.Map<RsWaiter>(vm);

        waiterModel.Dob = !string.IsNullOrEmpty(vm.DobStr) ? Utility.ConvertStrToDate(vm.DobStr) : null;
        waiterModel.JoinDate = !string.IsNullOrEmpty(vm.JoinDateStr) ? Utility.ConvertStrToDate(vm.JoinDateStr) : null;
        waiterModel.ActionById = CurrentUserId;
        waiterModel.ActionDate = Utility.GetBdDateTimeNow();
        waiterModel.IsActive = true;

        await _iRepository.AddAsync(waiterModel);
        var isAdded = await _iUnitOfWork.CompleteAsync();

        if (!isAdded) return false;

        return true;
    }
}