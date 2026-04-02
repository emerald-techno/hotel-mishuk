using Domain.Entities.Leave;
using Domain.Utility.Common;
using Domain.ViewModel.Leave.EmpLeaveApplication;
using Interface.Base;

namespace Interface.Repository.Leave
{
    public interface IEmpLeaveApplicationRepository : IRepository<EmpLeaveApplication>
    {
        Task<DataTablePagination<EmpLeaveApplicationSearchVm, EmpLeaveApplicationSearchVm>>
            SearchAsync(DataTablePagination<EmpLeaveApplicationSearchVm, EmpLeaveApplicationSearchVm> model);
        Task<List<EmpLeaveReportVm>> GetLeaveReportData(EmpLeaveReportVm vm);
        Task<EmpLeaveApplicationVm> GetEmployeeAppByIdAsync(long id);
    }
}
