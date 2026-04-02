using AutoMapper;
using Domain.Entities.Payroll;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.Attendance.MonthlyAttSheet;
using Interface.Repository.Payroll;
using Interface.Services.Payroll;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Payroll;

public class MonthlyAttSheetMstService : BaseService<MonthlyAttendanceSheetMst>, IMonthlyAttSheetMstService
{
    #region Config

    private IMonthlyAttSheetMstRepository Repository;
    private readonly IMonthlyAttSheetDtlRepository _iDtlRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;

    public MonthlyAttSheetMstService(IMonthlyAttSheetMstRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork, IMonthlyAttSheetDtlRepository iDtlRepository) : base(iRepository, iUnitOfWork)
    {
        Repository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
        _iDtlRepository = iDtlRepository;
    }

    #endregion

    #region Search

    public async Task<DataTablePagination<MonthlyAttSheetSearchVm, MonthlyAttSheetSearchVm>> SearchAsync(DataTablePagination<MonthlyAttSheetSearchVm, MonthlyAttSheetSearchVm> model)
    {
        var dataList = await Repository.SearchAsync(model);
        return dataList;
    }

    #endregion

    #region SheetAdd

    public async Task<bool> SheetAdd(MonthlyAttSheetMstVm modelVm)
    {
        if (modelVm.Year > 1999 && modelVm.Month > 0)
        {
            var exist = Repository.GetFirstOrDefault(c => c.Year == modelVm.Year && c.Month == modelVm.Month, d => d.MonthlyAttendanceSheetDtls);
            if (exist != null && exist.MonthlyAttendanceSheetDtls.Count > 0) _iDtlRepository.RemoveRange(exist.MonthlyAttendanceSheetDtls);
            if (exist != null) Repository.Remove(exist);
        }

        var model = _iMapper.Map<MonthlyAttendanceSheetMst>(modelVm);

        if (modelVm.Year > 1999 && modelVm.Month > 0)
        {
            DateTime firstDate = new DateTime(modelVm.Year, modelVm.Month, 1);
            DateTime lastDate = firstDate.AddMonths(1).AddSeconds(-1);

            model.DateFrom = firstDate;
            model.DateTo = lastDate;
        }

        model.ActionById = CurrentUserId;
        model.ActionDate = Utility.GetBdDateTimeNow();

        model.MonthlyAttendanceSheetDtls.ForEach(c => c.ActionById = CurrentUserId);
        model.MonthlyAttendanceSheetDtls.ForEach(c => c.ActionDate = Utility.GetBdDateTimeNow());

        await Repository.AddAsync(model);
        var isAdded = await _iUnitOfWork.CompleteAsync();
        if (!isAdded) return false;
        return true;
    }

    #endregion

    #region GetSheetByIdAsync

    public async Task<MonthlyAttSheetMstVm> GetSheetByIdAsync(long id)
    {
        var data = await Repository.GetSheetByIdAsync(id);
        var model = _iMapper.Map<MonthlyAttSheetMstVm>(data);

        if (model.MonthlyAttendanceSheetDtls.Count > 0)
        {
            foreach (var dtl in model.MonthlyAttendanceSheetDtls)
            {
                var filterData = data.MonthlyAttendanceSheetDtls.FirstOrDefault(c => c.Id == dtl.Id);

                dtl.EmployeeName = filterData?.Employee.Name;
                dtl.EmployeeCode = filterData?.Employee.Code;
                dtl.EmpDepartment = filterData?.Employee?.Department.Name;
                dtl.EmpDepartmentCode = filterData?.Employee?.Department.Code;
                dtl.EmpDesignation = filterData?.Employee?.Designation.Name;
                dtl.EmpDesignationCode = filterData?.Employee?.Designation.Code;
            }
        }

        return model;
    }

    #endregion

    #region MonthlyAttendanceReportHtml

    public async Task<string> MonthlyAttendanceReportHtml(long id)
    {
        var data = await GetSheetByIdAsync(id);
        var model = _iMapper.Map<MonthlyAttSheetMstVm>(data);
        var fullHtml = "";

        fullHtml += "<table class='table table-bordered' id='MonthlyAttendanceReportTablePrint' style='width:100%;text-align: center'>";
        fullHtml += "<thead>";
        fullHtml += "<tr>";
        fullHtml += "<th style='width:3%'>Sl.</th>";
        fullHtml += "<th style='width:12%'>Employee Name</th>";
        fullHtml += "<th style='width:5%'>Employee Code</th>";
        fullHtml += "<th style='width:12%'>Department</th>";
        fullHtml += "<th style='width:12%'>Designation</th>";
        fullHtml += "<th style='width:7%'>Total Days</th>";
        fullHtml += "<th style='width:7%'>W. Holidays</th>";
        fullHtml += "<th style='width:7%'>Festival Holidays</th>";
        fullHtml += "<th style='width:7%'>Present</th>";
        fullHtml += "<th style='width:7%'>Absent</th>";
        fullHtml += "<th style='width:7%'>Leave</th>";
        fullHtml += "<th style='width:7%'>Late</th>";
        fullHtml += "<th style='width:7%'>Pay Days</th>";
        fullHtml += "</tr>";
        fullHtml += "</thead>";

        fullHtml += "<tbody>";

        if (model.MonthlyAttendanceSheetDtls.Count > 0)
        {
            foreach (var (v, i) in model.MonthlyAttendanceSheetDtls.GetItemWithIndex())
            {
                fullHtml += "<tr>";

                fullHtml += $@"<td>{i + 1}</td>";
                fullHtml += $@"<td class='text-start'><b>{v.EmployeeName}</b></td>";
                fullHtml += $@"<td class='text-center'>{v.EmployeeCode}</td>";
                fullHtml += $@"<td class='text-center'>{v.EmpDepartment}</td>";
                fullHtml += $@"<td class='text-center'>{v.EmpDesignation}</td>";
                fullHtml += $@"<td class='text-center'>{v.TotalDays}</td>";
                fullHtml += $@"<td class='text-center'>{v.OffDays}</td>";
                fullHtml += $@"<td class='text-center'>{v.Holidays}</td>";
                fullHtml += $@"<td class='text-center'>{v.PresentDays}</td>";
                fullHtml += $@"<td class='text-center'>{v.AbsentDays}</td>";
                fullHtml += $@"<td class='text-center'>{v.LeaveDays}</td>";
                fullHtml += $@"<td class='text-center'>{v.LateDays}</td>";
                fullHtml += $@"<td class='text-center'>{v.PayDays}</td>";

                fullHtml += "</tr>";
            }
        }
        fullHtml += "</tbody>";
        fullHtml += "</table>";

        return fullHtml;
    }

    #endregion
}
