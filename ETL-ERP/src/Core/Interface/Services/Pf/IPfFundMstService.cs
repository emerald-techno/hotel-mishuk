using Domain.Entities.Pf;
using Domain.Utility.Common;
using Domain.ViewModel.Pf.PfFund;
using Domain.ViewModel.Report;
using Interface.Base;

namespace Interface.Services.Pf
{
    public interface IPfFundMstService : IService<PfFundMst>
    {
        Task<bool> PfReportAdd(PfFundMstVm modelVm);
        Task<PfFundMstVm> GetEmpPfFundData(PfFundMstVm vm);
        Task<PfFundMstVm> GetPfByIdAsync(long id);
        Task<DataTablePagination<PfFundMstSearchVm, PfFundMstSearchVm>> SearchAsync(DataTablePagination<PfFundMstSearchVm, PfFundMstSearchVm> model);
        Task<DataTablePagination<EmployeePfSummaryReportVm, EmployeePfSummaryReportVm>> SearchDtlAsync(DataTablePagination<EmployeePfSummaryReportVm, EmployeePfSummaryReportVm> model);
        Task<EmpPfSummaryDtlReportVm> GetEmployeePfInfo(long employeeId);
        Task<string> EmployeePFListReportHtml();
        Task<string> EmployeePFDetailsReportHtml(long empId);

        Task<string> GetPfScheduleReportHtml(PfFundMstVm model);
    }
}
