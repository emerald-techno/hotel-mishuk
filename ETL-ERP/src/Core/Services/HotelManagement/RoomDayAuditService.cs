using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Enums.AppEnums;
using Domain.Utility;
using Domain.ViewModel.HotelManagement.BusinessDaySummary;
using Domain.ViewModel.HotelManagement.FD_PaymentTranReport;
using Domain.ViewModel.HotelManagement.HotelReport;
using Domain.ViewModel.HotelManagement.RoomDayAudit;
using Domain.ViewModel.Report;
using Interface.Repository.HotelManagement;
using Interface.Repository.Restaurant;
using Interface.Services.HotelManagement;
using Interface.UnitOfWork;
using Services.Base;
using System.Globalization;
using System.Net;
using System.Text;
using DU = Domain.Utility;

namespace Services.HotelManagement;

public class RoomDayAuditService : BaseService<HtRoomDayAudit>, IRoomDayAuditService
{
    #region Config
    private IRoomDayAuditRepository _iRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly IBookingRoomRepository _iBookingRoomRepository;
    private readonly IFoodOrderRepository _iFoodOrderRepository;
    private readonly IRsOrderPaymentRepository _iRsOrderPaymentRepository;
    private readonly IBookingServiceRepository _iBookingServiceRepository;
    private readonly IBookingHallRepository _iBookingHallRepository;
    private readonly IBusinessDayRepository _iBusinessDayRepository;
    private readonly IDailyAuditSummaryRepository _iDailyAuditSummaryRepository;
    private readonly IBookingPaymentRepository _iBookingPaymentRepository;
    private readonly IBillingDetailRepository _iBillingDetailRepository;

    public RoomDayAuditService(IRoomDayAuditRepository iRepository,
        IMapper iMapper,
        IUnitOfWork iUnitOfWork,
        IBookingRoomRepository iBookingRoomRepository,
        IBookingServiceRepository iBookingServiceRepository,
        IBookingHallRepository iBookingHallRepository,
        IBusinessDayRepository iBusinessDayRepository,
        IFoodOrderRepository iFoodOrderRepository,
        IDailyAuditSummaryRepository iDailyAuditSummaryRepository,
        IRsOrderPaymentRepository iRsOrderPaymentRepository,
        IBookingPaymentRepository iBookingPaymentRepository,
        IBillingDetailRepository iBillingDetailRepository) : base(iRepository, iUnitOfWork)
    {
        _iRepository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
        _iBookingRoomRepository = iBookingRoomRepository;
        _iBookingServiceRepository = iBookingServiceRepository;
        _iBookingHallRepository = iBookingHallRepository;
        _iFoodOrderRepository = iFoodOrderRepository;
        _iBusinessDayRepository = iBusinessDayRepository;
        _iDailyAuditSummaryRepository = iDailyAuditSummaryRepository;
        _iRsOrderPaymentRepository = iRsOrderPaymentRepository;
        _iBookingPaymentRepository = iBookingPaymentRepository;
        _iBillingDetailRepository = iBillingDetailRepository;
    }
    #endregion

    #region PrepareRoomAudit
    public async Task<List<RoomDayAuditVm>> GenerateRoomDayAuditsForDateAsync(DateTime businessDate)
    {
        var targetDate = businessDate.Date;

        var bookingRooms = await _iBookingRoomRepository.GetAsync(br =>
            !br.IsDeleted &&
            br.CheckInTime.Date <= targetDate &&
            br.CheckOutTime.Date > targetDate);

        bookingRooms = bookingRooms.Where(x => x.ActualCheckInTime != null).ToList();

        if (bookingRooms == null || bookingRooms.Count == 0)
            return new List<RoomDayAuditVm>();

        var existingBookingList = await _iRepository.GetAsync(x => x.BusinessDate.Date == targetDate);
        var existingBookingIds = existingBookingList.Select(x => x.BookingRoomId).ToList();

        var entityList = new List<HtRoomDayAudit>();

        foreach (var br in bookingRooms)
        {
            if (existingBookingIds.Contains(br.Id))
                continue;

            double days = AppUtility.DaysDiffernceOnlyDate(br.CheckOutTime, br.CheckInTime);

            if (days <= 0) days = 1; // fallback

            if (br.ActualCheckOutTime != null && targetDate == br.ActualCheckOutTime.Value.Date && br.BookingDayStatus > 0)
            {
                if (br.BookingDayStatus == (int)BookingDayStatusEnum.HalfDay)
                {
                    days += 0.5;
                }
                else if (br.BookingDayStatus == (int)BookingDayStatusEnum.DayUse)
                {
                    days += 1;
                }
            }

            double discount = br.Discount;
            double roomRent = br.RoomRent;
            double baseRent = br.Rent;
            double service = br.ServiceCharge;
            double vat = br.Vat;
            double net = br.NetRent;

            double ratePerDay;
            double baseRatePerDay;

            if (discount > 0)
            {
                ratePerDay = roomRent - (discount / days);
                baseRatePerDay = (baseRent - discount) / days;
            }
            else
            {
                ratePerDay = roomRent;
                baseRatePerDay = baseRent / days;
            }

            double scPerDay = service / days;
            double vatPerDay = vat / days;
            double netPerDay = net / days;

            var entity = new HtRoomDayAudit
            {
                BusinessDate = targetDate,
                BookingRoomId = br.Id,
                RoomId = br.RoomId ?? 0,
                RoomCategoryId = br.RoomCategoryId,

                Adult = (int)Math.Max(0, br.Adult),
                Child = (int)Math.Max(0, br.Child),

                IsStay = true,
                IsOccupied = true,
                IsCharged = false,

                Status = (int)AuditStayStatus.Planned,

                RackRate = (double)roomRent,
                Rate = (double)ratePerDay,
                BaseRate = (double)baseRatePerDay,
                ServiceCharge = (double)scPerDay,
                Vat = (double)vatPerDay,
                NetAmount = (double)netPerDay,

                Remarks = null,
                ActionDate = DU.Utility.GetBdDateTimeNow(),
                ActionById = CurrentUserId
            };

            if (br.ActualCheckOutTime != null && targetDate == br.ActualCheckOutTime.Value.Date)
            {
                if (br.BookingDayStatus == (int)BookingDayStatusEnum.HalfDay)
                {
                    entity.Rate = (double)ratePerDay / 2;
                    entity.BaseRate = (double)baseRatePerDay / 2;
                    entity.ServiceCharge = (double)scPerDay / 2;
                    entity.Vat = (double)vatPerDay / 2;
                    entity.NetAmount = (double)netPerDay / 2;
                }

                entity.Status = AuditStayStatus.CheckedOut;

                if (br.BookingDayStatus > 0)
                    entityList.Add(entity);
            }
            else
            {
                entityList.Add(entity);
            }

        }

        if (entityList.Count > 0)
        {
            await _iRepository.AddRangeAsync(entityList);
            await _iUnitOfWork.CompleteAsync();
        }

        return await _iRepository.GetRoomAuditByBusinessDate(targetDate);
    }
    #endregion

    #region CurrentBusinessDate

    public async Task<HtBusinessDay> GetCurrentBusinessDay()
    {
        var buinessDay = await _iBusinessDayRepository.GetFirstOrDefaultAsync(x => x.AuditStatus == 0);
        return buinessDay;
    }

    #endregion

    #region RoomAuditHtml
    public async Task<string> RoomAuditHtml(DateTime businessDate)
    {
        var data = await _iRepository.GetRoomAuditByBusinessDate(businessDate);

        if (data == null || !data.Any())
            return "<p class='text-danger text-center'>No data found for the selected criteria.</p>";

        string html = "";

        html += "<table class='table table-bordered table-responsive' ";
        html += "style='width:100%;border-collapse:collapse;font-size:10px;' border='1'>";

        // ===== Header =====
        html += "<thead>";
        html += "  <tr style='text-align:center;height:32px;'>";
        html += "      <th style='width:8%'>Room No.</th>";
        html += "      <th style='width:15%'>Category Name</th>";
        html += "      <th style='width:20%'>Guest Info</th>";
        html += "      <th style='width:15%'>Booking Info</th>";
        html += "      <th style='width:8%'>Rack Rate</th>";
        html += "      <th style='width:8%'>Room Rate</th>";
        html += "      <th style='width:8%'>S. Charge</th>";
        html += "      <th style='width:8%'>VAT</th>";
        html += "      <th style='width:10%'>Total Amount</th>";
        html += "  </tr>";
        html += "</thead>";

        // ===== Body =====
        html += "<tbody>";

        foreach (var item in data.OrderBy(x => x.RoomNo))
        {
            var guestInfo = $"{item.GuestName}<br />{item.GuestMobile}";
            var bookingInfo = $"{item.BookingNo}<br />{(item.BookingDate?.ToString("dd-MMM-yyyy") ?? "-")}";

            html += "<tr style='height:30px;text-align:center;'>";
            html += $"  <td style='text-align:center;'>{item.RoomNo ?? "-"}</td>";
            html += $"  <td style='text-align:center;'>{item.RoomCategoryName ?? "-"}</td>";
            html += $"  <td style='text-align:center;'>{guestInfo}</td>";
            html += $"  <td style='text-align:center;'>{bookingInfo}</td>";
            html += $"  <td style='text-align:right;'>{item.RackRate:N2}</td>";
            html += $"  <td style='text-align:right;'>{item.Rate:N2}</td>";
            html += $"  <td style='text-align:right;'>{item.ServiceCharge:N2}</td>";
            html += $"  <td style='text-align:right;'>{item.Vat:N2}</td>";
            html += $"  <td style='text-align:right;'>{item.NetAmount:N2}</td>";
            html += "</tr>";
        }

        // ===== Totals =====
        var totalRack = data.Sum(x => x.RackRate);
        var totalRoomRate = data.Sum(x => x.Rate);
        var totalService = data.Sum(x => x.ServiceCharge);
        var totalVat = data.Sum(x => x.Vat);
        var totalNet = data.Sum(x => x.NetAmount);

        html += "<tr style='font-weight:600;height:30px;text-align:right;'>";
        html += "  <td colspan='4' style='text-align:right;'>Total:</td>";
        html += $"  <td style='text-align:right;'>{totalRack:N2}</td>";
        html += $"  <td style='text-align:right;'>{totalRoomRate:N2}</td>";
        html += $"  <td style='text-align:right;'>{totalService:N2}</td>";
        html += $"  <td style='text-align:right;'>{totalVat:N2}</td>";
        html += $"  <td style='text-align:right;'>{totalNet:N2}</td>";
        html += "</tr>";

        html += "</tbody>";
        html += "</table>";

        return html;
    }
    #endregion

