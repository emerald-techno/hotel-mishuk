using Domain.Entities.HotelManagement;
using Domain.ViewModel.HotelManagement.AdvanceRefund;
using Domain.ViewModel.HotelManagement.RefundReport;
using Domain.ViewModel.Report;
using Interface.Base;

namespace Interface.Repository.HotelManagement;

public interface IAdvanceRefundRepository : IRepository<HtAdvanceRefund>
{
    Task<AdvanceRefundVm> PrepareAdvanceRefundByBookingId(long bookingId);

    Task<List<RefundReportVm>> HtRefundDataAsync(RefundReportVm vm);
}