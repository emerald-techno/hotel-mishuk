using AutoMapper;
using Domain.Utility.Common;
using Domain.ViewModel.Pf.PfFund;
using Interface.Repository.Accounts;
using Interface.Repository.Pf;
using Interface.Services.Admin;
using Persistence.DapperModel;
using DU = Domain.Utility;

namespace Repository.Pf;

public class PfReportRepository : IPfReportRepository
{
    #region Config

    private readonly IMapper _iMapper;
    private readonly IApplicationReadDbConnection _iReadDbConnection;
    private readonly IAccTranMstRepository _AccTranRepository;

    public PfReportRepository(IMapper iMapper, IApplicationReadDbConnection iReadDbConnection, IAccTranMstRepository accTranRepository)
    {
        _iMapper = iMapper;
        _iReadDbConnection = iReadDbConnection;
        _AccTranRepository = accTranRepository;
    }

    #endregion

    #region GetPfScheduleReportData

    public async Task<List<PfFundSchduelVm>> GetPfScheduleReportData(PfFundMstVm vm)
    {
        vm.FormDateStr = (string.IsNullOrEmpty(vm.FormDateStr)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.FormDateStr;
        vm.ToDateStr = (string.IsNullOrEmpty(vm.ToDateStr)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.ToDateStr;
        var fromDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.FormDateStr)).ToString("dd/MMM/yyyy");
        var toDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.ToDateStr)).ToString("dd/MMM/yyyy");



        string query = $@"select p.EmployeeId,p.EmployeeName,p.EmployeePhoto,p.DesignationId,p.DesignationName,p.DepartmentId,p.DepartmentName,p.JoinDate,min(p.PfStartDate) PfStartDate,max(p.PfLastDate) PfLastDate
            ,round(sum(p.EmpConOp),2) EmpConOp,round(sum(p.CompConOp),2) CompConOp,round(sum(p.EmpInterestOp),2) EmpIntOp,round(sum(p.CompInterestOp),2) CompIntOp
            ,round(sum(p.TotalEmpOp),2) TotalEmpOp,round(sum(p.TotalCompOp),2) TotalCompOp,round(sum(p.TotalAmountOp),2) TotalAmountOp
            ,round(sum(p.EmpCon),2) EmpCon,round(sum(p.CompCon),2) CompCon,round(sum(p.EmpInterest),2) EmpInt,round(sum(p.CompInterest),2) CompInt
            ,round(sum(p.TotalAmount),2) TotalAmount
            from (
            select ei.Id EmployeeId,ei.Name EmployeeName,ei.PhotoUrl EmployeePhoto,sdg.Id DesignationId,sdg.Name DesignationName,dpt.Id DepartmentId,dpt.Name DepartmentName
            ,ei.JoinDate,min(convert(date,PfStartDate))PfStartDate,max(convert(date,PfLastDate)) PfLastDate,round(sum(d.EmpCon),0) EmpConOp,round(sum(d.CompCon),2)CompConOp
            ,case when sum(d.Interest) > 0 then round(sum(d.Interest)/2,2) else 0 end EmpInterestOp
            ,case when sum(d.Interest) > 0 then round(sum(d.Interest)/2,2) else 0 end CompInterestOp
            ,round(round(sum(d.EmpCon),0) + (case when sum(d.Interest) > 0 then round(sum(d.Interest)/2,2) else 0 end),2) TotalEmpOp
            ,round(round(sum(d.CompCon),2)+ (case when sum(d.Interest) > 0 then round(sum(d.Interest)/2,2) else 0 end),2) TotalCompOp
            ,round(round(sum(d.EmpCon),0) + round(sum(d.CompCon),2) + round(sum(d.Interest),2),2) TotalAmountOp
            ,0 EmpCon,0 CompCon,0 EmpInterest,0 CompInterest,0 TotalAmount
            from (
            select fo.EmployeeId,fo.EmpCon,fo.CompCon,fo.Interest,fo.PfStartDate,fo.PfStartDate PfLastDate from PfFundOpennings fo
            union all
            select fd.EmployeeId,fd.EmpCon,fd.CompCon,fd.Interest,fm.FundFromDate PfStartDate,fm.FundToDate PfLastDate
            from PfFundDtls fd inner join PfFundMsts fm on fm.Id = fd.FundMstId
            ) d
            inner join Employees ei on ei.Id = d.EmployeeId
            inner join Designations sdg on sdg.Id = ei.DesignationId
            inner join Departments dpt on dpt.Id = ei.DepartmentId
            where convert(date,d.PfLastDate) <= '{fromDate}'
            group by ei.Id,ei.Name,ei.PhotoUrl,sdg.Id,sdg.Name,dpt.Id,dpt.Name,ei.JoinDate

            union all

            select ei.Id EmployeeId,ei.Name EmployeeName,ei.PhotoUrl EmployeePhoto,sdg.Id DesignationId,sdg.Name DesignationName,dpt.Id DepartmentId,dpt.Name DepartmentName
            ,ei.JoinDate,min(convert(date,PfStartDate))PfStartDate,max(convert(date,PfLastDate)) PfLastDate
            ,0 EmpConOp,0 CompConOp,0 EmpInterestOp,0 CompInterestOp,0 TotalEmpOp,0 TotalCompOp,0 TotalAmountOp
            ,round(sum(d.EmpCon),2) EmpCon,round(sum(d.CompCon),2) CompCon
            ,case when sum(d.Interest) > 0 then round(sum(d.Interest)/2,2) else 0 end EmpInterest
            ,case when sum(d.Interest) > 0 then round(sum(d.Interest)/2,2) else 0 end CompInterest
            ,round(round(sum(d.EmpCon),2) + round(sum(d.CompCon),2)+ round(sum(d.Interest),2),2) TotalAmount
            from (
            select fd.EmployeeId,fd.EmpCon,fd.CompCon,fd.Interest,fm.FundFromDate PfStartDate,fm.FundToDate PfLastDate
            from PfFundDtls fd inner join PfFundMsts fm on fm.Id = fd.FundMstId
            ) d
            inner join Employees ei on ei.Id = d.EmployeeId
            inner join Designations sdg on sdg.Id = ei.DesignationId
            inner join Departments dpt on dpt.Id = ei.DepartmentId
            where convert(date,d.PfLastDate) > '{fromDate}' and  convert(date,d.PfLastDate) <= '{toDate}'
            group by ei.Id,ei.Name,ei.PhotoUrl,sdg.Id,sdg.Name,dpt.Id,dpt.Name,ei.JoinDate
            ) p where 1=1
            group by p.EmployeeId,p.EmployeeName,p.EmployeePhoto,p.DesignationId,p.DesignationName,p.DepartmentId,p.DepartmentName,p.JoinDate";


        var data = await _iReadDbConnection.QueryAsync<PfFundSchduelVm>(query);
        return data.ToList();
    }

    #endregion

    #region GetPfScheduleReportHtml

    public async Task<string> GetPfScheduleReportHtml(PfFundMstVm vm)
    {
        string fullHtml = "";
        var data = await GetPfScheduleReportData(vm);

        fullHtml = "<table class='table table-striped table-bordered table-md' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
        /*Header Start*/
        fullHtml += $@"<thead class='bg-primary'><tr style='height:26px;'>
            <th class='pf_tbl_head' rowspan='2'>SL No</th><th class='pf_tbl_head' rowspan='2'>Employee Name</th>
            <th class='pf_tbl_head' rowspan='2'>Designation</th><th class='pf_tbl_head' rowspan='2'>Department</th>
            <th class='pf_tbl_head' rowspan='2'>PF Start Dt</th>
            <th class='pf_tbl_head' colspan='5'>Employee's Contribution</th>
            <th class='pf_tbl_head' colspan='5'>Employer's Contribution</th>
            <th class='pf_tbl_head' rowspan='2'>Members Fund</th>
            <th class='pf_tbl_head' rowspan='2'>Remarks</th>
            </tr>
            <tr style='height:26px;'>
            <th class='pf_tbl_head'>Openning</th><th class='pf_tbl_head'>Contribution during this period</th><th class='pf_tbl_head'>Total Contribution</th><th class='pf_tbl_head'>Profit</th><th class='pf_tbl_head'>Total Amount</th>
            <th class='pf_tbl_head'>Openning</th><th class='pf_tbl_head'>Contribution during this period</th><th class='pf_tbl_head'>Total Contribution</th><th class='pf_tbl_head'>Profit</th><th class='pf_tbl_head'>Total Amount</th>
            </tr></thead><tbody>";
        /*Header End*/

        if (data != null && data.Count > 0)
        {
            int slNo = 1;
            decimal totalLeftAmount = 0;
            decimal totalTransferAmount = 0;
            decimal totalMemberFund = 0;

            for (int i = 0; i < data.Count; i++)
            {
                PfFundSchduelVm objFund = data[i];
                decimal totalEmpCon = objFund.EmpConOp + objFund.EmpCon;
                decimal totalEmpAcc = objFund.EmpConOp + objFund.EmpCon + objFund.EmpInt;
                decimal totalCompCon = objFund.CompConOp + objFund.CompCon;
                decimal totalCompAcc = objFund.CompConOp + objFund.CompCon + objFund.CompInt;
                decimal totalFund = totalEmpAcc + totalCompAcc;
                totalMemberFund += totalFund;

                fullHtml += $@"<tr>
                    <td class='pf_tbl_row' style='height:26px'>{slNo}</td>
                    <td class='pf_tbl_row'>{objFund.EmployeeName}</td>
                    <td class='pf_tbl_row'>{objFund.DesignationName}</td>
                    <td class='pf_tbl_row'>{objFund.DepartmentName}</td>
                    <td class='pf_tbl_row'>{Convert.ToDateTime(objFund.PfStartDate).ToString("dd/MMM/yy")}</td>
                    <td class='pf_tbl_row'>{objFund.EmpConOp}</td>
                    <td class='pf_tbl_row'>{objFund.EmpCon}</td>
                    <td class='pf_tbl_row'>{totalEmpCon.ToString("N2")}</td>
                    <td class='pf_tbl_row'>{objFund.EmpInt}</td>
                    <td class='pf_tbl_row'>{totalEmpAcc.ToString("N2")}</td>
                    <td class='pf_tbl_row'>{objFund.CompConOp}</td>
                    <td class='pf_tbl_row'>{objFund.CompCon}</td>
                    <td class='pf_tbl_row'>{totalCompCon.ToString("N2")}</td>
                    <td class='pf_tbl_row'>{objFund.CompInt}</td>
                    <td class='pf_tbl_row'>{totalCompAcc.ToString("N2")}</td>
                    <td class='pf_tbl_row'>{totalFund.ToString("N2")}</td>
                    <td class='pf_tbl_row'></td>
                    </tr>";

            }

            //total
            decimal grandEmpOp = data.Sum(o => o.TotalEmpOp);
            decimal grandEmpCon = data.Sum(o => o.EmpCon);
            decimal grandEmpInt = data.Sum(o => o.EmpInt);
            decimal grandTotalEmpCon = grandEmpOp + grandEmpCon;
            decimal grandEmpAcc = grandEmpOp + grandEmpCon + grandEmpInt;

            decimal grandCompOp = data.Sum(o => o.TotalCompOp);
            decimal grandCompCon = data.Sum(o => o.CompCon);
            decimal grandCompInt = data.Sum(o => o.CompInt);
            decimal grandTotalCompCon = grandCompOp + grandCompCon;
            decimal grandCompAcc = grandCompOp + grandCompCon + grandCompInt;


            fullHtml += $@"<tr style='font-weight:bold;'>
                <td class='pf_tbl_row' style='height:26px'></td><td class='pf_tbl_row'></td><td class='pf_tbl_row'></td><td class='pf_tbl_row'></td>
                <td class='pf_tbl_row'>Total :</td>
                <td class='pf_tbl_row'>{grandEmpOp.ToString("N2")}</td>
                <td class='pf_tbl_row'>{grandEmpCon.ToString("N2")}</td>
                <td class='pf_tbl_row'>{grandTotalEmpCon.ToString("N2")}</td>
                <td class='pf_tbl_row'>{grandEmpInt.ToString("N2")}</td>
                <td class='pf_tbl_row'>{grandEmpAcc.ToString("N2")}</td>
                <td class='pf_tbl_row'>{grandCompOp.ToString("N2")}</td>
                <td class='pf_tbl_row'>{grandCompCon.ToString("N2")}</td>
                <td class='pf_tbl_row'>{grandTotalCompCon.ToString("N2")}</td>
                <td class='pf_tbl_row'>{grandCompInt.ToString("N2")}</td>
                <td class='pf_tbl_row'>{grandCompAcc.ToString("N2")}</td>
                <td class='pf_tbl_row'>{totalMemberFund.ToString("N2")}</td>
                <td class='pf_tbl_row'></td>
                </tr>";


        }
        fullHtml += "</tbody></table>";

        return fullHtml;
    }

    #endregion

    #region GetVoucherDetailHtml

    public async Task<string> GetVoucherDetailHtml(long id)
    {
        string fullHtml = "";
        var data = await _AccTranRepository.GetJournalDetails(id);

        fullHtml += "<div class='table border-less table-md' style='border:0px!important'>";
        fullHtml += "<table id='pi_table' class='table border-less table-md'>";
        fullHtml += $"<tr><td colspan='6' style='text-align:center;font-size:14px;font-weight:bold;text-decoration:underline;height:30px;'>{data.VcTypeText}</td></tr>";//header



        fullHtml += $@"<tr><td style='width:13%;font-weight:bold;height:28px;'>Voucher Date :</td><td style='width:20%;text-align:left;' > {data.VcDate.ToString("dd/MMM/yyyy")} </td>
            <td style='width:13%;font-weight:bold;text-align:right;'>Voucher No : </td><td style='width:20%;'> {data.VcNo} </td>
            <td style='width:13%;font-weight:bold;text-align:right;'>Ref. No : </td><td style='width:20%;'> {data.RefNo} </td></tr>";//invoice no and invoice date

        fullHtml += $@"<tr><td style='width:13%;font-weight:bold;height:28px;'>Currency :</td><td style='width:20%;text-align:left;' > {data.CurrencyName} </td>
            <td style='width:13%;font-weight:bold;text-align:right;'>Ex. Rate : </td><td style='width:20%;'></td>
            <td style='width:13%;font-weight:bold;text-align:right;'>Financial Year : </td><td style='width:20%;'> {data.FinYearName} </td></tr>";


        //item table
        fullHtml += "<tr><td colspan='6'><table class='table border table-striped table-bordered' style='border: 0px;' id='tbl_item'>";
        //header
        fullHtml += "<tr style='height:28px;'><th rowspan='2'> Ledger Code</th><th rowspan='2'>Ledger Name</th><th colspan='2'>Amount</th></tr>";
        fullHtml += "<tr style='height:28px;'><th>Debit</th><th>Credit</th></tr>";
        //content

        foreach (var objDetail in data.JournalDtlVms)
        {
            string drAmount = (objDetail.AmountDr != 0) ? objDetail.AmountDr.ToString("N2") : "";
            string crAmount = (objDetail.AmountCr != 0) ? objDetail.AmountCr.ToString("N2") : "";

            fullHtml += "<tr style='height:28px;'>";
            if (objDetail.AmountDr > 0)
                fullHtml += $@"<td>{objDetail.LedgerCode} </td><td>{objDetail.LedgerName} </td><td style='text-align:right;'>{drAmount} </td><td></td>";
            else
                fullHtml += $@"<td>{objDetail.LedgerCode} </td><td>{objDetail.LedgerName} </td><td></td><td style='text-align:right;'>{crAmount} </td>";
            fullHtml += "</tr>";
        }

        //total
        double totalAmount = data.JournalDtlVms.Sum(o => o.AmountDr);
        fullHtml += $@"<tr style='height:28px;font-weight:bold;'><td colspan='2' style='text-align:right;'> Total : </td><td style='text-align:right;'>{totalAmount.ToString("N2")}</td><td style='text-align:right;'>{totalAmount.ToString("N2")}</td></tr>";

        fullHtml += "</table></td></tr>";



        if (data.CurrencyId == 1)//taka
            fullHtml += $"<tr style='height:28px;'><td colspan='6'><strong>In Word:</strong> {InWord.ConvertToWordTaka(Convert.ToDouble(totalAmount))} </td></tr>";
        else
            fullHtml += $"<tr style='height:28px;'><td colspan='6'><strong>In Word:</strong> {InWord.ConvertToWordDollar(Convert.ToDouble(totalAmount))} </td></tr>";

        fullHtml += $"<tr style='height:28px;'><td colspan='6'><strong>Narration :</strong> {data.Narration} </td></tr>";


        fullHtml += "</table>";
        fullHtml += "</div>";



        return fullHtml;
    }

    #endregion
}
