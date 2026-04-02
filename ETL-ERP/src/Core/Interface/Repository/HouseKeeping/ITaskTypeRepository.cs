using Domain.Entities.HouseKeeping;
using Domain.Utility.Common;
using Domain.ViewModel.HouseKeeping.TaskType;
using Interface.Base;

namespace Interface.Repository.HouseKeeping;

public interface ITaskTypeRepository : IRepository<HkTaskType>
{
    Task<DataTablePagination<HkTaskTypeSearchVm, HkTaskTypeSearchVm>>
       SearchAsync(DataTablePagination<HkTaskTypeSearchVm, HkTaskTypeSearchVm> vm);
}