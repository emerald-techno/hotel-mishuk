using Domain.Entities.Accounting;
using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.Billing;
using Domain.ViewModel.Restaurant.FoodOrder;
using Interface.Base;

namespace Interface.Services.HotelManagement;

public interface IBillService : IService<HtBilling>
{
    Task<DataTablePagination<BillingSearchVm, BillingSearchVm>>
        SearchAsync(DataTablePagination<BillingSearchVm, BillingSearchVm> model);
    Task<BillingVm> GetBillByIdAsync(long id);
    Task<bool> PayBill(PayBillVm vm);
    Task<string> GetBillByIdAsyncHtml(long id);
    Task<bool> BillDetailAddAsync(SaveBillingDetailVm vm);
    Task<bool> MakeComplimentary(MakeComplimentaryVm vm);
    Task<string> GetBillDetailHtmlById(long id);
    Task<bool> DiscountUpdateAsync(BillDiscountUpdateDto dto);
    Task<bool> FullPaymentBillClose(long billId);
    Task<bool> GenerateInitialBill(long bookingId);
    Task<bool> BillServiceRemoveAsync(long id);
}