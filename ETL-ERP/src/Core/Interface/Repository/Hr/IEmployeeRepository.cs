using Domain.Entities.HR;
using Domain.Utility.Common;
using Domain.ViewModel.Hr.Employees;
using Interface.Base;

namespace Interface.Repository.Hr
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<DataTablePagination<EmployeeSearchVm, EmployeeSearchVm>>
            SearchAsync(DataTablePagination<EmployeeSearchVm, EmployeeSearchVm> model);
        Task<EmployeeVm> GetEmployeeByIdAsync(long id);
        Task<DataTablePagination<EmployeeSearchVm, EmployeeSearchVm>> SearchAllAsync(DataTablePagination<EmployeeSearchVm, EmployeeSearchVm> vm);
        Task<long> GetEmpIdByUserId(long userId);
    }
}
