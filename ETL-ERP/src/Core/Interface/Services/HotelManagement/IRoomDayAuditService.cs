using Domain.Entities.HotelManagement;
using Domain.ViewModel.HotelManagement.BusinessDaySummary;
using Domain.ViewModel.HotelManagement.FD_PaymentTranReport;
using Domain.ViewModel.HotelManagement.HotelReport;
using Domain.ViewModel.HotelManagement.RoomDayAudit;
using Domain.ViewModel.Report;
using Interface.Base;

namespace Interface.Services.HotelManagement;

public interface IRoomDayAuditService : IService<HtRoomDayAudit>
{
    Task<List<RoomDayAuditVm>> GenerateRoomDayAuditsForDateAsync(DateTime businessDate);
    Task<HtBusinessDay> GetCurrentBusinessDay();
    Task<string> RoomAuditHtml(DateTime businessDate);
    Task<BusinessDaySummaryVm> GetNightAuditSummary(DateTime businessDate);
    Task<bool> CloseBusinessDay(DateTime businessDate);
    Task<List<PaymentTransactionReportVm>> GetPaymentTransactionForDateAsync(PaymentTransactionReportVm vm);
    Task<List<RsDailySalesReportVm>> GetRestaurantAuditDataForDateAsync(RsDailySalesReportVm vm);
    Task<List<RsTransectionReportVm>> GetRestaurentPaymentAuditDataForDateAsync(RsTransectionReportVm vm);
    Task<List<ExtraServiceReportVm>> GetServiceAuditDataForDateAsync(ExtraServiceReportVm vm);
    Task<List<BookingHallReportVm>> GetHallAuditDataForDateAsync(BookingHallReportVm vm);
    Task<bool> ApproveFoPaymentsAsync(List<long> ids, long auditById);
    Task<bool> ApproveRsPaymentsAsync(List<long> ids, long auditById);
    Task<bool> ApproveHallBookingAsync(List<long> ids, long auditById);
    Task<bool> ApproveExtraServiceAsync(List<long> ids, long auditById);
    Task<AuditStatusVm> IsAudited(DateTime businessDate);
    Task<string> GetAuditSummaryHtml(DateTime businessDate);
    Task<string> GenerateRoomAuditHtmlAsync(DateTime businessDate, bool isPrint = false);
    Task<string> GeneratePaymentTransactionHtmlAsync(string businessDate, bool isPrint = false);
    Task<string> GenerateRestaurantAuditHtmlAsync(string businessDate, bool isPrint = false);
    Task<string> GenerateRestaurantPaymentAuditHtmlAsync(string businessDate, bool isPrint = false);
    Task<string> GenerateServiceAuditHtmlAsync(string businessDate, bool isPrint = false);
    Task<string> GenerateHallAuditHtmlAsync(string businessDate, bool isPrint = false);
    Task<int> RoomReAuditAsync(DateTime businessDate);
}