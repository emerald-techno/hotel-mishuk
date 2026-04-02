using Domain.Entities.Payroll;
using Domain.Utility.Common;
using Domain.ViewModel.Payroll.PrArrearMst;
using Interface.Base;

namespace Interface.Services.Payroll;

public interface IPrArrearMstService : IService<PrArrearMst>
{
    Task<DataTablePagination<PrArrearMstSearchVm, PrArrearMstSearchVm>> SearchAsync(DataTablePagination<PrArrearMstSearchVm, PrArrearMstSearchVm> model);
    Task<PrArrearMstVm> GetPrArrearByIdAsync(long id);
    Task<string> ArrearDetailsReportHtml(long id);
}
