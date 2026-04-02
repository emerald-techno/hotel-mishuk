using Domain.Entities.Attendance;
using Domain.Utility.Common;
using Domain.ViewModel.Attendance.EmpAttendence;
using Domain.ViewModel.Attendance.MonthlyAttSheet;
using Interface.Base;

namespace Interface.Repository.Attendance;

public interface IEmpAttendanceRepository : IRepository<EmpAttendance>
{
    Task<DataTablePagination<EmpAttendanceSearchVm, EmpAttendanceSearchVm>>
        SearchAsync(DataTablePagination<EmpAttendanceSearchVm, EmpAttendanceSearchVm> model);
    Task<MonthlyAttSheetMstVm> GetMonthlyAttendanceData(MonthlyAttSheetMstVm vm);

    Task<DataTablePagination<EmpDailyAttendanceReportVm, EmpDailyAttendanceReportVm>>
        DailyReportAsync(DataTablePagination<EmpDailyAttendanceReportVm, EmpDailyAttendanceReportVm> vm);

    Task<DataTablePagination<EmpDailyAttendanceReportVm, EmpDailyAttendanceReportVm>>
        DayWiseReportAsync(DataTablePagination<EmpDailyAttendanceReportVm, EmpDailyAttendanceReportVm> vm);
    Task<MonthlyAttSheetMstVm> GetMonthlyManualAttendanceData(MonthlyAttSheetMstVm vm);
}