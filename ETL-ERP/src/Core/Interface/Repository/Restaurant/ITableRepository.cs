using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.Restaurant.Table;
using Interface.Base;

namespace Interface.Repository.Restaurant;

public interface ITableRepository : IRepository<RsTable>
{
    Task<DataTablePagination<RsTableSearchVm, RsTableSearchVm>>
       SearchAsync(DataTablePagination<RsTableSearchVm, RsTableSearchVm> vm);
}
