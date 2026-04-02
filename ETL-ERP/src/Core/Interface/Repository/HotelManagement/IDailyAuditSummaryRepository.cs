using Domain.Entities.HotelManagement;
using Domain.ViewModel.HotelManagement.BusinessDaySummary;
using Interface.Base;

namespace Interface.Repository.HotelManagement;

public interface IDailyAuditSummaryRepository : IRepository<HtDailyAuditSummary>
{
    Task<BusinessDaySummaryVm> GenerateBusinessDaySummaryPreviewAsync(DateTime businessDate);
    Task<bool> CloseBusinessDayAsync(DateTime businessDate, bool applyPreview = true, long currentUserId = 0);
    Task<BusinessDaySummaryVm> GetBusinessDaySummaryAsync(DateTime businessDate);
}