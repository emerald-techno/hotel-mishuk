using Domain.Entities.Leave;
using Domain.Utility.Common;
using Domain.ViewModel.Leave.LeaveType;
using Interface.Base;

namespace Interface.Services.Leave
{
    public interface ILeaveTypeService : IService<LeaveType>
    {
        Task<DataTablePagination<LeaveTypeSearchVm, LeaveTypeSearchVm>>
                                          SearchAsync(DataTablePagination<LeaveTypeSearchVm, LeaveTypeSearchVm> model);
    }
}
