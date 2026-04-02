using Domain.ViewModel.HotelManagement.HotelReport;

namespace Interface.Repository.HotelManagement;

public interface IHotelManagementRepository
{
    Task<List<BookingServiceReportVm>> GetBookingServiceData(BookingServiceReportVm vm);
    Task<string> GetBookingReportHtml(BookingServiceReportVm vm, bool isPrint = false);
    Task<string> GetBookingDailySalesReportHtml(BookingDailySalesReportVm vm, bool isPrint = false);
    Task<string> RoomOccupancyReportHtml(RoomOccupancyReportVm vm);
    Task<string> CategoryWiseRoomAvailableReportHtml(CategoryWiseRoomAvailableReportVm vm, bool isPrint = false);
    Task<string> CancelReportHtml(CancelReportVm vm, bool isPrint = false);
    Task<string> ReservationReportHtml(ReservationReportVm vm, bool isPrint = false);
}
