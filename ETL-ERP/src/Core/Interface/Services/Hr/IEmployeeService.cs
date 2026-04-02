using Domain.Entities.HR;
using Domain.Utility.Common;
using Domain.ViewModel.Hr.Employees;
using Interface.Base;
using Microsoft.AspNetCore.Http;

namespace Interface.Services.Hr
{
    public interface IEmployeeService : IService<Employee>
    {
        Task<bool> EmployeeAddAsync(EmployeeVm vm);
        Task<string> GetEmployeeCode();
        Task<EmployeeVm> GetEmployeeDataAsync(long id);

        Task<DataTablePagination<EmployeeSearchVm, EmployeeSearchVm>>
                                    SearchAsync(DataTablePagination<EmployeeSearchVm, EmployeeSearchVm> model);
        Task<bool> ImportAsync(IFormFile importFile);
        Task<string> EmployeeListPrintHtml(DataTablePagination<EmployeeSearchVm, EmployeeSearchVm> model);
        Task<string> EmployeeCVPrintHtml(long id, short refType);
        Task<byte[]> ExcelFileDownload(DataTablePagination<EmployeeSearchVm, EmployeeSearchVm> model);
    }
}
