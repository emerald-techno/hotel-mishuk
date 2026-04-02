using Domain.Entities.Admin;
using Domain.Utility.Common;
using Domain.ViewModel.Admin.Department;
using Interface.Base;

namespace Interface.Repository.Admin
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        Task<DataTablePagination<DepartmentSearchVm, DepartmentSearchVm>>
            SearchAsync(DataTablePagination<DepartmentSearchVm, DepartmentSearchVm> model);
    }
}
