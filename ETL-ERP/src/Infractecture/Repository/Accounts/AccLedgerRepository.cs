using AutoMapper;
using Domain.Entities.Accounting;
using Domain.Utility.Common;
using Domain.ViewModel.Accounting.AccLedger;
using Interface.Repository.Accounts;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Persistence.DapperModel;
using Repository.Base;

namespace Repository.Accounts
{
    public class AccLedgerRepository : BaseRepository<AccLedger>, IAccLedgerRepository, IDisposable
    {
        #region Config
        private ApplicationDbContext Context => Db as ApplicationDbContext;
        private readonly IApplicationReadDbConnection _iReadDbConnection;
        private readonly IMapper _iMapper;

        public AccLedgerRepository(ApplicationDbContext db, IMapper iMapper, IApplicationReadDbConnection iReadDbConnection) : base(db)
        {
            Db = db;
            _iMapper = iMapper;
            _iReadDbConnection = iReadDbConnection;
        }

        #endregion

        #region Search

        public async Task<DataTablePagination<AccLedgerSearchVm, AccLedgerSearchVm>> SearchAsync(DataTablePagination<AccLedgerSearchVm, AccLedgerSearchVm> vm)
        {
            var searchResult = Context.AccLedgers
                .Include(a => a.Head)
                .AsQueryable().Where(c => !c.IsDeleted);
            var model = vm.SearchModel;

            if (model == null) throw new Exception("Account Ledgers not found");

            /* For Advance Search */

            if (model.HeadId > 0)
            {
                searchResult = searchResult.Where(c => c.HeadId == model.HeadId);
            }
            if (!string.IsNullOrEmpty(model.LedgerName))
            {
                searchResult = searchResult.Where(c => c.LedgerName == model.LedgerName);
            }
            if (!string.IsNullOrEmpty(model.LedgerCode))
            {
                searchResult = searchResult.Where(c => c.LedgerCode == model.LedgerCode);
            }

            if (!string.IsNullOrEmpty(vm.Search.Value))
            {
                var value = vm.Search.Value.Trim().ToLower();
                searchResult = searchResult.Where(c => c.LedgerName.ToLower().Contains(value));
            }
            var totalRecords = await searchResult.CountAsync();
            if (totalRecords > 0)
            {
                vm.recordsTotal = totalRecords;
                vm.recordsFiltered = totalRecords;
                vm.draw = vm.LineDraw ?? 0;

                var data = await searchResult.OrderBy(c => c.LedgerCode)
                                             .Skip(vm.Start)
                                             .Take(vm.Length)
                                             .ToListAsync();

                vm.data = _iMapper.Map<List<AccLedgerSearchVm>>(data);

                var sl = vm.Start;

                foreach (var searchDto in vm.data)
                {
                    var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                    searchDto.SerialNo = ++sl;
                    searchDto.HeadName = filterData?.Head.HeadName;
                    searchDto.HeadCode = filterData?.Head.HeadCode;
                }
            }
            return vm;
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            Context.Dispose();
        }

        #endregion

        #region GetTotalLedgerCount
        public async Task<long> GetTotalLedgerByHeadId(long headId)
        {
            var query = $"select count(*) from AccLedgers where HeadId = {headId} and IsDeleted = 0";
            var count = await _iReadDbConnection.QueryFirstOrDefaultAsync<string>(query);
            if (!string.IsNullOrEmpty(count))
            {
                return Convert.ToInt64(count);
            }
            return 0;
            //var data = Context.AccLedgers.Where(o => o.HeadId == headId).Count();
            //return data;
        }
        #endregion
    }
}
