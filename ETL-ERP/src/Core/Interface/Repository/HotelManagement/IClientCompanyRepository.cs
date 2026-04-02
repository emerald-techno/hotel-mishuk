using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.ClientCompany;
using Interface.Base;

namespace Interface.Repository.HotelManagement;

public interface IClientCompanyRepository : IRepository<ClientCompany>
{
    Task<DataTablePagination<ClientCompanySearchVm, ClientCompanySearchVm>>
    SearchAsync(DataTablePagination<ClientCompanySearchVm, ClientCompanySearchVm> vm);
}
