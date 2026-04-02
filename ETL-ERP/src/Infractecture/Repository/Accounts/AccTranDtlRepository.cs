using AutoMapper;
using Domain.Entities.Accounting;
using Domain.Utility.Common;
using Domain.ViewModel.Accounting.AccTranDtl;
using Interface.Repository.Accounts;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Accounts
{
    public class AccTranDtlRepository : BaseRepository<AccTranDtl>, IAccTranDtlRepository, IDisposable
    {
        #region Config

        private ApplicationDbContext Context => Db as ApplicationDbContext;
        private readonly IMapper _iMapper;

        public AccTranDtlRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
        {
            Db = db;
            _iMapper = iMapper;
        }

        #endregion

        public async Task<DataTablePagination<AccTranDtlSearchVm, AccTranDtlSearchVm>> SearchAsync(DataTablePagination<AccTranDtlSearchVm, AccTranDtlSearchVm> vm)
        {
            var searchResult = Context.AccTranDtls
                 .Include(c => c.TranMst)
                 .Include(d => d.LedgerCr)
                 .Include(g => g.LedgerDr)
                 .AsQueryable()
                 .Where(c => !c.IsDeleted);

            var model = vm.SearchModel;

            if (model == null) throw new Exception("Search Account Opening not Found");

            if (!string.IsNullOrEmpty(model.VcType))
            {
                searchResult = searchResult.Where(c => c.TranMst.SubVacType == model.VcType);
            }

            if (!string.IsNullOrEmpty(vm.Search.Value))
            {
                var value = vm.Search.Value.Trim().ToLower();
                searchResult = searchResult.Where(c => c.TranMst.VcNo.ToLower().Contains(value));
            }
            var totalRecords = await searchResult.CountAsync();

            if (totalRecords > 0)
            {
                vm.recordsTotal = totalRecords;
                vm.recordsFiltered = totalRecords;
                vm.draw = vm.LineDraw ?? 0;

                var data = await searchResult.OrderByDescending(c => c.TranMst.VcDate)
                                             .Skip(vm.Start)
                                             .Take(vm.Length)
                                             .ToListAsync();

                vm.data = _iMapper.Map<List<AccTranDtlSearchVm>>(data);

                var sl = vm.Start;

                foreach (var searchDto in vm.data)
                {
                    var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                    searchDto.SerialNo = ++sl;
                    searchDto.Ledger = filterData?.LedgerDr?.LedgerName;
                }
            }
            return vm;
        }

        #region Dispose

        public void Dispose()
        {
            Context.Dispose();
        }

        #endregion
    }
}
