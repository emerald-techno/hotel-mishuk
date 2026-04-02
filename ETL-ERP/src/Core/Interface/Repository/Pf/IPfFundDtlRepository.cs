using Domain.Entities;
using Domain.Entities.Pf;
using Domain.Utility.Common;
using Domain.ViewModel.Report;
using Interface.Base;

namespace Interface.Repository.Pf
{
    public interface IPfFundDtlRepository : IRepository<PfFundDtl>
    {
        Task<DataTablePagination<EmployeePfSummaryReportVm, EmployeePfSummaryReportVm>> SearchAsync(DataTablePagination<EmployeePfSummaryReportVm, EmployeePfSummaryReportVm> vm);
        Task<EmpPfSummaryDtlReportVm> GetEmployeePfInfo(long employeeId);
    }
}
