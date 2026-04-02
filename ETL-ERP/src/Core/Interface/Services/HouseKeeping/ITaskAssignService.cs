using Domain.Entities.HouseKeeping;
using Domain.Utility.Common;
using Domain.ViewModel.HouseKeeping.TaskAssign;
using Interface.Base;

namespace Interface.Services.HouseKeeping;

public interface ITaskAssignService : IService<HkTaskAssign>
{
    Task<bool> AddAssignTask(TaskAssignSaveVm vm);

    Task<DataTablePagination<TaskAssignSearchVm, TaskAssignSearchVm>>
        SearchAsync(DataTablePagination<TaskAssignSearchVm, TaskAssignSearchVm> model);

    Task<List<TaskAssignSearchVm>> GetTaskByAssignId(long assignId);
}