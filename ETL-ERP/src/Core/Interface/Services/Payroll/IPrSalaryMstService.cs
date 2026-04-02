using Domain.Entities.Payroll;
using Domain.Utility.Common;
using Domain.ViewModel.Payroll.PrSalaryMst;
using Interface.Base;

namespace Interface.Services.Payroll;

public interface IPrSalaryMstService : IService<PrSalaryMst>
{
    Task<DataTablePagination<PrSalaryMstSearchVm, PrSalaryMstSearchVm>>
                                                                    SearchAsync(DataTablePagination<PrSalaryMstSearchVm, PrSalaryMstSearchVm> model);
    Task<PrSalaryMstVm> CalculatePayroll(PrSalaryMstVm vm);
    Task<PrSalaryMstVm> GetPrSalaryByIdAsync(long id);
    Task<bool> ApprovePrSalary(long id, string remarks);
    Task<string> PayrollHtml(long id);
    Task<string> PayrollBankHtml(long id);
    Task<string> DepartmentWisePayrollHtml(int year, int month, long? departmentId, bool isPrint = false);
}
