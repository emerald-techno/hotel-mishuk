using AutoMapper;
using Domain.Entities.Accounting;
using Domain.Utility.Common;
using Domain.ViewModel.Accounting.AccTranDtl;
using Domain.ViewModel.Accounting.AccTranMst;
using Interface.Repository.Accounts;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Persistence.DapperModel;
using Repository.Base;
using DU = Domain.Utility;

namespace Repository.Accounts
{
    public class AccTranMstRepository : BaseRepository<AccTranMst>, IAccTranMstRepository, IDisposable
    {
        #region Config
        private ApplicationDbContext Context => Db as ApplicationDbContext;
        private readonly IApplicationReadDbConnection _iReadDbConnection;
        private readonly IMapper _iMapper;

        public AccTranMstRepository(ApplicationDbContext db, IMapper iMapper,IApplicationReadDbConnection iReadDbConnection) : base(db)
        {
            Db = db;
            _iMapper = iMapper;
            _iReadDbConnection = iReadDbConnection;
        }

        #endregion

        #region Search

        public async Task<DataTablePagination<AccTranMstSearchVm, AccTranMstSearchVm>> SearchAsync(DataTablePagination<AccTranMstSearchVm, AccTranMstSearchVm> vm)
        {
            var searchResult = Context.AccTranMsts.AsQueryable().Where(c => !c.IsDeleted);
            var model = vm.SearchModel;

            if (model == null) throw new Exception("Account Transaction Master not found");

            searchResult = searchResult.Where(c => c.SubVacType != VoucherType.OpeningVoucher);

            if (!string.IsNullOrEmpty(model.VcType))
            {
                searchResult = searchResult.Where(c => c.VcType == model.VcType);
            }

            if (!string.IsNullOrEmpty(model.FormDateStr))
            {
                var formDate = (DateTime)(!string.IsNullOrEmpty(model.FormDateStr) ? DU.Utility.ConvertStrToDate(model.FormDateStr) : model.VcDate);
                searchResult = searchResult.Where(c => c.VcDate >= formDate);
            }

            if (!string.IsNullOrEmpty(model.ToDateStr))
            {
                var toDate = (DateTime)(!string.IsNullOrEmpty(model.ToDateStr) ? DU.Utility.ConvertStrToDate(model.ToDateStr) : model.VcDate);
                searchResult = searchResult.Where(c => c.VcDate <= toDate);
            }

            if (!string.IsNullOrEmpty(vm.Search.Value))
            {
                var value = vm.Search.Value.Trim().ToLower();
                searchResult = searchResult.Where(c => c.VcNo.ToLower().Contains(value));
            }

            var totalRecords = await searchResult.CountAsync();
            if (totalRecords > 0)
            {
                vm.recordsTotal = totalRecords;
                vm.recordsFiltered = totalRecords;
                vm.draw = vm.LineDraw ?? 0;

                var data = await searchResult.OrderByDescending(c => c.VcDate)
                                             .Skip(vm.Start)
                                             .Take(vm.Length)
                                             .ToListAsync();

                vm.data = _iMapper.Map<List<AccTranMstSearchVm>>(data);

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

        #region JournalDetails

        public async Task<AccTranMstVm> GetJournalDetails(long id)
        {
            if (id == 0) throw new Exception("No Data Found..!");
            var data = await Context.AccTranMsts
                                    .Include(c => c.Currency)
                                    .Include(c => c.FinYear)
                                    .Include(d => d.AccTranDtls)
                                        .ThenInclude(l => l.LedgerDr)
                                    .Include(d => d.AccTranDtls)
                                        .ThenInclude(l => l.LedgerCr)
                                    .FirstOrDefaultAsync(x => x.Id == id);

            if (data == null) throw new Exception("No Data Found..!");

            var model = _iMapper.Map<AccTranMstVm>(data);

            model.CurrencyName = data.Currency.CurrencyName;
            model.FinYearName = data.FinYear.YearName;

            if(model.AccAccountId > 0)
            {
                var accountInfo = await Context.AccLedgers.FirstOrDefaultAsync(x => x.Id == model.AccAccountId);
                if (accountInfo == null) 
                    throw new Exception("No Account Information Found..!");

                model.AccAccountName = accountInfo.LedgerName;
            }

            var dtlList = new List<JournalDtlVm>();

            if (data.AccTranDtls.Count > 0)
            {
                foreach (var item in data.AccTranDtls)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        var dtlModel = new JournalDtlVm();

                        dtlModel.LedgerId = (long)item.LedgerDrId;
                        dtlModel.LedgerName = item.LedgerDr.LedgerName;
                        dtlModel.LedgerCode = item.LedgerDr.LedgerCode;
                        dtlModel.AmountDr = item.AmountDr;

                        if (i == 1)
                        {
                            dtlModel.LedgerId = (long)item.LedgerCrId;
                            dtlModel.LedgerName = item.LedgerCr.LedgerName;
                            dtlModel.LedgerCode = item.LedgerCr.LedgerCode;
                            dtlModel.AmountCr = item.AmountCr;
                        }

                        dtlModel.TranMstId = item.TranMstId;

                        dtlList.Add(dtlModel);
                    }
                }
            }

            dtlList = dtlList.DistinctBy(c => c.LedgerId).ToList();

            if (dtlList.Count > 0)
            {
                foreach (var item in dtlList)
                {
                    item.AmountDr = data.AccTranDtls.Where(c => c.LedgerDrId == item.LedgerId).Sum(x => x.AmountDr);
                    item.AmountCr = data.AccTranDtls.Where(c => c.LedgerCrId == item.LedgerId).Sum(x => x.AmountCr);
                }
            }

            model.JournalDtlVms = dtlList;

            model.JournalDtlVms = dtlList.OrderByDescending(o => o.AmountDr).ThenBy(o => o.LedgerName).ToList();

            return model;
        }

