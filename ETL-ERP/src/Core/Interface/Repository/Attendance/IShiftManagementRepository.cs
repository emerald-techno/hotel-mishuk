using Domain.Entities.Attendance;
using Domain.Utility.Common;
using Domain.ViewModel.Attendance.ShiftManagement;
using Interface.Base;

namespace Interface.Repository.Attendance;

public interface IShiftManagementRepository : IRepository<ShiftManagement>
{
    Task<DataTablePagination<ShiftManagementSearchVm, ShiftManagementSearchVm>>
        SearchAsync(DataTablePagination<ShiftManagementSearchVm, ShiftManagementSearchVm> vm);
}