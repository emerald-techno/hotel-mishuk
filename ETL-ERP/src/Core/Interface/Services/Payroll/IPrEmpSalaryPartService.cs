using Domain.Entities.Payroll;
using Domain.Utility.Common;
using Domain.ViewModel.Payroll.PrEmpSalaryPart;
using Interface.Base;
using Microsoft.AspNetCore.Http;

namespace Interface.Services.Payroll;

public interface IPrEmpSalaryPartService : IService<PrEmpSalaryPart>
{
    Task<DataTablePagination<PrEmpSalaryPartSearchVm, PrEmpSalaryPartSearchVm>>
                                                                 SearchAsync(DataTablePagination<PrEmpSalaryPartSearchVm, PrEmpSalaryPartSearchVm> model);
    Task<bool> ImportAsync(IFormFile importFile);
}
