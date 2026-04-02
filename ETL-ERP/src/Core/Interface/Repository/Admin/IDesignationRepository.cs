using Domain.Entities.Admin;
using Domain.Utility.Common;
using Domain.ViewModel.Admin.Designation;
using Interface.Base;

namespace Interface.Repository.Admin
{
    public interface IDesignationRepository : IRepository<Designation>
    {
        Task<DataTablePagination<DesignationSearchVm, DesignationSearchVm>>
            SearchAsync(DataTablePagination<DesignationSearchVm, DesignationSearchVm> model);
    }
}
