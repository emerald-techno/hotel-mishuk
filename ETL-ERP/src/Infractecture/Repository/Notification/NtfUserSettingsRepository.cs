using AutoMapper;
using Domain.Entities.Notification;
using Domain.Utility.Common;
using Domain.ViewModel.Notification;
using Interface.Repository.Notification;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Notification;

public class NtfUserSettingsRepository : BaseRepository<NtfUserSettings>, INtfUserSettingsRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;

    public NtfUserSettingsRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
    {
        Db = db;
        _iMapper = iMapper;
    }

    #endregion

    #region Search

    public async Task<DataTablePagination<NtfUserSettingSearchVm, NtfUserSettingSearchVm>> SearchAsync(DataTablePagination<NtfUserSettingSearchVm, NtfUserSettingSearchVm> vm)
    {
        var searchResult = Context.NtfUserSettings
            .Include(x => x.Event)
            .Include(c => c.User)
            .AsQueryable();
        var model = vm.SearchModel;

        if (model == null) throw new Exception("Search User Notification Settings Not Found");

        if (!string.IsNullOrEmpty(vm.Search.Value))
        {
            var value = vm.Search.Value.Trim().ToLower();
            searchResult = searchResult.Where(c => c.Event.EventName.ToLower().Contains(value) || c.User.FullName.ToLower().Contains(value));
        }

        var totalRecords = await searchResult.CountAsync();
        if (totalRecords > 0)
        {
            vm.recordsTotal = totalRecords;
            vm.recordsFiltered = totalRecords;
            vm.draw = vm.LineDraw ?? 0;

            var data = await searchResult.OrderBy(c => c.Event.EventName)
                                         .Skip(vm.Start)
                                         .Take(vm.Length)
                                         .ToListAsync();

            vm.data = _iMapper.Map<List<NtfUserSettingSearchVm>>(data);

            var sl = vm.Start;

            foreach (var searchDto in vm.data)
            {
                var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                searchDto.SerialNo = ++sl;
                searchDto.EventName = filterData.Event.EventName;
                searchDto.UserName = filterData.User.FullName;
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
