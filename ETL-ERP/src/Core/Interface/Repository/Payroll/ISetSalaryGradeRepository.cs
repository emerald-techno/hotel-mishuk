using Domain.Entities.Payroll;
using Domain.Utility.Common;
using Domain.ViewModel.Payroll.SetSalaryGrade;
using Interface.Base;

namespace Interface.Repository.Payroll
{
    public interface ISetSalaryGradeRepository : IRepository<SetSalaryGrade>
    {
        Task<DataTablePagination<SetSalaryGradeSearchVm, SetSalaryGradeSearchVm>>
            SearchAsync(DataTablePagination<SetSalaryGradeSearchVm, SetSalaryGradeSearchVm> model);
    }
}
