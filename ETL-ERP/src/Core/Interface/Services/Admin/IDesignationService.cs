using Domain.Entities.Admin;
using Domain.Utility.Common;
using Domain.ViewModel.Admin.Designation;
using Interface.Base;

namespace Interface.Services.Admin
{
    public interface IDesignationService : IService<Designation>
    {
        Task<DataTablePagination<DesignationSearchVm, DesignationSearchVm>>
                                    SearchAsync(DataTablePagination<DesignationSearchVm, DesignationSearchVm> model);
    }
}
