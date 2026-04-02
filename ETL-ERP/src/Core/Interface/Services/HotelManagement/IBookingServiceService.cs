using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.Booking;
using Domain.ViewModel.HotelManagement.FD_PaymentTranReport;
using Domain.ViewModel.HotelManagement.HotelReport;
using Domain.ViewModel.HotelManagement.RoomBooking;
using Domain.ViewModel.Report;
using Domain.ViewModel.Website;
using Interface.Base;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Interface.Services.HotelManagement;

public interface IBookingServiceService : IService<HtBookingService>
{
    Task<(bool, long)> RoomBookingEntry(HtBookingServiceVm model);
    Task<HtBookingServiceVm> GetBookingInfoById(long bookingId);

    Task<DataTablePagination<BookingServiceSearchVm, BookingServiceSearchVm>>
        SearchAsync(DataTablePagination<BookingServiceSearchVm, BookingServiceSearchVm> model);
    Task<List<HtBookingRoomVm>> GetBookedRoomListByBookingId(long bookingId);
    Task<List<HtBookingGuestVm>> GetBookingGuestListByBookingId(long bookingId);
    Task<bool> RoomCheckIn(HtBookingServiceVm vm);
    Task<(bool, long)> RoomCheckOut(HtBookingServiceVm vm);
    Task<bool> PaymentEntry(BookingPaymentDto payment);
    Task<string> GetBookingServiceReportHtml(BookingServiceReportVm vm,bool isPrint = false);
    Task<IndexRoomVm> GetDashBoardData();
    Task<List<HtRoomInfo>> GetOccupiedRoomList();
    Task<IEnumerable<SelectListItem>> GetOccupiedRoomSelectListItems(bool isDefaultSelectAdd = true);
    Task<string> GetArrivalReportHtml(BookingArrivalVm vm, bool isPrint = false);
    Task<string> GetTodayCheckOutReportHtml(BookingArrivalVm vm, bool isPrint = false);
    Task<string> InHouseGuestReportHtml(bool isPrint = false);
    Task<string> GuestDueReportHtml(GuestDueReportVm vm, bool isPrint = false);
    Task<string> DailyInHouseGuestReportHtml(DailyInHouseGuestVm vm);
    Task<string> RoomDailySalesReportHtml(bool isPrint = false);
    Task<List<RoomReportVm>> GetRoomAvailabilityByDate(DateTime selectedDate, int roomStatus = 0, int cleanStatus = 0);
    Task<string> GetBookingDailySalesReportHtml(BookingDailySalesReportVm vm, bool isPrint = false);
    Task<bool> UpdateBooking(HtBookingServiceVm vm);
    Task<bool> RoomCheckInUpdate(HtBookingServiceVm vm);
    Task<(bool, long)> BookingNoShow(long bookingId);
    Task<bool> HallBookingEntry(HtBookingServiceVm vm);
    Task<dynamic> GetDynamicAvailableHallByDateAndShift(DateTime selectedDate, int? shift = null);
    Task<dynamic> GetDynamicAvailableHallByDateRange(DateTime fromDate, DateTime toDate, int? shift = null);
    Task<HtBookingServiceVm> GetHallBookingInfoById(long bookingId);
    Task<bool> HallBookingUpdate(HtBookingServiceVm vm);
    Task<List<HtBookingHallVm>> GetBookedHallListByBookingId(long bookingId);
    Task<(bool, long)> HallBookingComplete(HtBookingServiceVm vm);
    Task<string> GetHallAvailableReportHtml(HallAvaliableReportVm vm, bool isPrint = false);
    Task<string> RoomOccupancyReportHtml(RoomOccupancyReportVm vm);
    Task<string> AdvanceReportHtml(AdvanceReportVm vm);
    Task<string> GetBillDetailHtmlById(long bookingId);
    Task<bool> PaymentRemoveAsync(long paymentId);
    Task<bool> BookingRemoveAsync(long bookingId);
    Task<string> ReservationBillHtml(long bookingId);
    Task<string> ReservationCardHtml(long bookingId);
    Task<string> CategoryWiseRoomAvailableReportHtml(CategoryWiseRoomAvailableReportVm vm, bool isPrint = false);
    Task<string> GetCheckOutReportHtml(DepartureReportVm vm, bool isPrint = false);
    Task<string> GetPaymentTransactionReportHtml(PaymentTransactionReportVm vm, bool isPrint = false);
    Task<string> MoneyReceiptHtmlAsync(long id);
    Task<string> ReservationCardHtmlEmpty();
    Task<string> CancelReportHtml(CancelReportVm vm, bool isPrint = false);
    Task<bool> ReviceCancelBooking(HtBookingServiceVm vm);
    Task<IEnumerable<SelectListItem>> GetBookingOccupiedRoom(long bookingId, bool isDefaultSelectAdd = true);
    Task<string> ExtraServiceReportHtml(ExtraServiceReportVm vm, bool isPrint = false);
    Task<string> BookingHallReportHtml(BookingHallReportVm vm, bool isPrint = false);
    Task<string> ReservationReportHtml(ReservationReportVm vm, bool isPrint = false);
    Task<string> GetHtPaymentAutoCode();
    Task<bool> RemoveComplementary(long bookingRoomId);
    Task<bool> ComplementaryEntryAsync(long bookingRoomId, long complementaryId);
    Task<string> GetRoomChangeReportHtml(RoomChangeReportVm vm, bool isPrint = false);
    Task<string> InHouseGuestLedgerReportHtml(InHouseGuestLedgerReportVm vm);
    Task<double> TodayExtraServiceReportCount(ExtraServiceReportVm vm);
}
