using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.Restaurant.Table;
using Interface.Repository.Restaurant;
using Interface.Services.Restaurant;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Restaurant;

public class TableService : BaseService<RsTable>, ITableService
{
    #region Config
    private ITableRepository _iRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;

    public TableService(ITableRepository iRepository,
        IMapper iMapper,
        IUnitOfWork iUnitOfWork) : base(iRepository, iUnitOfWork)
    {
        _iRepository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
    }

    #endregion

    #region Search
    public async Task<DataTablePagination<RsTableSearchVm, RsTableSearchVm>> SearchAsync(DataTablePagination<RsTableSearchVm, RsTableSearchVm> model)
    {
        var dataList = await _iRepository.SearchAsync(model);
        return dataList;
    }
    #endregion
}
