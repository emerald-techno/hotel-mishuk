using Domain.Entities;
using Domain.Entities.Payroll;
using Domain.Utility.Common;
using Domain.ViewModel.Payroll.PrGuestSalary;
using Interface.Base;

namespace Interface.Repository.Payroll
{
    public interface IPrGuestSalaryMstRepository : IRepository<PrGuestSalaryMst>
    {
        Task<DataTablePagination<PrGuestSalaryMstSearchVm, PrGuestSalaryMstSearchVm>> SearchAsync(DataTablePagination<PrGuestSalaryMstSearchVm, PrGuestSalaryMstSearchVm> model);
        Task<PrGuestSalaryMst> GetPrGuestSalaryMstByIdAsync(long id);
    }
}
