using AutoMapper;
using Domain.Entities.Inventory;
using Domain.Enums.AppEnums;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.CategoryInfo;
using Interface.Repository.Common;
using Interface.Repository.Inventory;
using Interface.Services.Inventory;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Inventory;

public class CategoryInfoService : BaseService<CategoryInfo>, ICategoryInfoService
{

    #region Config
    private ICategoryInfoRepository Repository { get; }
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly IAutoCodeRepository _iAutoCodeRepository;

    public CategoryInfoService(ICategoryInfoRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork, IAutoCodeRepository iAutoCodeRepository) : base(iRepository, iUnitOfWork)
    {
        Repository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
        _iAutoCodeRepository = iAutoCodeRepository;
    }

    #endregion


    public async Task<DataTablePagination<CategoryInfoSearchVm, CategoryInfoSearchVm>>
        SearchAsync(DataTablePagination<CategoryInfoSearchVm, CategoryInfoSearchVm> model)
    {
        var dataList = await Repository.SearchAsync(model);
        return dataList;
    }

    public async Task<string> GetCategoryInfoCode()
    {
        var data = await _iAutoCodeRepository.GetMaxAutoCode(TableEnum.CategoryInfos.ToString(), "CategoryCode", "CAT", 3);
        return data;
    }
}
