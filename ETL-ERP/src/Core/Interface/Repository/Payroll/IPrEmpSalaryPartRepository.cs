using Domain.Entities.Payroll;
using Domain.Utility.Common;
using Domain.ViewModel.Payroll.PrEmpSalaryPart;
using Interface.Base;

namespace Interface.Repository.Payroll
{
    public interface IPrEmpSalaryPartRepository : IRepository<PrEmpSalaryPart>
    {
        Task<DataTablePagination<PrEmpSalaryPartSearchVm, PrEmpSalaryPartSearchVm>>
                                               SearchAsync(DataTablePagination<PrEmpSalaryPartSearchVm, PrEmpSalaryPartSearchVm> model);
    }
}
