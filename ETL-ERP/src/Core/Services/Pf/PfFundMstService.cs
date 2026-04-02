using AutoMapper;
using Domain.Entities.Pf;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.Pf.PfFund;
using Domain.ViewModel.Report;
using Interface.Repository.Pf;
using Interface.Services.Pf;
using Interface.UnitOfWork;
using Services.Base;

namespace Services.Pf;

public class PfFundMstService : BaseService<PfFundMst>, IPfFundMstService
{
    #region Config
    private IPfFundMstRepository Repository;
    private readonly IPfFundDtlRepository _iDtlRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly IPfReportRepository _iPfReportRepository;

    public PfFundMstService(IPfFundMstRepository iRepository, IMapper iMapper, IUnitOfWork iUnitOfWork, IPfFundDtlRepository iDtlRepository, IPfReportRepository iPfReportRepository)
        : base(iRepository, iUnitOfWork)
    {
        Repository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
        _iDtlRepository = iDtlRepository;
        _iPfReportRepository = iPfReportRepository;
    }

    #endregion

    #region Search

    public async Task<DataTablePagination<PfFundMstSearchVm, PfFundMstSearchVm>> SearchAsync(DataTablePagination<PfFundMstSearchVm, PfFundMstSearchVm> model)
    {
        var dataList = await Repository.SearchAsync(model);
        return dataList;
    }

    public async Task<DataTablePagination<EmployeePfSummaryReportVm, EmployeePfSummaryReportVm>> SearchDtlAsync(DataTablePagination<EmployeePfSummaryReportVm, EmployeePfSummaryReportVm> model)
    {
        var dataList = await _iDtlRepository.SearchAsync(model);
        return dataList;
    }

    #endregion

    #region PfReportAdd

    public async Task<bool> PfReportAdd(PfFundMstVm modelVm)
    {
        if (modelVm.Year > 1999 && modelVm.Month > 0)
        {
            DateTime firstDate = new DateTime(modelVm.Year, modelVm.Month, 1);
            DateTime lastDate = firstDate.AddMonths(1).AddSeconds(-1);

            var formDate = firstDate;
            var toDate = lastDate;

            var exist = Repository.GetFirstOrDefault(c => firstDate.Date == c.FundFromDate.Date && lastDate.Date == c.FundToDate.Date, d => d.PfFundDtls);
            if (exist != null && exist.PfFundDtls.Count > 0) _iDtlRepository.RemoveRange(exist.PfFundDtls);
            if (exist != null) Repository.Remove(exist);
        }

        var model = _iMapper.Map<PfFundMst>(modelVm);

        if (modelVm.Year > 1999 && modelVm.Month > 0)
        {
            DateTime firstDate = new DateTime(modelVm.Year, modelVm.Month, 1);
            DateTime lastDate = firstDate.AddMonths(1).AddSeconds(-1);

            model.FundFromDate = firstDate;
            model.FundToDate = lastDate;
        }

        model.EntryDate = Utility.GetBdDateTimeNow();
        model.ActionById = CurrentUserId;
        model.ActionDate = Utility.GetBdDateTimeNow();

        await Repository.AddAsync(model);
        var isAdded = await _iUnitOfWork.CompleteAsync();
        if (!isAdded) return false;
        return true;
    }

    #endregion

    #region GetPfById

    public async Task<PfFundMstVm> GetPfByIdAsync(long id)
    {
        var data = await Repository.GetPfByIdAsync(id);
        var model = _iMapper.Map<PfFundMstVm>(data);
        model.Month = (short)data.FundFromDate.Month;
        model.Year = (short)data.FundFromDate.Year;

        if (model.PfFundDtls.Count > 0)
        {
            foreach (var dtl in model.PfFundDtls)
            {
                var filterData = data.PfFundDtls.FirstOrDefault(c => c.Id == dtl.Id);

                dtl.EmployeeName = filterData?.Employee.Name;
                dtl.EmployeeCode = filterData?.Employee.Code;
                dtl.EmpDepartment = filterData?.Employee?.Department.Name;
                dtl.EmpDesignation = filterData?.Employee?.Designation.Name;
            }
        }

        return model;
    }

    #endregion

    #region GetEmpPfFundData

    public async Task<PfFundMstVm> GetEmpPfFundData(PfFundMstVm vm)
    {
        var data = await Repository.GetPfFundData(vm);
        return data;
    }

    #endregion

    #region GetEmployeePfInfo

    public async Task<EmpPfSummaryDtlReportVm> GetEmployeePfInfo(long employeeId)
    {
        var data = await _iDtlRepository.GetEmployeePfInfo(employeeId);
        return data;
    }

    #endregion

    #region ReportHtml

