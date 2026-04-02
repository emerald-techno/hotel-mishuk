using Domain.Entities.Attendance;
using Domain.Utility.Common;
using Domain.ViewModel.Attendance.EmpAttendence;
using Domain.ViewModel.Attendance.MonthlyAttSheet;
using Interface.Base;
using Microsoft.AspNetCore.Http;

namespace Interface.Services.Attendance;

public interface IEmpAttendanceService : IService<EmpAttendance>
{
    Task<DataTablePagination<EmpAttendanceSearchVm, EmpAttendanceSearchVm>>
        SearchAsync(DataTablePagination<EmpAttendanceSearchVm, EmpAttendanceSearchVm> model);
    Task<bool> AddEmployeeInOutTime(EmpAttendanceVm modelVm);
    Task<List<EmpAttendanceReportVm>> GetReportData(EmpAttendanceReportVm vm);
    Task<MonthlyAttSheetMstVm> GetMonthlyAttendanceData(MonthlyAttSheetMstVm vm);
    Task<DataTablePagination<EmpDailyAttendanceReportVm, EmpDailyAttendanceReportVm>>
        DailyReportAsync(DataTablePagination<EmpDailyAttendanceReportVm, EmpDailyAttendanceReportVm> model);
    Task<string> MonthlyAttReportHtml(EmpAttendanceReportVm vm);
    Task<string> DailyReportHtml(DataTablePagination<EmpDailyAttendanceReportVm, EmpDailyAttendanceReportVm> model);
    Task<bool> ImportAsync(IFormFile importFile);
    Task<bool> TestAttEntry();
    Task<string> DayWiseReportHtml(DataTablePagination<EmpDailyAttendanceReportVm, EmpDailyAttendanceReportVm> model);

    Task<DataTablePagination<EmpDailyAttendanceReportVm, EmpDailyAttendanceReportVm>>
        DayWiseReportAsync(DataTablePagination<EmpDailyAttendanceReportVm, EmpDailyAttendanceReportVm> model);
    Task<MonthlyAttSheetMstVm> GetMonthlyManualAttendanceData(MonthlyAttSheetMstVm vm);
}