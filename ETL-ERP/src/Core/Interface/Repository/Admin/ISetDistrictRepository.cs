using Domain.Entities.Admin;
using Domain.Utility.Common;
using Domain.ViewModel.Admin.District;
using Interface.Base;

namespace Interface.Repository.Admin
{
    public interface ISetDistrictRepository : IRepository<SetDistrict>
    {
        Task<DataTablePagination<SetDistrictSearchVm, SetDistrictSearchVm>>
            SearchAsync(DataTablePagination<SetDistrictSearchVm, SetDistrictSearchVm> model);
    }
}
