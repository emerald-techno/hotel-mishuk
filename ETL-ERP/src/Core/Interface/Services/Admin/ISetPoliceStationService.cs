using Domain.Entities.Admin;
using Domain.Utility.Common;
using Domain.ViewModel.Admin.PoliceStation;
using Interface.Base;

namespace Interface.Services.Admin
{
    public interface ISetPoliceStationService : IService<SetPoliceStation>
    {
        Task<DataTablePagination<SetPoliceStationSearchVm, SetPoliceStationSearchVm>>
               SearchAsync(DataTablePagination<SetPoliceStationSearchVm, SetPoliceStationSearchVm> model);
    }
}
