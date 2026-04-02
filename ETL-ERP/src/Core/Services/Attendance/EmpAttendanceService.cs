using AutoMapper;
using Domain.Entities.Attendance;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.Attendance.EmpAttendence;
using Domain.ViewModel.Attendance.MonthlyAttSheet;
using Interface.Repository.Admin;
using Interface.Repository.Attendance;
using Interface.Repository.Hr;
using Interface.Services.Attendance;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Services.Base;
using System.Data;
using System.Globalization;

namespace Services.Attendance
{
    public class EmpAttendanceService : BaseService<EmpAttendance>, IEmpAttendanceService
    {
        #region Config
        private IEmpAttendanceRepository Repository;
        private readonly IMapper _iMapper;
        private readonly IUnitOfWork _iUnitOfWork;

        private readonly IEmployeeRepository _iEmployeeRepository;
        private readonly IHrSettingRepository _iHrSettingRepository;
        private readonly ISetHolidayRepository _iSetHolidayRepository;

        public EmpAttendanceService(IEmpAttendanceRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork, IEmployeeRepository iEmployeeRepository, IHrSettingRepository iHrSettingRepository, ISetHolidayRepository iSetHolidayRepository)
            : base(iRepository, iUnitOfWork)
        {
            Repository = iRepository;
            _iMapper = iMapper;
            _iUnitOfWork = iUnitOfWork;
            _iEmployeeRepository = iEmployeeRepository;
            _iHrSettingRepository = iHrSettingRepository;
            _iSetHolidayRepository = iSetHolidayRepository;
        }
        #endregion

        #region Search

        public async Task<DataTablePagination<EmpAttendanceSearchVm, EmpAttendanceSearchVm>>
            SearchAsync(DataTablePagination<EmpAttendanceSearchVm, EmpAttendanceSearchVm> model)
        {
            var dataList = await Repository.SearchAsync(model);
            return dataList;
        }

        #endregion

        #region AddEmployeeInOutTime

        public async Task<bool> AddEmployeeInOutTime(EmpAttendanceVm modelVm)
        {
            var model = _iMapper.Map<EmpAttendance>(modelVm);

            model.AttendDate = (DateTime)(!string.IsNullOrEmpty(modelVm.AttendDateStr) ? Utility.ConvertStrToDate(modelVm.AttendDateStr) : modelVm.AttendDate);
            var empAtdTime = AppUtility.AddTimeSpanToDate(model.AttendDate, modelVm.AttendTimeStr);

            model.ActionById = CurrentUserId;
            model.ManualEntryById = CurrentUserId;
            model.ActionDate = Utility.GetBdDateTimeNow();

            var empAttdResult = Repository.GetFirstOrDefault(c => c.EmployeeId == model.EmployeeId && c.AttendDate == model.AttendDate);
            if (empAttdResult != null)
            {
                if (modelVm.IsLunch)
                {
                    if (empAttdResult.LunchIn == null) empAttdResult.LunchIn = empAtdTime;
                    else empAttdResult.LunchOut = empAtdTime;
                    Repository.Update(empAttdResult);
                    var res = await _iUnitOfWork.CompleteAsync();
                    return res;
                }
                else
                {
                    empAttdResult.OutTime = empAtdTime;
                    Repository.Update(empAttdResult);
                    var res = await _iUnitOfWork.CompleteAsync();
                    return res;
                }
            }
            else
            {
                if (modelVm.IsLunch)
                {
                    throw new Exception("Employee not attend today");
                }
                else
                {
                    model.InTime = empAtdTime;
                    Repository.Add(model);
                    var res = await _iUnitOfWork.CompleteAsync();
                    return res;
                }
            }
        }

        #endregion

        #region GetReportData

