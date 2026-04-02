using Domain.Entities.HouseKeeping;
using Domain.Utility.Common;
using Domain.ViewModel.HouseKeeping.TaskType;
using Interface.Base;

namespace Interface.Services.HouseKeeping;

public interface ITaskTypeService : IService<HkTaskType>
{
    Task<DataTablePagination<HkTaskTypeSearchVm, HkTaskTypeSearchVm>>
        SearchAsync(DataTablePagination<HkTaskTypeSearchVm, HkTaskTypeSearchVm> model);
}
