using Domain.Entities.Attendance;
using Domain.Utility.Common;
using Domain.ViewModel.Attendance.ShiftManagement;
using Interface.Base;

namespace Interface.Services.Attendance;

public interface IShiftManagementService : IService<ShiftManagement>
{
    Task<bool> AddOrUpdate(ShiftManagementVm vm);

    Task<DataTablePagination<ShiftManagementSearchVm, ShiftManagementSearchVm>>
        SearchAsync(DataTablePagination<ShiftManagementSearchVm, ShiftManagementSearchVm> model);
    Task<List<ShiftManagementVm>> GetShiftByMonthOfYearAsync(short year, short month, long? employeeId);
}
