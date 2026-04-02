using Domain.Entities.Admin;
using Domain.Utility.Common;
using Domain.ViewModel.Admin.Division;
using Interface.Base;

namespace Interface.Services.Admin
{
    public interface ISetDivisionService : IService<SetDivision>
    {
        Task<DataTablePagination<SetDivisionSearchVm, SetDivisionSearchVm>>
            SearchAsync(DataTablePagination<SetDivisionSearchVm, SetDivisionSearchVm> model);
    }
}
