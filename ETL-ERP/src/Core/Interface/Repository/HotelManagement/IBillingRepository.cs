using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.Billing;
using Interface.Base;

namespace Interface.Repository.HotelManagement;

public interface IBillingRepository : IRepository<HtBilling>
{
    Task<DataTablePagination<BillingSearchVm, BillingSearchVm>>
        SearchAsync(DataTablePagination<BillingSearchVm, BillingSearchVm> vm);
    Task<HtBilling> GetBillByIdAsync(long id);
    Task<string> GetBillDetailHtmlById(BillingVm vm);
}
