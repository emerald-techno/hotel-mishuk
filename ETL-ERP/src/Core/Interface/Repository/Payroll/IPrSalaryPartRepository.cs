using Domain.Entities.Payroll;
using Domain.Utility.Common;
using Domain.ViewModel.Payroll.PrSalaryPart;
using Interface.Base;

namespace Interface.Repository.Payroll
{
    public interface IPrSalaryPartRepository : IRepository<PrSalaryPart>
    {
        Task<DataTablePagination<PrSalaryPartSearchVm, PrSalaryPartSearchVm>>
                                               SearchAsync(DataTablePagination<PrSalaryPartSearchVm, PrSalaryPartSearchVm> model);
    }
}
