using AutoMapper;
using Domain.Entities.Inventory;
using Domain.Enums.AppEnums;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.ItemInfo;
using Domain.ViewModel.Report;
using Interface.Repository.Common;
using Interface.Repository.Inventory;
using Interface.Services.Inventory;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Inventory
{
    public class ItemInfoService : BaseService<ItemInfo>, IItemInfoService
    {
        #region Config
        private IItemInfoRepository Repository { get; }
        private readonly IMapper _iMapper;
        private readonly ICategoryInfoService _iCategoryInfoService;
        private readonly IUnitOfWork _iUnitOfWork;
        private readonly IAutoCodeRepository _iAutoCodeRepository;
        private readonly IInventoryReportRepository _iInventoryReportRepository;

        public ItemInfoService(IItemInfoRepository iRepository, IMapper iMapper, ICategoryInfoService iCategoryService, IUnitOfWork iUnitOfWork, IAutoCodeRepository iAutoCodeRepository, IInventoryReportRepository iInventoryReportRepository) : base(iRepository, iUnitOfWork)
        {
            Repository = iRepository;
            _iMapper = iMapper;
            _iCategoryInfoService = iCategoryService;
            _iUnitOfWork = iUnitOfWork;
            _iAutoCodeRepository = iAutoCodeRepository;
            _iInventoryReportRepository = iInventoryReportRepository;
        }

        #endregion

        public async Task<DataTablePagination<ItemInfoSearchVm, ItemInfoSearchVm>>
            SearchAsync(DataTablePagination<ItemInfoSearchVm, ItemInfoSearchVm> model)
        {
            var dataList = await Repository.SearchAsync(model);
            return dataList;
        }

        public List<ItemInfo> GetItemsByCategoryType(string categoryType)
        {
            var categoriesIds = _iCategoryInfoService.Get(c => c.CategoryType == categoryType).Select(c => c.Id);

            var items = Repository.Get(c => categoriesIds.Contains(c.CategoryId)).ToList();

            return items;
        }

        public async Task<string> GetItemInfoCode()
        {
            var data = await _iAutoCodeRepository.GetMaxAutoCode(TableEnum.ItemInfos.ToString(), "ItemCode", "I", 3);
            return data;
        }

        public async Task<double> GetItemCurrentStockByItemId(long itemId)
        {
            var reportItem = (await _iInventoryReportRepository.GetInventoryStockInfo(new StockVm { ItemId = itemId })).FirstOrDefault();

            return reportItem != null ? reportItem.Stock : 0;
        }

        #region Details
        public async Task<ItemInfoVm> Details(long id)
        {
            var model = await Repository.Details(id);
            return model;
        }
        #endregion
    }
}
