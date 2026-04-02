using AutoMapper;
using Domain.Entities.Attendance;
using Domain.Utility.Common;
using Domain.ViewModel.Attendance.ShiftManagement;
using Interface.Repository.Attendance;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Attendance;

public class ShiftManagementRepository : BaseRepository<ShiftManagement>, IShiftManagementRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;

    public ShiftManagementRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
    {
        Db = db;
        _iMapper = iMapper;
    }

    #endregion

    #region Search
    public async Task<DataTablePagination<ShiftManagementSearchVm, ShiftManagementSearchVm>> SearchAsync(DataTablePagination<ShiftManagementSearchVm, ShiftManagementSearchVm> vm)
    {
        var searchResult = Context.ShiftManagements
            .Include(x => x.PermanentShift)
            .Include(x => x.DutyShift)
            .Include(x => x.Employee)
            .AsQueryable().Where(c => !c.IsDeleted);
        var model = vm.SearchModel;

        if (model == null) throw new Exception("Search duty shift not found");

        if (!string.IsNullOrEmpty(vm.Search.Value))
        {
            var value = vm.Search.Value.Trim().ToLower();
            searchResult = searchResult.Where(c => c.PermanentShift.ShiftName.ToLower().Contains(value));
        }

        if (model.SelectYear > 0 && model.SelectMonth > 0)
        {
            searchResult = searchResult.Where(x => x.Month.Year == model.SelectYear && x.Month.Month == model.SelectMonth);
        }

        if (model.PermanentShiftId > 0)
        {
            searchResult = searchResult.Where(x => x.PermanentShiftId == model.PermanentShiftId);
        }

        if (model.DutyShiftId > 0)
        {
            searchResult = searchResult.Where(x => x.DutyShiftId == model.DutyShiftId);
        }

        if (model.EmployeeId > 0)
        {
            searchResult = searchResult.Where(x => x.EmployeeId == model.EmployeeId);
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

            vm.data = _iMapper.Map<List<ShiftManagementSearchVm>>(data);

            var sl = vm.Start;

            foreach (var searchDto in vm.data)
            {
                var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                searchDto.SerialNo = ++sl;
                searchDto.PermanentShiftName = filterData.PermanentShift.ShiftName;
                searchDto.DutyShiftName = filterData.DutyShift.ShiftName;
                searchDto.EmployeeName = filterData.Employee?.Name;
                searchDto.MonthStr = filterData.Month.ToString("dd/MM/yyyy");
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
