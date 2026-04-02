using Domain.Entities.Admin;
using Domain.Utility.Common;
using Domain.ViewModel.Admin.Department;
using Interface.Base;

namespace Interface.Services.Admin;

public interface IDepartmentService : IService<Department>
{
    Task<DataTablePagination<DepartmentSearchVm, DepartmentSearchVm>>
              SearchAsync(DataTablePagination<DepartmentSearchVm, DepartmentSearchVm> model);
    string GetLedgerCodeByDptCode(string code);
}