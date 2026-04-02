using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.FloorInfo;
using Interface.Repository.HotelManagement;
using Interface.Services.HotelManagement;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.HotelManagement;

public class FloorInfoService : BaseService<HtFloorInfo>, IFloorInfoService
{
    #region Config
    private IFloorInfoRepository _iRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;

    public FloorInfoService(IFloorInfoRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork) : base(iRepository, iUnitOfWork)
    {
        _iRepository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
    }

    #endregion

    public async Task<DataTablePagination<HtFloorInfoSearchVm, HtFloorInfoSearchVm>> SearchAsync(DataTablePagination<HtFloorInfoSearchVm, HtFloorInfoSearchVm> model)
    {
        var dataList = await _iRepository.SearchAsync(model);
        return dataList;
    }
}
