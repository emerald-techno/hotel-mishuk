using Domain.Entities.Payroll;
using Domain.Utility.Common;
using Domain.ViewModel.Payroll.PrSalaryMst;
using Interface.Base;

namespace Interface.Repository.Payroll;

public interface IPrSalaryMstRepository : IRepository<PrSalaryMst>
{
    Task<DataTablePagination<PrSalaryMstSearchVm, PrSalaryMstSearchVm>>
                                              SearchAsync(DataTablePagination<PrSalaryMstSearchVm, PrSalaryMstSearchVm> model);
    Task<PrSalaryMst> GetPrSalaryByIdAsync(long id);
    Task<PrSalaryMst> GetPrSalaryByMonthAsync(int year, int month);
}
