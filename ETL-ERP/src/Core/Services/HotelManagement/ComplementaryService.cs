using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.Complementary;
using Interface.Repository.HotelManagement;
using Interface.Services.HotelManagement;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.HotelManagement;

public class ComplementaryService : BaseService<HtComplementary>, IComplementaryService
{
    #region Config
    private IComplementaryRepository _iRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;

    public ComplementaryService(IComplementaryRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork) : base(iRepository, iUnitOfWork)
    {
        _iRepository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
    }

    #endregion

    public async Task<DataTablePagination<HtComplementarySearchVm, HtComplementarySearchVm>> SearchAsync(DataTablePagination<HtComplementarySearchVm, HtComplementarySearchVm> model)
    {
        var dataList = await _iRepository.SearchAsync(model);
        return dataList;
    }
}