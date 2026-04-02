using Domain.Entities.Payroll;
using Domain.Utility.Common;
using Domain.ViewModel.Payroll.SetSalaryGrade;
using Interface.Base;

namespace Interface.Services.Payroll;

public interface ISetSalaryGradeService : IService<SetSalaryGrade>
{
    Task<DataTablePagination<SetSalaryGradeSearchVm, SetSalaryGradeSearchVm>>
                                SearchAsync(DataTablePagination<SetSalaryGradeSearchVm, SetSalaryGradeSearchVm> model);
}
