using Domain.Entities.Payroll;
using Domain.Utility.Common;
using Domain.ViewModel.Payroll.PrSalaryPart;
using Interface.Base;

namespace Interface.Services.Payroll;

public interface IPrSalaryPartService : IService<PrSalaryPart>
{
    Task<DataTablePagination<PrSalaryPartSearchVm, PrSalaryPartSearchVm>>
                                                              SearchAsync(DataTablePagination<PrSalaryPartSearchVm, PrSalaryPartSearchVm> model);
}
