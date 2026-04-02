using Domain.Entities.Payroll;
using Domain.Utility.Common;
using Domain.ViewModel.Payroll.PrArrearMst;
using Interface.Base;

namespace Interface.Repository.Payroll;

public interface IPrArrearMstRepository : IRepository<PrArrearMst>
{
    Task<DataTablePagination<PrArrearMstSearchVm, PrArrearMstSearchVm>> SearchAsync(DataTablePagination<PrArrearMstSearchVm, PrArrearMstSearchVm> model);
    Task<PrArrearMst> GetPrArrearByIdAsync(long id);
}
