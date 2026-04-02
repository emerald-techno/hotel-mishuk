using Domain.Entities.Pf;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.HotelReport;
using Domain.ViewModel.Pf.EmpLoan;
using Interface.Base;

namespace Interface.Repository.Pf
{
    public interface IEmpLoanMstRepository : IRepository<EmpLoanMst>
    {
        Task<DataTablePagination<EmpLoanMstSearchVm, EmpLoanMstSearchVm>>
            SearchAsync(DataTablePagination<EmpLoanMstSearchVm, EmpLoanMstSearchVm> model);

        Task<string> EmployeeLoanReportHtml(EmployeeLoanReportVm vm);
    }
}
