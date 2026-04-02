using Domain.Entities.Inventory;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.UnitInfo;
using Interface.Base;

namespace Interface.Services.Inventory;

public interface IUnitInfoService : IService<UnitInfo>
{
    Task<DataTablePagination<UnitInfoSearchVm, UnitInfoSearchVm>>
                SearchAsync(DataTablePagination<UnitInfoSearchVm, UnitInfoSearchVm> model);

    Task<string> GetUnitInfoCode();
}
