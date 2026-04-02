using System.Web;
using AutoMapper;
using DocumentFormat.OpenXml.Wordprocessing;
using Domain.Entities.Admin;
using Domain.Utility.Common;
using Domain.ViewModel.Accounting.AccTranMst;
using Interface.Repository.Accounts;
using Interface.Repository.Admin;
using Persistence.DapperModel;
using DU = Domain.Utility;

namespace Repository.Accounts;

public class AccountReportRepository : IAccountReportRepository
{
    #region Config
    private readonly IMapper _iMapper;
    private readonly IApplicationReadDbConnection _iReadDbConnection;
    private readonly IAccTranMstRepository _AccTranRepository;
    private readonly IAccHeadRepository _iAccHeadRepository;
    private readonly IAccLedgerRepository _iAccLedgerRepository;
    private readonly ISetFincYearRepository _SetFincYearRepository;

    public AccountReportRepository(IMapper iMapper, IApplicationReadDbConnection iReadDbConnection, IAccTranMstRepository accTranRepository, ISetFincYearRepository setFincYearRepository, IAccHeadRepository iAccHeadRepository, IAccLedgerRepository iAccLedgerRepository)
    {
        _iMapper = iMapper;
        _iReadDbConnection = iReadDbConnection;
        _AccTranRepository = accTranRepository;
        _SetFincYearRepository = setFincYearRepository;
        _iAccHeadRepository = iAccHeadRepository;
        _iAccLedgerRepository = iAccLedgerRepository;
    }
    #endregion

    #region GetTrailBalanceData

    public async Task<List<AccTrialBalanceVm>> GetTrailBalanceData(AccReportVm vm)
    {
        vm.StrFromDate = (string.IsNullOrEmpty(vm.StrFromDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrFromDate;
        vm.StrToDate = (string.IsNullOrEmpty(vm.StrFromDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrToDate;
        var fromDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrFromDate)).ToString("dd/MMM/yyyy");
        var toDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrToDate)).ToString("dd/MMM/yyyy");
        string acccountIdFilter = (vm.MishukLedgerId > 0) ? $" and tm.AccAccountId = {vm.MishukLedgerId}" : "";

        string fromDateFilter = $" and convert(date,tm.VcDate) >= '{fromDate}'";
        string toDateFilter = $" and convert(date,tm.VcDate) <= '{toDate}'";
        string finYearFilter = (vm.FinYearId > 0) ? $" and tm.FinYearId = {vm.FinYearId}" : "";

        string query = $@"select f.LedgerId,f.LedgerName,f.OpDr,f.OpCr,f.AmountDr,f.AmountCr,f.BalanceDr,f.BalanceCr,d.LedgerCode
            ,d.HeadCode,d.HeadId,d.HeadName,d.HeadCode2,d.HeadId2,d.HeadName2,d.HeadCode3,d.HeadId3,d.HeadName3,d.HeadCode4,d.HeadId4,d.HeadName4
            ,d.HeadCode5,d.HeadId5,d.HeadName5,d.HeadCode6,d.HeadId6,d.HeadName6,d.HeadCode7,d.HeadId7,d.HeadName7
            from(
            select LedgerId,LedgerName
            ,case when sum(OpDr) - sum(OpCr) > 0 then sum(OpDr) - sum(OpCr) else 0 end OpDr
            ,case when sum(OpCr) - sum(OpDr) > 0 then sum(OpCr) - sum(OpDr) else 0 end OpCr
            ,sum(AmountDr) AmountDr,sum(AmountCr) AmountCr
            ,case when sum(OpDr) + sum(AmountDr) - sum(OpCr)-sum(AmountCr) > 0 then sum(OpDr) + sum(AmountDr) - sum(OpCr)-sum(AmountCr) else 0 end BalanceDr
            ,case when sum(OpCr) + sum(AmountCr) - sum(OpDr)-sum(AmountDr) > 0 then sum(OpCr) + sum(AmountCr) - sum(OpDr)-sum(AmountDr) else 0 end BalanceCr
            from (
            select td.LedgerDrId LedgerId,al.LedgerName,isnull(td.AmountDr,0) OpDr,0 OpCr,0 AmountDr,0 AmountCr
            from AccTranDtls td
            inner join AccTranMsts tm on tm.id = td.TranMstId
            inner join AccLedgers al on al.Id = td.LedgerDrId
            where tm.VcType = 'O' {fromDateFilter} {toDateFilter} {acccountIdFilter} 
            union all
            select td.LedgerCrId LedgerId,al.LedgerName,0 OpDr,isnull(td.AmountCr,0) OpCr,0 AmountDr,0 AmountCr
            from AccTranDtls td
            inner join AccTranMsts tm on tm.id = td.TranMstId
            inner join AccLedgers al on al.Id = td.LedgerCrId
            where tm.VcType = 'O' {fromDateFilter} {toDateFilter} {acccountIdFilter} 
            union all
            select td.LedgerDrId LedgerId,al.LedgerName,isnull(td.AmountDr,0) OpDr,0 OpCr,0 AmountDr,0 AmountCr
            from AccTranDtls td
            inner join AccTranMsts tm on tm.id = td.TranMstId
            inner join AccLedgers al on al.Id = td.LedgerDrId
            where tm.VcType != 'O' and convert(date,tm.VcDate) < '{fromDate}' {finYearFilter} {acccountIdFilter} 
            union all
            select td.LedgerCrId LedgerId,al.LedgerName,0 OpDr,isnull(td.AmountCr,0) OpCr,0 AmountDr,0 AmountCr
            from AccTranDtls td
            inner join AccTranMsts tm on tm.id = td.TranMstId
            inner join AccLedgers al on al.Id = td.LedgerCrId
            where tm.VcType != 'O' and convert(date,tm.VcDate) < '{fromDate}' {finYearFilter} {acccountIdFilter} 
            /*dr*/
            union all
            select td.LedgerDrId LedgerId,al.LedgerName,0 OpDr,0 OpCr,sum(isnull(td.AmountDr,0)) AmountDr,0 AmountCr
            from AccTranDtls td
            inner join AccTranMsts tm on tm.id = td.TranMstId
            inner join AccLedgers al on al.Id = td.LedgerDrId
            inner join AccLedgers alc on alc.Id = td.LedgerCrId
            where tm.VcType != 'O' and convert(date,tm.VcDate) >= '{fromDate}' and convert(date,tm.VcDate) <= '{toDate}'  {acccountIdFilter} 
            group by td.LedgerDrId,al.LedgerName
            /*cr*/
            union all
            select td.LedgerCrId LedgerId,al.LedgerName,0 OpDr,0 OpCr,0 AmountDr,sum(isnull(td.AmountCr,0)) AmountCr
            from AccTranDtls td
            inner join AccTranMsts tm on tm.id = td.TranMstId
            inner join AccLedgers al on al.Id = td.LedgerCrId
            inner join AccLedgers alc on alc.Id = td.LedgerDrId
            where tm.VcType != 'O' and convert(date,tm.VcDate) >= '{fromDate}' and convert(date,tm.VcDate) <= '{toDate}'  {acccountIdFilter} 
            group by td.LedgerCrId,al.LedgerName
            )d group by LedgerId,LedgerName
            )f
            inner join 
            (
            select al.Id LedgerId,al.LedgerCode,al.LedgerName ,h.HeadCode,h.Id HeadId,h.HeadName HeadName,h.ParentHeadId,h.LevelId
            ,h2.HeadCode HeadCode2,h2.Id HeadId2,h2.HeadName HeadName2,h3.HeadCode HeadCode3,h3.Id HeadId3,h3.HeadName HeadName3
            ,h4.HeadCode HeadCode4,h4.Id HeadId4,h4.HeadName HeadName4,h5.HeadCode HeadCode5,h5.Id HeadId5,h5.HeadName HeadName5
            ,h6.HeadCode HeadCode6,h6.Id HeadId6,h6.HeadName HeadName6,h7.HeadCode HeadCode7,h7.Id HeadId7,h7.HeadName HeadName7
            from AccHeads h
            left join AccHeads h2 on h2.ParentHeadId = h.Id and h2.LevelId = 2
            left join AccHeads h3 on h3.ParentHeadId = h2.Id and h3.LevelId = 3
            left join AccHeads h4 on h4.ParentHeadId = h3.Id and h4.LevelId = 4
            left join AccHeads h5 on h5.ParentHeadId = h4.Id and h5.LevelId = 5
            left join AccHeads h6 on h6.ParentHeadId = h5.Id and h6.LevelId = 6
            left join AccHeads h7 on h7.ParentHeadId = h6.Id and h7.LevelId = 7
            left join AccLedgers al on al.HeadId = h.Id or al.HeadId = h2.Id or al.HeadId = h3.Id or al.HeadId = h4.Id or al.HeadId = h5.Id 
            or al.HeadId = h6.Id or al.HeadId = h7.Id where h.LevelId = 1) d on d.LedgerId = f.LedgerId
            order by d.HeadId,d.HeadId2,d.HeadId3,d.HeadId4,d.HeadId5,d.HeadId6,d.HeadId7,d.LedgerId";

        var data = await _iReadDbConnection.QueryAsync<AccTrialBalanceVm>(query);
        return data.ToList();


    }

    #endregion

    #region GetTrialBalanceHtml

