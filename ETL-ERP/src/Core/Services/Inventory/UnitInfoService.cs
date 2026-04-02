using AutoMapper;
using Domain.Entities.Inventory;
using Domain.Enums.AppEnums;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.UnitInfo;
using Interface.Repository.Common;
using Interface.Repository.Inventory;
using Interface.Services.Inventory;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Inventory;

public class UnitInfoService : BaseService<UnitInfo>, IUnitInfoService
{
    #region CONFIG
    private IUnitInfoRepository _iRepository { get; }
    private IUnitOfWork _iUnitOfWork;
    private IAutoCodeRepository _iAutoCodeRepository;
    private readonly IMapper _iMapper;


    public UnitInfoService(IUnitInfoRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork, IAutoCodeRepository iAutoCodeRepository) : base(iRepository, iUnitOfWork)
    {
        _iRepository = iRepository;
        _iUnitOfWork = iUnitOfWork;
        _iAutoCodeRepository = iAutoCodeRepository;
        _iMapper = iMapper;
    }

    #endregion


    public async Task<DataTablePagination<UnitInfoSearchVm, UnitInfoSearchVm>>
        SearchAsync(DataTablePagination<UnitInfoSearchVm, UnitInfoSearchVm> model)
    {
        var dataList = await _iRepository.SearchAsync(model);
        return dataList;
    }

    public async Task<string> GetUnitInfoCode()
    {
        var data = await _iAutoCodeRepository.GetMaxAutoCode(TableEnum.UnitInfos.ToString(), "UnitCode", "U", 3);
        return data;
    }
}
