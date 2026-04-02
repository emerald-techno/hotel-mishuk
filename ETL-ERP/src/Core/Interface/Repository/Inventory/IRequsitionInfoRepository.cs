using Domain.Entities.Inventory;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.RequsitionInfo;
using Interface.Base;

namespace Interface.Repository.Inventory
{
    public interface IRequsitionInfoRepository : IRepository<RequsitionInfo>
    {
        Task<DataTablePagination<RequsitionInfoSearchVm, RequsitionInfoSearchVm>>
            SearchAsync(DataTablePagination<RequsitionInfoSearchVm, RequsitionInfoSearchVm> model);
        Task<RequsitionInfoVm> GetRequsitionInfoByIdAsync(long id);
    }
}
