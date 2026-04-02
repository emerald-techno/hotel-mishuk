using Domain.Entities.Leave;
using Domain.Utility.Common;
using Domain.ViewModel.Leave.EmpLeaveApplication;
using Interface.Base;

namespace Interface.Services.Leave
{
    public interface IEmpLeaveApplicationService : IService<EmpLeaveApplication>
    {
        Task<DataTablePagination<EmpLeaveApplicationSearchVm, EmpLeaveApplicationSearchVm>>
                                          SearchAsync(DataTablePagination<EmpLeaveApplicationSearchVm, EmpLeaveApplicationSearchVm> model);
        Task<EmpLeaveApplicationVm> GetLeaveAppPreviewData(EmpLeaveApplicationVm modelVm);
        Task<bool> LeaveApplyAsync(EmpLeaveApplicationVm vm);
        Task<EmpLeaveApplicationVm> GetLeaveAppDataById(long appId);
        Task<bool> CancelLeaveApp(long appId);
        Task<List<EmpLeaveReportVm>> GetLeaveReportData(EmpLeaveReportVm vm);
        Task<string> LeaveReportHtml(EmpLeaveReportVm vm);
        Task<bool> GeneralLeaveApplyAsync(EmpLeaveApplicationVm vm);
    }
}