    #region NightAuditSummary

    public async Task<BusinessDaySummaryVm> GetNightAuditSummary(DateTime businessDate)
    {
        var daySummary = await _iDailyAuditSummaryRepository.GenerateBusinessDaySummaryPreviewAsync(businessDate);
        return daySummary;
    }

    #endregion

    #region CloseBusinessDay

    public async Task<bool> CloseBusinessDay(DateTime businessDate)
    {
        var daySummary = await _iDailyAuditSummaryRepository.CloseBusinessDayAsync(businessDate, true, CurrentUserId);
        return daySummary;
    }

    #endregion

    #region GetPaymentTransactionForDateAsync

    public async Task<List<PaymentTransactionReportVm>> GetPaymentTransactionForDateAsync(PaymentTransactionReportVm vm)
    {
        var data = new List<PaymentTransactionReportVm>();
        data = await _iBookingServiceRepository.PaymentTransactionReportDataAsync(vm);

        return data;
    }

    #endregion

    #region GetRestaurentAuditDataForDateAsync
    public async Task<List<RsDailySalesReportVm>> GetRestaurantAuditDataForDateAsync(RsDailySalesReportVm vm)
    {
        var data = new List<RsDailySalesReportVm>();
        data = await _iFoodOrderRepository.RsMonthlySalesDataAsync(vm);

        return data;
    }

    #endregion

    #region GetRestaurentPaymentAuditDataForDateAsync
    public async Task<List<RsTransectionReportVm>> GetRestaurentPaymentAuditDataForDateAsync(RsTransectionReportVm vm)
    {
        var data = new List<RsTransectionReportVm>();
        data = await _iFoodOrderRepository.RsPaymentTransectionReportAsync(vm);

        return data;
    }

    #endregion

    #region GetServiceAuditDataForDateAsync
    public async Task<List<ExtraServiceReportVm>> GetServiceAuditDataForDateAsync(ExtraServiceReportVm vm)
    {
        var data = new List<ExtraServiceReportVm>();
        data = await _iBookingServiceRepository.ExtraServiceReportDataAsync(vm);

        return data;
    }

    #endregion

    #region GetServiceAuditDataForDateAsync
    public async Task<List<BookingHallReportVm>> GetHallAuditDataForDateAsync(BookingHallReportVm vm)
    {
        var data = new List<BookingHallReportVm>();
        data = await _iBookingHallRepository.BookingHallReportDataAsync(vm);

        return data;
    }

    #endregion

    #region ApproveFoPayments

    public async Task<bool> ApproveFoPaymentsAsync(List<long> ids, long auditById)
    {
        if (ids == null || ids.Count == 0)
            return false;

        var auditList = await _iBookingPaymentRepository.GetAsync(x => ids.Contains(x.Id));

        if (auditList == null || !auditList.Any())
            return false;

        var now = DU.Utility.GetBdDateTimeNow();

        foreach (var audit in auditList)
        {
            audit.AuditById = auditById;
            audit.AuditDate = now;
            audit.AuditRemarks = "Auto Audit Approve";
        }

        await _iBookingPaymentRepository.UpdateRangeAsync(auditList);
        var result = await _iUnitOfWork.CompleteAsync();
        return result;
    }

    #endregion

    #region ApproveRsPayments

    public async Task<bool> ApproveRsPaymentsAsync(List<long> ids, long auditById)
    {
        if (ids == null || ids.Count == 0)
            return false;

        var auditList = await _iRsOrderPaymentRepository.GetAsync(x => ids.Contains(x.Id));

        if (auditList == null || !auditList.Any())
            return false;

        var now = DU.Utility.GetBdDateTimeNow();

        foreach (var audit in auditList)
        {
            audit.AuditById = auditById;
            audit.AuditDate = now;
            audit.AuditRemarks = "Auto Audit Approve";
        }

        await _iRsOrderPaymentRepository.UpdateRangeAsync(auditList);
        var result = await _iUnitOfWork.CompleteAsync();
        return result;
    }

    #endregion

    #region ApproveHallBookingAsync

    public async Task<bool> ApproveHallBookingAsync(List<long> ids, long auditById)
    {
        if (ids == null || ids.Count == 0)
            return false;

        var auditList = await _iBookingHallRepository.GetAsync(x => ids.Contains(x.Id));

        if (auditList == null || !auditList.Any())
            return false;

        var now = DU.Utility.GetBdDateTimeNow();

        foreach (var audit in auditList)
        {
            audit.AuditById = auditById;
            audit.AuditDate = now;
            audit.AuditRemarks = "Auto Audit Approve";
        }

        await _iBookingHallRepository.UpdateRangeAsync(auditList);
        var result = await _iUnitOfWork.CompleteAsync();
        return result;
    }

    #endregion

    #region ApproveExtraServiceAsync

    public async Task<bool> ApproveExtraServiceAsync(List<long> ids, long auditById)
    {
        if (ids == null || ids.Count == 0)
            return false;

        var auditList = await _iBillingDetailRepository.GetAsync(x => ids.Contains(x.Id));

        if (auditList == null || !auditList.Any())
            return false;

        var now = DU.Utility.GetBdDateTimeNow();

        foreach (var audit in auditList)
        {
            audit.AuditById = auditById;
            audit.AuditDate = now;
            audit.AuditRemarks = "Auto Audit Approve";
        }

        await _iBillingDetailRepository.UpdateRangeAsync(auditList);
        var result = await _iUnitOfWork.CompleteAsync();
        return result;
    }

    #endregion

    #region IsAudited
    public async Task<AuditStatusVm> IsAudited(DateTime businessDate)
    {
        var model = await _iRepository.IsAudited(businessDate);
        return model;
    }
    #endregion

    #region GetAuditSummary

