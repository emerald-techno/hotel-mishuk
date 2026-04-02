using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.Service;
using Interface.Base;

namespace Interface.Repository.HotelManagement;

public interface IServiceRepository : IRepository<HtService>
{
    Task<DataTablePagination<ServiceSearchVm, ServiceSearchVm>>
       SearchAsync(DataTablePagination<ServiceSearchVm, ServiceSearchVm> vm);
}