        public async Task<List<EmpAttendanceReportVm>> GetReportData(EmpAttendanceReportVm vm)
        {
            var searchVm = new DataTablePagination<EmpAttendanceSearchVm, EmpAttendanceSearchVm>();
            var searchModel = new EmpAttendanceSearchVm();
            searchModel.EmployeeId = vm.EmployeeId;
            searchModel.FormDateStr = vm.FormDateStr;
            searchModel.ToDateStr = vm.ToDateStr;

            if (vm.Year > 1999 && vm.Month > 0)
            {
                DateTime firstDate = new DateTime(vm.Year, vm.Month, 1);
                DateTime lastDate = firstDate.AddMonths(1).AddSeconds(-1);

                searchModel.FormDate = firstDate;
                searchModel.ToDate = lastDate;
            }

            searchVm.SearchModel = searchModel;

            var dataList = new List<EmpAttendanceReportVm>();

            var dataTable = await Repository.SearchAsync(searchVm);

            if (dataTable.data.Count > 0)
            {
                foreach (var attandance in dataTable.data)
                {
                    var filterData = dataTable.data.FirstOrDefault(c => c.EmployeeId == attandance.EmployeeId);

                    var exist = dataList.Any(c => c.EmployeeId == filterData.EmployeeId);

                    if (!exist)
                    {
                        var reportModel = new EmpAttendanceReportVm();

                        reportModel.EmployeeId = filterData.EmployeeId;
                        reportModel.EmployeeName = filterData.EmployeeName;
                        reportModel.EmployeeCode = filterData.EmployeeCode;
                        reportModel.EmpDepartment = filterData.EmpDepartment;
                        reportModel.EmpDesignation = filterData.EmpDesignation;
                        reportModel.EmpPresent = dataTable.data.Where(c => c.EmployeeId == reportModel.EmployeeId && c.Status == EmpAttendanceStatus.Present).Count();
                        reportModel.EmpAbsent = dataTable.data.Where(c => c.EmployeeId == reportModel.EmployeeId && c.Status == EmpAttendanceStatus.Absent).Count();
                        reportModel.EmpLeave = dataTable.data.Where(c => c.EmployeeId == reportModel.EmployeeId && c.Status == EmpAttendanceStatus.Leave).Count();
                        reportModel.EmpOffDay = dataTable.data.Where(c => c.EmployeeId == reportModel.EmployeeId && c.Status == EmpAttendanceStatus.Offday).Count();
                        reportModel.EmpHoliday = dataTable.data.Where(c => c.EmployeeId == reportModel.EmployeeId && c.Status == EmpAttendanceStatus.Holiday).Count();
                        reportModel.EmpLateDay = dataTable.data.Where(c => c.EmployeeId == reportModel.EmployeeId && c.Status == EmpAttendanceStatus.Present && c.StatusDtl == "L").Count();

                        dataList.Add(reportModel);
                    }

                }

            }

            return dataList;
        }

        #endregion

        #region GetMonthlyAttendanceData

        public async Task<MonthlyAttSheetMstVm> GetMonthlyAttendanceData(MonthlyAttSheetMstVm vm)
        {
            var data = await Repository.GetMonthlyAttendanceData(vm);
            return data;
        }

        public async Task<MonthlyAttSheetMstVm> GetMonthlyManualAttendanceData(MonthlyAttSheetMstVm vm)
        {
            var data = await Repository.GetMonthlyManualAttendanceData(vm);
            return data;
        }

        public async Task<DataTablePagination<EmpDailyAttendanceReportVm, EmpDailyAttendanceReportVm>>
            DailyReportAsync(DataTablePagination<EmpDailyAttendanceReportVm, EmpDailyAttendanceReportVm> model)
        {
            var dataList = await Repository.DailyReportAsync(model);
            return dataList;
        }

        #endregion

        #region GetAttandanceDate

        private DateTime? GetAttandanceDate(string date)
        {
            if (string.IsNullOrEmpty(date)) return null;
            CultureInfo culture = new CultureInfo("en-US");
            DateTime convertedDate = Convert.ToDateTime(date, culture);

            //IFormatProvider culture = new CultureInfo("bn-BD", true);
            //var convertedDate = DateTime.Parse(date, culture, DateTimeStyles.AssumeLocal);
            return convertedDate;
        }

