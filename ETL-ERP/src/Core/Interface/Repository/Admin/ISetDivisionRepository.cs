using Domain.Entities.Admin;
using Domain.Utility.Common;
using Domain.ViewModel.Admin.Division;
using Interface.Base;

namespace Interface.Repository.Admin
{
    public interface ISetDivisionRepository : IRepository<SetDivision>
    {
        Task<DataTablePagination<SetDivisionSearchVm, SetDivisionSearchVm>>
            SearchAsync(DataTablePagination<SetDivisionSearchVm, SetDivisionSearchVm> model);
    }
}
