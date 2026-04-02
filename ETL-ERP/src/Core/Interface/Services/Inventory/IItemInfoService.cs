using Domain.Entities.Inventory;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.ItemInfo;
using Interface.Base;

namespace Interface.Services.Inventory
{
    public interface IItemInfoService : IService<ItemInfo>
    {
        Task<DataTablePagination<ItemInfoSearchVm, ItemInfoSearchVm>>
            SearchAsync(DataTablePagination<ItemInfoSearchVm, ItemInfoSearchVm> model);
        List<ItemInfo> GetItemsByCategoryType(string categoryType);
        Task<string> GetItemInfoCode();
        Task<double> GetItemCurrentStockByItemId(long itemId);
        Task<ItemInfoVm> Details(long id);
    }
}