        #endregion

        #region DayWiseReport

        public async Task<DataTablePagination<EmpDailyAttendanceReportVm, EmpDailyAttendanceReportVm>>
            DayWiseReportAsync(DataTablePagination<EmpDailyAttendanceReportVm, EmpDailyAttendanceReportVm> model)
        {
            var dataList = await Repository.DayWiseReportAsync(model);
            return dataList;
        }

        #endregion

        #region ReportHtml

        public async Task<string> DayWiseReportHtml(DataTablePagination<EmpDailyAttendanceReportVm, EmpDailyAttendanceReportVm> model)
        {
            var fullHtml = "";
            var dataTable = await Repository.DayWiseReportAsync(model);
            var data = dataTable.data;

            var employee = data.FirstOrDefault();

            fullHtml += "<div style='padding-bottom: 10px;'>";

            fullHtml += $@"<table class='master-table'>
                                <tbody>
                                    <tr>
                                        <td style='width:12%; font-size:12px;'><b> Report Date : </b></td>
                                        <td style='width:88%'>{DateTime.Now.ToString("dd-MM-yyyy")}</td>
                                    </tr>
                                </tbody>
                            </table>";

            fullHtml += "</div>";

            fullHtml += "<table class='table table-bordered' style='width:100%;text-align:center' style='repeat-header:yes;'>";

            fullHtml += "<thead>";
            fullHtml += "<tr>";
            fullHtml += "<th style='width:5%'>Sl.</th>";
            fullHtml += "<th style='width:20%'>Attendance Date</th>";
            fullHtml += "<th style='width:15%'>Designation</th>";
            fullHtml += "<th style='width:15%'>Department</th>";
            fullHtml += "<th style='width:15%'>In Time</th>";
            fullHtml += "<th style='width:15%'>Out Time</th>";
            fullHtml += "<th style='width:15%'>Status</th>";
            fullHtml += "</tr>";

            fullHtml += "</thead>";

            if (data.Count > 0)
            {
                foreach (var (v, i) in data.GetItemWithIndex())
                {
                    fullHtml += "<tr>";

                    fullHtml += $@"<td class='text-center'>{i + 1}</td>";
                    fullHtml += $@"<td class='text-start'>{v.EmployeeName}</td>";
                    fullHtml += $@"<td class='text-center'>{v.EmpDesignation}</td>";
                    fullHtml += $@"<td class='text-center'>{v.EmpDepartment}</td>";
                    fullHtml += $@"<td class='text-center'>{v.InTimeStr}</td>";
                    fullHtml += $@"<td class='text-center'>{v.OutTimeStr}</td>";
                    fullHtml += $@"<td class='text-center'>{v.StatusText}</td>";

                    fullHtml += "</tr>";
                }
            }

            fullHtml += "</table>";

            return fullHtml;
        }

