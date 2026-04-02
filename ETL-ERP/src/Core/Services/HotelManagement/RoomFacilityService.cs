using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.RoomFacility;
using Interface.Base;
using Interface.Repository.HotelManagement;
using Interface.Services.HotelManagement;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.HotelManagement;

public class RoomFacilityService : BaseService<HtRoomFacility>, IRoomFacilityService
{
    #region Config
    private IRoomFacilityRepository _iRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;

    public RoomFacilityService(IRoomFacilityRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork) : base(iRepository, iUnitOfWork)
    {
        _iRepository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
    }

    #endregion

    public async Task<DataTablePagination<HtRoomFacilitySearchVm, HtRoomFacilitySearchVm>> SearchAsync(DataTablePagination<HtRoomFacilitySearchVm, HtRoomFacilitySearchVm> model)
    {
        var dataList = await _iRepository.SearchAsync(model);
        return dataList;
    }
}