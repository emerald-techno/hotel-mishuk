using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.ClientCompany;
using Interface.Base;

namespace Interface.Services.HotelManagement;

public interface IClientCompanyService : IService<ClientCompany>
{
    Task<bool> AddAsync(ClientCompanyVm vm);

    Task<DataTablePagination<ClientCompanySearchVm, ClientCompanySearchVm>>
       SearchAsync(DataTablePagination<ClientCompanySearchVm, ClientCompanySearchVm> model);
}
