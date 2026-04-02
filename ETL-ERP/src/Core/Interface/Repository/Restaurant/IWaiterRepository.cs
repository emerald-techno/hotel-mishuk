using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.Restaurant.Waiter;
using Interface.Base;

namespace Interface.Repository.Restaurant;

public interface IWaiterRepository : IRepository<RsWaiter>
{
    Task<DataTablePagination<WaiterSearchVm, WaiterSearchVm>>
       SearchAsync(DataTablePagination<WaiterSearchVm, WaiterSearchVm> vm);
}