    public async Task<string> GetTrialBalanceHtml(AccReportVm vm)
    {
        string fullHtml = "";
        var data = await GetTrailBalanceData(vm);

        var mishukHead = _iAccHeadRepository.GetFirstOrDefault(x => x.HeadCode == AccHeadCode.HotelMisukHead);
        if (mishukHead == null)
            throw new Exception("No Mishuk Head Found !!");

        var mishulLagederList = await _iAccLedgerRepository.GetAsync(x => x.HeadId == mishukHead.Id);
        string mishulLageder = vm?.MishukLedgerId > 0 ? mishulLagederList.FirstOrDefault(x => x.Id == (long)vm?.MishukLedgerId).LedgerName : "All";

        if (data != null && data.Count > 0)
        {
            fullHtml += "<table class='table table-striped table-bordered md-font-table-2' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
            fullHtml += "<thead>";

            fullHtml += "<tr style='height:30px;'>";
            fullHtml += $@"<th colspan='8' style='text-align:center;'>{mishulLageder}</th>";
            fullHtml += "</tr>";

            fullHtml += "<tr style='height:30px;'>";
            fullHtml += "<th rowspan='2'>Code</th><th rowspan='2' style='width:250px;'>Name</th><th colspan='2'>Opening</th><th colspan='2'>Current</th><th colspan='2'>Balance</th>";
            fullHtml += "</tr>";
            fullHtml += "<tr style='height:30px;'>";
            fullHtml += "<th>Debit</th><th>Credit</th><th>Debit</th><th>Credit</th><th>Debit</th><th>Credit</th>";
            fullHtml += "</tr>";
            fullHtml += "</thead>";
            fullHtml += "<tbody>";

            long preHeadId = 0;
            long preHeadId2 = 0;
            long preHeadId3 = 0;
            long preHeadId4 = 0;
            long preHeadId5 = 0;
            long preHeadId6 = 0;
            long preHeadId7 = 0;

            for (int i = 0; i < data.Count; i++)
            {
                AccTrialBalanceVm objTrail = data[i];

                long rawHeadId = (objTrail.HeadId > 0) ? Convert.ToInt64(objTrail.HeadId) : 0;
                string head = objTrail.HeadName;
                string headCode = objTrail.HeadCode;

                long rawHeadId2 = (objTrail.HeadId2 > 0) ? Convert.ToInt64(objTrail.HeadId2) : 0;
                string head2 = objTrail.HeadName2;
                string headCode2 = objTrail.HeadCode2;

                long rawHeadId3 = (objTrail.HeadId3 > 0) ? Convert.ToInt64(objTrail.HeadId3) : 0;
                string head3 = objTrail.HeadName3;
                string headCode3 = objTrail.HeadCode3;

                long rawHeadId4 = (objTrail.HeadId4 > 0) ? Convert.ToInt64(objTrail.HeadId4) : 0;
                string head4 = objTrail.HeadName4;
                string headCode4 = objTrail.HeadCode4;

                long rawHeadId5 = (objTrail.HeadId5 > 0) ? Convert.ToInt64(objTrail.HeadId5) : 0;
                string head5 = objTrail.HeadName5;
                string headCode5 = objTrail.HeadCode5;

                long rawHeadId6 = (objTrail.HeadId6 > 0) ? Convert.ToInt64(objTrail.HeadId6) : 0;
                string head6 = objTrail.HeadName6;
                string headCode6 = objTrail.HeadCode6;

                long rawHeadId7 = (objTrail.HeadId7 > 0) ? Convert.ToInt64(objTrail.HeadId7) : 0;
                string head7 = objTrail.HeadName7;
                string headCode7 = objTrail.HeadCode7;

                decimal balanceDrDeci = (!string.IsNullOrEmpty(objTrail.BalanceDr.ToString())) ? Convert.ToDecimal(objTrail.BalanceDr) : 0;
                string balanceDrStr = (balanceDrDeci != 0) ? balanceDrDeci.ToString("N2") : "";

                decimal balanceCrDeci = (!string.IsNullOrEmpty(objTrail.BalanceCr.ToString())) ? Convert.ToDecimal(objTrail.BalanceCr) : 0;
                string balanceCrStr = (balanceCrDeci != 0) ? balanceCrDeci.ToString("N2") : "";


                fullHtml = (preHeadId != rawHeadId) ? addHeadNewSubTotal(fullHtml, data, headLevel: 1, headId: rawHeadId, headId2: rawHeadId2, headId3: rawHeadId3, headId4: rawHeadId4, headId5: rawHeadId5, headId6: rawHeadId6, headId7: rawHeadId7, headCode: headCode, head: head) : fullHtml;
                fullHtml = (preHeadId2 != rawHeadId2) ? addHeadNewSubTotal(fullHtml, data, headLevel: 2, headId: rawHeadId, headId2: rawHeadId2, headId3: rawHeadId3, headId4: rawHeadId4, headId5: rawHeadId5, headId6: rawHeadId6, headId7: rawHeadId7, headCode: headCode2, head: head2) : fullHtml;
                fullHtml = (preHeadId3 != rawHeadId3) ? addHeadNewSubTotal(fullHtml, data, headLevel: 3, headId: rawHeadId, headId2: rawHeadId2, headId3: rawHeadId3, headId4: rawHeadId4, headId5: rawHeadId5, headId6: rawHeadId6, headId7: rawHeadId7, headCode: headCode3, head: head3) : fullHtml;
                fullHtml = (preHeadId4 != rawHeadId4) ? addHeadNewSubTotal(fullHtml, data, headLevel: 4, headId: rawHeadId, headId2: rawHeadId2, headId3: rawHeadId3, headId4: rawHeadId4, headId5: rawHeadId5, headId6: rawHeadId6, headId7: rawHeadId7, headCode: headCode4, head: head4) : fullHtml;
                fullHtml = (preHeadId5 != rawHeadId5) ? addHeadNewSubTotal(fullHtml, data, headLevel: 5, headId: rawHeadId, headId2: rawHeadId2, headId3: rawHeadId3, headId4: rawHeadId4, headId5: rawHeadId5, headId6: rawHeadId6, headId7: rawHeadId7, headCode: headCode5, head: head5) : fullHtml;
                fullHtml = (preHeadId6 != rawHeadId6) ? addHeadNewSubTotal(fullHtml, data, headLevel: 6, headId: rawHeadId, headId2: rawHeadId2, headId3: rawHeadId3, headId4: rawHeadId4, headId5: rawHeadId5, headId6: rawHeadId6, headId7: rawHeadId7, headCode: headCode6, head: head6) : fullHtml;
                fullHtml = (preHeadId7 != rawHeadId7) ? addHeadNewSubTotal(fullHtml, data, headLevel: 7, headId: rawHeadId, headId2: rawHeadId2, headId3: rawHeadId3, headId4: rawHeadId4, headId5: rawHeadId5, headId6: rawHeadId6, headId7: rawHeadId7, headCode: headCode7, head: head7) : fullHtml;


                fullHtml += $@"<tr style='height:30px;'>
                    <td style='text-align:left;'>{objTrail.LedgerCode}</td><td style='text-align:left;'>{objTrail.LedgerName}</td>
                    <td style='text-align:right;'>{objTrail.OpDr.ToString("N2")}</td><td style='text-align:right;'>{objTrail.OpCr.ToString("N2")}</td>
                    <td style='text-align:right;'>{objTrail.AmountDr.ToString("N2")}</td><td style='text-align:right;'>{objTrail.AmountCr.ToString("N2")}</td>
                    <td style='text-align:right;'>{balanceDrStr}</td><td style='text-align:right;'>{balanceCrStr}</td>
                    </tr>";


                preHeadId = rawHeadId;
                preHeadId2 = rawHeadId2;
                preHeadId3 = rawHeadId3;
                preHeadId4 = rawHeadId4;
                preHeadId5 = rawHeadId5;
                preHeadId6 = rawHeadId6;
                preHeadId7 = rawHeadId7;

            }

            fullHtml += $@"<tr style='font-weight:bold;height:30px;'>
                <td colspan='2' style='text-align:right;'>Grand Total :</td>
                <td style='text-align:right;'>{data.Sum(o => o.OpDr).ToString("N2")}</td><td style='text-align:right;'>{data.Sum(o => o.OpCr).ToString("N2")}</td>
                <td style='text-align:right;'>{data.Sum(o => o.AmountDr).ToString("N2")}</td><td style='text-align:right;'>{data.Sum(o => o.AmountCr).ToString("N2")}</td>
                <td style='text-align:right;'>{data.Sum(o => o.BalanceDr).ToString("N2")}</td><td style='text-align:right;'>{data.Sum(o => o.BalanceCr).ToString("N2")}</td>
                </tr></tbody></table>";
        }
        return fullHtml;
    }

    #endregion

    #region GetHeadWiseBalanceData

    public async Task<List<AccTrialBalanceVm>> GetHeadWiseBalanceData(AccReportVm vm)
    {
        vm.StrFromDate = (string.IsNullOrEmpty(vm.StrFromDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrFromDate;
        vm.StrToDate = (string.IsNullOrEmpty(vm.StrFromDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrToDate;
        var fromDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrFromDate)).ToString("dd/MMM/yyyy");
        var toDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrToDate)).ToString("dd/MMM/yyyy");
        string headFilter = (vm.HeadId > 0) ? $" and ah.id = {vm.HeadId}" : "";
        string acccountIdFilter = (vm.MishukLedgerId > 0) ? $" and tm.AccAccountId = {vm.MishukLedgerId}" : "";

        string fromDateFilter = $" and convert(date,tm.VcDate) >= '{fromDate}'";
        string toDateFilter = $" and convert(date,tm.VcDate) <= '{toDate}'";
        string finYearFilter = (vm.FinYearId > 0) ? $" and tm.FinYearId = {vm.FinYearId}" : "";

        #region Old_Query
        //     string query = $@"select f.LedgerId,f.LedgerName,f.OpDr,f.OpCr,f.AmountDr,f.AmountCr,f.BalanceDr,f.BalanceCr,d.LedgerCode
        //         ,d.HeadCode,d.HeadId,d.HeadName,d.HeadCode2,d.HeadId2,d.HeadName2,d.HeadCode3,d.HeadId3,d.HeadName3,d.HeadCode4,d.HeadId4,d.HeadName4
        //         ,d.HeadCode5,d.HeadId5,d.HeadName5,d.HeadCode6,d.HeadId6,d.HeadName6,d.HeadCode7,d.HeadId7,d.HeadName7
        //         from(
        //         select LedgerId,LedgerName
        //         ,case when sum(OpDr) - sum(OpCr) > 0 then sum(OpDr) - sum(OpCr) else 0 end OpDr
        //         ,case when sum(OpCr) - sum(OpDr) > 0 then sum(OpCr) - sum(OpDr) else 0 end OpCr
        //         ,sum(AmountDr) AmountDr,sum(AmountCr) AmountCr
        //         ,case when sum(OpDr) + sum(AmountDr) - sum(OpCr)-sum(AmountCr) > 0 then sum(OpDr) + sum(AmountDr) - sum(OpCr)-sum(AmountCr) else 0 end BalanceDr
        //         ,case when sum(OpCr) + sum(AmountCr) - sum(OpDr)-sum(AmountDr) > 0 then sum(OpCr) + sum(AmountCr) - sum(OpDr)-sum(AmountDr) else 0 end BalanceCr
        //         from (
        //         select td.LedgerDrId LedgerId,al.LedgerName,isnull(td.AmountDr,0) OpDr,0 OpCr,0 AmountDr,0 AmountCr
        //         from AccTranDtls td
        //         inner join AccTranMsts tm on tm.id = td.TranMstId
        //         inner join AccLedgers al on al.Id = td.LedgerDrId
        //         inner join AccHeads ah on ah.Id = al.HeadId
        //         where tm.VcType = 'O' {fromDateFilter} {toDateFilter} {headFilter} {acccountIdFilter} 
        //         union all
        //         select td.LedgerDrId LedgerId,al.LedgerName,0 OpDr,isnull(td.AmountDr,0) OpCr,0 AmountDr,0 AmountCr
        //         from AccTranDtls td
        //         inner join AccTranMsts tm on tm.id = td.TranMstId
        //         inner join AccLedgers al on al.Id = td.LedgerCrId
        //         inner join AccHeads ah on ah.Id = al.HeadId
        //         where tm.VcType = 'O' {fromDateFilter} {toDateFilter} {headFilter} {acccountIdFilter} 
        //         union all
        //         select td.LedgerDrId LedgerId,al.LedgerName,isnull(td.AmountDr,0) OpDr,0 OpCr,0 AmountDr,0 AmountCr
        //         from AccTranDtls td
        //         inner join AccTranMsts tm on tm.id = td.TranMstId
        //         inner join AccLedgers al on al.Id = td.LedgerDrId
        //         inner join AccHeads ah on ah.Id = al.HeadId
        //         where tm.VcType != 'O' and convert(date,tm.VcDate) < '{fromDate}' {finYearFilter} {headFilter} {acccountIdFilter} 
        //         union all
        //         select td.LedgerDrId LedgerId,al.LedgerName,0 OpDr,isnull(td.AmountDr,0) OpCr,0 AmountDr,0 AmountCr
        //         from AccTranDtls td
        //         inner join AccTranMsts tm on tm.id = td.TranMstId
        //         inner join AccLedgers al on al.Id = td.LedgerCrId
        //         inner join AccHeads ah on ah.Id = al.HeadId
        //         where tm.VcType != 'O' and convert(date,tm.VcDate) < '{fromDate}' {finYearFilter} {headFilter}
        //         /*dr*/
        //         union all
        //         select td.LedgerDrId LedgerId,al.LedgerName,0 OpDr,0 OpCr,sum(isnull(td.AmountDr,0)) AmountDr,0 AmountCr
        //         from AccTranDtls td
        //         inner join AccTranMsts tm on tm.id = td.TranMstId
        //         inner join AccLedgers al on al.Id = td.LedgerDrId
        //         inner join AccLedgers alc on alc.Id = td.LedgerCrId
        //         inner join AccHeads ah on ah.Id = al.HeadId
        //         where tm.VcType != 'O' and convert(date,tm.VcDate) >= '{fromDate}' and convert(date,tm.VcDate) <= '{toDate}' {headFilter} {acccountIdFilter} 
        //         group by td.LedgerDrId,al.LedgerName
        //         /*cr*/
        //         union all
        //         select td.LedgerCrId LedgerId,al.LedgerName,0 OpDr,0 OpCr,0 AmountDr,sum(isnull(td.AmountDr,0)) AmountCr
        //         from AccTranDtls td
        //         inner join AccTranMsts tm on tm.id = td.TranMstId
        //         inner join AccLedgers al on al.Id = td.LedgerCrId
        //         inner join AccLedgers alc on alc.Id = td.LedgerDrId
        //         inner join AccHeads ah on ah.Id = al.HeadId
        //         where tm.VcType != 'O' and convert(date,tm.VcDate) >= '{fromDate}' and convert(date,tm.VcDate) <= '{toDate}' {headFilter} {acccountIdFilter} 
        //         group by td.LedgerCrId,al.LedgerName
        //         )d group by LedgerId,LedgerName
        //         )f
        //         inner join 
        //         (
        //         select al.Id LedgerId,al.LedgerCode,al.LedgerName ,h.HeadCode,h.Id HeadId,h.HeadName HeadName,h.ParentHeadId,h.LevelId
        //,h2.HeadCode HeadCode2,h2.Id HeadId2,h2.HeadName HeadName2,h3.HeadCode HeadCode3,h3.Id HeadId3,h3.HeadName HeadName3
        //,h4.HeadCode HeadCode4,h4.Id HeadId4,h4.HeadName HeadName4,h5.HeadCode HeadCode5,h5.Id HeadId5,h5.HeadName HeadName5
        //,h6.HeadCode HeadCode6,h6.Id HeadId6,h6.HeadName HeadName6,h7.HeadCode HeadCode7,h7.Id HeadId7,h7.HeadName HeadName7
        //from AccLedgers al
        //inner join AccHeads h on h.Id = al.HeadId
        //left join AccHeads h2 on h2.Id = h.ParentHeadId 
        //left join AccHeads h3 on h3.Id = h2.ParentHeadId
        //left join AccHeads h4 on h4.Id = h3.ParentHeadId
        //left join AccHeads h5 on h5.Id = h4.ParentHeadId
        //left join AccHeads h6 on h6.Id = h5.ParentHeadId
        //left join AccHeads h7 on h7.Id = h6.ParentHeadId) d on d.LedgerId = f.LedgerId
        //         order by d.HeadId,d.HeadId2,d.HeadId3,d.HeadId4,d.HeadId5,d.HeadId6,d.HeadId7,d.LedgerId";
        #endregion

        string query = $@"
                DECLARE @HeadCode VARCHAR(50);
                SELECT @HeadCode = HeadCode FROM AccHeads WHERE Id = {vm.HeadId};

                select f.LedgerId,f.LedgerName,f.OpDr,f.OpCr,f.AmountDr,f.AmountCr,f.BalanceDr,f.BalanceCr,d.LedgerCode
                ,d.HeadCode,d.HeadId,d.HeadName,d.HeadCode2,d.HeadId2,d.HeadName2,d.HeadCode3,d.HeadId3,d.HeadName3,d.HeadCode4,d.HeadId4,d.HeadName4
                ,d.HeadCode5,d.HeadId5,d.HeadName5,d.HeadCode6,d.HeadId6,d.HeadName6,d.HeadCode7,d.HeadId7,d.HeadName7
                from(
                select LedgerId,LedgerName
                ,case when sum(OpDr) - sum(OpCr) > 0 then sum(OpDr) - sum(OpCr) else 0 end OpDr
                ,case when sum(OpCr) - sum(OpDr) > 0 then sum(OpCr) - sum(OpDr) else 0 end OpCr
                ,sum(AmountDr) AmountDr,sum(AmountCr) AmountCr
                ,case when sum(OpDr) + sum(AmountDr) - sum(OpCr)-sum(AmountCr) > 0 then sum(OpDr) + sum(AmountDr) - sum(OpCr)-sum(AmountCr) else 0 end BalanceDr
                ,case when sum(OpCr) + sum(AmountCr) - sum(OpDr)-sum(AmountDr) > 0 then sum(OpCr) + sum(AmountCr) - sum(OpDr)-sum(AmountDr) else 0 end BalanceCr
                from (
                select td.LedgerDrId LedgerId,al.LedgerName,isnull(td.AmountDr,0) OpDr,0 OpCr,0 AmountDr,0 AmountCr
                from AccTranDtls td
                inner join AccTranMsts tm on tm.id = td.TranMstId
                inner join AccLedgers al on al.Id = td.LedgerDrId
                inner join AccHeads ah on ah.Id = al.HeadId
                where tm.VcType = 'O' {fromDateFilter} {toDateFilter} {acccountIdFilter}  
                and al.Id in (
                select l.Id from AccLedgers l
                inner join AccHeads h on h.Id = l.HeadId
                where h.HeadCode = @HeadCode or h.HeadCode like @HeadCode+'.%'
                ) 
                and tm.AccAccountId = 68 
                union all
                select td.LedgerDrId LedgerId,al.LedgerName,0 OpDr,isnull(td.AmountDr,0) OpCr,0 AmountDr,0 AmountCr
                from AccTranDtls td
                inner join AccTranMsts tm on tm.id = td.TranMstId
                inner join AccLedgers al on al.Id = td.LedgerCrId
                inner join AccHeads ah on ah.Id = al.HeadId
                where tm.VcType = 'O' {fromDateFilter} {toDateFilter} {acccountIdFilter}  
                and al.Id in (
                select l.Id from AccLedgers l
                inner join AccHeads h on h.Id = l.HeadId
                where h.HeadCode = @HeadCode or h.HeadCode like @HeadCode+'.%'
                ) 
                and tm.AccAccountId = 68 
                union all
                select td.LedgerDrId LedgerId,al.LedgerName,isnull(td.AmountDr,0) OpDr,0 OpCr,0 AmountDr,0 AmountCr
                from AccTranDtls td
                inner join AccTranMsts tm on tm.id = td.TranMstId
                inner join AccLedgers al on al.Id = td.LedgerDrId
                inner join AccHeads ah on ah.Id = al.HeadId
                where tm.VcType != 'O' {fromDateFilter} {toDateFilter} {finYearFilter} {acccountIdFilter}  
                and al.Id in (
                select l.Id from AccLedgers l
                inner join AccHeads h on h.Id = l.HeadId
                where h.HeadCode = @HeadCode or h.HeadCode like @HeadCode+'.%'
                ) 
                and tm.AccAccountId = 68 
                union all
                select td.LedgerDrId LedgerId,al.LedgerName,0 OpDr,isnull(td.AmountDr,0) OpCr,0 AmountDr,0 AmountCr
                from AccTranDtls td
                inner join AccTranMsts tm on tm.id = td.TranMstId
                inner join AccLedgers al on al.Id = td.LedgerCrId
                inner join AccHeads ah on ah.Id = al.HeadId
                where tm.VcType != 'O' and convert(date,tm.VcDate) < '{fromDate}' {finYearFilter}
                and al.Id in (
                select l.Id from AccLedgers l
                inner join AccHeads h on h.Id = l.HeadId
                where h.HeadCode = @HeadCode or h.HeadCode like @HeadCode+'.%'
                ) 
                --/dr/
                union all
                select td.LedgerDrId LedgerId,al.LedgerName,0 OpDr,0 OpCr,sum(isnull(td.AmountDr,0)) AmountDr,0 AmountCr
                from AccTranDtls td
                inner join AccTranMsts tm on tm.id = td.TranMstId
                inner join AccLedgers al on al.Id = td.LedgerDrId
                inner join AccLedgers alc on alc.Id = td.LedgerCrId
                inner join AccHeads ah on ah.Id = al.HeadId
                where tm.VcType != 'O' and convert(date,tm.VcDate) < '{fromDate}' {finYearFilter} {headFilter} 
                and al.Id in (
                select l.Id from AccLedgers l
                inner join AccHeads h on h.Id = l.HeadId
                where h.HeadCode = @HeadCode or h.HeadCode like @HeadCode+'.%'
                ) 
                and tm.AccAccountId = 68 
                group by td.LedgerDrId,al.LedgerName
                --/cr/
                union all
                select td.LedgerCrId LedgerId,al.LedgerName,0 OpDr,0 OpCr,0 AmountDr,sum(isnull(td.AmountDr,0)) AmountCr
                from AccTranDtls td
                inner join AccTranMsts tm on tm.id = td.TranMstId
                inner join AccLedgers al on al.Id = td.LedgerCrId
                inner join AccLedgers alc on alc.Id = td.LedgerDrId
                inner join AccHeads ah on ah.Id = al.HeadId
                where tm.VcType != 'O' and convert(date,tm.VcDate) >= '{fromDate}' and convert(date,tm.VcDate) <= '{toDate}' {acccountIdFilter} 
                and al.Id in (
                select l.Id from AccLedgers l
                inner join AccHeads h on h.Id = l.HeadId
                where h.HeadCode = @HeadCode or h.HeadCode like @HeadCode+'.%'
                )  
                and tm.AccAccountId = 68 
                group by td.LedgerCrId,al.LedgerName
                )d group by LedgerId,LedgerName
                )f
                inner join 
                (
                select al.Id LedgerId,al.LedgerCode,al.LedgerName ,h.HeadCode,h.Id HeadId,h.HeadName HeadName,h.ParentHeadId,h.LevelId
                ,h2.HeadCode HeadCode2,h2.Id HeadId2,h2.HeadName HeadName2,h3.HeadCode HeadCode3,h3.Id HeadId3,h3.HeadName HeadName3
                ,h4.HeadCode HeadCode4,h4.Id HeadId4,h4.HeadName HeadName4,h5.HeadCode HeadCode5,h5.Id HeadId5,h5.HeadName HeadName5
                ,h6.HeadCode HeadCode6,h6.Id HeadId6,h6.HeadName HeadName6,h7.HeadCode HeadCode7,h7.Id HeadId7,h7.HeadName HeadName7
                from AccLedgers al
                inner join AccHeads h on h.Id = al.HeadId
                left join AccHeads h2 on h2.Id = h.ParentHeadId 
                left join AccHeads h3 on h3.Id = h2.ParentHeadId
                left join AccHeads h4 on h4.Id = h3.ParentHeadId
                left join AccHeads h5 on h5.Id = h4.ParentHeadId
                left join AccHeads h6 on h6.Id = h5.ParentHeadId
                left join AccHeads h7 on h7.Id = h6.ParentHeadId) d on d.LedgerId = f.LedgerId
                order by d.HeadId,d.HeadId2,d.HeadId3,d.HeadId4,d.HeadId5,d.HeadId6,d.HeadId7,d.LedgerId
                ";

        var data = await _iReadDbConnection.QueryAsync<AccTrialBalanceVm>(query);
        return data.ToList();


    }

