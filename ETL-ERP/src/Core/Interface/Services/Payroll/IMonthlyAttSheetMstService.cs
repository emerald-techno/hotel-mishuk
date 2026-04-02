using Domain.Entities.Payroll;
using Domain.Utility.Common;
using Domain.ViewModel.Attendance.MonthlyAttSheet;
using Interface.Base;

namespace Interface.Services.Payroll;

public interface IMonthlyAttSheetMstService : IService<MonthlyAttendanceSheetMst>
{
    Task<bool> SheetAdd(MonthlyAttSheetMstVm modelVm);
    Task<MonthlyAttSheetMstVm> GetSheetByIdAsync(long id);
    Task<DataTablePagination<MonthlyAttSheetSearchVm, MonthlyAttSheetSearchVm>>
                                                              SearchAsync(DataTablePagination<MonthlyAttSheetSearchVm, MonthlyAttSheetSearchVm> model);
    Task<string> MonthlyAttendanceReportHtml(long id);
}
