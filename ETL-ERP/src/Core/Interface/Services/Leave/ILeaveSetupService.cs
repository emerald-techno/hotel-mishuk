using Domain.Entities.Leave;
using Domain.Utility.Common;
using Domain.ViewModel.Leave.LeaveSetup;
using Interface.Base;

namespace Interface.Services.Leave
{
    public interface ILeaveSetupService : IService<LeaveSetup>
    {
        Task<DataTablePagination<LeaveSetupSearchVm, LeaveSetupSearchVm>>
                              SearchAsync(DataTablePagination<LeaveSetupSearchVm, LeaveSetupSearchVm> model);
    }
}
