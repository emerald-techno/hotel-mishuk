using Domain.Entities.Inventory;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.UnitInfo;
using Interface.Base;

namespace Interface.Repository.Inventory
{
    public interface IUnitInfoRepository : IRepository<UnitInfo>
    {
        Task<DataTablePagination<UnitInfoSearchVm, UnitInfoSearchVm>>
            SearchAsync(DataTablePagination<UnitInfoSearchVm, UnitInfoSearchVm> model);
    }
}
