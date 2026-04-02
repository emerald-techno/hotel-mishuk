using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.Report;
using Domain.ViewModel.Restaurant.FoodOrder;
using Domain.ViewModel.Restaurant.OrderPayment;
using Interface.Base;

namespace Interface.Services.Restaurant;

public interface IFoodOrderService : IService<RsFoodOrder>
{
    Task<DataTablePagination<FoodOrderSearchVm, FoodOrderSearchVm>>
        SearchAsync(DataTablePagination<FoodOrderSearchVm, FoodOrderSearchVm> model);
    Task<string> GetFoodOrderCode();
    Task<(bool, long)> FoodOrderEntry(FoodOrderVm vm);
    Task<FoodOrderVm> GetFoodOrderByIdAsync(long id);
    Task<(string, float)> GetOrderBillByIdAsyncHtml(long id);
    Task<bool> PayFoodOrder(PayRsOrderVm vm);
    Task<string> RsMonthlySalesReportHtml(RsDailySalesReportVm vm, bool isPrint = false);
    Task<string> RsCustomerWiseDueReportHtml(RsCustomerWiseDueReportVm vm, bool isPrint = false);
    Task<string> RsSalesReportHtml(RsSalesReportVm vm, bool isPrint = false);
    Task<string> RsDailySalesSummaryReportHtml(RsDailySalesSummaryVm vm, bool isPrint = false);
    Task<bool> UpdateOrder(FoodOrderVm vm);
    Task<bool> RefundReservation(PayRsOrderVm vm);
    Task<(string, float)> GetReservationBillByIdAsyncHtml(long id);
    Task<(string, float)> GenerateKot(long id);
    Task<bool> FoodItemsAddAsync(SaveFoodOrderItemDto dto);
    Task<bool> DiscountUpdateAsync(DiscountUpdateDto dto);
    Task<bool> FoodItemRemoveAsync(long id);
    Task<(string, float)> SingleKotHtml(long id, string kotNo);
    Task<bool> FoodItemServedAsync(long id);
    Task<bool> FoodOrderStatusChangeAsync(long orderId, int orderStatus);
    Task<bool> FoodMultipleItemsAddAsync(List<SaveFoodOrderItemDto> dtos);
    Task<bool> RoomAssign(AssignRoomVm dto);
    Task<bool> CusomerTypeUpdateAsync(CustomerTypeUpdateDto dto);
    Task<bool> CustomerNameUpdateAsync(CustomerNameUpdateDto dto);
    Task<bool> PaymentRemoveAsync(long paymentId);
    Task<bool> OrderRemoveAsync(long orderId);
    Task<string> RsPaymentTransectionReportHtml(RsTransectionReportVm vm, bool isPrint = false);
}
