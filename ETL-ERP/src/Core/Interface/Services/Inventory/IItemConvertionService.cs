using Domain.Entities.Inventory;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.ItemConversion;
using Domain.ViewModel.Restaurant.FoodItem;
using Interface.Base;

namespace Interface.Services.Inventory
{
    public interface IItemConvertionService :IService<ItemConvertion>
    {
        Task<bool> AddAsync(ItemConvertionVm vm);
        Task<DataTablePagination<ItemConvertionSearchVm, ItemConvertionSearchVm>>
        SearchAsync(DataTablePagination<ItemConvertionSearchVm, ItemConvertionSearchVm> model);
    }
}
