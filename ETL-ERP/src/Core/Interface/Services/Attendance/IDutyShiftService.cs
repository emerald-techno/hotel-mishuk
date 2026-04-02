using Domain.Entities.Attendance;
using Domain.Utility.Common;
using Domain.ViewModel.Attendance.DutyShift;
using Interface.Base;

namespace Interface.Services.Attendance;

public interface IDutyShiftService : IService<DutyShift>
{
    Task<DataTablePagination<DutyShiftSearchVm, DutyShiftSearchVm>>
        SearchAsync(DataTablePagination<DutyShiftSearchVm, DutyShiftSearchVm> model);
}