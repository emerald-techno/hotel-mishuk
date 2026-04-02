using AutoMapper;
using Domain.Entities.HouseKeeping;
using Domain.Utility.Common;
using Domain.ViewModel.HouseKeeping.TaskAssign;
using Interface.Repository.HouseKeeping;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.HouseKeeping;

public class TaskAssignRepository : BaseRepository<HkTaskAssign>, ITaskAssignRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;

    public TaskAssignRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
    {
        Db = db;
        _iMapper = iMapper;
    }
    #endregion

    #region AssignTasks

    public async Task<DataTablePagination<TaskAssignSearchVm, TaskAssignSearchVm>> AssignTasks(DataTablePagination<TaskAssignSearchVm, TaskAssignSearchVm> vm)
    {
        var searchResult = Context.HkTaskAssigns
            .Include(t => t.Task)
            .Include(c => c.Assign)
                .ThenInclude(x => x.Room)
             .Include(x => x.Assign)
                .ThenInclude(f => f.HouseKeeper)
            .AsQueryable().Where(c => !c.IsDeleted);

        var model = vm.SearchModel;

        if (model == null)
        {
            throw new Exception("Search assign task not found..!!");
        }

        if (!string.IsNullOrEmpty(vm.Search.Value))
        {
            var value = vm.Search.Value.Trim().ToLower();
            searchResult = searchResult.Where(c => c.Task.Name.ToLower().Contains(value));
        }

        if (model.AssignId > 0)
        {
            searchResult = searchResult.Where(c => c.AssignId == model.AssignId);
        }

        var totalRecords = await searchResult.CountAsync();

        if (totalRecords > 0)
        {
            vm.recordsTotal = totalRecords;
            vm.recordsFiltered = totalRecords;
            vm.draw = vm.LineDraw ?? 0;

            var data = await searchResult.OrderBy(c => c.Task.Name)
                                         .Skip(vm.Start)
                                         .Take(vm.Length)
                                         .ToListAsync();

            vm.data = _iMapper.Map<List<TaskAssignSearchVm>>(data);

            var sl = vm.Start;

            foreach (var searchDto in vm.data)
            {
                var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                searchDto.SerialNo = ++sl;
                searchDto.TaskName = filterData.Task.Name;
                searchDto.AssignRoomNo = filterData.Assign.Room.RoomNo;
            }
        }
        return vm;
    }

    #endregion

    #region Search

    public async Task<DataTablePagination<TaskAssignSearchVm, TaskAssignSearchVm>> SearchAsync(DataTablePagination<TaskAssignSearchVm, TaskAssignSearchVm> vm)
    {
        var searchResult = Context.HkTaskAssigns
            .Include(a => a.ActionBy)
            .Include(t => t.Task)
            .Include(c => c.Assign)
                .ThenInclude(x => x.Room)
             .Include(x => x.Assign)
                .ThenInclude(f => f.HouseKeeper)
            .AsQueryable().Where(c => !c.IsDeleted);

        var model = vm.SearchModel;

        if (model == null) throw new Exception("Task not found");

        if (!string.IsNullOrEmpty(vm.Search.Value))
        {
            var value = vm.Search.Value.Trim().ToLower();
            searchResult = searchResult.Where(c => c.Assign.HouseKeeper.Name.ToLower().Contains(value) || c.Assign.Room.RoomNo.ToLower().Contains(value));
        }

        if (model.HouseKeeperId != null && model.HouseKeeperId > 0)
        {
            searchResult = searchResult.Where(c => c.Assign.HouseKeeperId == model.HouseKeeperId);
        }

        if (model.RoomId != null && model.RoomId > 0)
        {
            searchResult = searchResult.Where(c => c.Assign.RoomId == model.RoomId);
        }

        var resultData = await searchResult.ToListAsync();

        var result = resultData.GroupBy(c => c.AssignId).Select(x => x).ToList();

        var totalRecords = result.Count();
        if (totalRecords > 0)
        {
            vm.recordsTotal = totalRecords;
            vm.recordsFiltered = totalRecords;
            vm.draw = vm.LineDraw ?? 0;

            var data = resultData.GroupBy(c => c.AssignId)
                                         .Select(c => new TaskAssignSearchVm { AssignId = c.Key, TaskCount = c.ToList().Count })
                                         .Skip(vm.Start)
                                         .Take(vm.Length)
                                         .ToList();

            vm.data = _iMapper.Map<List<TaskAssignSearchVm>>(data);

            var sl = vm.Start;

            foreach (var searchDto in vm.data)
            {
                var filterData = resultData.Where(c => c.AssignId == searchDto.AssignId).FirstOrDefault();

                searchDto.AssignDate = filterData.AssignDate;
                searchDto.AssignStatus = filterData.Assign.Status;
                searchDto.AssignRoomNo = filterData?.Assign.Room.RoomNo;
                searchDto.AssignKepperName = filterData?.Assign.HouseKeeper.Name;
                searchDto.ActionByName = filterData?.ActionBy.FullName;
                searchDto.RoomWiseTaskList = string.Join(", ", resultData.Where(c => c.AssignId == searchDto.AssignId).Select(s => s.Task.Name).ToList());
            }

            vm.data = vm.data.OrderBy(c => c.AssignRoomNo).ToList();

            foreach (var item in vm.data)
            {
                item.SerialNo = ++sl;
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