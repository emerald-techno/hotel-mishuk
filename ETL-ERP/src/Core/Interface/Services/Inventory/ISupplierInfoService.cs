using Domain.Entities.Inventory;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.SupplierInfo;
using Interface.Base;

namespace Interface.Services.Inventory
{
    public interface ISupplierInfoService : IService<SupplierInfo>
    {
        Task<DataTablePagination<SupplierInfoSearchVm, SupplierInfoSearchVm>>
            SearchAsync(DataTablePagination<SupplierInfoSearchVm, SupplierInfoSearchVm> model);
        Task<string> GetSupplierInfoCode();
    }
}