    #endregion

    #region GetHeadWiseBalanceHtml

    public async Task<string> GetHeadWiseBalanceHtml(AccReportVm vm)
    {
        string fullHtml = "";
        var data = await GetHeadWiseBalanceData(vm);

        var mishukHead = _iAccHeadRepository.GetFirstOrDefault(x => x.Id == vm.HeadId);
        if (mishukHead == null)
            throw new Exception("No Mishuk Head Found !!");

        if (data != null && data.Count > 0)
        {
            fullHtml += "<table class='table table-striped table-bordered md-font-table-2' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
            fullHtml += "<thead>";

            fullHtml += "<tr style='height:30px;'>";
            fullHtml += $@"<th colspan='8' style='text-align:center;'>{mishukHead.HeadName}</th>";
            fullHtml += "</tr>";


            fullHtml += "<tr style='height:30px;'>";
            fullHtml += "<th rowspan='2'>Code</th><th rowspan='2' style='width:250px;'>Name</th><th colspan='2'>Opening</th><th colspan='2'>Current</th><th colspan='2'>Balance</th>";
            fullHtml += "</tr>";
            fullHtml += "<tr style='height:30px;'>";
            fullHtml += "<th>Debit</th><th>Credit</th><th>Debit</th><th>Credit</th><th>Debit</th><th>Credit</th>";
            fullHtml += "</tr>";
            fullHtml += "</thead>";
            fullHtml += "<tbody>";

            for (int i = 0; i < data.Count; i++)
            {
                AccTrialBalanceVm objTrail = data[i];

                decimal balanceDrDeci = (!string.IsNullOrEmpty(objTrail.BalanceDr.ToString())) ? Convert.ToDecimal(objTrail.BalanceDr) : 0;
                string balanceDrStr = (balanceDrDeci != 0) ? balanceDrDeci.ToString("N2") : "";

                decimal balanceCrDeci = (!string.IsNullOrEmpty(objTrail.BalanceCr.ToString())) ? Convert.ToDecimal(objTrail.BalanceCr) : 0;
                string balanceCrStr = (balanceCrDeci != 0) ? balanceCrDeci.ToString("N2") : "";

                fullHtml += $@"<tr style='height:30px;'>
                    <td style='text-align:left;'>{objTrail.LedgerCode}</td><td style='text-align:left;'>{objTrail.LedgerName}</td>
                    <td style='text-align:right;'>{objTrail.OpDr:N2}</td><td style='text-align:right;'>{objTrail.OpCr:N2}</td>
                    <td style='text-align:right;'>{objTrail.AmountDr:N2}</td><td style='text-align:right;'>{objTrail.AmountCr:N2}</td>
                    <td style='text-align:right;'>{balanceDrStr}</td><td style='text-align:right;'>{balanceCrStr}</td>
                    </tr>";

            }

            fullHtml += $@"<tr style='font-weight:bold;height:30px;'>
                <td colspan='2' style='text-align:right;'>Grand Total :</td>
                <td style='text-align:right;'>{data.Sum(o => o.OpDr):N2}</td><td style='text-align:right;'>{data.Sum(o => o.OpCr):N2}</td>
                <td style='text-align:right;'>{data.Sum(o => o.AmountDr):N2}</td><td style='text-align:right;'>{data.Sum(o => o.AmountCr):N2}</td>
                <td style='text-align:right;'>{data.Sum(o => o.BalanceDr):N2}</td><td style='text-align:right;'>{data.Sum(o => o.BalanceCr):N2}</td>
                </tr></tbody></table>";
        }
        return fullHtml;
    }

    #endregion

    #region addHeadNewSubTotal

