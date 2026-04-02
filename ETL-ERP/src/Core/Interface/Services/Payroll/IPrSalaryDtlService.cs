using Domain.Entities.Payroll;
using Domain.ViewModel.Payroll.PrSalaryDtl;
using Interface.Base;

namespace Interface.Services.Payroll;

public interface IPrSalaryDtlService : IService<PrSalaryDtl>
{
    Task<PayslipVm> GeneratePayslip(long id);
    Task<bool> PayMultiSalary(List<PrSalaryDtl> salaryDtls);
    Task<string> PayslipHtml(long id);
}
