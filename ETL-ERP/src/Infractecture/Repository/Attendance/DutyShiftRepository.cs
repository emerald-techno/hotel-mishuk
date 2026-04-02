using AutoMapper;
using Domain.Entities.Attendance;
using Domain.Utility.Common;
using Domain.ViewModel.Attendance.DutyShift;
using Interface.Repository.Attendance;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Attendance;

public class DutyShiftRepository : BaseRepository<DutyShift>, IDutyShiftRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;

    public DutyShiftRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
    {
        Db = db;
        _iMapper = iMapper;
    }

    #endregion

    #region Search
    public async Task<DataTablePagination<DutyShiftSearchVm, DutyShiftSearchVm>> SearchAsync(DataTablePagination<DutyShiftSearchVm, DutyShiftSearchVm> vm)
    {
        var searchResult = Context.DutyShifts.AsQueryable().Where(c => !c.IsDeleted);
        var model = vm.SearchModel;

        if (model == null) throw new Exception("Search duty shift not found");

        if (!string.IsNullOrEmpty(vm.Search.Value))
        {
            var value = vm.Search.Value.Trim().ToLower();
            searchResult = searchResult.Where(c => c.ShiftName.ToLower().Contains(value));
        }
        var totalRecords = await searchResult.CountAsync();
        if (totalRecords > 0)
        {
            vm.recordsTotal = totalRecords;
            vm.recordsFiltered = totalRecords;
            vm.draw = vm.LineDraw ?? 0;

            var data = await searchResult.OrderBy(c => c.ShiftName)
                                         .Skip(vm.Start)
                                         .Take(vm.Length)
                                         .ToListAsync();

            vm.data = _iMapper.Map<List<DutyShiftSearchVm>>(data);

            var sl = vm.Start;

            foreach (var searchDto in vm.data)
            {
                var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                searchDto.SerialNo = ++sl;
                searchDto.StartTimeStr = searchDto.StartTime.ToString("t");
                searchDto.EndTimeStr = searchDto.EndTime.ToString("t");
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
}