    private string addHeadNewSubTotal(string fullHtml, List<AccTrialBalanceVm> data, int headLevel, long headId, long headId2, long headId3, long headId4, long headId5, long headId6, long headId7, string headCode, string head)
    {
        decimal opDrsubTotalAmount = 0;
        decimal opCrsubTotalAmount = 0;
        decimal subTotalDrAmount = 0;
        decimal subTotalCrAmount = 0;
        decimal balanceDr = 0;
        decimal balanceCr = 0;
        try
        {
            switch (headLevel)
            {
                case 1:
                    opDrsubTotalAmount = data.Where(o => o.HeadId == headId).Sum(o => o.OpDr);
                    opCrsubTotalAmount = data.Where(o => o.HeadId == headId).Sum(o => o.OpCr);
                    subTotalDrAmount = data.Where(o => o.HeadId == headId).Sum(o => o.AmountDr);
                    subTotalCrAmount = data.Where(o => o.HeadId == headId).Sum(o => o.AmountCr);
                    balanceDr = data.Where(o => o.HeadId == headId).Sum(o => o.BalanceDr);
                    balanceCr = data.Where(o => o.HeadId == headId).Sum(o => o.BalanceCr);
                    break;
                case 2:
                    opDrsubTotalAmount = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2).Sum(o => o.OpDr);
                    opCrsubTotalAmount = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2).Sum(o => o.OpCr);
                    subTotalDrAmount = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2).Sum(o => o.AmountDr);
                    subTotalCrAmount = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2).Sum(o => o.AmountCr);
                    balanceDr = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2).Sum(o => o.BalanceDr);
                    balanceCr = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2).Sum(o => o.BalanceCr);
                    break;
                case 3:
                    opDrsubTotalAmount = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2 && o.HeadId3 == headId3).Sum(o => o.OpDr);
                    opCrsubTotalAmount = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2 && o.HeadId3 == headId3).Sum(o => o.OpCr);
                    subTotalDrAmount = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2 && o.HeadId3 == headId3).Sum(o => o.AmountDr);
                    subTotalCrAmount = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2 && o.HeadId3 == headId3).Sum(o => o.AmountCr);
                    balanceDr = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2 && o.HeadId3 == headId3).Sum(o => o.BalanceDr);
                    balanceCr = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2 && o.HeadId3 == headId3).Sum(o => o.BalanceCr);
                    break;
                case 4:
                    opDrsubTotalAmount = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2 && o.HeadId3 == headId3 && o.HeadId4 == headId4).Sum(o => o.OpDr);
                    opCrsubTotalAmount = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2 && o.HeadId3 == headId3 && o.HeadId4 == headId4).Sum(o => o.OpCr);
                    subTotalDrAmount = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2 && o.HeadId3 == headId3 && o.HeadId4 == headId4).Sum(o => o.AmountDr);
                    subTotalCrAmount = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2 && o.HeadId3 == headId3 && o.HeadId4 == headId4).Sum(o => o.AmountCr);
                    balanceDr = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2 && o.HeadId3 == headId3 && o.HeadId4 == headId4).Sum(o => o.BalanceDr);
                    balanceCr = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2 && o.HeadId3 == headId3 && o.HeadId4 == headId4).Sum(o => o.BalanceCr);
                    break;
                case 5:
                    opDrsubTotalAmount = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2 && o.HeadId3 == headId3 && o.HeadId4 == headId4 && o.HeadId5 == headId5).Sum(o => o.OpDr);
                    opCrsubTotalAmount = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2 && o.HeadId3 == headId3 && o.HeadId4 == headId4 && o.HeadId5 == headId5).Sum(o => o.OpCr);
                    subTotalDrAmount = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2 && o.HeadId3 == headId3 && o.HeadId4 == headId4 && o.HeadId5 == headId5).Sum(o => o.AmountDr);
                    subTotalCrAmount = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2 && o.HeadId3 == headId3 && o.HeadId4 == headId4 && o.HeadId5 == headId5).Sum(o => o.AmountCr);
                    balanceDr = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2 && o.HeadId3 == headId3 && o.HeadId4 == headId4 && o.HeadId5 == headId5).Sum(o => o.BalanceDr);
                    balanceCr = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2 && o.HeadId3 == headId3 && o.HeadId4 == headId4 && o.HeadId5 == headId5).Sum(o => o.BalanceCr);
                    break;
                case 6:
                    opDrsubTotalAmount = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2 && o.HeadId3 == headId3 && o.HeadId4 == headId4 && o.HeadId5 == headId5 && o.HeadId6 == headId6).Sum(o => o.OpDr);
                    opCrsubTotalAmount = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2 && o.HeadId3 == headId3 && o.HeadId4 == headId4 && o.HeadId5 == headId5 && o.HeadId6 == headId6).Sum(o => o.OpCr);
                    subTotalDrAmount = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2 && o.HeadId3 == headId3 && o.HeadId4 == headId4 && o.HeadId5 == headId5 && o.HeadId6 == headId6).Sum(o => o.AmountDr);
                    subTotalCrAmount = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2 && o.HeadId3 == headId3 && o.HeadId4 == headId4 && o.HeadId5 == headId5 && o.HeadId6 == headId6).Sum(o => o.AmountCr);
                    balanceDr = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2 && o.HeadId3 == headId3 && o.HeadId4 == headId4 && o.HeadId5 == headId5 && o.HeadId6 == headId6).Sum(o => o.BalanceDr);
                    balanceCr = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2 && o.HeadId3 == headId3 && o.HeadId4 == headId4 && o.HeadId5 == headId5 && o.HeadId6 == headId6).Sum(o => o.BalanceCr);
                    break;
                case 7:
                    opDrsubTotalAmount = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2 && o.HeadId3 == headId3 && o.HeadId4 == headId4 && o.HeadId5 == headId5 && o.HeadId6 == headId6 && o.HeadId7 == headId7).Sum(o => o.OpDr);
                    opCrsubTotalAmount = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2 && o.HeadId3 == headId3 && o.HeadId4 == headId4 && o.HeadId5 == headId5 && o.HeadId6 == headId6 && o.HeadId7 == headId7).Sum(o => o.OpCr);
                    subTotalDrAmount = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2 && o.HeadId3 == headId3 && o.HeadId4 == headId4 && o.HeadId5 == headId5 && o.HeadId6 == headId6 && o.HeadId7 == headId7).Sum(o => o.AmountDr);
                    subTotalCrAmount = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2 && o.HeadId3 == headId3 && o.HeadId4 == headId4 && o.HeadId5 == headId5 && o.HeadId6 == headId6 && o.HeadId7 == headId7).Sum(o => o.AmountCr);
                    balanceDr = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2 && o.HeadId3 == headId3 && o.HeadId4 == headId4 && o.HeadId5 == headId5 && o.HeadId6 == headId6 && o.HeadId7 == headId7).Sum(o => o.BalanceDr);
                    balanceCr = data.Where(o => o.HeadId == headId && o.HeadId2 == headId2 && o.HeadId3 == headId3 && o.HeadId4 == headId4 && o.HeadId5 == headId5 && o.HeadId6 == headId6 && o.HeadId7 == headId7).Sum(o => o.BalanceCr);
                    break;
                default:
                    break;
            }
            string opDrHeadSubTotal = (opDrsubTotalAmount != 0) ? opDrsubTotalAmount.ToString("N2") : "";
            string opCrHeadSubTotal = (opCrsubTotalAmount != 0) ? opCrsubTotalAmount.ToString("N2") : "";
            string amountDrHeadSubTotal = (subTotalDrAmount != 0) ? subTotalDrAmount.ToString("N2") : "";
            string amountCrHeadSubTotal = (subTotalCrAmount != 0) ? subTotalCrAmount.ToString("N2") : "";
            string strBalanceDr = (balanceDr > balanceCr) ? (balanceDr - balanceCr).ToString("N2") : "";
            string strBalanceCr = (balanceCr > balanceDr) ? (balanceCr - balanceDr).ToString("N2") : "";


            fullHtml += $@"<tr style='font-weight:bold;height:30px;'>
                <td style='text-align:left;'>{headCode}</td><td style='text-align:left;'>{head}</td>
                <td style='text-align:right;'>{opDrHeadSubTotal}</td><td style='text-align:right;'>{opCrHeadSubTotal}</td>
                <td style='text-align:right;'>{amountDrHeadSubTotal}</td><td style='text-align:right;'>{amountCrHeadSubTotal}</td>
                <td style='text-align:right;'>{strBalanceDr}</td><td style='text-align:right;'>{strBalanceCr}</td>
                </tr>";
        }
        catch (Exception ex)
        {
        }


        return fullHtml;
    }

    #endregion

    #region GetLedgerReportData
    public async Task<List<AccLedgerReportVm>> GetQuickVoucherLedgerReportData(AccReportVm vm)
    {
        vm.StrFromDate = (string.IsNullOrEmpty(vm.StrFromDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrFromDate;
        vm.StrToDate = (string.IsNullOrEmpty(vm.StrFromDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrToDate;
        var fromDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrFromDate)).ToString("dd/MMM/yyyy");
        var toDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrToDate)).ToString("dd/MMM/yyyy");
        SetFincYear finYear = _SetFincYearRepository.GetFirstOrDefault(c => DU.Utility.ConvertStrToDate(vm.StrFromDate) >= c.YearStartDate && DU.Utility.ConvertStrToDate(vm.StrFromDate) <= c.YearEndDate);
        vm.FinYearId = finYear.Id;


        string fromDateFilter = $" and convert(date,tm.VcDate) >= '{fromDate}'";
        string toDateFilter = $" and convert(date,tm.VcDate) <= '{toDate}'";
        string finYearFilter = (vm.FinYearId > 0) ? $" and tm.FinYearId = {vm.FinYearId}" : "";

        string query = $@"select Sl,AccTranMstId,SlNo,LedgerId,LedgerName,convert(varchar, VcDate, 6) VcDate,nr,Narration,VcNo,IsApproved,case when IsApproved = 0 then '(Not Yet Approved)' else null end VcStatus
        ,case when OpBal< 0 then convert(char, format(round(OpBal * -1, 2), 'N')) else convert(char, format(round(OpBal, 2), 'N')) end OpBal
        ,case when AmountDr < 0 then '( '+convert(char, format(round(AmountDr * -1, 2), 'N'))+' )' else convert(char, format(round(AmountDr, 2), 'N')) end AmountDr
        ,case when AmountCr < 0 then '( '+convert(char, format(round(AmountCr * -1, 2), 'N'))+' )' else convert(char, format(round(AmountCr, 2), 'N')) end AmountCr
        ,case when Balance < 0 then '( '+convert(char, format(round(Balance * -1, 2), 'N'))+' )' else convert(char, format(round(Balance, 2), 'N')) end Balance
        ,round(AmountDr,2) AmountDrNum,round(AmountCr,2) AmountCrNum,round(Balance,2) BalanceNum 
        from (
        select count(*) over(partition by LedgerId order by VcDate, AccTranMstId, SlNo) Sl, AccTranMstId, SlNo, IsApproved, LedgerId, LedgerName, VcDate, nr, Narration, VcNo, sum(OpBal) OpBal, sum(AmountDr) AmountDr, sum(AmountCr) AmountCr,
        sum(sum(OpBal) + sum(AmountCr) - sum(AmountDr)) over(partition by LedgerId order by VcDate, AccTranMstId, SlNo) Balance
        from(
        /*Opening*/
        select 1 IsApproved, 0 AccTranMstId, 0 SlNo, LedgerId, LedgerName, '{fromDate}' VcDate, nr, Narration, '01' VcNo,Sum(CreditAmount) -  sum(DebitAmount) OpBal, 0 AmountDr, 0 AmountCr
        from(
        select td.LedgerDrId LedgerId, al.LedgerName, NULL VcDate, '' nr, 'OPENING BALANCE' Narration, '' VcNo, 0 OpBal, ISNULL(td.AmountDr, 0) DebitAmount, 0 CreditAmount
        from AccTranDtls td
        inner join AccTranMsts tm on tm.id = td.TranMstId
        inner join AccLedgers al on al.Id = td.LedgerDrId
        where tm.VcType = 'O' and al.Id = {vm.LedgerId} {finYearFilter} /* {fromDateFilter} {toDateFilter}*/
        union all

        select td.LedgerCrId LedgerId, al.LedgerName, NULL VcDate, '' nr, 'OPENING BALANCE' Narration, '' VcNo, 0 OpBal, 0 DebitAmount, ISNULL(td.AmountCr, 0) CreditAmount
        from AccTranDtls td
        inner join AccTranMsts tm on tm.id = td.TranMstId
        inner join AccLedgers al on al.Id = td.LedgerCrId
        where tm.VcType = 'O' and al.Id = {vm.LedgerId} {finYearFilter} /* {fromDateFilter} {toDateFilter}*/

        union all

        select td.LedgerDrId LedgerId, al.LedgerName, NULL VcDate, '' nr, 'OPENING BALANCE' Narration, '' VcNo, 0 OpBal, ISNULL(td.AmountDr, 0) DebitAmount, 0 CreditAmount
        from AccTranDtls td
        inner join AccTranMsts tm on tm.id = td.TranMstId
        inner join AccLedgers al on al.Id = td.LedgerDrId
        where tm.VcType != 'O' and convert(date, tm.VcDate) < '{fromDate}' and al.Id = {vm.LedgerId} {finYearFilter}

        union all

        select td.LedgerCrId LedgerId, al.LedgerName, NULL VcDate, '' nr, 'OPENING BALANCE' Narration, '' VcNo, 0 OpBal, 0 DebitAmount, ISNULL(td.AmountCr, 0) CreditAmount
        from AccTranDtls td
        inner join AccTranMsts tm on tm.id = td.TranMstId
        inner join AccLedgers al on al.Id = td.LedgerCrId
        where tm.VcType != 'O' and convert(date, tm.VcDate) < '{fromDate}' and al.Id = {vm.LedgerId} {finYearFilter}
        ) d group by LedgerId, LedgerName, nr, Narration

        /*dr balance*/
        union all

        select tm.IsApproved, tm.Id AccTranMstId, td.SlNo, td.LedgerDrId LedgerId, al.LedgerName, tm.VcDate, 'To ' + alc.LedgerName nr, tm.Narration, tm.VcNo, 0 OpBal,
        sum(isnull(td.AmountDr, 0)) AmountDr, 0 AmountCr
        from AccTranDtls td
        inner join AccTranMsts tm on tm.id = td.TranMstId
        inner join AccLedgers al on al.Id = td.LedgerDrId
        inner join AccLedgers alc on alc.Id = td.LedgerCrId
        where tm.VcType != 'O' and convert(date, VcDate) >= '{fromDate}' and convert(date, VcDate) <= '{toDate}' and al.Id = {vm.LedgerId}
        group by tm.IsApproved, tm.Id, td.SlNo, td.LedgerDrId, al.LedgerName, tm.VcDate, tm.Narration, tm.VcNo, alc.LedgerName

        /*Cr balance*/

        union all

        select tm.IsApproved, tm.Id AccTranMstId, td.SlNo, td.LedgerCrId LedgerId, al.LedgerName, tm.VcDate, 'To ' + alc.LedgerName nr, tm.Narration, tm.VcNo, 0 OpBal,
        0 AmountDr, sum(isnull(td.AmountCr, 0)) AmountCr
        from AccTranDtls td
        inner join AccTranMsts tm on tm.id = td.TranMstId
        inner join AccLedgers al on al.Id = td.LedgerCrId
        inner join AccLedgers alc on alc.Id = td.LedgerDrId
        where tm.VcType != 'O' and convert(date, VcDate) >= '{fromDate}' and convert(date, VcDate) <= '{toDate}' and al.Id = {vm.LedgerId}
        group by tm.IsApproved, tm.Id, td.SlNo, td.LedgerCrId, al.LedgerName, tm.VcDate, tm.Narration, tm.VcNo, alc.LedgerName
        ) g group by AccTranMstId, SlNo, IsApproved, LedgerId, LedgerName, VcDate, nr, Narration, VcNo
        ) t ";
        var data = await _iReadDbConnection.QueryAsync<AccLedgerReportVm>(query);
        return data.ToList();
    }
    #endregion

    #region GetLedgerReportData

    public async Task<List<AccLedgerReportVm>> GetLedgerReportData(AccReportVm vm)
    {
        vm.StrFromDate = (string.IsNullOrEmpty(vm.StrFromDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrFromDate;
        vm.StrToDate = (string.IsNullOrEmpty(vm.StrFromDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrToDate;
        var fromDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrFromDate)).ToString("dd/MMM/yyyy");
        var toDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrToDate)).ToString("dd/MMM/yyyy");

        string fromDateFilter = $" and convert(date,tm.VcDate) >= '{fromDate}'";
        string toDateFilter = $" and convert(date,tm.VcDate) <= '{toDate}'";
        string finYearFilter = (vm.FinYearId > 0) ? $" and tm.FinYearId = {vm.FinYearId}" : "";

        string accountForDrFilter = (vm.MishukLedgerId > 0) ? $@" and (td.LedgerDrId = {vm.MishukLedgerId} or tm.AccAccountId = {vm.MishukLedgerId} ) " : "";
        string accountForCrFilter = (vm.MishukLedgerId > 0) ? $@" and (td.LedgerCrId = {vm.MishukLedgerId} or tm.AccAccountId = {vm.MishukLedgerId} )" : "";

        #region
        //string query = $@"select Sl,AccTranMstId,SlNo,LedgerId,LedgerName,convert(varchar, VcDate, 6) VcDate,nr,Narration,VcNo,IsApproved,case when IsApproved = 0 then '(Not Yet Approved)' else null end VcStatus
        //    ,case when OpBal< 0 then '(' + convert(char, format(round(OpBal * -1, 2), 'N')) + ')' else convert(char, format(round(OpBal, 2), 'N')) end OpBal
        //    ,case when AmountDr< 0 then '(' + convert(char, format(round(AmountDr * -1, 2), 'N')) + ')' else convert(char, format(round(AmountDr, 2), 'N')) end AmountDr
        //    ,case when AmountCr< 0 then '(' + convert(char, format(round(AmountCr * -1, 2), 'N')) + ')' else convert(char, format(round(AmountCr, 2), 'N')) end AmountCr
        //    ,case when Balance< 0 then '(' + convert(char, format(round(Balance * -1, 2), 'N')) + ')' else convert(char, format(round(Balance, 2), 'N')) end Balance
        //    ,round(AmountDr,2) AmountDrNum,round(AmountCr,2) AmountCrNum,round(Balance,2) BalanceNum 
        //    from (
        //    select count(*) over(partition by LedgerId order by VcDate, AccTranMstId, SlNo) Sl, AccTranMstId, SlNo, IsApproved, LedgerId, LedgerName, VcDate, nr, Narration, VcNo, sum(OpBal) OpBal, sum(AmountDr) AmountDr, sum(AmountCr) AmountCr,
        //    sum(sum(OpBal) + sum(AmountDr) - sum(AmountCr)) over(partition by LedgerId order by VcDate, AccTranMstId, SlNo) Balance
        //    from(
        //    /*Opening*/
        //    select 1 IsApproved, 0 AccTranMstId, 0 SlNo, LedgerId, LedgerName, '{fromDate}' VcDate, nr, Narration, '01' VcNo, sum(DebitAmount) - Sum(CreditAmount) OpBal, 0 AmountDr, 0 AmountCr
        //    from(
        //    select td.LedgerDrId LedgerId, al.LedgerName, NULL VcDate, '' nr, 'OPENING BALANCE' Narration, '' VcNo, 0 OpBal, ISNULL(td.AmountDr, 0) DebitAmount, 0 CreditAmount
        //    from AccTranDtls td
        //    inner join AccTranMsts tm on tm.id = td.TranMstId
        //    inner join AccLedgers al on al.Id = td.LedgerDrId
        //    where tm.VcType = 'O' and al.Id = {vm.LedgerId}
        //    union all

        //    select td.LedgerCrId LedgerId, al.LedgerName, NULL VcDate, '' nr, 'OPENING BALANCE' Narration, '' VcNo, 0 OpBal, 0 DebitAmount, ISNULL(td.AmountDr, 0) CreditAmount
        //    from AccTranDtls td
        //    inner join AccTranMsts tm on tm.id = td.TranMstId
        //    inner join AccLedgers al on al.Id = td.LedgerCrId
        //    where tm.VcType = 'O' and al.Id = {vm.LedgerId}

        //    union all

        //    select td.LedgerDrId LedgerId, al.LedgerName, NULL VcDate, '' nr, 'OPENING BALANCE' Narration, '' VcNo, 0 OpBal, ISNULL(td.AmountDr, 0) DebitAmount, 0 CreditAmount
        //    from AccTranDtls td
        //    inner join AccTranMsts tm on tm.id = td.TranMstId
        //    inner join AccLedgers al on al.Id = td.LedgerDrId
        //    where tm.VcType != 'O' and convert(date, tm.VcDate) < '{fromDate}' and al.Id = {vm.LedgerId}

        //    union all

        //    select td.LedgerCrId LedgerId, al.LedgerName, NULL VcDate, '' nr, 'OPENING BALANCE' Narration, '' VcNo, 0 OpBal, 0 DebitAmount, ISNULL(td.AmountDr, 0) CreditAmount
        //    from AccTranDtls td
        //    inner join AccTranMsts tm on tm.id = td.TranMstId
        //    inner join AccLedgers al on al.Id = td.LedgerCrId
        //    where tm.VcType != 'O' and convert(date, tm.VcDate) < '{fromDate}' and al.Id = {vm.LedgerId}
        //    ) d group by LedgerId, LedgerName, nr, Narration

        //    /*dr balance*/
        //    union all

        //    select tm.IsApproved, tm.Id AccTranMstId, td.SlNo, td.LedgerDrId LedgerId, al.LedgerName, tm.VcDate, 'To ' + alc.LedgerName nr, tm.Narration, tm.VcNo, 0 OpBal,
        //    sum(isnull(td.AmountDr, 0)) AmountDr, 0 AmountCr
        //    from AccTranDtls td
        //    inner join AccTranMsts tm on tm.id = td.TranMstId
        //    inner join AccLedgers al on al.Id = td.LedgerDrId
        //    inner join AccLedgers alc on alc.Id = td.LedgerCrId
        //    where tm.VcType != 'O' and convert(date, VcDate) >= '{fromDate}' and convert(date, VcDate) <= '{toDate}' and al.Id = {vm.LedgerId}
        //    group by tm.IsApproved, tm.Id, td.SlNo, td.LedgerDrId, al.LedgerName, tm.VcDate, tm.Narration, tm.VcNo, alc.LedgerName

        //    /*Cr balance*/

        //    union all

        //    select tm.IsApproved, tm.Id AccTranMstId, td.SlNo, td.LedgerCrId LedgerId, al.LedgerName, tm.VcDate, 'To ' + alc.LedgerName nr, tm.Narration, tm.VcNo, 0 OpBal,
        //    0 AmountDr, sum(isnull(td.AmountCr, 0)) AmountCr
        //    from AccTranDtls td
        //    inner join AccTranMsts tm on tm.id = td.TranMstId
        //    inner join AccLedgers al on al.Id = td.LedgerCrId
        //    inner join AccLedgers alc on alc.Id = td.LedgerDrId
        //    where tm.VcType != 'O' and convert(date, VcDate) >= '{fromDate}' and convert(date, VcDate) <= '{toDate}' and al.Id = {vm.LedgerId}
        //    group by tm.IsApproved, tm.Id, td.SlNo, td.LedgerCrId, al.LedgerName, tm.VcDate, tm.Narration, tm.VcNo, alc.LedgerName
        //    ) g group by AccTranMstId, SlNo, IsApproved, LedgerId, LedgerName, VcDate, nr, Narration, VcNo
        //    ) t ";
        #endregion

        string query = $@"select Sl,AccTranMstId,SlNo,LedgerId,LedgerName,convert(varchar, VcDate, 6) VcDate,nr,Narration,VcNo,IsApproved,case when IsApproved = 0 then '(Not Yet Approved)' else null end VcStatus, IsAuto
        ,case when OpBal< 0 then convert(char, format(round(OpBal * -1, 2), 'N')) else convert(char, format(round(OpBal, 2), 'N')) end OpBal
        ,case when AmountDr < 0 then '( '+convert(char, format(round(AmountDr * -1, 2), 'N'))+' )' else convert(char, format(round(AmountDr, 2), 'N')) end AmountDr
        ,case when AmountCr < 0 then '( '+convert(char, format(round(AmountCr * -1, 2), 'N'))+' )' else convert(char, format(round(AmountCr, 2), 'N')) end AmountCr
        ,case when Balance < 0 then '( '+convert(char, format(round(Balance * -1, 2), 'N'))+' )' else convert(char, format(round(Balance, 2), 'N')) end Balance
        ,round(AmountDr,2) AmountDrNum,round(AmountCr,2) AmountCrNum,round(Balance,2) BalanceNum 
        from (
        select count(*) over(partition by LedgerId order by VcDate, AccTranMstId, SlNo) Sl, AccTranMstId, SlNo, IsApproved, LedgerId, LedgerName, VcDate, nr, Narration, VcNo, sum(OpBal) OpBal, sum(AmountDr) AmountDr, sum(AmountCr) AmountCr,IsAuto,
        sum(sum(OpBal) + sum(AmountDr) - sum(AmountCr)) over(partition by LedgerId order by VcDate, AccTranMstId, SlNo) Balance
        from(
        /*Opening*/
        select 1 IsApproved, 0 AccTranMstId, 0 SlNo, LedgerId, LedgerName, '{fromDate}' VcDate, nr, Narration, '01' VcNo, sum(DebitAmount) - Sum(CreditAmount) OpBal, 0 AmountDr, 0 AmountCr,0 IsAuto
        from(
        select td.LedgerDrId LedgerId, al.LedgerName, NULL VcDate, '' nr, 'OPENING BALANCE' Narration, '' VcNo, 0 OpBal, ISNULL(td.AmountDr, 0) DebitAmount, 0 CreditAmount
        from AccTranDtls td
        inner join AccTranMsts tm on tm.id = td.TranMstId
        inner join AccLedgers al on al.Id = td.LedgerDrId
        where tm.VcType = 'O' and al.Id = {vm.LedgerId} /*{fromDateFilter} {toDateFilter}*/ {finYearFilter} 
        union all

        select td.LedgerCrId LedgerId, al.LedgerName, NULL VcDate, '' nr, 'OPENING BALANCE' Narration, '' VcNo, 0 OpBal, 0 DebitAmount, ISNULL(td.AmountCr, 0) CreditAmount
        from AccTranDtls td
        inner join AccTranMsts tm on tm.id = td.TranMstId
        inner join AccLedgers al on al.Id = td.LedgerCrId
        where tm.VcType = 'O' and al.Id = {vm.LedgerId} /*{fromDateFilter} {toDateFilter}*/ {finYearFilter} 

        union all

        select td.LedgerDrId LedgerId, al.LedgerName, NULL VcDate, '' nr, 'OPENING BALANCE' Narration, '' VcNo, 0 OpBal, ISNULL(td.AmountDr, 0) DebitAmount, 0 CreditAmount
        from AccTranDtls td
        inner join AccTranMsts tm on tm.id = td.TranMstId
        inner join AccLedgers al on al.Id = td.LedgerDrId
        where tm.VcType != 'O' and convert(date, tm.VcDate) < '{fromDate}' and al.Id = {vm.LedgerId} {finYearFilter} {accountForCrFilter}

        union all

        select td.LedgerCrId LedgerId, al.LedgerName, NULL VcDate, '' nr, 'OPENING BALANCE' Narration, '' VcNo, 0 OpBal, 0 DebitAmount, ISNULL(td.AmountCr, 0) CreditAmount
        from AccTranDtls td
        inner join AccTranMsts tm on tm.id = td.TranMstId
        inner join AccLedgers al on al.Id = td.LedgerCrId
        where tm.VcType != 'O' and convert(date, tm.VcDate) < '{fromDate}' and al.Id = {vm.LedgerId} {finYearFilter} {accountForDrFilter}
        ) d group by LedgerId, LedgerName, nr, Narration

        /*dr balance*/
        union all

        select tm.IsApproved, tm.Id AccTranMstId, td.SlNo, td.LedgerDrId LedgerId, al.LedgerName, tm.VcDate, 'To ' + alc.LedgerName nr, tm.Narration, tm.VcNo, 0 OpBal,
        sum(isnull(td.AmountDr, 0)) AmountDr, 0 AmountCr, tm.IsAuto
        from AccTranDtls td
        inner join AccTranMsts tm on tm.id = td.TranMstId
        inner join AccLedgers al on al.Id = td.LedgerDrId
        inner join AccLedgers alc on alc.Id = td.LedgerCrId
        where tm.VcType != 'O' and convert(date, VcDate) >= '{fromDate}' and convert(date, VcDate) <= '{toDate}' and al.Id = {vm.LedgerId} {accountForCrFilter}
        group by tm.IsApproved, tm.Id, td.SlNo, td.LedgerDrId, al.LedgerName, tm.VcDate, tm.Narration, tm.VcNo, alc.LedgerName, tm.IsAuto

        /*Cr balance*/

        union all

        select tm.IsApproved, tm.Id AccTranMstId, td.SlNo, td.LedgerCrId LedgerId, al.LedgerName, tm.VcDate, 'To ' + alc.LedgerName nr, tm.Narration, tm.VcNo, 0 OpBal,
        0 AmountDr, sum(isnull(td.AmountCr, 0)) AmountCr, tm.IsAuto
        from AccTranDtls td
        inner join AccTranMsts tm on tm.id = td.TranMstId
        inner join AccLedgers al on al.Id = td.LedgerCrId
        inner join AccLedgers alc on alc.Id = td.LedgerDrId
        where tm.VcType != 'O' and convert(date, VcDate) >= '{fromDate}' and convert(date, VcDate) <= '{toDate}' and al.Id = {vm.LedgerId} {accountForDrFilter} 
        group by tm.IsApproved, tm.Id, td.SlNo, td.LedgerCrId, al.LedgerName, tm.VcDate, tm.Narration, tm.VcNo, alc.LedgerName, tm.IsAuto
        ) g group by AccTranMstId, SlNo, IsApproved, LedgerId, LedgerName, VcDate, nr, Narration, VcNo, IsAuto
        ) t ";
        var data = await _iReadDbConnection.QueryAsync<AccLedgerReportVm>(query);
        return data.ToList();
    }

    #endregion

    #region GetLedgerReportHtml

    public async Task<string> GetLedgerReportHtml(AccReportVm vm, bool isPrint = false)
    {
        string fullHtml = "";
        var data = await GetLedgerReportData(vm);

        #region Ledger Name
        var allHeads = await _iAccHeadRepository.GetAllAsync();
        var mishukHead = allHeads.FirstOrDefault(x => x.HeadCode == AccHeadCode.HotelMisukHead);
        if (mishukHead == null)
            throw new Exception("No Mishuk Head Found !!");

        var allLagederList = await _iAccLedgerRepository.GetAllAsync();
        if (allLagederList == null)
            throw new Exception("No Ledger Found !!");

        var mishulLagederList = allLagederList.Where(x => x.HeadId == mishukHead.Id);
        if (allLagederList == null)
            throw new Exception("No Mishuk-Ledger Found !!");

        var mishulLageder = vm?.MishukLedgerId > 0 ? mishulLagederList.FirstOrDefault(x => x.Id == (long)vm?.MishukLedgerId) : null;
        var ledger = vm?.LedgerId > 0 ? allLagederList.FirstOrDefault(x => x.Id == (long)vm?.LedgerId) : null;

        string mishukLedgerName = mishulLageder?.LedgerName ?? "N/A";
        string ledgerName = ledger is not null ? $"{ledger.LedgerCode + "-" + ledger.LedgerName}" : "N/A";

        #endregion

        if (data != null && data.Count > 0)
        {
            fullHtml += "<table class='table table-striped table-bordered table-md' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
            fullHtml += "<thead>";

            fullHtml += "<tr style='height:30px;'>";
            fullHtml += $@"<th colspan='7' style='text-align:center;'>{mishukLedgerName} <br/> {ledgerName}</th>";
            fullHtml += "</tr>";

            fullHtml += "<tr style='height:30px;'>";
            fullHtml += "<th style='width:50px;'>SL No</th><th style='width:93px;'>Date</th><th style='width:425px;'>Particulars</th><th style='width:150px;'>Voucher No</th><th style='width:150px;'>Debit Amount</th><th style='width:150px;'>Credit Amount</th><th style='width:150px;'>Balance</th>";
            fullHtml += "</tr>";
            fullHtml += "</thead>";
            fullHtml += "<tbody>";

            decimal lastBalance = 0;
            for (int i = 0; i < data.Count; i++)
            {
                AccLedgerReportVm objLeger = data[i];

                string rowVcNo = !isPrint ? $"<a target='_blank' href='../AccTranMst/JournalDetails/{objLeger.AccTranMstId}'>{objLeger.VcNo}</a>" : $"{objLeger.VcNo}";
                string rowDr = (objLeger.AmountDrNum != 0) ? objLeger.AmountDr : "";
                string rowCr = (objLeger.AmountCrNum != 0) ? objLeger.AmountCr : "";
                string rowBalance = (objLeger.BalanceNum != 0) ? objLeger.Balance : "";

                fullHtml += "<tr>";
                fullHtml += $@"<td style='text-align:left;'>{objLeger.Sl}</td><td style='text-align:left;'>{objLeger.VcDate}</td><td style='text-align:left;padding:5px;'><b>{objLeger.nr}</b><br />{objLeger.Narration}<br />{objLeger.VcStatus}</td>
                    <td style='text-align:left;'>{rowVcNo}</td><td style='text-align:right;'>{rowDr}</td><td style='text-align:right;'>{rowCr}</td><td style='text-align:right;'>{rowBalance}</td>";
                fullHtml += "</tr>";

                lastBalance = objLeger.BalanceNum;
            }
            decimal declastBalance = Convert.ToDecimal(lastBalance);
            string strlastBalance = (declastBalance < 0) ? "(" + (declastBalance * -1).ToString("N2") + ")" : declastBalance.ToString("N2");


            fullHtml += $@"<tr style='font-weight:bold;height:30px;'>
                <td></td><td></td><td></td><td>Total (TK)</td>
                <td style='text-align:right;'>{data.Sum(o => o.AmountDrNum):N2}</td>
                <td style='text-align:right;'>{data.Sum(o => o.AmountCrNum):N2}</td>
                <td style='text-align:right;'>{strlastBalance}</td>
                </tr>";
        }
        fullHtml += "</tbody>";

        fullHtml += "</table>";

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

    #region GetReceiptAndPaymentsData
    public async Task<List<ReceiptAndPaymentVM>> GetReceiptAndPaymentsData(AccReportVm vm)
    {
        vm.StrFromDate = (string.IsNullOrEmpty(vm.StrFromDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrFromDate;
        vm.StrToDate = (string.IsNullOrEmpty(vm.StrFromDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrToDate;
        var fromDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrFromDate)).ToString("dd/MMM/yyyy");
        var toDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrToDate)).ToString("dd/MMM/yyyy");
        string accLedgerFilter = (vm.MishukLedgerId > 0) ? $" and ( al.Id = {vm.MishukLedgerId} or tm.AccAccountId = {vm.MishukLedgerId}) " : "";
        string accLedgerFilterE = (vm.MishukLedgerId > 0) ? $" and ( alc.Id = {vm.MishukLedgerId} or tm.AccAccountId = {vm.MishukLedgerId}) " : "";
        string openingFilterMaster = (vm.MishukLedgerId > 0) ? $" and tm.AccAccountId = {vm.MishukLedgerId}" : "";

        #region -- off for get all transaction related cash & cash equivalent onlye
        //string query = $@"/*Expenses*/
        //    select 'E' ActionType,'L' HeadType,HeadId,HeadCode,HeadName,sum(AmountDr) AmountDr,sum(AmountCr)AmountCr,case when sum(AmountDr) >= sum(AmountCr) then 'D' else 'C' end BalanceType
        //    ,case when sum(AmountDr) >= sum(AmountCr) then sum(AmountDr) - sum(AmountCr) else sum(AmountCr) - sum(AmountDr) end BalanceAmount
        //    from (
        //    select al.Id HeadId,al.LedgerCode HeadCode,al.LedgerName HeadName,sum(td.AmountDr) AmountDr,0 AmountCr
        //    from AccTranDtls td
        //    inner join AccTranMsts tm on tm.Id = td.TranMstId
        //    inner join AccLedgers al on al.Id = td.LedgerDrId
        //    where al.Id in (
        //    select l.Id from AccLedgers l
        //    inner join AccHeads h on h.Id = l.HeadId
        //    where h.HeadCode = '4' or h.HeadCode like '4.%'
        //    ) and tm.VcDate >= '{fromDate}' and tm.VcDate <= '{toDate}'
        //    group by al.Id,al.LedgerCode,al.LedgerName
        //    union all
        //    select al.Id HeadId,al.LedgerCode HeadCode,al.LedgerName HeadName,0 AmountDr,sum(td.AmountDr) AmountCr
        //    from AccTranDtls td
        //    inner join AccTranMsts tm on tm.Id = td.TranMstId
        //    inner join AccLedgers al on al.Id = td.LedgerCrId
        //    where al.Id in (
        //    select l.Id from AccLedgers l
        //    inner join AccHeads h on h.Id = l.HeadId
        //    where h.HeadCode = '4' or h.HeadCode like '4.%'
        //    ) and tm.VcDate >= '{fromDate}' and tm.VcDate <= '{toDate}'
        //    group by al.Id,al.LedgerCode,al.LedgerName
        //    )d group by d.HeadId,d.HeadCode,d.HeadName

        //    /*Revenue*/
        //    union all

        //    select 'R' ActionType,'L' HeadType,HeadId,HeadCode,HeadName,sum(AmountDr) AmountDr,sum(AmountCr)AmountCr,case when sum(AmountDr) >= sum(AmountCr) then 'D' else 'C' end BalanceType
        //    ,case when sum(AmountDr) >= sum(AmountCr) then sum(AmountDr) - sum(AmountCr) else sum(AmountCr) - sum(AmountDr) end BalanceAmount
        //    from (
        //    select al.Id HeadId,al.LedgerCode HeadCode,al.LedgerName HeadName,sum(td.AmountDr) AmountDr,0 AmountCr
        //    from AccTranDtls td
        //    inner join AccTranMsts tm on tm.Id = td.TranMstId
        //    inner join AccLedgers al on al.Id = td.LedgerDrId
        //    where al.Id in (
        //    select l.Id from AccLedgers l
        //    inner join AccHeads h on h.Id = l.HeadId
        //    where h.HeadCode = '2' or h.HeadCode like '2.%'
        //    ) and tm.VcDate >= '{fromDate}' and tm.VcDate <= '{toDate}'
        //    group by al.Id,al.LedgerCode,al.LedgerName
        //    union all
        //    select al.Id HeadId,al.LedgerCode HeadCode,al.LedgerName HeadName,0 AmountDr,sum(td.AmountDr) AmountCr
        //    from AccTranDtls td
        //    inner join AccTranMsts tm on tm.Id = td.TranMstId
        //    inner join AccLedgers al on al.Id = td.LedgerCrId
        //    where al.Id in (
        //    select l.Id from AccLedgers l
        //    inner join AccHeads h on h.Id = l.HeadId
        //    where h.HeadCode = '2' or h.HeadCode like '2.%'
        //    ) and tm.VcDate >= '{fromDate}' and tm.VcDate <= '{toDate}'
        //    group by al.Id,al.LedgerCode,al.LedgerName
        //    )d group by d.HeadId,d.HeadCode,d.HeadName

        //    /*Opening*/
        //    union all

        //    select 'O' ActionType,'L' HeadType,HeadId,HeadCode,HeadName,sum(AmountDr) AmountDr,sum(AmountCr)AmountCr,case when sum(AmountDr) >= sum(AmountCr) then 'D' else 'C' end BalanceType
        //    ,case when sum(AmountDr) >= sum(AmountCr) then sum(AmountDr) - sum(AmountCr) else sum(AmountCr) - sum(AmountDr) end BalanceAmount
        //    from (
        //    select al.Id HeadId,al.LedgerCode HeadCode,al.LedgerName HeadName,sum(td.AmountDr) AmountDr,0 AmountCr
        //    from AccTranDtls td
        //    inner join AccTranMsts tm on tm.Id = td.TranMstId
        //    inner join AccLedgers al on al.Id = td.LedgerDrId
        //    where al.Id in (
        //    select l.Id from AccLedgers l
        //    inner join AccHeads h on h.Id = l.HeadId
        //    where h.HeadCode = '1.2.5' or h.HeadCode like '1.2.5.%'
        //    ) and tm.VcDate >= '{fromDate}' and tm.VcDate <= '{toDate}' and tm.VcType = 'O'
        //    group by al.Id,al.LedgerCode,al.LedgerName
        //    union all
        //    select al.Id HeadId,al.LedgerCode HeadCode,al.LedgerName HeadName,0 AmountDr,sum(td.AmountDr) AmountCr
        //    from AccTranDtls td
        //    inner join AccTranMsts tm on tm.Id = td.TranMstId
        //    inner join AccLedgers al on al.Id = td.LedgerCrId
        //    where al.Id in (
        //    select l.Id from AccLedgers l
        //    inner join AccHeads h on h.Id = l.HeadId
        //    where h.HeadCode = '1.2.5' or h.HeadCode like '1.2.5.%'
        //    ) and tm.VcDate >= '{fromDate}' and tm.VcDate <= '{toDate}' and tm.VcType = 'O'
        //    group by al.Id,al.LedgerCode,al.LedgerName
        //    )d group by d.HeadId,d.HeadCode,d.HeadName
        //    /*Closing*/
        //    union all
        //    select 'C' ActionType,'L' HeadType,HeadId,HeadCode,HeadName,sum(AmountDr) AmountDr,sum(AmountCr)AmountCr,case when sum(AmountDr) >= sum(AmountCr) then 'D' else 'C' end BalanceType
        //    ,case when sum(AmountDr) >= sum(AmountCr) then sum(AmountDr) - sum(AmountCr) else sum(AmountCr) - sum(AmountDr) end BalanceAmount
        //    from (
        //    select al.Id HeadId,al.LedgerCode HeadCode,al.LedgerName HeadName,sum(td.AmountDr) AmountDr,0 AmountCr
        //    from AccTranDtls td
        //    inner join AccTranMsts tm on tm.Id = td.TranMstId
        //    inner join AccLedgers al on al.Id = td.LedgerDrId
        //    where al.Id in (
        //    select l.Id from AccLedgers l
        //    inner join AccHeads h on h.Id = l.HeadId
        //    where h.HeadCode = '1.2.5' or h.HeadCode like '1.2.5.%'
        //    ) and tm.VcDate >= '{fromDate}' and tm.VcDate <= '{toDate}' 
        //    group by al.Id,al.LedgerCode,al.LedgerName
        //    union all
        //    select al.Id HeadId,al.LedgerCode HeadCode,al.LedgerName HeadName,0 AmountDr,sum(td.AmountDr) AmountCr
        //    from AccTranDtls td
        //    inner join AccTranMsts tm on tm.Id = td.TranMstId
        //    inner join AccLedgers al on al.Id = td.LedgerCrId
        //    where al.Id in (
        //    select l.Id from AccLedgers l
        //    inner join AccHeads h on h.Id = l.HeadId
        //    where h.HeadCode = '1.2.5' or h.HeadCode like '1.2.5.%'
        //    ) and tm.VcDate >= '{fromDate}' and tm.VcDate <= '{toDate}'
        //    group by al.Id,al.LedgerCode,al.LedgerName
        //    ) d group by d.HeadId,d.HeadCode,d.HeadName";
        #endregion

        string query = $@"/*Receive*/
        select 'R' ActionType,'L' HeadType,HeadId,HeadCode,HeadName,sum(AmountDr) AmountDr,sum(AmountCr)AmountCr,case when sum(AmountDr) >= sum(AmountCr) then 'D' else 'C' end BalanceType
        ,case when sum(AmountDr) >= sum(AmountCr) then sum(AmountDr) - sum(AmountCr) else sum(AmountCr) - sum(AmountDr) end BalanceAmount
        from (
        select alc.Id HeadId,alc.LedgerCode HeadCode,alc.LedgerName HeadName,sum(td.AmountDr) AmountDr,0 AmountCr
        from AccTranDtls td
        inner join AccTranMsts tm on tm.Id = td.TranMstId
        inner join AccLedgers al on al.Id = td.LedgerDrId
        inner join AccLedgers alc on alc.Id = td.LedgerCrId
        where al.Id in (
        select l.Id from AccLedgers l
        inner join AccHeads h on h.Id = l.HeadId
        where h.HeadCode = '1.2.5' or h.HeadCode like '1.2.5.%' or h.HeadCode = '1.2.6' or h.HeadCode like '1.2.6.%'
        ) {accLedgerFilter} and tm.VcDate >= '{fromDate}' and tm.VcDate <= '{toDate}'
        group by alc.Id,alc.LedgerCode,alc.LedgerName
        ) d group by d.HeadId,d.HeadCode,d.HeadName
        union all
        /*expenses*/
        select 'E' ActionType,'L' HeadType,HeadId,HeadCode,HeadName,sum(AmountDr) AmountDr,sum(AmountCr)AmountCr,case when sum(AmountDr) >= sum(AmountCr) then 'D' else 'C' end BalanceType
        ,case when sum(AmountDr) >= sum(AmountCr) then sum(AmountDr) - sum(AmountCr) else sum(AmountCr) - sum(AmountDr) end BalanceAmount
        from (
        select al.Id HeadId,al.LedgerCode HeadCode,al.LedgerName HeadName,sum(td.AmountDr) AmountDr,0 AmountCr
        from AccTranDtls td
        inner join AccTranMsts tm on tm.Id = td.TranMstId
        inner join AccLedgers al on al.Id = td.LedgerDrId
        inner join AccLedgers alc on alc.Id = td.LedgerCrId
        where alc.Id in (
        select l.Id from AccLedgers l
        inner join AccHeads h on h.Id = l.HeadId
        where h.HeadCode = '1.2.5' or h.HeadCode like '1.2.5.%' or h.HeadCode = '1.2.6' or h.HeadCode like '1.2.6.%'
        ) {accLedgerFilterE} and tm.VcDate >= '{fromDate}' and tm.VcDate <= '{toDate}'
        group by al.Id,al.LedgerCode,al.LedgerName
        )d group by d.HeadId,d.HeadCode,d.HeadName

        /*Opening*/
        union all

        select 'O' ActionType,'L' HeadType,HeadId,HeadCode,HeadName,sum(AmountDr) AmountDr,sum(AmountCr)AmountCr,case when sum(AmountDr) >= sum(AmountCr) then 'D' else 'C' end BalanceType
        ,case when sum(AmountDr) >= sum(AmountCr) then sum(AmountDr) - sum(AmountCr) else sum(AmountCr) - sum(AmountDr) end BalanceAmount
        from (
        select al.Id HeadId,al.LedgerCode HeadCode,al.LedgerName HeadName,sum(td.AmountDr) AmountDr,0 AmountCr
        from AccTranDtls td
        inner join AccTranMsts tm on tm.Id = td.TranMstId
        inner join AccLedgers al on al.Id = td.LedgerDrId
        where al.Id in (
        select l.Id from AccLedgers l
        inner join AccHeads h on h.Id = l.HeadId
        where h.HeadCode = '1.2.5' or h.HeadCode like '1.2.5.%' or h.HeadCode = '1.2.6' or h.HeadCode like '1.2.6.%'
        ) and tm.VcDate >= '{fromDate}' and tm.VcDate <= '{toDate}' and tm.VcType = 'O' {openingFilterMaster}
        group by al.Id,al.LedgerCode,al.LedgerName
        union all
        select al.Id HeadId,al.LedgerCode HeadCode,al.LedgerName HeadName,0 AmountDr,sum(td.AmountDr) AmountCr
        from AccTranDtls td
        inner join AccTranMsts tm on tm.Id = td.TranMstId
        inner join AccLedgers al on al.Id = td.LedgerCrId
        where al.Id in (
        select l.Id from AccLedgers l
        inner join AccHeads h on h.Id = l.HeadId
        where h.HeadCode = '1.2.5' or h.HeadCode like '1.2.5.%' or h.HeadCode = '1.2.6' or h.HeadCode like '1.2.6.%' 
        ) and tm.VcDate >= '{fromDate}' and tm.VcDate <= '{toDate}' and tm.VcType = 'O' {openingFilterMaster} 
        group by al.Id,al.LedgerCode,al.LedgerName
        )d group by d.HeadId,d.HeadCode,d.HeadName
        /*Closing*/
        union all
        select 'C' ActionType,'L' HeadType,HeadId,HeadCode,HeadName,sum(AmountDr) AmountDr,sum(AmountCr)AmountCr,case when sum(AmountDr) >= sum(AmountCr) then 'D' else 'C' end BalanceType
        ,case when sum(AmountDr) >= sum(AmountCr) then sum(AmountDr) - sum(AmountCr) else sum(AmountCr) - sum(AmountDr) end BalanceAmount
        from (
        select al.Id HeadId,al.LedgerCode HeadCode,al.LedgerName HeadName,sum(td.AmountDr) AmountDr,0 AmountCr
        from AccTranDtls td
        inner join AccTranMsts tm on tm.Id = td.TranMstId
        inner join AccLedgers al on al.Id = td.LedgerDrId
        where al.Id in (
        select l.Id from AccLedgers l
        inner join AccHeads h on h.Id = l.HeadId
        where h.HeadCode = '1.2.5' or h.HeadCode like '1.2.5.%' or h.HeadCode = '1.2.6' or h.HeadCode like '1.2.6.%'
        ) {accLedgerFilter} and tm.VcDate >= '{fromDate}' and tm.VcDate <= '{toDate}'
        group by al.Id,al.LedgerCode,al.LedgerName
        union all
        select al.Id HeadId,al.LedgerCode HeadCode,al.LedgerName HeadName,0 AmountDr,sum(td.AmountDr) AmountCr
        from AccTranDtls td
        inner join AccTranMsts tm on tm.Id = td.TranMstId
        inner join AccLedgers al on al.Id = td.LedgerCrId
        where al.Id in (
        select l.Id from AccLedgers l
        inner join AccHeads h on h.Id = l.HeadId
        where h.HeadCode = '1.2.5' or h.HeadCode like '1.2.5.%' or h.HeadCode = '1.2.6' or h.HeadCode like '1.2.6.%'
        ) {accLedgerFilter} and tm.VcDate >= '{fromDate}' and tm.VcDate <= '{toDate}'
        group by al.Id,al.LedgerCode,al.LedgerName
        ) d group by d.HeadId,d.HeadCode,d.HeadName";

        var data = await _iReadDbConnection.QueryAsync<ReceiptAndPaymentVM>(query);
        return data.ToList();
    }
    #endregion

    #region GetReceiptAndPaymentsHtml
    public async Task<string> GetReceiptAndPaymentsHtml(AccReportVm vm)
    {
        string fullHtml = "";
        var data = await GetReceiptAndPaymentsData(vm);

        var mishukHead = _iAccHeadRepository.GetFirstOrDefault(x => x.HeadCode == AccHeadCode.HotelMisukHead);
        if (mishukHead == null)
            throw new Exception("No Mishuk Head Found !!");

        var mishulLagederList = await _iAccLedgerRepository.GetAsync(x => x.HeadId == mishukHead.Id);
        string mishulLageder = vm?.MishukLedgerId > 0 ? mishulLagederList.FirstOrDefault(x => x.Id == (long)vm?.MishukLedgerId).LedgerName : "All";

        if (data != null && data.Count > 0)
        {

            fullHtml += $@"<div style='text-align:center;margin-bottom:20px;'>
                                    <p style='margin:4px 0;color:#555;font-size:14px;'>
                                        <b>From:</b> {vm.StrFromDate}  |  <b>To:</b> {vm.StrToDate}
                                    </p>
                                    <hr style='border:1px solid #ddd;width:80%;margin:10px auto;' />
                                </div>";

            fullHtml += "<table class='table table-striped table-bordered md-font-table-2' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
            fullHtml += "<thead>";

            fullHtml += "<tr style='height:30px;'>";
            fullHtml += $@"<th colspan='3' style='text-align:center;'>{mishulLageder}</th>";
            fullHtml += "</tr>";



            fullHtml += "<tr style='height:30px;'>";
            fullHtml += "<th rowspan='2'>Particular</th><th colspan='2' style='text-align:center;'>Amount In Taka</th>";
            fullHtml += "</tr>";
            fullHtml += "<tr style='height:30px;'>";
            fullHtml += "<th style='text-align:center;'>Amount</th><th style='text-align:center;'>Balance</th>";
            fullHtml += "</tr>";
            fullHtml += "</thead>";


            fullHtml += "<tbody>";

            var opData = data.Where(o => o.ActionType == "O").ToList();
            var clData = data.Where(o => o.ActionType == "C").ToList();
            var rcvData = data.Where(o => o.ActionType == "R").ToList();
            var payData = data.Where(o => o.ActionType == "E").ToList();


            /*Cash And Cash Equivalents*/
            fullHtml += $@"<tr style='height:30px;font-weight:bold;font-size:13px;'>
                <td style='text-align:right;'>Opening Cash & Cash Equivalents</td><td style='text-align:left;'></td><td style='text-align:right;'>{opData.Sum(o => o.BalanceAmount):N2}</td>
                </tr>";
            foreach (var item in opData)
            {
                fullHtml += $@"<tr style='height:30px;'>
                    <td style='text-align:left;'>{item.HeadName}</td><td style='text-align:right;'>{item.BalanceAmount:N2}</td><td style='text-align:right;'></td>
                    </tr>";
            }

            /*Receipt*/
            fullHtml += $@"<tr style='height:30px;font-weight:bold;font-size:13px;'>
                <td style='text-align:right;'>Receipts</td><td style='text-align:left;'></td><td style='text-align:right;'>{rcvData.Sum(o => o.BalanceAmount):N2}</td>
                </tr>";
            foreach (var item in rcvData)
            {
                fullHtml += $@"<tr style='height:30px;'>
                    <td style='text-align:left;'>{item.HeadName}</td><td style='text-align:right;'>{item.BalanceAmount:N2}</td><td style='text-align:right;'></td>
                    </tr>";
            }
            /*Payments*/
            fullHtml += $@"<tr style='height:30px;font-weight:bold;font-size:13px;'>
                <td style='text-align:right;'>Payments</td><td style='text-align:right;'></td><td style='text-align:right;'>{payData.Sum(o => o.BalanceAmount):N2}</td>
                </tr>";
            foreach (var item in payData)
            {
                fullHtml += $@"<tr style='height:30px;'>
                    <td style='text-align:left;'>{item.HeadName}</td><td style='text-align:right;'>{item.BalanceAmount:N2}</td><td style='text-align:right;'></td>
                    </tr>";
            }
            /*Closing Cash & Cash Equivalents*/
            fullHtml += $@"<tr style='height:30px;font-weight:bold;font-size:13px;'>
                <td style='text-align:right;'>Closing Cash & Cash Equivalents</td><td style='text-align:left;'></td><td style='text-align:right;'>{clData.Sum(o => o.BalanceAmount):N2}</td>
                </tr>";
            foreach (var item in clData)
            {
                fullHtml += $@"<tr style='height:30px;'>
                    <td style='text-align:left;'>{item.HeadName}</td><td style='text-align:right;'>{item.BalanceAmount:N2}</td><td style='text-align:right;'></td>
                    </tr>";
            }
            fullHtml += "</tbody></table>";

        }
        return fullHtml;
    }

    #endregion

    #region GetProfitOrLossStatement
    public async Task<List<ProfitLossVM>> GetProfitOrLossStatementData(AccReportVm vm)
    {
        vm.StrFromDate = (string.IsNullOrEmpty(vm.StrFromDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrFromDate;
        vm.StrToDate = (string.IsNullOrEmpty(vm.StrFromDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrToDate;
        var fromDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrFromDate)).ToString("dd/MMM/yyyy");
        var toDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrToDate)).ToString("dd/MMM/yyyy");

        //string accountForDrFilter = (vm.MishukLedgerId > 0) ? $@" and td.LedgerDrId = {vm.MishukLedgerId}" : "";
        //string accountForCrFilter = (vm.MishukLedgerId > 0) ? $@" and td.LedgerCrId = {vm.MishukLedgerId}" : "";

        string accountForDrFilter = (vm.MishukLedgerId > 0) ? $@" and tm.AccAccountId = {vm.MishukLedgerId}" : "";
        string accountForCrFilter = (vm.MishukLedgerId > 0) ? $@" and tm.AccAccountId = {vm.MishukLedgerId}" : "";

        #region --
        string query = $@"/*Expenses*/
        select 'E' ActionType,'L' HeadType,HeadId,HeadCode,HeadName,sum(AmountDr) AmountDr,sum(AmountCr)AmountCr,case when sum(AmountDr) >= sum(AmountCr) then 'D' else 'C' end BalanceType
        ,case when sum(AmountDr) >= sum(AmountCr) then sum(AmountDr) - sum(AmountCr) else sum(AmountCr) - sum(AmountDr) end BalanceAmount
        from (
        select al.Id HeadId,al.LedgerCode HeadCode,al.LedgerName HeadName,sum(td.AmountDr) AmountDr,0 AmountCr
        from AccTranDtls td
        inner join AccTranMsts tm on tm.Id = td.TranMstId
        inner join AccLedgers al on al.Id = td.LedgerDrId
        where al.Id in (
        select l.Id from AccLedgers l
        inner join AccHeads h on h.Id = l.HeadId
        where h.HeadCode = '4' or h.HeadCode like '4.%'
        ) and tm.VcDate >= '{fromDate}' and tm.VcDate <= '{toDate}' {accountForCrFilter}
        group by al.Id,al.LedgerCode,al.LedgerName
        union all
        select al.Id HeadId,al.LedgerCode HeadCode,al.LedgerName HeadName,0 AmountDr,sum(td.AmountDr) AmountCr
        from AccTranDtls td
        inner join AccTranMsts tm on tm.Id = td.TranMstId
        inner join AccLedgers al on al.Id = td.LedgerCrId
        where al.Id in (
        select l.Id from AccLedgers l
        inner join AccHeads h on h.Id = l.HeadId
        where h.HeadCode = '4' or h.HeadCode like '4.%'
        ) and tm.VcDate >= '{fromDate}' and tm.VcDate <= '{toDate}' {accountForDrFilter}
        group by al.Id,al.LedgerCode,al.LedgerName
        )d group by d.HeadId,d.HeadCode,d.HeadName

        /*Revenue*/
        union all

        select 'R' ActionType,'L' HeadType,HeadId,HeadCode,HeadName,sum(AmountDr) AmountDr,sum(AmountCr)AmountCr,case when sum(AmountDr) >= sum(AmountCr) then 'D' else 'C' end BalanceType
        ,case when sum(AmountDr) >= sum(AmountCr) then sum(AmountDr) - sum(AmountCr) else sum(AmountCr) - sum(AmountDr) end BalanceAmount
        from (
        select al.Id HeadId,al.LedgerCode HeadCode,al.LedgerName HeadName,sum(td.AmountDr) AmountDr,0 AmountCr
        from AccTranDtls td
        inner join AccTranMsts tm on tm.Id = td.TranMstId
        inner join AccLedgers al on al.Id = td.LedgerDrId
        where al.Id in (
        select l.Id from AccLedgers l
        inner join AccHeads h on h.Id = l.HeadId
        where h.HeadCode = '2' or h.HeadCode like '2.%'
        ) and tm.VcDate >= '{fromDate}' and tm.VcDate <= '{toDate}' {accountForCrFilter} 
        group by al.Id,al.LedgerCode,al.LedgerName
        union all
        select al.Id HeadId,al.LedgerCode HeadCode,al.LedgerName HeadName,0 AmountDr,sum(td.AmountDr) AmountCr
        from AccTranDtls td
        inner join AccTranMsts tm on tm.Id = td.TranMstId
        inner join AccLedgers al on al.Id = td.LedgerCrId
        where al.Id in (
        select l.Id from AccLedgers l
        inner join AccHeads h on h.Id = l.HeadId
        where h.HeadCode = '2' or h.HeadCode like '2.%'
        ) and tm.VcDate >= '{fromDate}' and tm.VcDate <= '{toDate}' {accountForDrFilter}
        group by al.Id,al.LedgerCode,al.LedgerName
        ) d group by d.HeadId,d.HeadCode,d.HeadName";
        #endregion
        var data = await _iReadDbConnection.QueryAsync<ProfitLossVM>(query);
        return data.ToList();
    }
    #endregion

    #region GetProfitOrLossStatementHtml
    public async Task<string> GetProfitOrLossStatementHtml(AccReportVm vm)
    {
        string fullHtml = "";
        var data = await GetProfitOrLossStatementData(vm);

        var mishukHead = _iAccHeadRepository.GetFirstOrDefault(x => x.HeadCode == AccHeadCode.HotelMisukHead);
        if (mishukHead == null)
            throw new Exception("No Mishuk Head Found !!");

        var mishulLagederList = await _iAccLedgerRepository.GetAsync(x => x.HeadId == mishukHead.Id);
        string mishulLageder = vm?.MishukLedgerId > 0 ? mishulLagederList.FirstOrDefault(x => x.Id == (long)vm?.MishukLedgerId).LedgerName : "All";

        if (data != null && data.Count > 0)
        {
            fullHtml += "<table class='table table-striped table-bordered md-font-table-2' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
            fullHtml += "<thead>";

            fullHtml += "<tr style='height:30px;'>";
            fullHtml += $@"<th colspan='3' style='text-align:center;'>{mishulLageder}</th>";
            fullHtml += "</tr>";

            fullHtml += "<tr style='height:30px;'>";
            fullHtml += "<th rowspan='2'>Particular</th><th colspan='2' style='text-align:center;'>Amount In Taka</th>";
            fullHtml += "</tr>";
            fullHtml += "<tr style='height:30px;'>";
            fullHtml += "<th style='text-align:center;'>Amount</th><th style='text-align:center;'>Balance</th>";
            fullHtml += "</tr>";
            fullHtml += "</thead>";


            fullHtml += "<tbody>";
            var rcvData = data.Where(o => o.ActionType == "R").ToList();
            var payData = data.Where(o => o.ActionType == "E").ToList();
            decimal totalIncome = rcvData.Sum(o => o.BalanceAmount);
            decimal totalExpenses = payData.Sum(o => o.BalanceAmount);
            decimal profitOrLoss = (totalIncome - totalExpenses);
            string strProfitLoss = (profitOrLoss > 0) ? profitOrLoss.ToString("N2") : $"({profitOrLoss:N2})";


            /*Receipt*/
            fullHtml += $@"<tr style='height:30px;font-weight:bold;font-size:13px;'>
                <td style='text-align:right;'>Revenue</td><td style='text-align:left;'></td><td style='text-align:right;'>{rcvData.Sum(o => o.BalanceAmount):N2}</td>
                </tr>";
            foreach (var item in rcvData)
            {
                fullHtml += $@"<tr style='height:30px;'>
                    <td style='text-align:left;'>{item.HeadName}</td><td style='text-align:right;'>{item.BalanceAmount:N2}</td><td style='text-align:right;'></td>
                    </tr>";
            }
            /*Payments*/
            fullHtml += $@"<tr style='height:30px;font-weight:bold;font-size:13px;'>
                <td style='text-align:right;'>Expenses</td><td style='text-align:right;'></td><td style='text-align:right;'>{payData.Sum(o => o.BalanceAmount):N2}</td>
                </tr>";
            foreach (var item in payData)
            {
                fullHtml += $@"<tr style='height:30px;'>
                    <td style='text-align:left;'>{item.HeadName}</td><td style='text-align:right;'>{item.BalanceAmount:N2}</td><td style='text-align:right;'></td>
                    </tr>";
            }
            /*Net Profit or Loss*/
            fullHtml += $@"<tr style='height:30px;font-weight:bold;font-size:13px;'>
                <td style='text-align:right;'>Net Profit</td><td style='text-align:left;'></td><td style='text-align:right;'>{profitOrLoss}</td>
                </tr>";
            fullHtml += "</tbody></table>";

        }
        return fullHtml;
    }

    #endregion

    #region GetQuickLedgerReportHtml

    public async Task<string> GetQuickViewLedgerReportHtml(AccReportVm vm, bool isPrint = false)
    {
        string fullHtml = "";
        //var data = await GetQuickVoucherLedgerReportData(vm);// await GetLedgerReportData(vm);
        var data = await GetLedgerReportData(vm);

        var mishukHead = _iAccHeadRepository.GetFirstOrDefault(x => x.HeadCode == AccHeadCode.HotelMisukHead);
        if (mishukHead == null)
            throw new Exception("No Mishuk Head Found !!");

        var mishulLagederList = await _iAccLedgerRepository.GetAsync(x => x.HeadId == mishukHead.Id);
        string mishulLageder = vm?.MishukLedgerId > 0 ? mishulLagederList.FirstOrDefault(x => x.Id == (long)vm?.MishukLedgerId).LedgerName : "All";

        if (data != null && data.Count > 0)
        {
            fullHtml += "<table class='table table-striped table-bordered table-md' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
            fullHtml += "<thead>";

            fullHtml += "<tr style='height:30px;'>";
            fullHtml += $@"<th colspan='7' style='text-align:center;'>{mishulLageder}</th>";
            fullHtml += "</tr>";

            fullHtml += "<tr style='height:30px;'>";
            fullHtml += "<th style='width:63px;'>SL No</th><th style='width:93px;'>Date</th><th style='width:150px;'>Voucher No</th><th style='width:425px;'>Transaction</th><th style='width:150px;'>Increase Amount</th><th style='width:150px;'>Decrease Amount</th><th style='width:150px;'>Balance</th>";
            fullHtml += "</tr>";
            fullHtml += "</thead>";
            fullHtml += "<tbody>";

            decimal lastBalance = 0;
            for (int i = 0; i < data.Count; i++)
            {
                AccLedgerReportVm objLeger = data[i];

                string rowVcNo = !isPrint ? $"<a target='_blank' href='../AccTranMst/JournalDetails/{objLeger.AccTranMstId}'>{objLeger.VcNo}</a>" : $"{objLeger.VcNo}";
                string editIcon = !isPrint ? $"<a class='UpdateVcBtn' style='margin-left:5px;font-size:18px;' href='#' data-bs-toggle='modal' data-bs-target='#quickUpdateModal' data-id='{objLeger.AccTranMstId}'><i class='fa fa-edit'></i></a>" : "";
                string rowDr = (objLeger.AmountDrNum != 0) ? objLeger.AmountDr : "";
                string rowCr = (objLeger.AmountCrNum != 0) ? objLeger.AmountCr : "";
                string rowBalance = (objLeger.BalanceNum != 0) ? objLeger.Balance : "";

                fullHtml += "<tr>";
                fullHtml += $@"<td style='text-center:left;'>{objLeger.Sl}<span style='margin-left:3px;'>{(objLeger.IsAuto is true ? "" : editIcon)}</span></td><td style='text-align:left;'>{objLeger.VcDate}</td><td style='text-align:left;'>{rowVcNo}</td><td style='text-align:left;padding:5px;'><b>{objLeger.nr}</b><br />{objLeger.Narration}</td>
                    <td style='text-align:right;'>{rowDr}</td><td style='text-align:right;'>{rowCr}</td><td style='text-align:right;'>{rowBalance}</td>";
                fullHtml += "</tr>";

                lastBalance = objLeger.BalanceNum;
            }
            decimal declastBalance = Convert.ToDecimal(lastBalance);
            string strlastBalance = (declastBalance < 0) ? "(" + (declastBalance * -1).ToString("N2") + ")" : declastBalance.ToString("N2");


            fullHtml += $@"<tr style='font-weight:bold;height:30px;'>
                <td></td><td></td><td></td>
                <td style='text-align:right;'>Total (TK)</td>
                <td style='text-align:right;'>{data.Sum(o => o.AmountDrNum).ToString("N2")}</td>
                <td style='text-align:right;'>{data.Sum(o => o.AmountCrNum).ToString("N2")}</td>
                <td style='text-align:right;'>{strlastBalance}</td>
                </tr>";
        }
        fullHtml += "</tbody>";

        fullHtml += "</table>";

        return fullHtml;
    }

    #endregion
}
