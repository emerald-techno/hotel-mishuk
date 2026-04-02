using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Enums.AppEnums;
using Domain.ViewModel.HotelManagement.AdvanceRefund;
using Domain.ViewModel.HotelManagement.RefundReport;
using Domain.ViewModel.Report;
using Interface.Repository.HotelManagement;
using Interface.Services.HotelManagement;
using Interface.UnitOfWork;
using Services.Base;
using DU = Domain.Utility;

namespace Services.HotelManagement;

public class AdvanceRefundService : BaseService<HtAdvanceRefund>, IAdvanceRefundService
{
    #region Config
    private IAdvanceRefundRepository _iRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly IBookingServiceRepository _iBookingServiceRepository;

    public AdvanceRefundService(IAdvanceRefundRepository iRepository,
        IMapper iMapper,
        IUnitOfWork iUnitOfWork,
        IBookingServiceRepository iBookingServiceRepository) : base(iRepository, iUnitOfWork)
    {
        _iRepository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
        _iBookingServiceRepository = iBookingServiceRepository;
    }

    #endregion

    #region PrepareAdvanceRefund

    public async Task<AdvanceRefundVm> PrepareAdvanceRefund(long bookingId)
    {
        var result = await _iRepository.PrepareAdvanceRefundByBookingId(bookingId);
        return result;
    }

    #endregion

    public async Task<string> HtRefundReportHtml(RefundReportVm vm, bool isPrint = false)
    {
        try
        {
            string fullHtml = "";
            var data = await _iRepository.HtRefundDataAsync(vm);
            //data = data.OrderBy(x => x.OrderNo).ToList();

            vm.StrFromDate = (string.IsNullOrEmpty(vm.StrFromDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrFromDate;
            vm.StrToDate = (string.IsNullOrEmpty(vm.StrToDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrToDate;
            var queryFromDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrFromDate));
            var queryToDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrToDate));

            if (data != null && data.Count > 0)
            {
                if (isPrint)
                {
                    fullHtml += $@"<h6 style='text-align:center;padding-bottom:5px;'>Date: {queryFromDate.ToString("dd/MM/yyyy")} To {queryToDate.ToString("dd/MM/yyyy")}</h6>";
                }

                fullHtml += "<table class='table report-table table-bordered table-md' id='print_table' style='width:100%;padding-bottom:10px;repeat-header:yes;' border='1'>";
                fullHtml += "<thead>";
                fullHtml += $@"<tr style='height:30px;'><td colspan='13' class='text-center'><b>Date: {queryFromDate.ToString("dd/MM/yyyy")} To {queryToDate.ToString("dd/MM/yyyy")}</b></td></tr>";
                fullHtml += "<tr style='height:30px;font-size: 12px;'>";

                fullHtml += $@"<th style='width:50px;text-align:center;'>SL No</th>
                            <th style='width:150px;text-align:center;'>Booking</th>
                            <th class='text-center' style='width:100px;'>Guest</th>
                            <th class='text-center' style='width:100px;'>Estimated Check-In Time</th>
                            <th class='text-center' style='width:120px;'>Estimated Check-Out Time</th>
                            <th class='text-center' style='width:80px;'>Refund Amount</th>
                            <th class='text-center' style='width:80px;'>Refund Date</th>
                            <th class='text-center' style='width:80px;'>Refund Mode</th>
                            <th class='text-center' style='width:150px;'>Description</th>
                            <th class='text-center' style='width:80px;'>TransactionNo</th>";

                fullHtml += "</tr>";
                fullHtml += "</thead>";
                fullHtml += "<tbody>";

                for (int i = 0; i < data.Count; i++)
                {
                    RefundReportVm objBooking = data[i];

                    string bookingNo = !isPrint ? $"<a target='_blank' href='../BookingService/Details/{objBooking.BookingId}'>{objBooking.BookingNo}</a>" : $"{objBooking.BookingNo}";

                    fullHtml += "<tr>";

                    fullHtml += $@"<td style='text-align:left;text-align:center;'>{i + 1}</td>
                                <td style='text-align:center;'><b>{bookingNo}</b><br/>{objBooking.BookingDate?.ToString("dd/MMM/yyyy")}</td>
                               <td style='text-align:left;'><b>{objBooking.GuestName}</b>{(string.IsNullOrEmpty(objBooking.CompanyName) ? "" : $"<br/>{objBooking.CompanyName}")}<br/>{objBooking.GuestMobile}</td>
                                <td style='text-align:center;'>{objBooking.EstCheckInTime?.ToString("dd/MMM/yyyy")}</td>
                                <td style='text-align:center;'>{objBooking.EstCheckOutTime?.ToString("dd/MMM/yyyy")}</td>
                                <td style='text-align:right;'>{objBooking.RefundAmount:N2}</td>
                                <td style='text-align:center;'>{objBooking.RefundDate?.ToString("dd/MMM/yyyy")}</td>
                                <td style='text-align:center;'>{(PayModeEnum)objBooking.RefundMode}</td>
                                <td style='text-align:left;'>{objBooking.Description}</td>
                                <td style='text-align:left;'>{objBooking.TransactionNo}</td>";

                    fullHtml += " </tr>";
                }
            }
            fullHtml += "</tbody>";

            fullHtml += "</table>";

            // === Footer Section ===
            fullHtml += @"<div class='report-footer'>
    			<hr style='width:60%;margin:10px auto;border:1px solid #eee;' />
    			<p>Generated on: " + DateTime.Now.ToString("dd MMM yyyy hh:mm tt") + "</p>";

            fullHtml += @"</div>";

            return fullHtml;
        }
        catch (Exception e)
        {
            return e.Message;
        }
        
    }


}