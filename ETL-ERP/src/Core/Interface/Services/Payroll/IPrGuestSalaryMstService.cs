using Domain.Entities.Payroll;
using Domain.Utility.Common;
using Domain.ViewModel.Payroll.PrGuestSalary;
using Interface.Base;

namespace Interface.Services.Payroll;

public interface IPrGuestSalaryMstService : IService<PrGuestSalaryMst>
{
    Task<DataTablePagination<PrGuestSalaryMstSearchVm, PrGuestSalaryMstSearchVm>> SearchAsync(DataTablePagination<PrGuestSalaryMstSearchVm, PrGuestSalaryMstSearchVm> model);

    Task<PrGuestSalaryMstVm> GetGuestSalaryMstByIdAsync(long id);
    Task<string> GuestSalaryDetailHtml(long id);
}
