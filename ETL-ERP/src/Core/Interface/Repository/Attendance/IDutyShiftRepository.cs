using Domain.Entities.Attendance;
using Domain.Utility.Common;
using Domain.ViewModel.Attendance.DutyShift;
using Interface.Base;

namespace Interface.Repository.Attendance;

public interface IDutyShiftRepository : IRepository<DutyShift>
{
    Task<DataTablePagination<DutyShiftSearchVm, DutyShiftSearchVm>>
        SearchAsync(DataTablePagination<DutyShiftSearchVm, DutyShiftSearchVm> vm);
}