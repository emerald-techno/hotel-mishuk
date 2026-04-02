using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.Restaurant.Waiter;
using Interface.Base;

namespace Interface.Services.Restaurant;

public interface IWaiterService : IService<RsWaiter>
{
    Task<bool> WaiterAddAsync(WaiterVm vm);

    Task<DataTablePagination<WaiterSearchVm, WaiterSearchVm>>
        SearchAsync(DataTablePagination<WaiterSearchVm, WaiterSearchVm> model);
}
