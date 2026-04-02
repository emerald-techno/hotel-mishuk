using Domain.Entities.Leave;
using Domain.Utility.Common;
using Domain.ViewModel.Leave.LeaveSetup;
using Interface.Base;

namespace Interface.Repository.Leave
{
    public interface ILeaveSetupRepository : IRepository<LeaveSetup>
    {
        Task<DataTablePagination<LeaveSetupSearchVm, LeaveSetupSearchVm>>
                                  SearchAsync(DataTablePagination<LeaveSetupSearchVm, LeaveSetupSearchVm> model);
    }
}