        public async Task<string> DailyReportHtml(DataTablePagination<EmpDailyAttendanceReportVm, EmpDailyAttendanceReportVm> model)
        {
            var fullHtml = "";
            var dataTable = await DailyReportAsync(model);
            var data = dataTable.data;

            var employee = data.FirstOrDefault();

            fullHtml += "<div style='padding-bottom: 10px;'>";

            fullHtml += $@"<table class='master-table'>
                                <tbody>
                                    <tr>
                                        <td style='width:20%'><b> Employee Name </b></td>
                                        <td style='width:30%'>{employee.EmployeeName}</td>
                                        <td style='width:20%'><b> Designation </b></td>
                                        <td style='width:30%'>{employee.EmpDesignation}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:20%'><b> Employee Code </b></td>
                                        <td style='width:30%'>{employee.EmployeeCode}</td>
                                        <td style='width:20%'><b> Department </b></td>
                                        <td style='width:30%'>{employee.EmpDepartment}</td>
                                    </tr>
                                </tbody>
                            </table>";

            fullHtml += "</div>";

            fullHtml += "<table class='table table-bordered' style='width:100%;text-align:center'>";

            fullHtml += "<thead>";
            fullHtml += "<tr>";
            fullHtml += "<th style='width:5%'>Sl.</th>";
            fullHtml += "<th style='width:35%'>Attendance Date</th>";
            fullHtml += "<th style='width:10%'>In Time</th>";
            fullHtml += "<th style='width:10%'>Out Time</th>";
            fullHtml += "<th style='width:10%'>Late-In</th>";
            fullHtml += "<th style='width:10%'>Lunch</th>";
            fullHtml += "<th style='width:20%'>Status</th>";
            fullHtml += "</tr>";

            fullHtml += "</thead>";

            if (data.Count > 0)
            {
                foreach (var (v, i) in data.GetItemWithIndex())
                {
                    fullHtml += "<tr>";

                    fullHtml += $@"<td class='text-center'>{i + 1}</td>";
                    fullHtml += $@"<td class='text-center'>{v.AttendDate.ToString("dd MMMM,yyyy")}</td>";
                    fullHtml += $@"<td class='text-center'>{v.InTimeStr}</td>";
                    fullHtml += $@"<td class='text-center'>{v.OutTimeStr}</td>";
                    fullHtml += $@"<td class='text-center'>{(v.Late ? "Late" : "")}</td>";
                    fullHtml += $@"<td class='text-center'>{(v.LunchIn != null ? $"{v.LunchIn?.ToString("t")} to {v.LunchOut?.ToString("t")}" : "")}</td>";
                    fullHtml += $@"<td class='text-center'>{v.StatusText}</td>";

                    fullHtml += "</tr>";
                }
            }

            fullHtml += "</table>";

            return fullHtml;
        }

        public async Task<string> MonthlyAttReportHtml(EmpAttendanceReportVm vm)
        {
            var fullHtml = "";
            var data = await GetReportData(vm);

            fullHtml += "<table class='table table-bordered' style='width:100%;text-align:center'>";

            fullHtml += "<thead>";
            fullHtml += "<tr>";
            fullHtml += "<th style='width:4%'>Sl.</th>";
            fullHtml += "<th style='width:18%'>Employee </th>";
            fullHtml += "<th style='width:15%'>Department</th>";
            fullHtml += "<th style='width:15%'>Designation</th>";
            fullHtml += "<th style='width:8%'>Present</th>";
            fullHtml += "<th style='width:8%'>Absent</th>";
            fullHtml += "<th style='width:8%'>Leave</th>";
            fullHtml += "<th style='width:8%'>Holiday</th>";
            fullHtml += "<th style='width:8%'>Offday</th>";
            fullHtml += "<th style='width:8%'>Late</th>";
            fullHtml += "</tr>";

            fullHtml += "</thead>";

            if (data.Count > 0)
            {
                foreach (var (v, i) in data.GetItemWithIndex())
                {
                    fullHtml += "<tr>";

                    fullHtml += $@"<td>{i + 1}</td>";
                    fullHtml += $@"<td class='text-start'><b>{v.EmployeeName}</b><br/>{v.EmployeeCode}</td>";
                    fullHtml += $@"<td class='text-center'>{v.EmpDepartment}</td>";
                    fullHtml += $@"<td class='text-center'>{v.EmpDesignation}</td>";
                    fullHtml += $@"<td class='text-center'>{v.EmpPresent}</td>";
                    fullHtml += $@"<td class='text-center'>{v.EmpAbsent}</td>";
                    fullHtml += $@"<td class='text-center'>{v.EmpLeave}</ td >";
                    fullHtml += $@"<td class='text-center'>{v.EmpHoliday}</ td >";
                    fullHtml += $@"<td class='text-center'>{v.EmpOffDay}</ td >";
                    fullHtml += $@"<td class='text-center'>{v.EmpLateDay}</ td >";

                    fullHtml += "</tr>";
                }
            }

            fullHtml += "</table>";

            return fullHtml;
        }

        #endregion

        #region Import

