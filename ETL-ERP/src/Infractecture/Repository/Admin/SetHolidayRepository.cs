using AutoMapper;
using Domain.Entities.Admin;
using Domain.Utility.Common;
using Domain.ViewModel.Admin.SetHoliday;
using Interface.Repository.Admin;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Admin
{
    public class SetHolidayRepository : BaseRepository<SetHoliday>, ISetHolidayRepository, IDisposable
    {
        #region Config
        private ApplicationDbContext Context => Db as ApplicationDbContext;
        private readonly IMapper _iMapper;

        public SetHolidayRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
        {
            Db = db;
            _iMapper = iMapper;
        }
        #endregion

        #region Search

        public async Task<DataTablePagination<SetHolidaySearchVm, SetHolidaySearchVm>> SearchAsync(DataTablePagination<SetHolidaySearchVm, SetHolidaySearchVm> vm)
        {
            var searchResult = Context.SetHolidays.AsQueryable().Where(c => !c.IsDeleted);
            var model = vm.SearchModel;

            if (model == null) throw new Exception("Holiday not found");

            if (model.HolidayYear >= 1999)
            {
                searchResult = searchResult.Where(x => x.HolidayYear == model.HolidayYear);
            }

            if (!string.IsNullOrEmpty(vm.Search.Value))
            {
                var value = vm.Search.Value.Trim().ToLower();
                searchResult = searchResult.Where(c => c.HolidayName.ToLower().Contains(value));
            }

            var totalRecords = await searchResult.CountAsync();
            if (totalRecords > 0)
            {
                vm.recordsTotal = totalRecords;
                vm.recordsFiltered = totalRecords;
                vm.draw = vm.LineDraw ?? 0;

                var data = await searchResult.OrderBy(c => c.HolidayName)
                                             .Skip(vm.Start)
                                             .Take(vm.Length)
                                             .ToListAsync();

                vm.data = _iMapper.Map<List<SetHolidaySearchVm>>(data);

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

        #region IsHoliday

        public async Task<bool> IsHoliday(DateTime date)
        {
            var year = date.Year;
            var month = date.Month; // 5 = may, 6 = june, 7 = july
            var monthFirstDay = new DateTime(year, month, 1);
            var monthLastDay = monthFirstDay.AddMonths(1).AddDays(-1);

            if (date.Date == monthFirstDay.AddDays(14))
            {
                var result = date.Date;
            }

            var holidays = await Context.SetHolidays.Where(c => c.HolidayYear == date.Year && monthFirstDay.Date <= c.StartDate.Date && monthLastDay.Date >= c.EndDate.Date && !c.IsDeleted).ToListAsync();

            var holidayDateList = new List<DateTime>();

            if (holidays != null && holidays.Count > 0)
            {
                foreach (var holiday in holidays)
                {
                    var start = holiday.StartDate;
                    var end = holiday.EndDate;

                    if (end >= start)
                    {
                        for (var dt = start; dt <= end; dt = dt.AddDays(1))
                        {
                            holidayDateList.Add(dt);
                        }
                    }
                }
            }

            var isHoliday = holidayDateList.Any(c => c.Date == date);

            return isHoliday;
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
