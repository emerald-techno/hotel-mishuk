using AutoMapper;
using Domain.Entities;
using Domain.Entities.Payroll;
using Domain.Utility.Common;
using Domain.ViewModel.Payroll.PrArrearMst;
using Interface.Repository.Payroll;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Payroll
{
    public class PrArrearMstRepository : BaseRepository<PrArrearMst>, IPrArrearMstRepository, IDisposable
    {
        #region Config

        private ApplicationDbContext Context => Db as ApplicationDbContext;
        private readonly IMapper _iMapper;
        public PrArrearMstRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
        {
            Db = db;
            _iMapper = iMapper;
        }

        #endregion

        #region Search

        public async Task<DataTablePagination<PrArrearMstSearchVm, PrArrearMstSearchVm>> SearchAsync(DataTablePagination<PrArrearMstSearchVm, PrArrearMstSearchVm> vm)
        {
            var searchResult = Context.PrArrearMsts
                .Include(c => c.PrArrearDtls)
                .AsQueryable().Where(c => !c.IsDeleted);

            var model = vm.SearchModel;

            if (model == null) throw new Exception("Arrear not found");

            if (!string.IsNullOrEmpty(vm.Search.Value))
            {
                var value = vm.Search.Value.Trim().ToLower();
                searchResult = searchResult.Where(c => c.Year.ToString().ToLower().Contains(value));
            }
            var totalRecords = await searchResult.CountAsync();
            if (totalRecords > 0)
            {
                vm.recordsTotal = totalRecords;
                vm.recordsFiltered = totalRecords;
                vm.draw = vm.LineDraw ?? 0;

                var data = await searchResult.OrderBy(c => c.Year)
                                             .Skip(vm.Start)
                                             .Take(vm.Length)
                                             .ToListAsync();

                vm.data = _iMapper.Map<List<PrArrearMstSearchVm>>(data);

                var sl = vm.Start;

                foreach (var searchDto in vm.data)
                {
                    var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                    searchDto.SerialNo = ++sl;
                    searchDto.EmployeeCount = filterData?.PrArrearDtls.Count() ?? 0;
                }
            }
            return vm;
        }

        #endregion

        #region GetPrArrear

        public async Task<PrArrearMst> GetPrArrearByIdAsync(long id)
        {
            var data = await Context.PrArrearMsts
                .Include(c => c.ApprovedBy)
                .Include(c => c.PrArrearDtls)
                .ThenInclude(e => e.Employee)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (data == null) throw new Exception("No Data Found");

            return data;
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
