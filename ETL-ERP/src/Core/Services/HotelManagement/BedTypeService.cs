using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.BedType;
using Interface.Repository.HotelManagement;
using Interface.Services.HotelManagement;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.HotelManagement;

public class BedTypeService : BaseService<HtBedType>, IBedTypeService
{
    #region Config
    private IBedTypeRepository _iRepository;
    private readonly IMapper _imapper;
    private readonly IUnitOfWork _unitOfWork;

    public BedTypeService(IBedTypeRepository repository, IMapper imapper, IUnitOfWork unitOfWork) : base(repository, unitOfWork)
    {
        _iRepository = repository;
        _imapper = imapper;
        _unitOfWork = unitOfWork;
    }

    #endregion

    public async Task<DataTablePagination<HtBedTypeSearchVm, HtBedTypeSearchVm>> SearchAsync(DataTablePagination<HtBedTypeSearchVm, HtBedTypeSearchVm> model)
    {
        var dataList = await _iRepository.SearchAsync(model);
        return dataList;
    }
}