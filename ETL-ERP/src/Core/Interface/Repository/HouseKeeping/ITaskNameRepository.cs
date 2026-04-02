using Domain.Entities.HouseKeeping;
using Domain.Utility.Common;
using Domain.ViewModel.HouseKeeping.Dashboard;
using Domain.ViewModel.HouseKeeping.TaskName;
using Interface.Base;

namespace Interface.Repository.HouseKeeping;

public interface ITaskNameRepository : IRepository<HkTaskName>
{
    Task<DataTablePagination<HkTaskNameSearchVm, HkTaskNameSearchVm>>
       SearchAsync(DataTablePagination<HkTaskNameSearchVm, HkTaskNameSearchVm> vm);

    Task<HkDashboard> GetHkDashboardData();
}