    public async Task<string> GetAuditSummaryHtml(DateTime businessDate)
    {
        string fullHtml = "";

        var daySummary = await _iDailyAuditSummaryRepository.GetBusinessDaySummaryAsync(businessDate);

        if (daySummary == null)
            return fullHtml;

        // compute intermediate values used in the view
        decimal netFnbRevenue = (daySummary.TotalRestaurantSales)
                                - (daySummary.TotalFnbDiscounts)
                                + (daySummary.TotalFnbServiceCharge)
                                + (daySummary.TotalFnbVat);

        decimal totalAmount = (daySummary.TotalRackRate)
                            + (daySummary.TotalRestaurantSales)
                            + (daySummary.TotalBanquetRevenue)
                            + (daySummary.TotalOtherRevenue);

        decimal totalDiscount = (daySummary.TotalRoomDiscounts)
                            + (daySummary.TotalFnbDiscounts);

        decimal totalSC = (daySummary.TotalRoomServiceCharge)
                        + (daySummary.TotalFnbServiceCharge)
                        + (daySummary.TotalOtherServiceCharge);

        decimal totalVat = (daySummary.TotalRoomVat)
                         + (daySummary.TotalFnbVat)
                         + (daySummary.TotalOtherVat);

        decimal grandTotal = (daySummary.TotalRoomRevenue)
                           + (daySummary.TotalRestaurantRevenue)
                           + (daySummary.TotalBanquetRevenue)
                           + (daySummary.TotalOtherRevenue);

        decimal totalFrontOfficeCover = (daySummary.FrontOfficeCashCover)
                                      + (daySummary.FrontOfficeCardCover)
                                      + (daySummary.FrontOfficeMbankingCover)
                                      + (daySummary.FrontOfficeBankCover);

        decimal totalRsPaymentCover = (daySummary.RestaurantCashCover)
                                    + (daySummary.RestaurantCardCover)
                                    + (daySummary.RestaurantMbankingCover)
                                    + (daySummary.RestaurantBankCover);

        // Total collection breakdown
        decimal totalCashCover = (daySummary.FrontOfficeCashCover) + (daySummary.RestaurantCashCover);
        decimal totalCardCover = (daySummary.FrontOfficeCardCover) + (daySummary.RestaurantCardCover);
        decimal totalMbankingCover = (daySummary.FrontOfficeMbankingCover) + (daySummary.RestaurantMbankingCover);
        decimal totalBankCover = (daySummary.FrontOfficeBankCover) + (daySummary.RestaurantBankCover);

        decimal totalCollectionCover = totalCashCover + totalCardCover + totalMbankingCover + totalBankCover;

        decimal totalCashCollection = (daySummary.TotalFrontOfficeCash) + (daySummary.TotalRestaurantCash);
        decimal totalCardCollection = (daySummary.TotalFrontOfficeCard) + (daySummary.TotalRestaurantCard);
        decimal totalMbankingCollection = (daySummary.TotalFrontOfficeMbanking) + (daySummary.TotalRestaurantMbanking);
        decimal totalBankCollection = (daySummary.TotalFrontOfficeBank) + (daySummary.TotalRestaurantBank);

        decimal totalCollectionAmount = (daySummary.TotalFrontOfficeCollections) + (daySummary.TotalRestaurantCollections);

        var ci = CultureInfo.InvariantCulture; // use invariant so ToString("N2") formats consistently

        var sb = new StringBuilder();

        sb.AppendLine("<div class='table-responsive'>");

        // Business Day Summary table
        sb.AppendLine("    <table id='BusinessDaySummaryTable' class='table table-bordered' style='width:100%; text-align:center;'>");
        sb.AppendLine("        <thead class='table-light'>");
        sb.AppendLine("            <tr>");
        sb.AppendLine($"               <th class='text-start' style='width:70%;'>Business Day Summary | {daySummary.BusinessDate.ToString("dd-MMM-yyyy", ci)}</th>");
        sb.AppendLine("                <th class='text-end' style='width:30%;'>Total</th>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("        </thead>");
        sb.AppendLine("        <tbody>");

        // ROOM REVENUE SECTION
        sb.AppendLine("            <tr style='background-color:#E2E3E5;'>");
        sb.AppendLine("                <td colspan='2' class='text-start'><b>Room Revenue</b></td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'>Revenue From Room Rentals</td>");
        sb.AppendLine($"                <td class='text-end'>{daySummary.TotalRoomRevenue.ToString("N2", ci)}</td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'>Room Rental Discounts</td>");
        sb.AppendLine($"                <td class='text-end'>{daySummary.TotalRoomDiscounts.ToString("N2", ci)}</td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'>Service Charge (Rooms)</td>");
        sb.AppendLine($"                <td class='text-end'>{daySummary.TotalRoomServiceCharge.ToString("N2", ci)}</td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'>VAT (Rooms)</td>");
        sb.AppendLine($"                <td class='text-end'>{daySummary.TotalRoomVat.ToString("N2", ci)}</td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'>Number of Rooms Occupied</td>");
        sb.AppendLine($"                <td class='text-end'>{daySummary.TotalRoomsOccupied}</td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'>% of Occupancy</td>");
        sb.AppendLine($"                <td class='text-end'>{daySummary.OccupancyPercentage.ToString("N2", ci)}%</td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'>No. of Guests (Adult + Child)</td>");
        sb.AppendLine("                <td class='text-end'>");
        sb.AppendLine($"                    {daySummary.TotalAdults} + {daySummary.TotalChildren} = {daySummary.TotalGuests}");
        sb.AppendLine("                </td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'>Average Room Rate (ARR)</td>");
        sb.AppendLine($"                <td class='text-end'>{daySummary.ARR.ToString("N2", ci)}</td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'>Average Daily Rate (ADR)</td>");
        sb.AppendLine($"                <td class='text-end'>{daySummary.ADR.ToString("N2", ci)}</td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'>Revenue per Available Room (RevPAR)</td>");
        sb.AppendLine($"                <td class='text-end'>{daySummary.RevPAR.ToString("N2", ci)}</td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'>Average Guests per Room</td>");
        sb.AppendLine($"                <td class='text-end'>{daySummary.AvgGuestsPerRoom.ToString("N2", ci)}</td>");
        sb.AppendLine("            </tr>");

        // FOOD & BEVERAGE SECTION
        sb.AppendLine("            <tr style='background-color:#E2E3E5;'>");
        sb.AppendLine("                <td colspan='2' class='text-start'><b>Food & Beverage Revenue</b></td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'>Revenue from Restaurant & Room Service</td>");
        sb.AppendLine($"                <td class='text-end'>{daySummary.TotalRestaurantSales.ToString("N2", ci)}</td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'>Food and Beverage Discounts</td>");
        sb.AppendLine($"                <td class='text-end'>{daySummary.TotalFnbDiscounts.ToString("N2", ci)}</td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'>Service Charge (Food and Beverage)</td>");
        sb.AppendLine($"                <td class='text-end'>{daySummary.TotalFnbServiceCharge.ToString("N2", ci)}</td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'>VAT (Food and Beverage)</td>");
        sb.AppendLine($"                <td class='text-end'>{daySummary.TotalFnbVat.ToString("N2", ci)}</td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'><b>Net Food & Beverage Revenue</b></td>");
        sb.AppendLine("                <td class='text-end'>");
        sb.AppendLine($"                    {netFnbRevenue.ToString("N2", ci)}");
        sb.AppendLine("                </td>");
        sb.AppendLine("            </tr>");

        // SERVICE & OTHER REVENUE SECTION
        sb.AppendLine("            <tr style='background-color:#E2E3E5;'>");
        sb.AppendLine("                <td colspan='2' class='text-start'><b>Service & Other Revenue</b></td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'>Other Service Revenue (Laundry, Transport, etc.)</td>");
        sb.AppendLine($"                <td class='text-end'>{daySummary.TotalOtherRevenue.ToString("N2", ci)}</td>");
        sb.AppendLine("            </tr>");

        // Banquet/Hall
        sb.AppendLine("            <tr style='background-color:#E2E3E5;'>");
        sb.AppendLine("                <td colspan='2' class='text-start'><b>Hall Revenue</b></td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'>Banquet Hall Revenue</td>");
        sb.AppendLine($"                <td class='text-end'>{daySummary.TotalBanquetRevenue.ToString("N2", ci)}</td>");
        sb.AppendLine("            </tr>");

        // GRAND TOTAL
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'><b>Total Sales (All Sources)</b></td>");
        sb.AppendLine($"                <td class='text-end'><b>{totalAmount.ToString("N2", ci)}</b></td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'><b>Total Discount </b></td>");
        sb.AppendLine($"                <td class='text-end'><b>{totalDiscount.ToString("N2", ci)}</b></td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr >");
        sb.AppendLine("                <td class='text-start'><b>Total Service Charge</b></td>");
        sb.AppendLine($"                <td class='text-end'><b>{totalSC.ToString("N2", ci)}</b></td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr >");
        sb.AppendLine("                <td class='text-start'><b>Total VAT</b></td>");
        sb.AppendLine($"                <td class='text-end'><b>{totalVat.ToString("N2", ci)}</b></td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr >");
        sb.AppendLine("                <td class='text-start'><b>Grand Total Revenue</b></td>");
        sb.AppendLine($"                <td class='text-end'><b>{grandTotal.ToString("N2", ci)}</b></td>");
        sb.AppendLine("            </tr>");

        sb.AppendLine("        </tbody>");
        sb.AppendLine("    </table>");

        var frontCollectionWithoutAdvance = daySummary.TotalFrontOfficeCollections - daySummary.TotalFrontOfficeAdvance;

        // Front Office Collection SECTION
        sb.AppendLine();
        sb.AppendLine("    <table class='table table-bordered' style='width:100%;margin-top:5px'>");
        sb.AppendLine("        <thead>");
        sb.AppendLine("            <tr style='background-color:#E2E3E5;'>");
        sb.AppendLine("                <th class='text-start' style='width:50%;'><b>Front Office Payment Collection</b></th>");
        sb.AppendLine("                <th class='text-center' style='width:20%;'><b>Cover</b></th>");
        sb.AppendLine("                <th class='text-end' style='width:30%;'><b>Total</b></th>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("        </thead>");
        sb.AppendLine("        <tbody>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'>Cash Collection</td>");
        sb.AppendLine($"                <td class='text-center'>{daySummary.FrontOfficeCashCover.ToString("N2", ci)}</td>");
        sb.AppendLine($"                <td class='text-end'>{daySummary.TotalFrontOfficeCash.ToString("N2", ci)}</td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'>Card Collection</td>");
        sb.AppendLine($"                <td class='text-center'>{daySummary.FrontOfficeCardCover.ToString("N2", ci)}</td>");
        sb.AppendLine($"                <td class='text-end'>{daySummary.TotalFrontOfficeCard.ToString("N2", ci)}</td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'>M-Banking Collection</td>");
        sb.AppendLine($"                <td class='text-center'>{daySummary.FrontOfficeMbankingCover.ToString("N2", ci)}</td>");
        sb.AppendLine($"                <td class='text-end'>{daySummary.TotalFrontOfficeMbanking.ToString("N2", ci)}</td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'>Bank Collection</td>");
        sb.AppendLine($"                <td class='text-center'>{daySummary.FrontOfficeBankCover.ToString("N2", ci)}</td>");
        sb.AppendLine($"                <td class='text-end'>{daySummary.TotalFrontOfficeBank.ToString("N2", ci)}</td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr >");
        sb.AppendLine("                <td class='text-start'><b>Total Collection (Front Office)</b></td>");
        sb.AppendLine($"                <td class='text-center'><b>{totalFrontOfficeCover.ToString("N2", ci)}</b></td>");
        sb.AppendLine($"                <td class='text-end'><b>{frontCollectionWithoutAdvance.ToString("N2", ci)}</b></td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("        </tbody>");
        sb.AppendLine("    </table>");

        var totalFoPaymentCover = daySummary.FrontOfficeAdvanceCover + totalFrontOfficeCover;

        // Front Office Advance Collection SECTION
        sb.AppendLine();
        sb.AppendLine("    <table class='table table-bordered' style='width:100%;margin-top:5px'>");
        sb.AppendLine("        <thead>");
        sb.AppendLine("            <tr style='background-color:#E2E3E5;'>");
        sb.AppendLine("                <th class='text-start' style='width:50%;'><b>Front Office Advance Payment Collection</b></th>");
        sb.AppendLine("                <th class='text-center' style='width:20%;'><b>Cover</b></th>");
        sb.AppendLine("                <th class='text-end' style='width:30%;'><b>Total</b></th>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("        </thead>");
        sb.AppendLine("        <tbody>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'>Total Advance Collection</td>");
        sb.AppendLine($"                <td class='text-center'>{daySummary.FrontOfficeAdvanceCover.ToString("N2", ci)}</td>");
        sb.AppendLine($"                <td class='text-end'>{daySummary.TotalFrontOfficeAdvance.ToString("N2", ci)}</td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr >");
        sb.AppendLine("                <td class='text-start'><b>Total Advance Collection (Front Office)</b></td>");
        sb.AppendLine($"                <td class='text-center'><b>{daySummary.FrontOfficeAdvanceCover.ToString("N2", ci)}</b></td>");
        sb.AppendLine($"                <td class='text-end'><b>{daySummary.TotalFrontOfficeAdvance.ToString("N2", ci)}</b></td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr >");
        sb.AppendLine("                <td class='text-start'><b>Total Collection With Advance (Front Office)</b></td>");
        sb.AppendLine($"                <td class='text-center'><b>{totalFoPaymentCover.ToString("N2", ci)}</b></td>");
        sb.AppendLine($"                <td class='text-end'><b>{daySummary.TotalFrontOfficeCollections.ToString("N2", ci)}</b></td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("        </tbody>");
        sb.AppendLine("    </table>");

        // Restaurant Collection SECTION
        sb.AppendLine();
        sb.AppendLine("    <table class='table table-bordered' style='width:100%;margin-top:5px'>");
        sb.AppendLine("        <thead>");
        sb.AppendLine("            <tr style='background-color:#E2E3E5;'>");
        sb.AppendLine("                <th class='text-start' style='width:50%;'><b>Restaurant Payment Collection</b></th>");
        sb.AppendLine("                <th class='text-center' style='width:20%;'><b>Cover</b></th>");
        sb.AppendLine("                <th class='text-end' style='width:30%;'><b>Total</b></th>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("        </thead>");
        sb.AppendLine("        <tbody>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'>Cash Collection</td>");
        sb.AppendLine($"                <td class='text-center'>{daySummary.RestaurantCashCover.ToString("N2", ci)}</td>");
        sb.AppendLine($"                <td class='text-end'>{daySummary.TotalRestaurantCash.ToString("N2", ci)}</td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'>Card Collection</td>");
        sb.AppendLine($"                <td class='text-center'>{daySummary.RestaurantCardCover.ToString("N2", ci)}</td>");
        sb.AppendLine($"                <td class='text-end'>{daySummary.TotalRestaurantCard.ToString("N2", ci)}</td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'>M-Banking Collection</td>");
        sb.AppendLine($"                <td class='text-center'>{daySummary.RestaurantMbankingCover.ToString("N2", ci)}</td>");
        sb.AppendLine($"                <td class='text-end'>{daySummary.TotalRestaurantMbanking.ToString("N2", ci)}</td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'>Bank Collection</td>");
        sb.AppendLine($"                <td class='text-center'>{daySummary.RestaurantBankCover.ToString("N2", ci)}</td>");
        sb.AppendLine($"                <td class='text-end'>{daySummary.TotalRestaurantBank.ToString("N2", ci)}</td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'><b>Total Collection (Restaurant)</b></td>");
        sb.AppendLine($"                <td class='text-center'><b>{totalRsPaymentCover.ToString("N2", ci)}</b></td>");
        sb.AppendLine($"                <td class='text-end'><b>{daySummary.TotalRestaurantCollections.ToString("N2", ci)}</b></td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("        </tbody>");
        sb.AppendLine("    </table>");

        // Total Collection BreakDown SECTION
        sb.AppendLine();
        sb.AppendLine("    <table class='table table-bordered' style='width:100%;margin-top:5px;'>");
        sb.AppendLine("        <thead>");
        sb.AppendLine("            <tr style='background-color:#E2E3E5;'>");
        sb.AppendLine("                <th class='text-start' style='width:50%;'><b>Total Payment Collection</b></th>");
        sb.AppendLine("                <th class='text-center' style='width:20%;'><b>Cover</b></th>");
        sb.AppendLine("                <th class='text-end' style='width:30%;'><b>Total</b></th>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("        </thead>");
        sb.AppendLine("        <tbody>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'>Cash Collection</td>");
        sb.AppendLine($"                <td class='text-center'>{totalCashCover.ToString("N2", ci)}</td>");
        sb.AppendLine($"                <td class='text-end'>{totalCashCollection.ToString("N2", ci)}</td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'>Card Collection</td>");
        sb.AppendLine($"                <td class='text-center'>{totalCardCover.ToString("N2", ci)}</td>");
        sb.AppendLine($"                <td class='text-end'>{totalCardCollection.ToString("N2", ci)}</td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'>M-Banking Collection</td>");
        sb.AppendLine($"                <td class='text-center'>{totalMbankingCover.ToString("N2", ci)}</td>");
        sb.AppendLine($"                <td class='text-end'>{totalMbankingCollection.ToString("N2", ci)}</td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'>Bank Collection</td>");
        sb.AppendLine($"                <td class='text-center'>{totalBankCover.ToString("N2", ci)}</td>");
        sb.AppendLine($"                <td class='text-end'>{totalBankCollection.ToString("N2", ci)}</td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'><b>Total Collection</b></td>");
        sb.AppendLine($"                <td class='text-center'><b>{totalCollectionCover.ToString("N2", ci)}</b></td>");
        sb.AppendLine($"                <td class='text-end'><b>{totalCollectionAmount.ToString("N2", ci)}</b></td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("        </tbody>");
        sb.AppendLine("    </table>");

        // Receivable SECTION
        sb.AppendLine();
        sb.AppendLine("    <table class='table table-bordered' style='width:100%;margin-top:5px'>");
        sb.AppendLine("        <thead>");
        sb.AppendLine("            <tr style='background-color:#E2E3E5;'>");
        sb.AppendLine("                <th class='text-start' style='width:70%;'><b>Receivable</b></th>");
        sb.AppendLine("                <th class='text-end' style='width:30%;'><b>Total</b></th>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("        </thead>");
        sb.AppendLine("        <tbody>");
        sb.AppendLine("            <tr>");
        sb.AppendLine("                <td class='text-start'>Front Office Receivable</td>");
        sb.AppendLine($"                <td class='text-end'>{daySummary.FrontOfficeReceivable.ToString("N2", ci)}</td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr >");
        sb.AppendLine("                <td class='text-start'>Food & Beverage Receivable</td>");
        sb.AppendLine($"                <td class='text-end'>{daySummary.FNBReceivable.ToString("N2", ci)}</td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("            <tr >");
        sb.AppendLine("                <td class='text-start'><b>Total Receivable (All Source)</b></td>");
        sb.AppendLine($"                <td class='text-end'><b>{daySummary.TotalReceivable.ToString("N2", ci)}</b></td>");
        sb.AppendLine("            </tr>");
        sb.AppendLine("        </tbody>");
        sb.AppendLine("    </table>");

        sb.AppendLine("</div>");

        fullHtml = sb.ToString();
        return fullHtml;
    }


    #endregion

    #region RoomAudit All Report Section

    #region Room Audit Report html
    public async Task<string> GenerateRoomAuditHtmlAsync(DateTime businessDate, bool isPrint = false)
    {
        var model = await GenerateRoomDayAuditsForDateAsync(businessDate);
        model = model.OrderBy(x => x.RoomNo).ToList();

        string fullHtml = "";
        if (!isPrint)
        {
            fullHtml += "<div class='card'>";
            fullHtml += "<div class='card-header p-4' style='display:flex;align-items:center;justify-content:space-between;'>";
            fullHtml += "<h5>Room Audit Report</h5>";
            fullHtml += "<div class='setting-list'>";
            fullHtml += "<button class='btn btn-primary' id='RoomAuditPrintBtn' style='margin-left:15px;'>PDF</button>";
            fullHtml += "</div>";
            fullHtml += "</div>";

            fullHtml += "<div class='card-body'>";

        }
        if (isPrint)
        {
            fullHtml += $@"<p style='text-align:center;padding-bottom:5px;font-size: 10px;'>Date: {businessDate.ToString("dd-MMM-yyyy")} </p>";
        }

        fullHtml += "<table id='ReportTable' class='table table-bordered report-table' style='width:100%; text-align:center; border-collapse:collapse;'>";
        fullHtml += "<thead>";
        fullHtml += "<tr>";
        fullHtml += "<th style='width:5%'>Serial</th>";
        fullHtml += "<th>Room No.</th>";
        fullHtml += "<th>Category Name</th>";
        fullHtml += "<th>Guest Info</th>";
        fullHtml += "<th>Booking Info</th>";
        fullHtml += "<th class='text-end'>Rack Rate</th>";
        fullHtml += "<th class='text-end'>Room Rate</th>";
        fullHtml += "<th class='text-end'>S. Charge</th>";
        fullHtml += "<th class='text-end'>Discount(%)</th>";
        fullHtml += "<th class='text-end'>Total Amount</th>";
        fullHtml += "</tr>";
        fullHtml += "</thead>";
        fullHtml += "<tbody>";

        if (model != null && model.Count > 0)
        {
            for (int i = 0; i < model.Count; i++)
            {
                var room = model[i];

                string roomNo = room.RoomNo ?? "";
                string category = room.RoomCategoryName ?? "";
                string guestName = room.GuestName ?? "";
                string guestMobile = room.GuestMobile ?? "";
                string bookingNo = room.BookingNo ?? "";
                string bookingDate = room.BookingDate?.ToString("dd-MMM-yyyy") ?? "";

                string rackRate = room.RackRate.ToString("N2");
                string baseRate = room.BaseRate.ToString("N2");
                string serviceCharge = room.ServiceCharge.ToString("N2");
                string vat = room.Vat.ToString("N2");
                string discountPercent = room.DiscountPercent.ToString("N2");
                string netAmount = room.NetAmount.ToString("N2");

                var bookingHtml = $"<a href='../../BookingService/Details/{room.BookingId}' target='_blank'>{room.BookingNo}</a>";

                fullHtml += "<tr>";

                fullHtml += $"<td>{i + 1}</td>";
                fullHtml += $"<td>{roomNo}</td>";
                fullHtml += $"<td>{category}</td>";
                fullHtml += $"<td>{guestName}<br/>{guestMobile}</td>";
                fullHtml += $"<td>{bookingHtml}<br/>{bookingDate}</td>";
                fullHtml += $"<td class='text-end'>{rackRate}</td>";
                fullHtml += $"<td class='text-end'>{baseRate}</td>";
                fullHtml += $"<td class='text-end'>{serviceCharge}</td>";
                fullHtml += $"<td class='text-center'>{discountPercent}</td>";
                fullHtml += $"<td class='text-end'>{netAmount}</td>";

                fullHtml += "</tr>";
            }

            fullHtml += "<tr>";
            fullHtml += $"<td colspan='5' style='text-align:right;font-weight:600;'>Total</td>";
            fullHtml += $"<td style='text-align:right;font-weight:600;color:#2c3e50;'>{model.Sum(x => x.RackRate):N2}</td>";
            fullHtml += $"<td style='text-align:right;font-weight:600;color:#2c3e50;'>{model.Sum(x => x.BaseRate):N2}</td>";
            fullHtml += $"<td style='text-align:right;font-weight:600;color:#2c3e50;'>{model.Sum(x => x.ServiceCharge):N2}</td>";
            fullHtml += $"<td style='text-align:right;font-weight:600;color:#2c3e50;'></td>";
            fullHtml += $"<td style='text-align:right;font-weight:600;color:#2c3e50;'>{model.Sum(x => x.NetAmount):N2}</td>";
            fullHtml += "</tr>";
        }
        else
        {
            fullHtml += "<tr>";
            fullHtml += "<td style='text-align:center' colspan='10'><b>No Generated Audit Found..</b></td>";
            fullHtml += "</tr>";
        }

        fullHtml += "</tbody>";
        fullHtml += "</table>";
        if (!isPrint)
        {
            fullHtml += "</div>";
            fullHtml += "</div>";

        }
        // === Footer Section ===
        fullHtml += @"<div class='report-footer'>
 			<hr style='width:60%;margin:10px auto;border:1px solid #eee;' />
 			<p>Generated on: " + DateTime.Now.ToString("dd MMM yyyy hh:mm tt") + "</p>";

        fullHtml += @"</div>";

        return fullHtml;
    }



    #endregion

    #region Payment Transaction
    public async Task<string> GeneratePaymentTransactionHtmlAsync(string businessDate, bool isPrint = false)
    {
        var model = await GetPaymentTransactionForDateAsync(new PaymentTransactionReportVm { StrFromDate = businessDate, StrToDate = businessDate });

        string fullHtml = "";

        if (!isPrint)
        {
            fullHtml += "<div class='card'>";
            fullHtml += "<div class='card-header p-4' style='display:flex;align-items:center;justify-content:space-between;'>";
            fullHtml += "<h5>FO Transaction Audit Report";
            fullHtml += "</h5>";
            fullHtml += "<div class='setting-list'>";
            fullHtml += "<button class='btn btn-primary' id='PaymentTransAuditPrintBtn' style='margin-left:15px;'>PDF</button>";
            fullHtml += "</div>";
            fullHtml += "</div>";

            fullHtml += "<div class='card-body'>";

        }
        if (isPrint)
        {
            fullHtml += $@"<p style='text-align:center;padding-bottom:5px;font-size: 10px;'>Date: {businessDate} </p>";
        }
        fullHtml += "<table id='ReportTable' class='table table-bordered report-table' style='width:100%; text-align:center; border-collapse:collapse;'>";
        fullHtml += "<thead>";
        fullHtml += "<tr>";
        fullHtml += "<th style='width:5%;'>SL.</th>";
        fullHtml += "<th style='width:10%;'>Guest</th>";
        fullHtml += "<th style='width:7%;'>Service Date</th>";
        fullHtml += "<th style='width:7%;'>Pay Mode</th>";
        fullHtml += "<th style='width:10%;'>Particulars</th>";
        fullHtml += "<th style='width:8%;'>Bill No./Trans. No.</th>";
        fullHtml += "<th style='width:8%;'>Paid Date</th>";
        fullHtml += "<th style='width:5%;'>Paid Amount</th>";
        fullHtml += "<th style='width:20%;'>Remarks</th>";
        fullHtml += "<th style='width:15%;'>Received By</th>";
        fullHtml += "</tr>";
        fullHtml += "</thead>";
        fullHtml += "<tbody>";

        if (model != null && model.Count > 0)
        {
            var groupedData = model.GroupBy(x => x.PayMode).ToList();
            double grandTotal = 0;

            foreach (var group in groupedData)
            {
                string payModeName = ((PayModeEnum)group.Key).ToString();

                // Group Header Row
                fullHtml += $"<tr><td colspan='10' style='background:#dfe6e9;font-weight:600;color:#2c3e50;text-align:center;'>{payModeName}</td></tr>";

                double subTotal = 0;
                int index = 0;

                foreach (var obj in group)
                {
                    index++;

                    string guestInfo = $"{WebUtility.HtmlEncode(obj.GuestName)}<br/><small style='color:#555;'>{WebUtility.HtmlEncode(obj.GuestMobile)}</small>";

                    string bookingLink = string.IsNullOrEmpty(obj.BookingNo)
                        ? ""
                        : $"<a href='../../BookingService/Details/{obj.BookingId}' target='_blank' style='color:#2980b9;text-decoration:none;font-weight:600;'>{obj.BookingNo}</a>";

                    string billLink = string.IsNullOrEmpty(obj.BillNumber)
                        ? ""
                        : $"<br/><a href='../../Bill/Details/{obj.BillId}' target='_blank' style='color:#27ae60;text-decoration:none;font-size:12px;'>{obj.BillNumber}</a>";

                    fullHtml += "<tr>";

                    fullHtml += $"<td>{index}</td>";
                    fullHtml += $"<td>{guestInfo}</td>";
                    fullHtml += $"<td>{obj.BookingDate:dd/MM/yyyy}</td>";
                    fullHtml += $"<td>{payModeName}</td>";

                    fullHtml += $"<td>{bookingLink}<br/>(Room#{obj.RoomNoList})</td>";

                    fullHtml += $"<td>{WebUtility.HtmlEncode(obj.TransactionNo)}{billLink}</td>";

                    fullHtml += $"<td>{obj.PaidDate:dd/MM/yyyy}</td>";

                    fullHtml += $"<td style='text-align:right;font-weight:600;color:#2c3e50;'>{obj.PaidAmount:N2}</td>";

                    fullHtml += $"<td style='text-align:left;'>{WebUtility.HtmlEncode(obj.Description)}</td>";

                    fullHtml += $"<td style='text-align:left;'>{WebUtility.HtmlEncode(obj.ReceivedBy)}</td>";

                    fullHtml += "</tr>";

                    subTotal += obj.PaidAmount;
                }

                // Subtotal Row
                fullHtml += "<tr class='subtotal-row'>";
                fullHtml += $"<td colspan='7' style='text-align:right;font-weight:600;'>{payModeName} Received:</td>";
                fullHtml += $"<td style='text-align:right;font-weight:600;color:#2c3e50;'>{subTotal:N2}</td>";
                fullHtml += "<td colspan='2'></td>";
                fullHtml += "</tr>";

                grandTotal += subTotal;
            }
        }
        else
        {
            fullHtml += "<tr><td colspan='10' style='text-align:center'><b>No Payment Transactions Found..</b></td></tr>";
        }

        fullHtml += "</tbody>";
        fullHtml += "</table>";
        if (!isPrint)
        {
            fullHtml += "</div>";
            fullHtml += "</div>";
        }
        // === Footer Section ===
        fullHtml += @"<div class='report-footer'>
 			<hr style='width:60%;margin:10px auto;border:1px solid #eee;' />
 			<p>Generated on: " + DateTime.Now.ToString("dd MMM yyyy hh:mm tt") + "</p>";

        fullHtml += @"</div>";

        return fullHtml;
    }

    #endregion

    #region RS order Audit Report html
    public async Task<string> GenerateRestaurantAuditHtmlAsync(string businessDate, bool isPrint = false)
    {

        var model = await GetRestaurantAuditDataForDateAsync(new RsDailySalesReportVm { StrFromDate = businessDate, StrToDate = businessDate });

        string fullHtml = "";
        if (!isPrint)
        {
            fullHtml += "<div class='card'>";
            fullHtml += "<div class='card-header p-4' style='display:flex;align-items:center;justify-content:space-between;'>";
            fullHtml += "<h5>Restaurant Audit Report";
            fullHtml += "</h5>";
            fullHtml += "<div class='setting-list'>";
            fullHtml += "<button class='btn btn-primary' id='RestaurantAuditPrintBtn' style='margin-left:15px;'>PDF</button>";
            fullHtml += "</div>";
            fullHtml += "</div>";

            fullHtml += "<div class='card-body'>";

        }
        if (isPrint)
        {
            fullHtml += $@"<p style='text-align:center;padding-bottom:5px;font-size: 10px;'>Date: {businessDate} </p>";
        }
        fullHtml += "<table id='ReportTable' class='table table-bordered report-table' style='width:100%; text-align:center; border-collapse:collapse;'>";
        fullHtml += "<thead>";
        fullHtml += "<tr>";
        fullHtml += "<th style='width:50px;text-align:center;'>SL No</th>";
        fullHtml += "<th style='width:80px;text-align:center;'>Order</th>";
        fullHtml += "<th style='width:160px;text-align:center;'>Customer</th>";
        fullHtml += "<th style='width:100px;text-align:center;'>Customer Type</th>";
        fullHtml += "<th style='width:100px;text-align:center;'>Room</th>";
        fullHtml += "<th style='width:120px;text-align:center;'>Waiter</th>";
        fullHtml += "<th style='width:80px;text-align:center;'>Table No</th>";
        fullHtml += "<th style='width:80px;text-align:center;'>Total Amount</th>";
        fullHtml += "</tr>";
        fullHtml += "</thead>";
        fullHtml += "<tbody>";

        if (model != null && model.Count > 0)
        {
            int idx = 0;
            double grandTotal = 0;

            foreach (var obj in model)
            {
                idx++;

                string orderLink = $"<a target='_blank' href='../FoodOrder/Details/{obj.OrderId}'>{WebUtility.HtmlEncode(obj.OrderNo ?? "")}</a>";
                string orderDate = obj.OrderDate.ToString("dd/MMM/yyyy", CultureInfo.CurrentCulture);
                string customerName = WebUtility.HtmlEncode(obj.CustomerName ?? "");
                string mobile = WebUtility.HtmlEncode(obj.Mobile ?? "");
                string customerType = WebUtility.HtmlEncode(obj.CustomerType ?? "");
                string roomNo = WebUtility.HtmlEncode(obj.RoomNo ?? "");
                string waiter = WebUtility.HtmlEncode(obj.WaiterName ?? "");
                string tableNo = WebUtility.HtmlEncode(obj.TableNo ?? "");
                string orderAmount = obj.OrderAmount.ToString("N2", CultureInfo.CurrentCulture);

                fullHtml += "<tr>";

                fullHtml += $"<td style='text-align:center;'>{idx}</td>";
                fullHtml += $"<td style='text-align:left;'><b>{orderLink}</b><br/>{orderDate}</td>";
                fullHtml += $"<td style='text-align:left;'><b>{customerName}</b><br/>{mobile}</td>";
                fullHtml += $"<td style='text-align:center;'>{customerType}</td>";
                fullHtml += $"<td style='text-align:center;'>{roomNo}</td>";
                fullHtml += $"<td style='text-align:center;'>{waiter}</td>";
                fullHtml += $"<td style='text-align:center;'>{tableNo}</td>";
                fullHtml += $"<td style='text-align:right;'>{orderAmount}</td>";

                fullHtml += "</tr>";

                grandTotal += obj.OrderAmount;
            }

            fullHtml += "<tr>";
            fullHtml += "<td colspan='7' style='text-align:right;'><b>Total</b></td>";
            fullHtml += $"<td style='text-align:right;'><b>{grandTotal.ToString("N2", CultureInfo.CurrentCulture)}</b></td>";
            fullHtml += "</tr>";
        }
        else
        {
            fullHtml += "<tr>";
            fullHtml += "<td colspan='8' style='text-align:center;'><b>No Records Found..</b></td>";
            fullHtml += "</tr>";
        }

        fullHtml += "</tbody>";
        fullHtml += "</table>";
        if (!isPrint)
        {
            fullHtml += "</div>";
            fullHtml += "</div>";
        }
        // === Footer Section ===
        fullHtml += @"<div class='report-footer'>
 			<hr style='width:60%;margin:10px auto;border:1px solid #eee;' />
 			<p>Generated on: " + DateTime.Now.ToString("dd MMM yyyy hh:mm tt") + "</p>";

        fullHtml += @"</div>";

        return fullHtml;
    }

    #endregion

    #region Rs Payment Audit
    public async Task<string> GenerateRestaurantPaymentAuditHtmlAsync(string businessDate, bool isPrint = false)
    {
        var model = await GetRestaurentPaymentAuditDataForDateAsync(new RsTransectionReportVm { StrFromDate = businessDate, StrToDate = businessDate });

        string fullHtml = "";

        if (!isPrint)
        {
            fullHtml += "<div class='card'>";
            fullHtml += "<div class='card-header p-4' style='display:flex;align-items:center;justify-content:space-between;'>";
            fullHtml += "<h5>Restaurant Payment Audit Report";
            fullHtml += "</h5>";
            fullHtml += "<div class='setting-list'>";
            fullHtml += "<button class='btn btn-primary' id='RestaurantPaymentAuditPrintBtn' style='margin-left:15px;'>PDF</button>";
            fullHtml += "</div>";
            fullHtml += "</div>";

            fullHtml += "<div class='card-body'>";

        }
        if (isPrint)
        {
            fullHtml += $@"<p style='text-align:center;padding-bottom:5px;font-size: 10px;'>Date: {businessDate} </p>";
        }
        fullHtml += "<table id='ReportTable' class='table table-bordered report-table' style='width:100%; text-align:center; border-collapse:collapse;'>";
        fullHtml += "<thead>";
        fullHtml += "<tr>";
        fullHtml += "<th>SL</th>";
        fullHtml += "<th>Order No</th>";
        fullHtml += "<th>Customer</th>";
        fullHtml += "<th>Customer Type</th>";
        fullHtml += "<th>Pay Mode</th>";
        fullHtml += "<th>Paid Date</th>";
        fullHtml += "<th>Net Amount</th>";
        fullHtml += "<th>Paid Amount</th>";
        fullHtml += "<th>Collection</th>";
        fullHtml += "<th>Remarks</th>";
        fullHtml += "</tr>";
        fullHtml += "</thead>";
        fullHtml += "<tbody>";

        if (model != null && model.Count > 0)
        {
            int idx = 0;
            double totalNet = 0;
            double totalPaid = 0;

            foreach (var obj in model)
            {
                idx++;

                string orderNoLink = $"<a target='_blank' href='../FoodOrder/Details/{obj.OrderId}'>{WebUtility.HtmlEncode(obj.OrderNo ?? "")}</a>";
                string orderDate = obj.OrderDate?.ToString("dd/MMM/yyyy", CultureInfo.CurrentCulture) ?? "";
                string customerName = WebUtility.HtmlEncode(obj.CustomerName ?? "");
                string mobile = WebUtility.HtmlEncode(obj.Mobile ?? "");
                string customerType = WebUtility.HtmlEncode(obj.CustomerType ?? "");
                string payMode = WebUtility.HtmlEncode(((PayModeEnum)obj.PayMode).ToString());
                string paidDate = obj.PaidDate?.ToString("dd/MMM/yyyy", CultureInfo.CurrentCulture) ?? "";
                string netAmount = (obj.NetAmount).ToString("N2", CultureInfo.CurrentCulture);
                string paidAmount = (obj.PaidAmount).ToString("N2", CultureInfo.CurrentCulture);
                string collectionFrom = obj.BillDtlId != null ? "Front-Desk" : "Restaurant";
                string remarks = WebUtility.HtmlEncode(obj.Remarks ?? "");

                fullHtml += "<tr>";

                fullHtml += $"<td style='text-align:center;'>{idx}</td>";
                fullHtml += $"<td style='text-align:center;'><b>{orderNoLink}</b><br/>{WebUtility.HtmlEncode(orderDate)}</td>";
                fullHtml += $"<td style='text-align:center;'><b>{customerName}</b><br/>{mobile}</td>";
                fullHtml += $"<td style='text-align:center;'>{customerType}</td>";
                fullHtml += $"<td style='text-align:center;'>{payMode}</td>";
                fullHtml += $"<td style='text-align:center;'>{WebUtility.HtmlEncode(paidDate)}</td>";
                fullHtml += $"<td style='text-align:right;'>{netAmount}</td>";
                fullHtml += $"<td style='text-align:right;'>{paidAmount}</td>";
                fullHtml += $"<td style='text-align:center;'>{WebUtility.HtmlEncode(collectionFrom)}</td>";
                fullHtml += $"<td style='text-align:right;'>{remarks}</td>";

                fullHtml += "</tr>";

                totalNet += obj.NetAmount;
                totalPaid += obj.PaidAmount;
            }

            // Totals row (colspan 6 before amounts, then two amounts, then 2 empty cols)
            fullHtml += "<tr>";
            fullHtml += "<td colspan='6' style='text-align:right;'><b>Total</b></td>";
            fullHtml += $"<td style='text-align:right;'><b>{totalNet.ToString("N2", CultureInfo.CurrentCulture)}</b></td>";
            fullHtml += $"<td style='text-align:right;'><b>{totalPaid.ToString("N2", CultureInfo.CurrentCulture)}</b></td>";
            fullHtml += "<td colspan='2'></td>";
            fullHtml += "</tr>";
        }
        else
        {
            fullHtml += "<tr>";
            fullHtml += "<td colspan='10' style='text-align:center;'><b>No Records Found..</b></td>";
            fullHtml += "</tr>";
        }

        fullHtml += "</tbody>";
        fullHtml += "</table>";
        if (!isPrint)
        {
            fullHtml += "</div>";
            fullHtml += "</div>";
        }
        // === Footer Section ===
        fullHtml += @"<div class='report-footer'>
 			<hr style='width:60%;margin:10px auto;border:1px solid #eee;' />
 			<p>Generated on: " + DateTime.Now.ToString("dd MMM yyyy hh:mm tt") + "</p>";

        fullHtml += @"</div>";
        return fullHtml;
    }
    #endregion

    #region GenerateServiceAuditHtml
    public async Task<string> GenerateServiceAuditHtmlAsync(string businessDate, bool isPrint = false)
    {
        string businessDateStr = businessDate;

        var model = await GetServiceAuditDataForDateAsync(new ExtraServiceReportVm
        {
            StrFromDate = businessDateStr,
            StrToDate = businessDateStr
        });

        string fullHtml = "";

        if (!isPrint)
        {
            fullHtml += "<div class='card'>";
            fullHtml += "<div class='card-header p-4' style='display:flex;align-items:center;justify-content:space-between;'>";
            fullHtml += "<h5>Service Audit Report";
            fullHtml += "</h5>";
            fullHtml += "<div class='setting-list'>";
            fullHtml += "<button class='btn btn-primary' id='ServiceAuditPrintBtn' style='margin-left:15px;'>PDF</button>";
            fullHtml += "</div>";
            fullHtml += "</div>";

            fullHtml += "<div class='card-body'>";
        }
        if (isPrint)
        {
            fullHtml += $@"<p style='text-align:center;padding-bottom:5px;font-size: 10px;'>Date: {businessDate} </p>";
        }

        fullHtml += "<table id='ReportTable' class='table table-hovered report-table' style='width:100%; text-align:center; border-collapse:collapse;'>";
        fullHtml += "<thead>";
        fullHtml += "<tr>";
        fullHtml += "<th style='width:5%;text-align:center;'>SL.</th>";
        fullHtml += "<th style='width:12%;text-align:center;'>Service</th>";
        fullHtml += "<th style='width:8%;text-align:center;'>Service Date</th>";
        fullHtml += "<th style='width:8%;text-align:center;'>Booking/Bill No.</th>";
        fullHtml += "<th style='width:6%;text-align:center;'>Room No</th>";
        fullHtml += "<th style='width:6%;text-align:center;'>Rate</th>";
        fullHtml += "<th style='width:6%;text-align:center;'>Discounted Rate</th>";
        fullHtml += "<th style='width:6%;text-align:center;'>Qty</th>";
        fullHtml += "<th style='width:6%;text-align:center;'>Amount</th>";
        fullHtml += "<th style='width:8%;text-align:center;'>Service Charge</th>";
        fullHtml += "<th style='width:6%;text-align:center;'>VAT</th>";
        fullHtml += "<th style='width:8%;text-align:center;'>Discount</th>";
        fullHtml += "<th style='width:8%;text-align:center;'>Total Amount</th>";
        fullHtml += "</tr>";
        fullHtml += "</thead>";
        fullHtml += "<tbody>";

        if (model != null && model.Count > 0)
        {
            double grandTotal = 0;
            double grandDiscount = 0;
            double grandVat = 0;
            double grandServiceCharge = 0;
            double totalDiscountedRate = 0;

            int idx = 0;

            foreach (var obj in model)
            {
                idx++;

                string bookingLink = string.IsNullOrEmpty(obj.BookingNo)
                    ? ""
                    : $"<a href='../../BookingService/Details/{obj.BookingId}' target='_blank' style='color:#2980b9;text-decoration:none;font-weight:600;'>{WebUtility.HtmlEncode(obj.BookingNo)}</a>";

                string billLink = string.IsNullOrEmpty(obj.BillNo)
                    ? ""
                    : $"<a href='../../Bill/Details/{obj.BillId}' target='_blank' style='color:#27ae60;text-decoration:none;font-size:12px;'>{WebUtility.HtmlEncode(obj.BillNo)}</a>";

                double discountedRate = obj.Rate - (obj.Discount / (obj.Quantity == 0 ? 1 : obj.Quantity));

                // calculate totals
                grandTotal += obj.TotalAmount;
                grandDiscount += obj.Discount;
                grandVat += obj.VAT;
                grandServiceCharge += obj.ServiceCharge;
                totalDiscountedRate += discountedRate;

                fullHtml += "<tr>";

                fullHtml += $"<td style='text-align:center;'>{idx}</td>";

                fullHtml += $"<td style='text-align:left;'>{WebUtility.HtmlEncode(obj.ServiceName)}</td>";

                fullHtml += $"<td style='text-align:center;'>{obj.ServiceDate.ToString("dd/MM/yyyy")}</td>";

                fullHtml += $"<td style='text-align:center;'>{bookingLink}<br/>{billLink}</td>";

                fullHtml += $"<td style='text-align:center;font-weight:600;'>{WebUtility.HtmlEncode(obj.RoomNo ?? "")}</td>";

                fullHtml += $"<td style='text-align:right;'>{obj.Rate.ToString("N2")}</td>";

                fullHtml += $"<td style='text-align:right;'>{discountedRate.ToString("N2")}</td>";

                fullHtml += $"<td style='text-align:center;'>{obj.Quantity.ToString("N2")}</td>";

                fullHtml += $"<td style='text-align:right;'>{obj.Amount.ToString("N2")}</td>";

                fullHtml += $"<td style='text-align:right;'>{obj.ServiceCharge.ToString("N2")}</td>";

                fullHtml += $"<td style='text-align:right;'>{obj.VAT.ToString("N2")}</td>";

                fullHtml += $"<td style='text-align:right;'>{obj.Discount.ToString("N2")}</td>";

                fullHtml += $"<td style='text-align:right;font-weight:600;'>{obj.TotalAmount.ToString("N2")}</td>";

                fullHtml += "</tr>";
            }

            // Grand totals row
            fullHtml += "<tr style='font-weight:700;background-color:#f8f9fa;'>";
            fullHtml += "<td colspan='6' style='text-align:right;'>Grand Totals:</td>";
            fullHtml += $"<td style='text-align:right;'>{totalDiscountedRate.ToString("N2")}</td>";
            fullHtml += $"<td style='text-align:center;'>{model.Sum(x => x.Quantity)}</td>";
            fullHtml += $"<td style='text-align:right;'>{model.Sum(x => x.Amount).ToString("N2")}</td>";
            fullHtml += $"<td style='text-align:right;'>{grandServiceCharge.ToString("N2")}</td>";
            fullHtml += $"<td style='text-align:right;'>{grandVat.ToString("N2")}</td>";
            fullHtml += $"<td style='text-align:right;'>{grandDiscount.ToString("N2")}</td>";
            fullHtml += $"<td style='text-align:right;'>{grandTotal.ToString("N2")}</td>";
            fullHtml += "</tr>";
        }
        else
        {
            fullHtml += "<tr>";
            fullHtml += "<td colspan='13' style='text-align:center;'><b>No Records Found..</b></td>";
            fullHtml += "</tr>";
        }

        fullHtml += "</tbody>";
        fullHtml += "</table>";
        if (!isPrint)
        {
            fullHtml += "</div>";
            fullHtml += "</div>";
        }

        // === Footer Section ===
        fullHtml += @"<div class='report-footer'>
 			<hr style='width:60%;margin:10px auto;border:1px solid #eee;' />
 			<p>Generated on: " + DateTime.Now.ToString("dd MMM yyyy hh:mm tt") + "</p>";

        fullHtml += @"</div>";
        return fullHtml;
    }

    #endregion

    #region GenerateHallAuditHtml
    public async Task<string> GenerateHallAuditHtmlAsync(string businessDate, bool isPrint = false)
    {
        string businessDateStr = businessDate;

        var model = await GetHallAuditDataForDateAsync(new BookingHallReportVm
        {
            StrFromDate = businessDateStr,
            StrToDate = businessDateStr
        });

        string fullHtml = "";

        if (!isPrint)
        {
            fullHtml += "<div class='card'>";
            fullHtml += "<div class='card-header p-4' style='display:flex;align-items:center;justify-content:space-between;'>";
            fullHtml += "<h5>Hall Audit Report";
            fullHtml += "</h5>";
            fullHtml += "<div class='setting-list'>";
            fullHtml += "<button class='btn btn-primary' id='HallAuditPrintBtn' style='margin-left:15px;'>PDF</button>";
            fullHtml += "</div>";
            fullHtml += "</div>";

            fullHtml += "<div class='card-body'>";
        }
        if (isPrint)
        {
            fullHtml += $@"<p style='text-align:center;padding-bottom:5px;font-size: 10px;'>Date: {businessDate} </p>";
        }
        fullHtml += "<table id='ReportTable' class='table table-bordered report-table' style='width:100%; text-align:center; border-collapse:collapse;'>";
        fullHtml += "<thead>";
        fullHtml += "<tr style='background-color:#f2f2f2;'>";
        fullHtml += "<th style='width:5%;text-align:center;'>SL.</th>";
        fullHtml += "<th style='width:10%;text-align:center;'>Booking Info</th>";
        fullHtml += "<th style='width:10%;text-align:center;'>Guest Info</th>";
        fullHtml += "<th style='width:10%;text-align:center;'>Party Date</th>";
        fullHtml += "<th style='width:10%;text-align:center;'>Hall Name</th>";
        fullHtml += "<th style='width:8%;text-align:center;'>Hall Shift</th>";
        fullHtml += "<th style='width:8%;text-align:center;'>Hall Rent</th>";
        fullHtml += "<th style='width:8%;text-align:center;'>Service Charge</th>";
        fullHtml += "<th style='width:8%;text-align:center;'>VAT</th>";
        fullHtml += "<th style='width:8%;text-align:center;'>Discount</th>";
        fullHtml += "<th style='width:8%;text-align:center;'>Net Rent</th>";
        fullHtml += "</tr>";
        fullHtml += "</thead>";
        fullHtml += "<tbody>";

        if (model != null && model.Count > 0)
        {
            int idx = 0;
            double totalHallRent = 0;
            double totalServiceCharge = 0;
            double totalVat = 0;
            double totalDiscount = 0;
            double totalNetRent = 0;

            foreach (var obj in model)
            {
                idx++;

                string bookingLink = string.IsNullOrEmpty(obj.BookingNo)
                    ? ""
                    : $"<a href='../../BookingService/HallDetails/{obj.BookingId}' target='_blank' style='color:#2980b9;text-decoration:none;font-weight:600;'>{WebUtility.HtmlEncode(obj.BookingNo ?? "")}</a>";

                string bookingDate = obj.BookingDate.ToString("dd/MMM/yyyy", CultureInfo.CurrentCulture);
                string guestInfo = $"{WebUtility.HtmlEncode(obj.GuestName ?? "")}<br/>{WebUtility.HtmlEncode(obj.Mobile ?? "")}";
                string partyDate = obj.PartyDate.ToString("dd/MMM/yyyy");
                string hallName = WebUtility.HtmlEncode(obj.HallName ?? "");
                string hallShift = WebUtility.HtmlEncode(obj.HallShift ?? "");

                string hallRent = obj.HallRent.ToString("N2", CultureInfo.CurrentCulture);
                string serviceCharge = obj.ServiceCharge.ToString("N2", CultureInfo.CurrentCulture);
                string vat = obj.Vat.ToString("N2", CultureInfo.CurrentCulture);
                string discount = obj.Discount.ToString("N2", CultureInfo.CurrentCulture);
                string netRent = obj.NetRent.ToString("N2", CultureInfo.CurrentCulture);

                // accumulate totals
                totalHallRent += obj.HallRent;
                totalServiceCharge += obj.ServiceCharge;
                totalVat += obj.Vat;
                totalDiscount += obj.Discount;
                totalNetRent += obj.NetRent;

                fullHtml += "<tr>";
                fullHtml += $"<td style='text-align:center;'>{idx}</td>";
                fullHtml += $"<td style='text-align:center;font-weight:600;'>{bookingLink} <br />{WebUtility.HtmlEncode(bookingDate)}</td>";
                fullHtml += $"<td style='text-align:center;'>{guestInfo}</td>";
                fullHtml += $"<td style='text-align:center;'>{WebUtility.HtmlEncode(partyDate)}</td>";
                fullHtml += $"<td style='text-align:center;'>{hallName}</td>";
                fullHtml += $"<td style='text-align:center;'>{hallShift}</td>";
                fullHtml += $"<td style='text-align:right;'>{hallRent}</td>";
                fullHtml += $"<td style='text-align:right;'>{serviceCharge}</td>";
                fullHtml += $"<td style='text-align:right;'>{vat}</td>";
                fullHtml += $"<td style='text-align:right;'>{discount}</td>";
                fullHtml += $"<td style='text-align:right;font-weight:600;'>{netRent}</td>";
                fullHtml += "</tr>";
            }

            // Grand totals row: colspan 6 then 5 amount columns
            fullHtml += "<tr style='font-weight:700;background-color:#f8f9fa;'>";
            fullHtml += "<td colspan='6' style='text-align:right;'>Grand Totals:</td>";
            fullHtml += $"<td style='text-align:right;'>{totalHallRent.ToString("N2", CultureInfo.CurrentCulture)}</td>";
            fullHtml += $"<td style='text-align:right;'>{totalServiceCharge.ToString("N2", CultureInfo.CurrentCulture)}</td>";
            fullHtml += $"<td style='text-align:right;'>{totalVat.ToString("N2", CultureInfo.CurrentCulture)}</td>";
            fullHtml += $"<td style='text-align:right;'>{totalDiscount.ToString("N2", CultureInfo.CurrentCulture)}</td>";
            fullHtml += $"<td style='text-align:right;'>{totalNetRent.ToString("N2", CultureInfo.CurrentCulture)}</td>";
            fullHtml += "</tr>";
        }
        else
        {
            fullHtml += "<tr>";
            fullHtml += "<td colspan='11' style='text-align:center;'><b>No Records Found..</b></td>";
            fullHtml += "</tr>";
        }

        fullHtml += "</tbody>";
        fullHtml += "</table>";
        if (!isPrint)
        {
            fullHtml += "</div>";
            fullHtml += "</div>";
        }
        // === Footer Section ===
        fullHtml += @"<div class='report-footer'>
 			<hr style='width:60%;margin:10px auto;border:1px solid #eee;' />
 			<p>Generated on: " + DateTime.Now.ToString("dd MMM yyyy hh:mm tt") + "</p>";

        fullHtml += @"</div>";
        return fullHtml;
    }
    #endregion

    #endregion

    #region RoomReAudit

    public async Task<int> RoomReAuditAsync(DateTime businessDate)
    {
        var targetDate = businessDate.Date;

        // 1️⃣ Delete old audits
        var oldAudits = await _iRepository.GetAsync(a => a.BusinessDate == targetDate);
        if (oldAudits.Any())
        {
            _iRepository.RemoveRange(oldAudits);
        }

        // 2️⃣ Get booking rooms valid for this date
        var bookingRooms = await _iBookingRoomRepository.GetAsync(br =>
            !br.IsDeleted &&
            br.CheckInTime.Date <= targetDate &&
            br.CheckOutTime.Date > targetDate &&
            br.ActualCheckInTime != null);

        if (!bookingRooms.Any())
        {
            await _iUnitOfWork.CompleteAsync();
            return 0;
        }

        var newAudits = new List<HtRoomDayAudit>();

        foreach (var br in bookingRooms)
        {
            var dailyRates = CalculateDailyRates(br, targetDate);

            var audit = new HtRoomDayAudit
            {
                BusinessDate = targetDate,
                BookingRoomId = br.Id,
                RoomId = br.RoomId ?? 0,
                RoomCategoryId = br.RoomCategoryId,

                Adult = (int)br.Adult,
                Child = (int)br.Child,

                IsStay = true,
                IsOccupied = true,
                IsCharged = false,

                Status = GetAuditStatus(br, targetDate),

                RackRate = dailyRates.RackRate,
                Rate = dailyRates.Rate,
                BaseRate = dailyRates.BaseRate,
                ServiceCharge = dailyRates.ServiceCharge,
                Vat = dailyRates.Vat,
                NetAmount = dailyRates.NetAmount,

                ActionDate = Utility.GetBdDateTimeNow(),
                ActionById = CurrentUserId
            };

            newAudits.Add(audit);
        }

        await _iRepository.AddRangeAsync(newAudits);
        await _iUnitOfWork.CompleteAsync();

        return newAudits.Count;
    }

    // Helper method to calculate daily rates
    private DailyRates CalculateDailyRates(HtBookingRoom br, DateTime targetDate)
    {
        // ===== Day Calculation =====
        double days = AppUtility.DaysDiffernceOnlyDate(br.CheckOutTime, br.CheckInTime);
        if (days <= 0) days = 1;

        // Half / Day use adjustment
        if (br.ActualCheckOutTime != null &&
            targetDate == br.ActualCheckOutTime.Value.Date &&
            br.BookingDayStatus > 0)
        {
            if (br.BookingDayStatus == (int)BookingDayStatusEnum.HalfDay)
                days += 0.5;
            else if (br.BookingDayStatus == (int)BookingDayStatusEnum.DayUse)
                days += 1;
        }

        // ===== Rate Calculation =====
        double discount = br.Discount;
        double roomRent = br.RoomRent;
        double baseRent = br.Rent;

        double ratePerDay = discount > 0
            ? roomRent - (discount / days)
            : roomRent;

        double baseRatePerDay = discount > 0
            ? (baseRent - discount) / days
            : baseRent / days;

        double scPerDay = br.ServiceCharge / days;
        double vatPerDay = br.Vat / days;
        double netPerDay = br.NetRent / days;

        // Handle half day checkout
        bool isHalfDayCheckout =
            br.ActualCheckOutTime != null &&
            targetDate == br.ActualCheckOutTime.Value.Date &&
            br.BookingDayStatus == (int)BookingDayStatusEnum.HalfDay;

        if (isHalfDayCheckout)
        {
            ratePerDay /= 2;
            baseRatePerDay /= 2;
            scPerDay /= 2;
            vatPerDay /= 2;
            netPerDay /= 2;
        }

        return new DailyRates
        {
            RackRate = roomRent,
            Rate = ratePerDay,
            BaseRate = baseRatePerDay,
            ServiceCharge = scPerDay,
            Vat = vatPerDay,
            NetAmount = netPerDay
        };
    }

    private AuditStayStatus GetAuditStatus(HtBookingRoom br, DateTime targetDate)
    {
        return (br.ActualCheckOutTime != null &&
                targetDate == br.ActualCheckOutTime.Value.Date)
                ? AuditStayStatus.CheckedOut
                : AuditStayStatus.Planned;
    }

    private class DailyRates
    {
        public double RackRate { get; set; }
        public double Rate { get; set; }
        public double BaseRate { get; set; }
        public double ServiceCharge { get; set; }
        public double Vat { get; set; }
        public double NetAmount { get; set; }
    }

    #endregion
}
