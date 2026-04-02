using Domain.Entities.Inventory;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.CategoryInfo;
using Interface.Base;


namespace Interface.Repository.Inventory
{
    public interface ICategoryInfoRepository : IRepository<CategoryInfo>
    {
        Task<DataTablePagination<CategoryInfoSearchVm, CategoryInfoSearchVm>>
            SearchAsync(DataTablePagination<CategoryInfoSearchVm, CategoryInfoSearchVm> model);
    }
}
