using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.Restaurant.Table;
using Interface.Base;

namespace Interface.Services.Restaurant;

public interface ITableService : IService<RsTable>
{
    Task<DataTablePagination<RsTableSearchVm, RsTableSearchVm>>
        SearchAsync(DataTablePagination<RsTableSearchVm, RsTableSearchVm> model);
}