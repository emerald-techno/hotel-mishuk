using Domain.Entities.Payroll;
using Domain.Utility.Common;
using Domain.ViewModel.Attendance.MonthlyAttSheet;
using Interface.Base;

namespace Interface.Repository.Payroll;

public interface IMonthlyAttSheetMstRepository : IRepository<MonthlyAttendanceSheetMst>
{
    Task<MonthlyAttendanceSheetMst> GetSheetByIdAsync(long id);
    Task<DataTablePagination<MonthlyAttSheetSearchVm, MonthlyAttSheetSearchVm>>
                                           SearchAsync(DataTablePagination<MonthlyAttSheetSearchVm, MonthlyAttSheetSearchVm> model);
}
