using Domain.Entities.Admin;
using Domain.Utility.Common;
using Domain.ViewModel.Admin.District;
using Interface.Base;

namespace Interface.Services.Admin
{
    public interface ISetDistrictService : IService<SetDistrict>
    {
        Task<DataTablePagination<SetDistrictSearchVm, SetDistrictSearchVm>>
            SearchAsync(DataTablePagination<SetDistrictSearchVm, SetDistrictSearchVm> model);
    }
}
