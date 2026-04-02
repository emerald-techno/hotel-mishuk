using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Enums.AppEnums;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.Service;
using Interface.Repository.Common;
using Interface.Repository.HotelManagement;
using Interface.Services.HotelManagement;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.HotelManagement;

public class HtServiceService : BaseService<HtService>, IHtServiceService
{
    #region Config
    private IServiceRepository _iRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly IAutoCodeRepository _iAutoCodeRepository;

    public HtServiceService(IServiceRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork, IAutoCodeRepository iAutoCodeRepository) : base(iRepository, iUnitOfWork)
    {
        _iRepository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
        _iAutoCodeRepository = iAutoCodeRepository;
    }

    #endregion

    public async Task<DataTablePagination<ServiceSearchVm, ServiceSearchVm>> SearchAsync(DataTablePagination<ServiceSearchVm, ServiceSearchVm> model)
    {
        var dataList = await _iRepository.SearchAsync(model);
        return dataList;
    }

    #region GetServiceCode

    public async Task<string> GetServiceCode()
    {
        var data = await _iAutoCodeRepository.GetMaxAutoCode(TableEnum.HtServices.ToString(), "ServiceCode", "SR", 6);
        return data;
    }

    #endregion
}
