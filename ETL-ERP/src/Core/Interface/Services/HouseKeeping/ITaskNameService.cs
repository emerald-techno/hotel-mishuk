using Domain.Entities.HouseKeeping;
using Domain.Utility.Common;
using Domain.ViewModel.HouseKeeping.Dashboard;
using Domain.ViewModel.HouseKeeping.TaskName;
using Interface.Base;

namespace Interface.Services.HouseKeeping;

public interface ITaskNameService : IService<HkTaskName>
{
    Task<DataTablePagination<HkTaskNameSearchVm, HkTaskNameSearchVm>>
        SearchAsync(DataTablePagination<HkTaskNameSearchVm, HkTaskNameSearchVm> model);
    Task<HkDashboard> GetDashboardData();

    Task<List<HKDashBoardDataVm>> GetRoomAvailabilityByDate(DateTime selectedDate, int roomStatus = 0, int cleanStatus = 0, int categoryId = 0);
    Task<HKDashboardResponseVm> GetRoomAvailabilityByDateV2(DateTime selectedDate,string roomNo, int roomStatus = -1, int cleanStatus = -1, int categoryId = 0, int houseKeeperId = 0);
}
