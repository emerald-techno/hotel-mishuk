using Domain.Entities.Admin;
using Domain.Utility.Common;
using Domain.ViewModel.Admin.PoliceStation;
using Interface.Base;

namespace Interface.Repository.Admin
{
    public interface ISetPoliceStationRepository : IRepository<SetPoliceStation>
    {
        Task<DataTablePagination<SetPoliceStationSearchVm, SetPoliceStationSearchVm>>
            SearchAsync(DataTablePagination<SetPoliceStationSearchVm, SetPoliceStationSearchVm> model);
    }
}
