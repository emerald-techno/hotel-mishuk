using AutoMapper;
using Domain.Entities.HouseKeeping;
using Domain.Utility.Common;
using Domain.ViewModel.HouseKeeping.Dashboard;
using Domain.ViewModel.HouseKeeping.TaskName;
using Interface.Repository.HouseKeeping;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.HouseKeeping;

public class TaskNameRepository : BaseRepository<HkTaskName>, ITaskNameRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;

    public TaskNameRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
    {
        Db = db;
        _iMapper = iMapper;
    }
    #endregion

    public async Task<DataTablePagination<HkTaskNameSearchVm, HkTaskNameSearchVm>> SearchAsync(DataTablePagination<HkTaskNameSearchVm, HkTaskNameSearchVm> vm)
    {
        var searchResult = Context.HkTaskNames
            .Include(x => x.Type)
            .AsQueryable().Where(c => !c.IsDeleted);
        var model = vm.SearchModel;

        if (model == null) throw new Exception("Search task type not found");

        if (!string.IsNullOrEmpty(vm.Search.Value))
        {
            var value = vm.Search.Value.Trim().ToLower();
            searchResult = searchResult.Where(c => c.Name.ToLower().Contains(value));
        }
        var totalRecords = await searchResult.CountAsync();
        if (totalRecords > 0)
        {
            vm.recordsTotal = totalRecords;
            vm.recordsFiltered = totalRecords;
            vm.draw = vm.LineDraw ?? 0;

            var data = await searchResult.OrderBy(c => c.Name)
                                         .Skip(vm.Start)
                                         .Take(vm.Length)
                                         .ToListAsync();

            vm.data = _iMapper.Map<List<HkTaskNameSearchVm>>(data);

            var sl = vm.Start;

            foreach (var searchDto in vm.data)
            {
                var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                searchDto.SerialNo = ++sl;
                searchDto.TypeName = filterData.Type.TypeName;
            }
        }
        return vm;
    }

    public async Task<HkDashboard> GetHkDashboardData()
    {
        var roomAssignData = Context.HkRoomAssigns.AsNoTracking();
        var roomData = Context.HtRoomInfos
            .Where(x => x.CleaningStatus != Domain.Enums.AppEnums.CleaningStatusEnum.VC && x.CleaningStatus != Domain.Enums.AppEnums.CleaningStatusEnum.O)
            .AsNoTracking();
        var empData = Context.Employees.AsNoTracking();

        var model = new HkDashboard();

        model.DirtyRooms = await roomData.OrderBy(x => x.RoomNo).ToListAsync();
        model.DirtyRoom = await roomData.Where(x => !x.IsDeleted && x.CleaningStatus == Domain.Enums.AppEnums.CleaningStatusEnum.VD).CountAsync();
        model.AvailableWorker = await empData.Where(x => !x.IsDeleted).CountAsync();
        model.TaskCompleted = await roomAssignData.Where(x => x.Status == Domain.Enums.AppEnums.RoomAssignEnum.Completed).CountAsync();
        model.TaskRemaining = await roomAssignData.Where(x => x.Status == Domain.Enums.AppEnums.RoomAssignEnum.Assigned || 
                                    x.Status == Domain.Enums.AppEnums.RoomAssignEnum.Running).CountAsync();
        var searchData = await roomAssignData.ToListAsync();

        var listItem = new List<TaskList>();

        foreach (var item in searchData)
        {
            var houseKeeper = empData.Where(x => x.Id == item.HouseKeeperId).FirstOrDefault();
            var room = roomData.Where(x => x.Id == item.RoomId).FirstOrDefault();

            if (houseKeeper != null && room != null)
            {
                var data = new TaskList
                {
                    HouseKeeperName = houseKeeper.Name,
                    RoomNo = room.RoomNo,
                    Status = item.Status,
                };

                listItem.Add(data);
            }
        }

        model.TaskLists = listItem;

        return model;
    }

    #region Dispose

    public void Dispose()
    {
        Context.Dispose();
    }
    #endregion
}
