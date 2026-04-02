using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.Restaurant.Customer;
using Interface.Base;

namespace Interface.Services.Restaurant;

public interface ICustomerService : IService<RsCustomer>
{
    Task<bool> CustomerEntry(CustomerVm vm);

    Task<DataTablePagination<CustomerSearchVm, CustomerSearchVm>>
        SearchAsync(DataTablePagination<CustomerSearchVm, CustomerSearchVm> model);
    Task<RsCustomer?> GetCurrentGuestCustomerByRoomId(long roomId);
}