        #endregion

        #region OpeningDetails

        public async Task<AccTranMstVm?> GetOpeningDetails(long finYearId)
        {
            if (finYearId == 0) throw new Exception("No Data Found..!");

            var data = await Context.AccTranMsts
                                    .Include(c => c.Currency)
                                    .Include(c => c.FinYear)
                                    .Include(d => d.AccTranDtls)
                                        .ThenInclude(l => l.LedgerDr)
                                    .Include(d => d.AccTranDtls)
                                        .ThenInclude(l => l.LedgerCr)
                                    .FirstOrDefaultAsync(x => x.FinYearId == finYearId && x.VcType == VoucherType.OpeningVoucher && !x.IsDeleted);

            if (data == null)
                return null;

            var model = _iMapper.Map<AccTranMstVm>(data);

            model.CurrencyName = data.Currency.CurrencyName;
            model.FinYearName = data.FinYear.YearName;

            if (model.AccTranDtls.Count > 0)
            {
                foreach (var item in model.AccTranDtls)
                {
                    var filterData = data.AccTranDtls.FirstOrDefault(x=>x.Id == item.Id);

                    item.LedgerDrName = filterData.LedgerDr?.LedgerName;
                    item.LedgerCrName = filterData.LedgerCr?.LedgerName;
                    item.LedgerDrId = filterData.LedgerDr?.Id;
                    item.LedgerCrId = filterData.LedgerCr?.Id;
                }
            }

            return model;
        }

        #endregion

        #region ClosingDetails

        public async Task<AccTranMstVm?> GetClosingDetails(long finYearId)
        {
            if (finYearId == 0) throw new Exception("No Data Found..!");

            string query = $@"select f.LedgerId,f.LedgerName,f.LedgerId LedgerDrId,f.LedgerName LedgerDrName,f.AmountDr,f.AmountCr,d.LedgerCode,d.LedgerName,d.HeadId,d.HeadCode,d.HeadName,d.HeadId2,d.HeadName2,d.HeadCode2,d.HeadId3,d.HeadCode3,d.HeadName3,d.HeadId4,d.HeadName4,d.HeadCode4
            from (
            select d.LedgerId,d.LedgerName, case when sum(OpeningDr) > sum(OpeningCr) then sum(OpeningDr) - sum(OpeningCr) else 0 end AmountDr
            , case when sum(OpeningDr) < sum(OpeningCr) then sum(OpeningCr) - sum(OpeningDr)  else 0 end AmountCr
            from (
            select td.LedgerDrId LedgerId,al.LedgerName,isnull(td.AmountDr,0) OpeningDr,0 OpeningCr,0 AmountDr,0 AmountCr
            from AccTranMsts tm 
            inner join AccTranDtls td on tm.Id = td.TranMstId
            inner join AccLedgers al on al.Id = td.LedgerDrId
            where td.IsDeleted = 0 and tm.IsDeleted = 0 and tm.FinYearId = {finYearId-1}
            union all
            select td.LedgerCrId LedgerId,al.LedgerName,0 OpeningDr,isnull(td.AmountCr,0) OpeningCr,0 AmountDr,0 AmountCr
            from AccTranMsts tm 
            inner join AccTranDtls td on tm.Id = td.TranMstId
            inner join AccLedgers al on al.Id = td.LedgerCrId
            where td.IsDeleted = 0 and tm.IsDeleted = 0 and tm.FinYearId = {finYearId - 1}
            )d group by d.LedgerId,d.LedgerName
            )f
            inner join (
            select l.Id LedgerId,l.LedgerName,l.LedgerCode,h.Id HeadId,h.HeadCode,h.HeadName,h2.Id HeadId2,h2.HeadName HeadName2,h2.HeadCode HeadCode2,h3.Id HeadId3,h3.HeadName HeadName3,h3.HeadCode HeadCode3,
            h4.Id HeadId4,h4.HeadName HeadName4,h4.HeadCode HeadCode4
            from AccLedgers l
            inner join AccHeads h on h.Id = l.HeadId
            left join AccHeads h2 on h2.Id = h.ParentHeadId
            left join AccHeads h3 on h3.Id = h2.ParentHeadId
            left join AccHeads h4 on h4.Id = h3.ParentHeadId
            ) d on d.LedgerId = f.LedgerId
            WHERE 1 = 1 
            and d.HeadCode not like '2%' and HeadCode not like '4%'
            order by d.HeadId,D.HeadId2,d.HeadId3,d.HeadCode4";

            var data = await _iReadDbConnection.QueryAsync<AccTranDtlVm>(query);


            var model = new AccTranMstVm();

            model.CurrencyName = "";
            model.FinYearName = "";
            model.AccTranDtls = data.ToList();

            return model;
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            Context.Dispose();
        }

        #endregion
    }
}
