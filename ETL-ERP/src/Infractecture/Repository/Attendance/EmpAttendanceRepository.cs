using AutoMapper;
using Domain.Entities.Attendance;
using Domain.Enums.AppEnums;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.Attendance.EmpAttendence;
using Domain.ViewModel.Attendance.MonthlyAttSheet;
using Interface.Repository.Attendance;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Attendance
{
    public class EmpAttendanceRepository : BaseRepository<EmpAttendance>, IEmpAttendanceRepository, IDisposable
    {
        #region Config
        private ApplicationDbContext Context => Db as ApplicationDbContext;
        private readonly IMapper _iMapper;

        public EmpAttendanceRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
        {
            Db = db;
            _iMapper = iMapper;
        }

        #endregion

        #region Search

        public async Task<DataTablePagination<EmpAttendanceSearchVm, EmpAttendanceSearchVm>> SearchAsync(DataTablePagination<EmpAttendanceSearchVm, EmpAttendanceSearchVm> vm)
        {
            var searchResult = Context.EmpAttendances
                .Include(b => b.Employee)
                    .ThenInclude(d => d.Department)
                .Include(b => b.Employee)
                    .ThenInclude(d => d.Designation)
                .AsQueryable().Where(c => !c.IsDeleted);

            var model = vm.SearchModel;

            if (model == null) throw new Exception("Search Attendances Not Found");

            if (model.EmployeeId > 0)
            {
                searchResult = searchResult.Where(c => c.EmployeeId == model.EmployeeId);
            }

            if (model.FormDate != null && model.ToDate != null)
            {
                searchResult = searchResult.Where(c => c.AttendDate >= model.FormDate && c.AttendDate <= model.ToDate);
            }

            if (!string.IsNullOrEmpty(model.FormDateStr))
            {
                var formDate = (DateTime)(!string.IsNullOrEmpty(model.FormDateStr) ? Utility.ConvertStrToDate(model.FormDateStr) : model.AttendDate);
                searchResult = searchResult.Where(c => c.AttendDate >= formDate);
            }

            if (!string.IsNullOrEmpty(model.ToDateStr))
            {
                var toDate = (DateTime)(!string.IsNullOrEmpty(model.ToDateStr) ? Utility.ConvertStrToDate(model.ToDateStr) : model.AttendDate);
                searchResult = searchResult.Where(c => c.AttendDate <= toDate);
            }

            if (!string.IsNullOrEmpty(vm.Search.Value))
            {
                var value = vm.Search.Value.Trim().ToLower();
                searchResult = searchResult.Where(c => c.Employee.Name.ToLower().Contains(value));
            }

            var totalRecords = await searchResult.CountAsync();

            if (totalRecords > 0)
            {
                vm.recordsTotal = totalRecords;
                vm.recordsFiltered = totalRecords;
                vm.draw = vm.LineDraw ?? 0;

                //var data = await searchResult.OrderBy(c => c.Employee.Name)
                //                             .Skip(vm.Start)
                //                             .Take(vm.Length)
                //                             .ToListAsync();

                var data = await searchResult.OrderBy(c => c.Employee.Designation.Code)
                                             .ToListAsync();

                vm.data = _iMapper.Map<List<EmpAttendanceSearchVm>>(data);

                var sl = vm.Start;

                foreach (var searchDto in vm.data)
                {
                    var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                    searchDto.SerialNo = ++sl;
                    searchDto.EmployeeName = filterData?.Employee.Name;
                    searchDto.EmployeeCode = filterData?.Employee.Code;
                    searchDto.EmpDepartment = filterData?.Employee.Department?.Name;
                    searchDto.EmpDesignation = filterData?.Employee.Designation?.Name;
                }
            }
            return vm;
        }

        #endregion

        #region GetMonthlyAttendanceData

        public async Task<MonthlyAttSheetMstVm> GetMonthlyAttendanceData(MonthlyAttSheetMstVm vm)
        {
            var searchResult = Context.EmpAttendances
                .Include(b => b.Employee)
                .ThenInclude(d => d.Designation)
                .Include(c => c.Employee)
                .ThenInclude(dp => dp.Department)
                .AsQueryable().Where(c => !c.IsDeleted);

            if (vm == null) throw new Exception("Attendances not found");

            var startOfMonth = new DateTime(vm.Year, vm.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            if (vm.Year > 0 && vm.Month > 0)
            {
                searchResult = searchResult.Where(c => c.AttendDate.Date >= startOfMonth.Date && c.AttendDate.Date <= endOfMonth.Date);
            }

            var dataList = await searchResult.OrderBy(c => c.Employee.Designation.Code).ToListAsync();
            var dataDateList = dataList.Select(c => c.AttendDate.Date).ToList();

            var monthlyAttendance = new MonthlyAttSheetMstVm();

            monthlyAttendance.Year = vm.Year;
            monthlyAttendance.Month = vm.Month;
            monthlyAttendance.DateForm = startOfMonth;
            monthlyAttendance.DateTo = endOfMonth;

            var dtlDataList = new List<MonthlyAttSheetDtlVm>();

            foreach (var attandance in dataList)
            {
                var filterData = dataList.FirstOrDefault(c => c.EmployeeId == attandance.EmployeeId);
                if (filterData == null) continue;

                var exist = dtlDataList.Any(c => c.EmployeeId == filterData.EmployeeId);

                if (!exist)
                {
                    var reportModel = new MonthlyAttSheetDtlVm();

                    reportModel.EmployeeId = filterData.EmployeeId;
                    reportModel.EmployeeName = filterData.Employee.Name;
                    reportModel.EmployeeCode = filterData.Employee.Code;
                    reportModel.EmpDesignation = filterData?.Employee?.Designation?.Name;
                    reportModel.EmpDepartment = filterData?.Employee?.Department?.Name;

                    var monthDays = (short)(endOfMonth - startOfMonth).TotalDays;
                    reportModel.TotalDays = (short)(monthDays + 1);

                    var unPaidLeaves = Context.EmpLeaveApplications.Where(c => c.EmployeeId == reportModel.EmployeeId && (c.LeaveTypeId == (short)LeaveTypeEnum.LeaveWithoutPay || c.LeaveTypeId == (short)LeaveTypeEnum.StudyLeave)).ToList();
                    unPaidLeaves = unPaidLeaves.Where(c => dataDateList.Contains(c.FromDate.Date) || dataDateList.Contains(c.ToDate.Date)).ToList();
                    var unpaidLeavesCount = unPaidLeaves.Count();
                    var totalLeaves = (short)dataList.Where(c => c.EmployeeId == reportModel.EmployeeId && c.Status == "L").Count();

                    reportModel.PresentDays = (short)dataList.Where(c => c.EmployeeId == reportModel.EmployeeId && c.Status == "P").Count();
                    reportModel.AbsentDays = (short)dataList.Where(c => c.EmployeeId == reportModel.EmployeeId && c.Status == "A").Count();
                    reportModel.LeaveDays = totalLeaves;
                    reportModel.UnPaidLeaveDays = (short)unpaidLeavesCount;
                    reportModel.Holidays = (short)dataList.Where(c => c.EmployeeId == reportModel.EmployeeId && c.Status == "H").Count();
                    reportModel.OffDays = (short)dataList.Where(c => c.EmployeeId == reportModel.EmployeeId && c.Status == "O").Count();
                    reportModel.LateDays = (short)dataList.Where(c => c.EmployeeId == reportModel.EmployeeId && c.Status == "P" && c.StatusDtl == "L").Count();

                    var totalAdditon = reportModel.PresentDays + reportModel.OffDays + reportModel.Holidays + reportModel.LeaveDays;

                    reportModel.PayDays = (short)(totalAdditon - unpaidLeavesCount);


                    //var deductDays = 0;

                    //if (reportModel.LateDays > 3)
                    //{
                    //    for (int i = 0; i < reportModel.LateDays; i++)
                    //    {
                    //        while (i == 2)
                    //        {
                    //            deductDays += 1;
                    //            i = 0;
                    //        }
                    //    }
                    //}

                    //reportModel.PayDays = (short)(reportModel.PresentDays - (reportModel.AbsentDays + deductDays));

                    dtlDataList.Add(reportModel);
                }

            }

            monthlyAttendance.MonthlyAttendanceSheetDtls = dtlDataList;

            return monthlyAttendance;

        }

        #endregion

        #region DailyReport

        public async Task<DataTablePagination<EmpDailyAttendanceReportVm, EmpDailyAttendanceReportVm>> DailyReportAsync(DataTablePagination<EmpDailyAttendanceReportVm, EmpDailyAttendanceReportVm> vm)
        {
            var searchResult = Context.EmpAttendances
                .Include(b => b.Employee)
                    .ThenInclude(d => d.Designation)
                .Include(c => c.Employee)
                    .ThenInclude(dp => dp.Department)
                .AsQueryable().Where(c => !c.IsDeleted);

            var model = vm.SearchModel;

            if (model == null) throw new Exception("Search Attendances not found");

            if (model.EmployeeId > 0)
            {
                searchResult = searchResult.Where(c => c.EmployeeId == model.EmployeeId);
            }

            if (model.DesignationId > 0)
            {
                searchResult = searchResult.Where(c => c.Employee.DesignationId == model.DesignationId);
            }

            if (model.DepartmentId > 0)
            {
                searchResult = searchResult.Where(c => c.Employee.DepartmentId == model.DepartmentId);
            }

            if (model.SearchDateStr != null)
            {
                var currentDate = Utility.ConvertStrToDate(model.SearchDateStr);
                searchResult = searchResult.Where(c => c.AttendDate == currentDate);
            }

            if (!string.IsNullOrEmpty(model.FormDateStr))
            {
                var formDate = (DateTime)(!string.IsNullOrEmpty(model.FormDateStr) ? Utility.ConvertStrToDate(model.FormDateStr) : model.AttendDate);
                searchResult = searchResult.Where(c => c.AttendDate >= formDate);
            }

            if (!string.IsNullOrEmpty(model.ToDateStr))
            {
                var toDate = (DateTime)(!string.IsNullOrEmpty(model.ToDateStr) ? Utility.ConvertStrToDate(model.ToDateStr) : model.AttendDate);
                searchResult = searchResult.Where(c => c.AttendDate <= toDate);
            }

            if (!string.IsNullOrEmpty(vm.Search.Value))
            {
                var value = vm.Search.Value.Trim().ToLower();
                searchResult = searchResult.Where(c => c.Employee.Name.ToLower().Contains(value));
            }

            var totalRecords = await searchResult.CountAsync();

            if (totalRecords > 0)
            {
                vm.recordsTotal = totalRecords;
                vm.recordsFiltered = totalRecords;
                vm.draw = vm.LineDraw ?? 0;

                var data = await searchResult.OrderBy(c => c.AttendDate).ToListAsync();

                if (model.IsDayWise)
                {
                    data = data.OrderBy(c => c.Employee.Designation.Code).ToList();
                }

                data = data.Skip(vm.Start).Take(vm.Length).ToList();

                var start = (DateTime)(!string.IsNullOrEmpty(model.FormDateStr) ? Utility.ConvertStrToDate(model.FormDateStr) : model.AttendDate);
                var end = (DateTime)(!string.IsNullOrEmpty(model.ToDateStr) ? Utility.ConvertStrToDate(model.ToDateStr) : model.AttendDate);
                var dates = GetDateRange(start, end);

                var dataList = new List<EmpDailyAttendanceReportVm>();

                if (dates.Count > 0)
                {
                    foreach (var date in dates)
                    {
                        var item = data.FirstOrDefault(c => c.AttendDate.Date == date.Date);
                        var reportModel = new EmpDailyAttendanceReportVm();

                        if (item == null)
                        {
                            reportModel.AttendDate = date;
                            reportModel.AttendDateStr = date.ToString("d");
                            reportModel.EmployeeId = model.EmployeeId;
                            reportModel.NoEntry = true;
                            reportModel.Status = "N";
                            reportModel.InTimeStr = string.Empty;
                            reportModel.OutTimeStr = string.Empty;
                        }
                        else
                        {
                            reportModel.AttendDate = item.AttendDate;
                            reportModel.AttendDateStr = item.AttendDate.ToString("d");
                            reportModel.EmployeeId = item.EmployeeId;
                            reportModel.EmployeeName = item.Employee.Name;
                            reportModel.EmployeeCode = item.Employee.Code;
                            reportModel.EmpDesignation = item.Employee.Designation?.Name;
                            reportModel.EmpDepartment = item.Employee.Department?.Name;
                            reportModel.Present = item.Status == "P" ? true : false;
                            reportModel.Absent = item.Status == "A" ? true : false;
                            reportModel.Leave = item.Status == "L" ? true : false;
                            reportModel.OffDay = item.Status == "O" ? true : false;
                            reportModel.Holiday = item.Status == "H" ? true : false;
                            reportModel.Late = item.Status == "P" && item.StatusDtl == "L" ? true : false;
                            reportModel.InTimeStr = item.Status == "P" ? item.InTime?.ToString("t") : string.Empty;
                            reportModel.OutTimeStr = item.Status == "P" ? item.OutTime?.ToString("t") : string.Empty;
                            reportModel.Status = item.Status;
                            reportModel.LunchIn = item.LunchIn;
                            reportModel.LunchOut = item.LunchOut;
                        }


                        //if (reportModel.Late)
                        //{
                        //    var startTime = Context.HrSettings.FirstOrDefault().OfficeStartTime;
                        //    var startDateTime = 
                        //    var letTime = item.InTime - startTime;
                        //}


                        dataList.Add(reportModel);

                    }
                }

                vm.data = _iMapper.Map<List<EmpDailyAttendanceReportVm>>(dataList);

                var sl = vm.Start;

                foreach (var searchDto in vm.data)
                {
                    var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                    searchDto.SerialNo = ++sl;
                }
            }

            return vm;
        }

        #endregion

        #region DayWiseReport

        public async Task<DataTablePagination<EmpDailyAttendanceReportVm, EmpDailyAttendanceReportVm>> DayWiseReportAsync(DataTablePagination<EmpDailyAttendanceReportVm, EmpDailyAttendanceReportVm> vm)
        {
            var searchResult = Context.EmpAttendances
                .Include(b => b.Employee)
                    .ThenInclude(d => d.Designation)
                .Include(c => c.Employee)
                    .ThenInclude(dp => dp.Department)
                .AsQueryable().Where(c => !c.IsDeleted);

            var model = vm.SearchModel;

            if (model == null) throw new Exception("Search Attendances not found");

            if (model.EmployeeId > 0)
            {
                searchResult = searchResult.Where(c => c.EmployeeId == model.EmployeeId);
            }

            if (model.DesignationId > 0)
            {
                searchResult = searchResult.Where(c => c.Employee.DesignationId == model.DesignationId);
            }

            if (model.DepartmentId > 0)
            {
                searchResult = searchResult.Where(c => c.Employee.DepartmentId == model.DepartmentId);
            }

            if (model.SearchDateStr != null)
            {
                var currentDate = Utility.ConvertStrToDate(model.SearchDateStr);
                searchResult = searchResult.Where(c => c.AttendDate == currentDate);
            }

            if (!string.IsNullOrEmpty(model.FormDateStr))
            {
                var formDate = (DateTime)(!string.IsNullOrEmpty(model.FormDateStr) ? Utility.ConvertStrToDate(model.FormDateStr) : model.AttendDate);
                searchResult = searchResult.Where(c => c.AttendDate >= formDate);
            }

            if (!string.IsNullOrEmpty(model.ToDateStr))
            {
                var toDate = (DateTime)(!string.IsNullOrEmpty(model.ToDateStr) ? Utility.ConvertStrToDate(model.ToDateStr) : model.AttendDate);
                searchResult = searchResult.Where(c => c.AttendDate <= toDate);
            }

            if (!string.IsNullOrEmpty(vm.Search.Value))
            {
                var value = vm.Search.Value.Trim().ToLower();
                searchResult = searchResult.Where(c => c.Employee.Name.ToLower().Contains(value));
            }

            var totalRecords = await searchResult.CountAsync();

            if (totalRecords > 0)
            {
                vm.recordsTotal = totalRecords;
                vm.recordsFiltered = totalRecords;
                vm.draw = vm.LineDraw ?? 0;

                var data = await searchResult.OrderBy(c => c.Employee.Designation.Code)
                                             .ToListAsync();

                var dataList = new List<EmpDailyAttendanceReportVm>();

                if (data.Count > 0)
                {
                    foreach (var item in data)
                    {
                        var reportModel = new EmpDailyAttendanceReportVm();

                        reportModel.AttendDate = item.AttendDate;
                        reportModel.AttendDateStr = item.AttendDate.ToString("d");
                        reportModel.EmployeeId = item.EmployeeId;
                        reportModel.EmployeeName = item.Employee.Name;
                        reportModel.EmployeeCode = item.Employee.Code;
                        reportModel.EmpDesignation = item.Employee.Designation?.Name;
                        reportModel.EmpDepartment = item.Employee.Department?.Name;
                        reportModel.Present = item.Status == "P" ? true : false;
                        reportModel.Absent = item.Status == "A" ? true : false;
                        reportModel.Leave = item.Status == "L" ? true : false;
                        reportModel.OffDay = item.Status == "O" ? true : false;
                        reportModel.Holiday = item.Status == "H" ? true : false;
                        reportModel.Late = item.Status == "P" && item.StatusDtl == "L" ? true : false;
                        reportModel.InTimeStr = item.Status == "P" ? item.InTime?.ToString("t") : string.Empty;
                        reportModel.OutTimeStr = item.Status == "P" ? item.OutTime?.ToString("t") : string.Empty;
                        reportModel.Status = item.Status;
                        reportModel.LunchIn = item.LunchIn;
                        reportModel.LunchOut = item.LunchOut;
                        //if (reportModel.Late)
                        //{
                        //    var startTime = Context.HrSettings.FirstOrDefault().OfficeStartTime;
                        //    var startDateTime = 
                        //    var letTime = item.InTime - startTime;
                        //}


                        dataList.Add(reportModel);

                    }
                }

                vm.data = _iMapper.Map<List<EmpDailyAttendanceReportVm>>(dataList);

                vm.data = vm.data.Skip(vm.Start).Take(vm.Length).ToList();

                var sl = vm.Start;

                foreach (var searchDto in vm.data)
                {
                    var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                    searchDto.SerialNo = ++sl;
                }
            }

            return vm;
        }

        #endregion

        #region GetMonthlyManualAttendanceData

        public async Task<MonthlyAttSheetMstVm> GetMonthlyManualAttendanceData(MonthlyAttSheetMstVm vm)
        {
            var employeeList = await Context.Employees
                .Include(d => d.Department)
                .Include(c => c.Designation)
                .Where(x => !x.IsDeleted && x.IsEnable).AsNoTracking().ToListAsync();

            if (!(employeeList.Count > 0))
                throw new Exception("Employee not found");

            if (vm == null)
                throw new Exception("Information not found");

            var startOfMonth = new DateTime(vm.Year, vm.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var monthlyAttendance = new MonthlyAttSheetMstVm();

            monthlyAttendance.Year = vm.Year;
            monthlyAttendance.Month = vm.Month;
            monthlyAttendance.DateForm = startOfMonth;
            monthlyAttendance.DateTo = endOfMonth;

            employeeList = employeeList.OrderBy(c => c.Designation.Code).ToList();

            if (employeeList.Count > 0)
            {
                var dtlDataList = new List<MonthlyAttSheetDtlVm>();

                foreach (var employee in employeeList)
                {
                    var reportModel = new MonthlyAttSheetDtlVm();

                    reportModel.EmployeeId = employee.Id;
                    reportModel.EmployeeName = employee.Name;
                    reportModel.EmployeeCode = employee.Code;
                    reportModel.EmpDesignation = employee.Designation?.Name;
                    reportModel.EmpDepartment = employee.Department?.Name;

                    var monthDays = (short)(endOfMonth - startOfMonth).TotalDays;
                    reportModel.TotalDays = (short)(monthDays + 1);

                    //var unPaidLeaves = Context.EmpLeaveApplications
                    //    .Where(c => c.EmployeeId == reportModel.EmployeeId && (c.LeaveTypeId == (short)LeaveTypeEnum.LeaveWithoutPay || c.LeaveTypeId == (short)LeaveTypeEnum.StudyLeave)).ToList();

                    reportModel.PresentDays = reportModel.TotalDays;
                    reportModel.AbsentDays = 0;
                    reportModel.LeaveDays = 0;
                    reportModel.UnPaidLeaveDays = 0;
                    reportModel.Holidays = 0;
                    reportModel.OffDays = 0;
                    reportModel.LateDays = 0;

                    var totalAdditon = reportModel.PresentDays + reportModel.OffDays + reportModel.Holidays + reportModel.LeaveDays;

                    reportModel.PayDays = (short)(totalAdditon - reportModel.UnPaidLeaveDays);

                    dtlDataList.Add(reportModel);
                }

                monthlyAttendance.MonthlyAttendanceSheetDtls = dtlDataList;
            }

            return monthlyAttendance;

        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            Context.Dispose();
        }

        #endregion

        private static List<DateTime> GetDateRange(DateTime start, DateTime end)
        {
            var dates = new List<DateTime>();
            for (var date = start; date <= end; date = date.AddDays(1))
            {
                dates.Add(date);
            }
            return dates;
        }
    }
}
