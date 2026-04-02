using Domain.Entities.Pf;
using Domain.Utility.Common;
using Domain.ViewModel.Pf.EmpLoan;
using Interface.Base;

namespace Interface.Services.Pf;

public interface IEmpLoanMstService : IService<EmpLoanMst>
{
    Task<bool> AddAsync(EmpLoanMstVm vm);
    Task<DataTablePagination<EmpLoanMstSearchVm, EmpLoanMstSearchVm>> SearchAsync(DataTablePagination<EmpLoanMstSearchVm, EmpLoanMstSearchVm> model);
    Task<string> EmpLoanDetailsReportHtml(long id);
    Task<bool> AdvanceLoanCalculation(long id, double advAmount);
    Task<string> EmployeeLoanReportHtml(EmployeeLoanReportVm vm);
    Task<bool> LoanPaid(InstalmentPaidVm vm);
}