        public async Task<bool> ImportAsync(IFormFile importFile)
        {
            if (importFile == null || importFile.Length <= 0) return false;
            var empAttModels = GetEmpAttendaneDataListFromExcel(importFile);

            if (empAttModels.Count() == 0) throw new Exception("No Data Found In Excel..!");

            var hrSettings = _iHrSettingRepository.GetFirstOrDefault();

            var empAttendanceList = new List<EmpAttendance>();

            foreach (var emp in empAttModels)
            {
                var attModel = new EmpAttendance();

                attModel.ActionById = CurrentUserId;
                attModel.ActionDate = Utility.GetBdDateTimeNow();

                if (string.IsNullOrEmpty(emp.Name)) throw new Exception($"Name Not Found");
                if (string.IsNullOrEmpty(emp.Code)) throw new Exception($"{emp.Name} Code Not Found");

                var existEmployee = await _iEmployeeRepository.GetFirstOrDefaultAsync(c => c.Code.Equals(emp.Code) && !c.IsDeleted);
                if (existEmployee == null) throw new Exception($"{emp.Code} Employee Not Found");
                if (!existEmployee.IsEnable) throw new Exception($"{emp.Code} Employee Not Active");

                var attendDate = !string.IsNullOrEmpty(emp.Date) ? GetAttandanceDate(emp.Date) : null;
                if (attendDate == null) throw new Exception($"{emp.Code} Attend Date Not Found");

                attModel.EmployeeId = existEmployee.Id;
                attModel.AttendDate = (DateTime)attendDate;
                attModel.IsManual = false;

                if (!string.IsNullOrEmpty(emp.EntryTime))
                {
                    DateTime entryTime = DateTime.ParseExact(emp.EntryTime, "hh:mm tt", CultureInfo.InvariantCulture);

                    DateTime entryDateTime = attModel.AttendDate.Date
                        .AddHours(entryTime.Hour)
                        .AddMinutes(entryTime.Minute)
                        .AddSeconds(entryTime.Second);

                    attModel.InTime = entryDateTime;
                }

                if (!string.IsNullOrEmpty(emp.ExitTime))
                {
                    DateTime exitTime = DateTime.ParseExact(emp.ExitTime,
                                    "hh:mm tt", CultureInfo.InvariantCulture);

                    DateTime exitDateTime = attModel.AttendDate.Date
                        .AddHours(exitTime.Hour)
                        .AddMinutes(exitTime.Minute)
                        .AddSeconds(exitTime.Second);

                    attModel.OutTime = exitDateTime;
                }

                var isHoliday = await _iSetHolidayRepository.IsHoliday(attModel.AttendDate);

                if (attModel.InTime != null)
                {
                    attModel.Status = EmpAttendanceStatus.Present;
                }
                else if (attModel.InTime == null && attModel.AttendDate.DayOfWeek.ToString() == hrSettings.HolidayOne)
                {
                    attModel.Status = EmpAttendanceStatus.Offday;
                }
                else if (attModel.InTime == null && isHoliday)
                {
                    attModel.Status = EmpAttendanceStatus.Holiday;
                }
                else
                {
                    attModel.Status = EmpAttendanceStatus.Absent;
                }

                var existAtt = Repository.GetFirstOrDefault(c => c.EmployeeId == attModel.EmployeeId && c.AttendDate.Date == attModel.AttendDate.Date);

                if (existAtt != null) continue;

                empAttendanceList.Add(attModel);
            }

            //var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

            await Repository.AddRangeAsync(empAttendanceList);
            var isAdded = await _iUnitOfWork.CompleteAsync();
            if (!isAdded) return false;

            //ts.Complete();
            return true;

        }

