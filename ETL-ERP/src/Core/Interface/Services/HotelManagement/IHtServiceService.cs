using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.Service;
using Interface.Base;

namespace Interface.Services.HotelManagement;

public interface IHtServiceService : IService<HtService>
{
    Task<DataTablePagination<ServiceSearchVm, ServiceSearchVm>>
        SearchAsync(DataTablePagination<ServiceSearchVm, ServiceSearchVm> model);
    Task<string> GetServiceCode();
}
