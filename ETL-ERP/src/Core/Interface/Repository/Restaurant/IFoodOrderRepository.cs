using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.Report;
using Domain.ViewModel.Restaurant.FoodOrder;
using Interface.Base;

namespace Interface.Repository.Restaurant;

public interface IFoodOrderRepository : IRepository<RsFoodOrder>
{
    Task<DataTablePagination<FoodOrderSearchVm, FoodOrderSearchVm>>
       SearchAsync(DataTablePagination<FoodOrderSearchVm, FoodOrderSearchVm> vm);
    Task<RsFoodOrder> GetFoodOrderByIdAsync(long id);
    Task<List<RsDailySalesReportVm>> RsMonthlySalesDataAsync(RsDailySalesReportVm vm);

    Task<string> RsCustomerWiseDueReportHtml(RsCustomerWiseDueReportVm vm, bool isPrint = false);
    Task<string> RsSalesReportHtml(RsSalesReportVm vm, bool isPrint = false);
    Task<string> RsDailySalesSummaryReportHtml(RsDailySalesSummaryVm vm, bool isPrint = false);
    Task<List<RsTransectionReportVm>> RsPaymentTransectionReportAsync(RsTransectionReportVm vm);
    Task<List<RsDailySalesSummaryVm>> GetRsDailySalesSummaryReportData(RsDailySalesSummaryVm vm);
}
