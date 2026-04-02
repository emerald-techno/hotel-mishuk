using Domain.Entities.Leave;
using Domain.Utility.Common;
using Domain.ViewModel.Leave.LeaveType;
using Interface.Base;

namespace Interface.Repository.Leave
{
    public interface ILeaveTypeRepository : IRepository<LeaveType>
    {
        Task<DataTablePagination<LeaveTypeSearchVm, LeaveTypeSearchVm>>
                             SearchAsync(DataTablePagination<LeaveTypeSearchVm, LeaveTypeSearchVm> model);
    }
}
