using Domain.Entities.HouseKeeping;
using Domain.Utility.Common;
using Domain.ViewModel.HouseKeeping.TaskAssign;
using Interface.Base;

namespace Interface.Repository.HouseKeeping;

public interface ITaskAssignRepository : IRepository<HkTaskAssign>
{
    Task<DataTablePagination<TaskAssignSearchVm, TaskAssignSearchVm>> 
        AssignTasks(DataTablePagination<TaskAssignSearchVm, TaskAssignSearchVm> vm);

    Task<DataTablePagination<TaskAssignSearchVm, TaskAssignSearchVm>>
        SearchAsync(DataTablePagination<TaskAssignSearchVm, TaskAssignSearchVm> vm);
}