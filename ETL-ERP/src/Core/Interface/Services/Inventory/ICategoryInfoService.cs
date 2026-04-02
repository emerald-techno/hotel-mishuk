using Domain.Entities.Inventory;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.CategoryInfo;
using Interface.Base;

namespace Interface.Services.Inventory
{
    public interface ICategoryInfoService : IService<CategoryInfo>
    {
        Task<DataTablePagination<CategoryInfoSearchVm, CategoryInfoSearchVm>>
            SearchAsync(DataTablePagination<CategoryInfoSearchVm, CategoryInfoSearchVm> model);

        Task<string> GetCategoryInfoCode();
    }
}
