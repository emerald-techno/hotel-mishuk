using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.Restaurant.Customer;
using Interface.Base;

namespace Interface.Repository.Restaurant;

public interface ICustomerRepository : IRepository<RsCustomer>
{
    Task<DataTablePagination<CustomerSearchVm, CustomerSearchVm>>
       SearchAsync(DataTablePagination<CustomerSearchVm, CustomerSearchVm> vm);
}
