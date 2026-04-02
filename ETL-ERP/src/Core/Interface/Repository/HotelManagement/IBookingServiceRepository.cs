using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.Booking;
using Domain.ViewModel.HotelManagement.FD_PaymentTranReport;
using Domain.ViewModel.HotelManagement.HotelReport;
using Domain.ViewModel.HotelManagement.MoneyReceipt;
using Domain.ViewModel.HotelManagement.RoomBooking;
using Domain.ViewModel.Website;
using Interface.Base;

namespace Interface.Repository.HotelManagement;

public interface IBookingServiceRepository : IRepository<HtBookingService>
{
    Task<DataTablePagination<BookingServiceSearchVm, BookingServiceSearchVm>> SearchAsync(DataTablePagination<BookingServiceSearchVm, BookingServiceSearchVm> vm);
    Task<HtBookingServiceVm> GetBookingByIdAsync(long id);
    Task<IndexRoomVm> GetDashboardDataAsync();
    Task<List<BookingArrivalVm>> TodayArrivalDataAsync(BookingArrivalVm vm);
    Task<List<BookingArrivalVm>> TodayExpectedCheckOutDataAsync(BookingArrivalVm vm);
    Task<List<RoomWiseGuestVm>> InHouseGuestDataAsync();
    Task<List<RoomDailySalesReportVm>> RoomDailySalesDataAsync();
    Task<HtBookingServiceVm> GetHallBookingByIdAsync(long id);

    Task<List<RoomWiseGuestVm>> DailyInHouseGuestReportData(DailyInHouseGuestVm vm);
    Task<DailyInHouseGuestVm> DailyInHouseRoomStatusReportData(DailyInHouseGuestVm vm);
    Task<DailyInHouseGuestVm> DailyInHouseNoShowRoomReportData(DailyInHouseGuestVm vm, DailyInHouseGuestVm data);
    Task<string> DailyInHouseGuestReportHtml(DailyInHouseGuestVm vm);
    Task<string> GuestDueReportHtml(GuestDueReportVm vm, bool isPrint = false);
    Task<string> AdvanceReportHtml(AdvanceReportVm vm);
    Task<List<DepartureReportVm>> TodayDepartureDataAsync(DepartureReportVm vm);
    Task<List<PaymentTransactionReportVm>> PaymentTransactionReportDataAsync(PaymentTransactionReportVm vm);
    Task<List<MoneyReceiptPrintVm>> MoneyReceiptDataAsync(long id);
    Task<List<ExtraServiceReportVm>> ExtraServiceReportDataAsync(ExtraServiceReportVm vm);
    Task<List<RoomChangeReportVm>> RoomChangeReportDataAsync(RoomChangeReportVm vm);
    Task<List<GuestDueReportVm>> GetGuestDueReport(GuestDueReportVm vm);
    Task<string> InHouseGuestLedgerReportUpdateHtml(InHouseGuestLedgerReportVm vm);
}