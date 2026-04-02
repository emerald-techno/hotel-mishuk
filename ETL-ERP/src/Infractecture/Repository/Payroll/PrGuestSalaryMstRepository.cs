using AutoMapper;
using Domain.Entities;
using Domain.Entities.Payroll;
using Domain.Utility.Common;
using Domain.ViewModel.Payroll.PrGuestSalary;
using Interface.Repository.Payroll;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Payroll
{
    public class PrGuestSalaryMstRepository : BaseRepository<PrGuestSalaryMst>, IPrGuestSalaryMstRepository, IDisposable
    {
        #region Config

        private ApplicationDbContext Context => Db as ApplicationDbContext;
        private readonly IMapper _iMapper;
        public PrGuestSalaryMstRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
        {
            Db = db;
            _iMapper = iMapper;
        }

        #endregion

        #region Search

        public async Task<DataTablePagination<PrGuestSalaryMstSearchVm, PrGuestSalaryMstSearchVm>> SearchAsync(DataTablePagination<PrGuestSalaryMstSearchVm, PrGuestSalaryMstSearchVm> vm)
        {
            var searchResult = Context.PrGuestSalaryMsts
                .Include(c => c.PrGuestSalaryDtls)
                .AsQueryable().Where(c => !c.IsDeleted);

            var model = vm.SearchModel;

            if (model == null) throw new Exception("Guest salary not found");

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

                vm.data = _iMapper.Map<List<PrGuestSalaryMstSearchVm>>(data);

                var sl = vm.Start;

                foreach (var searchDto in vm.data)
                {
                    var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                    searchDto.SerialNo = ++sl;
                    searchDto.EmployeeCount = filterData?.PrGuestSalaryDtls.Count() ?? 0;
                }
            }
            return vm;
        }

        #endregion

        #region GetPrGuestSalary

        public async Task<PrGuestSalaryMst> GetPrGuestSalaryMstByIdAsync(long id)
        {
            var data = await Context.PrGuestSalaryMsts
                .Include(c => c.ApprovedBy)
                .Include(c => c.PrGuestSalaryDtls)
                .ThenInclude(e => e.Employee)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (data == null) throw new Exception("No data found");

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
