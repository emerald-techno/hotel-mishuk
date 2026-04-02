using Domain.Entities.Inventory;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.ItemInfo;
using Interface.Base;


namespace Interface.Repository.Inventory
{
    public interface IItemInfoRepository : IRepository<ItemInfo>
    {
        Task<DataTablePagination<ItemInfoSearchVm, ItemInfoSearchVm>>
            SearchAsync(DataTablePagination<ItemInfoSearchVm, ItemInfoSearchVm> model);
        Task<ItemInfoVm> Details(long id);
    }
}
