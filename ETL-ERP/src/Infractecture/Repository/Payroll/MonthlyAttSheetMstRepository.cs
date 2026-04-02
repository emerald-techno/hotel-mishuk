using AutoMapper;
using Domain.Entities.Payroll;
using Domain.Utility.Common;
using Domain.ViewModel.Attendance.MonthlyAttSheet;
using Interface.Repository.Payroll;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Payroll
{
    public class MonthlyAttSheetMstRepository : BaseRepository<MonthlyAttendanceSheetMst>, IMonthlyAttSheetMstRepository, IDisposable
    {
        #region Config
        private ApplicationDbContext Context => Db as ApplicationDbContext;
        private readonly IMapper _iMapper;

        public MonthlyAttSheetMstRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
        {
            Db = db;
            _iMapper = iMapper;
        }

        #endregion

        #region Search

        public async Task<DataTablePagination<MonthlyAttSheetSearchVm, MonthlyAttSheetSearchVm>> SearchAsync(DataTablePagination<MonthlyAttSheetSearchVm, MonthlyAttSheetSearchVm> vm)
        {
            var searchResult = Context.MonthlyAttendanceSheetMsts
                                      .Include(c => c.MonthlyAttendanceSheetDtls)
                                      .AsQueryable()
                                      .Where(c => !c.IsDeleted);

            var model = vm.SearchModel;

            if (model == null) throw new Exception("Search Attendance not found");

            if (!string.IsNullOrEmpty(vm.Search.Value))
            {
                var value = vm.Search.Value.Trim().ToLower();
                searchResult = searchResult.Where(c => c.Month.ToString().ToLower().Contains(value));
            }

            var totalRecords = await searchResult.CountAsync();

            if (totalRecords > 0)
            {
                vm.recordsTotal = totalRecords;
                vm.recordsFiltered = totalRecords;
                vm.draw = vm.LineDraw ?? 0;

                var data = await searchResult.OrderBy(c => c.Month)
                                             .Skip(vm.Start)
                                             .Take(vm.Length)
                                             .ToListAsync();

                vm.data = _iMapper.Map<List<MonthlyAttSheetSearchVm>>(data);

                var sl = vm.Start;

                foreach (var searchDto in vm.data)
                {
                    var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                    searchDto.SerialNo = ++sl;
                    searchDto.TotalDay = (short)(filterData?.MonthlyAttendanceSheetDtls.FirstOrDefault().TotalDays);
                    searchDto.Holiday = (short)(filterData?.MonthlyAttendanceSheetDtls.FirstOrDefault().Holidays);
                    searchDto.OffDay = (short)(filterData?.MonthlyAttendanceSheetDtls.FirstOrDefault().OffDays);
                }
            }
            return vm;
        }

        #endregion

        #region GetMonthlyAttendanceSheet

        public async Task<MonthlyAttendanceSheetMst> GetSheetByIdAsync(long id)
        {
            var data = await Context.MonthlyAttendanceSheetMsts
                .Include(c => c.MonthlyAttendanceSheetDtls)
                    .ThenInclude(e => e.Employee)
                        .ThenInclude(d => d.Designation)
                .Include(c => c.MonthlyAttendanceSheetDtls)
                    .ThenInclude(e => e.Employee)
                        .ThenInclude(dp => dp.Department)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (data == null) throw new Exception("No Payroll Data Found");

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