    public async Task<string> EmployeePFListReportHtml()
    {
        var searchVm = new DataTablePagination<EmployeePfSummaryReportVm, EmployeePfSummaryReportVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new EmployeePfSummaryReportVm();
        var dataTable = await SearchDtlAsync(searchVm);
        var dataList = _iMapper.Map<List<EmployeePfSummaryReportVm>>(dataTable.data);

        var fullHtml = "";

        fullHtml += "<table class='table table-bordered' id='EmployeePFListReportTablePrint' style='width:100%;text-align: center'>";
        fullHtml += "<thead>";
        fullHtml += "<tr>";
        fullHtml += "<th style='width:5%'>Sl.</th>";
        fullHtml += "<th style='width:20%'>Employee</th>";
        fullHtml += "<th style='width:20%'>Designation</th>";
        fullHtml += "<th style='width:20%'>Department</th>";
        fullHtml += "<th style='width:10%'>Employee Total Contribution</th>";
        fullHtml += "<th style='width:10%'>Company Total Contribution</th>";
        fullHtml += "<th style='width:5%'>Interest</th>";
        fullHtml += "<th style='width:10%'>Total PF Amount</th>";
        fullHtml += "</tr>";
        fullHtml += "</thead>";

        fullHtml += "<tbody>";

        if (dataList.Count > 0)
        {
            foreach (var (v, i) in dataList.GetItemWithIndex())
            {
                fullHtml += "<tr>";

                fullHtml += $@"<td>{i + 1}</td>";
                fullHtml += $@"<td class='text-start'><b>{v.EmployeeName}</b></td>";
                fullHtml += $@"<td class='text-center'>{v.EmpDesignation}</td>";
                fullHtml += $@"<td class='text-center'>{v.EmpDepartment}</td>";
                fullHtml += $@"<td class='text-center'>{v.TotalEmpCon}</td>";
                fullHtml += $@"<td class='text-center'>{v.TotalCompCon}</td>";
                fullHtml += $@"<td class='text-center'>{v.TotalInterest}</td>";
                fullHtml += $@"<td class='text-center'>{v.TotalPfAmount}</td>";

                fullHtml += "</tr>";
            }
        }
        fullHtml += "</tbody>";
        fullHtml += "</table>";

        return fullHtml;
    }

    public async Task<string> EmployeePFDetailsReportHtml(long empId)
    {
        var model = await GetEmployeePfInfo(empId);

        var fullHtml = "";

        fullHtml += "<div style='padding-bottom: 10px;'>";
        fullHtml += $@"<table class='PfFundDetailSummaryTable'>
                                <tbody>
                                    <tr>
                                        <td style='width:20%'><b> Employee Name </b></td>
                                        <td colspan='3'>{model.EmployeeName} - {model.EmployeeCode}</td>
                                    </tr>
                                    <tr>
                                        <td style='width:20%'><b> Employee Contibute </b></td>
                                        <td style='width:30%'>{model.EmpCon} </td>
                                        <td style='width:20%'><b> Company Contibute </b></td>
                                        <td style='width:30%'>{model.CompCon} </td>
                                    </tr>
                                    <tr>
                                        <td style='width:20%'><b> Interest Rate </b></td>
                                        <td style='width:30%'>{model.Interest} </td>
                                        <td style='width:20%'><b> Total Amount </b></td>
                                        <td style='width:30%'>{model.TotalAmount} </td>
                                    </tr>
                                </tbody>
                            </table>";

        fullHtml += "</div>";

        fullHtml += "<table class='table' style='width:100%;text-align: center;'>";
        fullHtml += "<thead>";
        fullHtml += "<tr>";
        fullHtml += "<th style='width:5%'>Month</th>";
        fullHtml += "<th style='width:20%'>Employee Con</th>";
        fullHtml += "<th style='width:20%'>Company Con</th>";
        fullHtml += "<th style='width:20%'>Total Con</th>";
        fullHtml += "<th style='width:10%'>Interest</th>";
        fullHtml += "<th style='width:25%'>Total Amount</th>";
        fullHtml += "</tr>";
        fullHtml += "</thead>";

        fullHtml += "<tbody>";

        if (model.PfFundDtlVms.Count > 0)
        {
            foreach (var (v, i) in model.PfFundDtlVms.GetItemWithIndex())
            {
                fullHtml += "<tr>";

                fullHtml += $@"<td class='text-center'>{i + 1}</td>";
                fullHtml += $@"<td class='text-center'><b>{v.EmpCon}</b></td>";
                fullHtml += $@"<td class='text-center'>{v.CompCon}</td>";
                fullHtml += $@"<td class='text-center'>{v.EmpCon + v.CompCon}</td>";
                fullHtml += $@"<td class='text-center'>{v.Interest}</td>";
                fullHtml += $@"<td class='text-center'>{v.TotalAmount}</td>";

                fullHtml += "</tr>";
            }
            fullHtml += "<tr>";
            fullHtml += "<td class='text-end'><b>Total</b></td>";
            fullHtml += $@"<td class='text-center'><b>{model.PfFundDtlVms.Sum(c => c.EmpCon)}</b></td>";
            fullHtml += $@"<td class='text-center'><b>{model.PfFundDtlVms.Sum(c => c.CompCon)}</b></td>";
            fullHtml += $@"<td class='text-center'><b>{model.PfFundDtlVms.Sum(c => c.EmpCon + c.CompCon)}</b></td>";
            fullHtml += $@"<td class='text-center'><b>{model.PfFundDtlVms.Sum(c => c.Interest)}</b></td>";
            fullHtml += $@"<td class='text-center'><b>{model.PfFundDtlVms.Sum(c => c.TotalAmount)}</b></td>";
            fullHtml += "</tr>";
        }
        else
        {
            fullHtml += "<tr>";
            fullHtml += "<td style='text-align:center' colspan='9'><b>Employee Provident Fund Not Found</b></td>";
            fullHtml += "</tr>";
        }

        fullHtml += "";

        fullHtml += "</tbody>";
        fullHtml += "</table>";

        return fullHtml;
    }

    public async Task<string> GetPfScheduleReportHtml(PfFundMstVm vm)
    {
        string fullHtml = await _iPfReportRepository.GetPfScheduleReportHtml(vm);
        return fullHtml;
    }

    #endregion
}
