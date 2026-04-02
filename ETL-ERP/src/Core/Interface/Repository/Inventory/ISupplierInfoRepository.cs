using Domain.Entities.Inventory;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.SupplierInfo;
using Interface.Base;

namespace Interface.Repository.Inventory
{
    public interface ISupplierInfoRepository : IRepository<SupplierInfo>
    {
        Task<DataTablePagination<SupplierInfoSearchVm, SupplierInfoSearchVm>>
            SearchAsync(DataTablePagination<SupplierInfoSearchVm, SupplierInfoSearchVm> model);
    }
}
