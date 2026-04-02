using Domain.Entities.HotelManagement;
using Domain.ViewModel.HotelManagement.AdvanceRefund;
using Domain.ViewModel.HotelManagement.RefundReport;
using Interface.Base;

namespace Interface.Services.HotelManagement;

public interface IAdvanceRefundService : IService<HtAdvanceRefund>
{
    Task<AdvanceRefundVm> PrepareAdvanceRefund(long bookingId);
    Task<string> HtRefundReportHtml(RefundReportVm vm, bool isPrint = false);
}