        private IEnumerable<EmpAttendanceExcelModel> GetEmpAttendaneDataListFromExcel(IFormFile importFile)
        {
            if (importFile == null || importFile.Length <= 0) return null;
            var employeeAttModels = new List<EmpAttendanceExcelModel>();

            var dt = Utility.ConvertExcelOrCsVToDataTable(importFile);

            foreach (DataRow dataRow in dt.Rows)
            {
                var model = new EmpAttendanceExcelModel
                {
                    Date = dataRow["Date"].ToString().Trim(),
                    Name = dataRow["Name"].ToString().Trim(),
                    Code = dataRow["ID"].ToString().Trim(),
                    Designation = dataRow["Designation"].ToString().Trim(),
                    Department = dataRow["Department"].ToString().Trim(),
                    EntryTime = dataRow["Entry Time"].ToString().Trim(),
                    ExitTime = dataRow["Exit Time"].ToString().Trim(),
                    OfficeDuration = dataRow["Office Duration"].ToString().Trim()
                };

                if (string.IsNullOrEmpty(model.Date)) continue;
                if (string.IsNullOrEmpty(model.Name)) continue;
                if (string.IsNullOrEmpty(model.Code)) continue;

                employeeAttModels.Add(model);
            }

            return employeeAttModels;
        }

        #endregion

        #region TestAttendanceEntry

        public async Task<bool> TestAttEntry()
        {
            var year = 2024;
            var month = 11; // 5 = may, 6 = june, 7 = july
            var monthFirstDay = new DateTime(year, month, 1);
            var monthLastDay = monthFirstDay.AddMonths(1).AddDays(-1);
            TimeSpan span = monthLastDay.Subtract(monthFirstDay);
            var totalDays = (int)span.TotalDays + 1;

            var employees = _iEmployeeRepository.Get(c => !c.IsDeleted);
            var hrSettings = _iHrSettingRepository.GetFirstOrDefault();
            var holidays = _iSetHolidayRepository.Get(c => c.HolidayYear == year && monthFirstDay.Date <= c.StartDate.Date && monthLastDay.Date >= c.EndDate.Date);

            var holidayDateList = new List<DateTime>();

            if (holidays != null && holidays.Count > 0)
            {
                foreach (var holiday in holidays)
                {
                    var start = holiday.StartDate;
                    var end = holiday.EndDate;

                    if (end >= start)
                    {
                        for (var dt = start; dt <= end; dt = dt.AddDays(1))
                        {
                            holidayDateList.Add(dt);
                        }
                    }
                }
            }

            var entryList = new List<EmpAttendance>();

            if (employees != null && employees.Count > 0)
            {
                foreach (var (employee, i) in employees.GetItemWithIndex())
                {
                    if (employee.IsDeleted == false)
                    {
                        for (int j = 0; j < totalDays; j++)
                        {
                            var empAttModel = new EmpAttendance();
                            empAttModel.AttendDate = monthFirstDay.AddDays(j);
                            empAttModel.InTime = empAttModel.AttendDate.AddHours(10);
                            empAttModel.OutTime = empAttModel.AttendDate.AddHours(18);
                            empAttModel.Status = EmpAttendanceStatus.Present;
                            empAttModel.EmployeeId = employee.Id;
                            empAttModel.ActionById = CurrentUserId;
                            empAttModel.ActionDate = DateTime.Now;

                            if (empAttModel.AttendDate.DayOfWeek.ToString() == hrSettings.HolidayOne || empAttModel.AttendDate.DayOfWeek.ToString() == hrSettings.HolidayTwo)
                            {
                                empAttModel.Status = EmpAttendanceStatus.Offday;
                                empAttModel.InTime = null;
                                empAttModel.OutTime = null;
                            }

                            var isHoliday = holidayDateList.Any(c => c.Date == empAttModel.AttendDate.Date);

                            if (isHoliday)
                            {
                                empAttModel.Status = EmpAttendanceStatus.Holiday;
                                empAttModel.InTime = null;
                                empAttModel.OutTime = null;
                            }

                            entryList.Add(empAttModel);
                        }
                    }
                }
            }

            if (entryList.Count > 0)
            {
                try
                {
                    Repository.AddRange(entryList);
                    var result = await _iUnitOfWork.CompleteAsync();
                    return result;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

            return false;
        }

        #endregion
    }
}
