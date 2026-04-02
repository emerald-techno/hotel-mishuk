using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Enums.AppEnums;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.BusinessDaySummary;
using Domain.ViewModel.HotelManagement.HotelReport;
using Domain.ViewModel.Report;
using Interface.Repository.HotelManagement;
using Interface.Repository.Restaurant;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.HotelManagement;

public class DailyAuditSummaryRepository : BaseRepository<HtDailyAuditSummary>, IDailyAuditSummaryRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;
    private readonly IBookingServiceRepository _iBookingServiceRepository;
    private readonly IFoodOrderRepository _iFoodOrderRepository;
    public DailyAuditSummaryRepository(ApplicationDbContext db, IMapper iMapper,
        IBookingServiceRepository iBookingServiceRepository,
        IFoodOrderRepository iFoodOrderRepository) : base(db)
    {
        Db = db;
        _iMapper = iMapper;
        _iBookingServiceRepository = iBookingServiceRepository;
        _iFoodOrderRepository = iFoodOrderRepository;
    }
    #endregion

    #region Dispose

    public void Dispose()
    {
        Context.Dispose();
    }

    #endregion

    #region BusinessDaySummaryPreview

    public async Task<BusinessDaySummaryVm> GenerateBusinessDaySummaryPreviewAsync(DateTime businessDate)
    {
        var targetDate = businessDate.Date;

        var totalRoomsAvailable = await Context.HtRoomInfos.CountAsync(r => !r.IsDeleted);
        var totalRoomsOutOfOrder = await Context.HtRoomInfos.CountAsync(r => r.HouseKeeperAvailabilityStatus == AvailabilityStatusEnum.OutOfOrder);

        var roomAgg = await Context.HtRoomDayAudits
            .Where(r => r.BusinessDate == targetDate && r.IsCharged)
            .GroupBy(r => 1)
            .Select(g => new
            {
                TotalAdults = g.Sum(x => (int?)x.Adult) ?? 0,
                TotalChildren = g.Sum(x => (int?)x.Child) ?? 0,
                RoomsOccupied = g.Count(x => x.IsOccupied),
                TotalRackRate = g.Sum(x => (decimal?)x.RackRate) ?? 0m,
                TotalBaseRate = g.Sum(x => (decimal?)x.BaseRate) ?? 0m,
                TotalNetAmount = g.Sum(x => (decimal?)x.NetAmount) ?? 0m,
                TotalServiceCharge = g.Sum(x => (decimal?)x.ServiceCharge) ?? 0m,
                TotalVat = g.Sum(x => (decimal?)x.Vat) ?? 0m,
                TotalDiscounts = g.Sum(x => (decimal?)((x.RackRate - x.Rate))) ?? 0m,
                TotalComplimentary = g.Sum(x => (decimal?)(x.IsCharged ? 0m : 0m)) ?? 0m, // placeholder
                CheckIns = g.Count(x => x.Status == AuditStayStatus.Stayed || x.Status == AuditStayStatus.Planned),
                CheckOuts = g.Count(x => x.Status == AuditStayStatus.CheckedOut),
                NoShows = g.Count(x => x.Status == AuditStayStatus.NoShow)
            })
            .FirstOrDefaultAsync();


        // Restaurant revenue totals (gross/net/service/vat/discount)
        var restRevenueAgg = await Context.RsFoodOrders
            .Where(i => i.OrderDate.Date == targetDate && i.AuditDate != null)
            .GroupBy(i => 1)
            .Select(g => new
            {
                TotalGross = g.Sum(x => (decimal?)x.OrderAmount) ?? 0m,        // if you have TotalAmount/Gross
                TotalServiceCharge = g.Sum(x => (decimal?)x.ServiceCharge) ?? 0m,
                TotalVat = g.Sum(x => (decimal?)x.VAT) ?? 0m,
                TotalDiscounts = g.Sum(x => (decimal?)x.Discount) ?? 0m,
                TotalNet = g.Sum(x => (decimal?)x.NetAmount) ?? 0m
            })
            .FirstOrDefaultAsync();

        //Front Office Due Report
        var guestDueData = await _iBookingServiceRepository.GetGuestDueReport(new GuestDueReportVm
        {
            StrFromDate = targetDate.ToString("dd/MM/yyyy"),
            StrToDate = targetDate.ToString("dd/MM/yyyy")
        });

        var guestDueAmount = guestDueData.Count > 0 ? guestDueData.Sum(x => x.DueAmount) : 0;

        //F&B Due Report
        var restaurentDueData = await _iFoodOrderRepository.GetRsDailySalesSummaryReportData(new RsDailySalesSummaryVm
        {
            StrQueryDate = targetDate.ToString("dd/MM/yyyy")
        });

        var resDueAmount = restaurentDueData.Count > 0 ? restaurentDueData.Sum(x => x.DueAmount) : 0;

        decimal combinedDueAmount = guestDueAmount + (decimal)resDueAmount;

        // Banquet revenue
        var banquetTotal = await Context.HtBookingHalls
            .Where(b => b.BookingDate.Date == targetDate && b.AuditDate != null)
            .Select(b => (decimal?)b.NetRent)
            .SumAsync() ?? 0m;

        // Other extra services (laundry, transport, extras, extra bed, late check-out)
        var extraServiceTotalList = await Context.HtBillingDetails
            .Where(o => o.Service.ServiceCode != HtServiceCode.RoomRent
            && o.Service.ServiceCode != HtServiceCode.FoodService
            && o.Service.ServiceCode != HtServiceCode.HallRent).ToListAsync();

        var extraServiceTotal = extraServiceTotalList.Where(o => o.ServiceDate != null && o.ServiceDate.Value.Date == targetDate && o.AuditDate != null)
            .Select(o => (decimal?)o.NetAmount)
            .Sum() ?? 0m;

        var extraServiceServiceChargeTotal = extraServiceTotalList.Where(o => o.ServiceDate != null && o.ServiceDate.Value.Date == targetDate && o.AuditDate != null)
            .Select(o => (decimal?)o.ServiceCharge)
            .Sum() ?? 0m;

        var extraServiceVatTotal = extraServiceTotalList.Where(o => o.ServiceDate != null && o.ServiceDate.Value.Date == targetDate && o.AuditDate != null)
            .Select(o => (decimal?)o.VAT)
            .Sum() ?? 0m;

        // Combine F&B totals as appropriate
        var totalRestaurantSales = restRevenueAgg?.TotalGross ?? 0m;
        var totalRestaurantRevenue = restRevenueAgg?.TotalNet ?? 0m;
        var totalFnbServiceCharge = restRevenueAgg?.TotalServiceCharge ?? 0m;
        var totalFnbVat = restRevenueAgg?.TotalVat ?? 0m;
        var totalFnbDiscounts = restRevenueAgg?.TotalDiscounts ?? 0m;

        var totalBanquetRevenue = banquetTotal;

        var totalOtherRevenue = extraServiceTotal; // this can include non-room F&B extras

        // Optionally you can compute a combined F&B revenue (restaurant + banquet + extra service)
        var combinedFnbRevenue = totalRestaurantRevenue + totalBanquetRevenue + totalOtherRevenue;

        // Restaurant payments for the day
        var restaurantPayments = Context.RsOrderPayments
            .Where(p => p.PaidDate.Date == targetDate);

        restaurantPayments = restaurantPayments.Where(x => x.BillDtlId == null);

        restaurantPayments = restaurantPayments.Where(x => x.AuditById > 0 && x.AuditDate != null);

        //var restAdvancePayments = restaurantPayments.Where(p => p.IsAdvance);
        //var restCurrentPayments = restaurantPayments.Where(p => !p.IsAdvance);

        var restByMethod = await restaurantPayments
            .GroupBy(p => p.PayMode)
            .Select(g => new { Method = g.Key, Total = g.Sum(x => (decimal?)x.PaidAmount) ?? 0m, Cover = g.Count() })
            .ToListAsync();

        // Front office payments for the day
        var frontPayments = Context.HtBookingPayments
            .Where(p => p.PaidDate.Date == targetDate);

        frontPayments = frontPayments.Where(x => x.AuditById > 0 && x.AuditDate != null);

        var frontAdvancePayments = frontPayments.Where(p => p.IsAdvance);
        var frontCurrentPayments = frontPayments.Where(p => !p.IsAdvance);

        var frontByMethod = await frontCurrentPayments
            .GroupBy(p => p.PayMode)
            .Select(g => new { Method = g.Key, Total = g.Sum(x => (decimal?)x.PaidAmount) ?? 0m, Cover = g.Count() })
            .ToListAsync();

        var frontAdvanceByMethod = await frontAdvancePayments
            .GroupBy(p => p.PayMode)
            .Select(g => new { Method = g.Key, Total = g.Sum(x => (decimal?)x.PaidAmount) ?? 0m, Cover = g.Count() })
            .ToListAsync();

        decimal SumByMethod(IEnumerable<dynamic> list, PayModeEnum method)
        {
            var item = list.FirstOrDefault(x => x.Method == method);
            return item == null ? 0m : (decimal)item.Total;
        }

        // fill restaurant method totals
        var restCash = SumByMethod(restByMethod, PayModeEnum.Cash);
        var restCard = SumByMethod(restByMethod, PayModeEnum.Card);
        var restMbank = SumByMethod(restByMethod, PayModeEnum.Bkash);
        var restBank = SumByMethod(restByMethod, PayModeEnum.Bank);

        // fill front office method totals
        var frontCash = SumByMethod(frontByMethod, PayModeEnum.Cash);
        var frontCard = SumByMethod(frontByMethod, PayModeEnum.Card);
        var frontMbank = SumByMethod(frontByMethod, PayModeEnum.Bkash);
        var frontBank = SumByMethod(frontByMethod, PayModeEnum.Bank);

        // fill front office advance method totals
        var frontAdvanceCash = SumByMethod(frontAdvanceByMethod, PayModeEnum.Cash);
        var frontAdvanceCard = SumByMethod(frontAdvanceByMethod, PayModeEnum.Card);
        var frontAdvanceMbank = SumByMethod(frontAdvanceByMethod, PayModeEnum.Bkash);
        var frontAdvanceBank = SumByMethod(frontAdvanceByMethod, PayModeEnum.Bank);

        // compute overall collections (restaurant + front office; add other departments if you have them)
        var totalCashCollection = restCash + frontCash + frontAdvanceCash;
        var totalCardCollection = restCard + frontCard + frontAdvanceCard;
        var totalMbankingCollection = restMbank + frontMbank + frontAdvanceMbank;
        var totalBankCollection = restBank + frontBank + frontAdvanceBank;

        //FO And Restaurant Collection Covers
        var totalResCashCover = restByMethod.FirstOrDefault(x => x.Method == PayModeEnum.Cash)?.Cover ?? 0;
        var totalResCardCover = restByMethod.FirstOrDefault(x => x.Method == PayModeEnum.Card)?.Cover ?? 0;
        var totalResMBankingCover = restByMethod.FirstOrDefault(x => x.Method == PayModeEnum.Bkash)?.Cover ?? 0;
        var totalResBankCover = restByMethod.FirstOrDefault(x => x.Method == PayModeEnum.Bank)?.Cover ?? 0;

        var totalFrontCashCover = frontByMethod.FirstOrDefault(x => x.Method == PayModeEnum.Cash)?.Cover ?? 0;
        var totalFrontCardCover = frontByMethod.FirstOrDefault(x => x.Method == PayModeEnum.Card)?.Cover ?? 0;
        var totalFrontMBankingCover = frontByMethod.FirstOrDefault(x => x.Method == PayModeEnum.Bkash)?.Cover ?? 0;
        var totalFrontBankCover = frontByMethod.FirstOrDefault(x => x.Method == PayModeEnum.Bank)?.Cover ?? 0;

        var totalFrontCover = totalFrontCashCover + totalFrontCardCover + totalFrontMBankingCover + totalFrontBankCover;
        var totalFrontAdvanceCover = frontAdvanceByMethod.Sum(x => x.Cover);

        var frontAdvance = frontAdvancePayments.Sum(x => x.PaidAmount);

        var preview = new BusinessDaySummaryVm
        {
            BusinessDate = targetDate,
            TotalRoomsAvailable = totalRoomsAvailable,
            TotalRoomsOutOfOrder = totalRoomsOutOfOrder,
            TotalRoomsOccupied = roomAgg?.RoomsOccupied ?? 0,
            TotalRoomsVacant = totalRoomsAvailable - (roomAgg?.RoomsOccupied ?? 0) - totalRoomsOutOfOrder,
            TotalCheckIns = roomAgg?.CheckIns ?? 0,
            TotalCheckOuts = roomAgg?.CheckOuts ?? 0,
            TotalNoShows = roomAgg?.NoShows ?? 0,
            TotalRoomMoves = 0, // compute from room status history if you maintain it
            TotalAdults = roomAgg?.TotalAdults ?? 0,
            TotalChildren = roomAgg?.TotalChildren ?? 0,
            TotalRackRate = roomAgg?.TotalRackRate ?? 0m,
            TotalBaseRate = roomAgg?.TotalBaseRate ?? 0m,
            TotalRoomRevenue = roomAgg?.TotalNetAmount ?? 0m,
            TotalRoomServiceCharge = roomAgg?.TotalServiceCharge ?? 0m,
            TotalRoomVat = roomAgg?.TotalVat ?? 0m,
            TotalRoomDiscounts = roomAgg?.TotalDiscounts ?? 0m,
            TotalComplimentary = roomAgg?.TotalComplimentary ?? 0m,
            TotalAdvanceCollection = 0m, // if you store advances, aggregate from advance receipts

            // -- F&B revenue fields filled from invoice aggregations --
            TotalRestaurantSales = totalRestaurantSales,
            TotalRestaurantRevenue = totalRestaurantRevenue,
            TotalFnbServiceCharge = totalFnbServiceCharge,
            TotalFnbVat = totalFnbVat,
            TotalFnbDiscounts = totalFnbDiscounts,
            TotalBanquetRevenue = totalBanquetRevenue,
            TotalOtherRevenue = totalOtherRevenue,

            // --- Front Office Collections ---
            TotalFrontOfficeCash = frontCash,
            TotalFrontOfficeCard = frontCard,
            TotalFrontOfficeMbanking = frontMbank,
            TotalFrontOfficeBank = frontBank,

            TotalFrontOfficeAdvance = (decimal)frontAdvance,

            // --- per-department method fields (make sure your VM has these props) ---
            TotalRestaurantCash = restCash,
            TotalRestaurantCard = restCard,
            TotalRestaurantMbanking = restMbank,
            TotalRestaurantBank = restBank,

            //---Restaurant Collection Covers ---
            RestaurantCashCover = totalResCashCover,
            RestaurantCardCover = totalResCardCover,
            RestaurantMbankingCover = totalResMBankingCover,
            RestaurantBankCover = totalResBankCover,

            //--- Front Collection Covers ---

            FrontOfficeCashCover = totalFrontCashCover,
            FrontOfficeCardCover = totalFrontCardCover,
            FrontOfficeMbankingCover = totalFrontMBankingCover,
            FrontOfficeBankCover = totalFrontBankCover,
            FrontOfficeAdvanceCover = totalFrontAdvanceCover,

            //---  Due ---
            FrontOfficeReceivable = guestDueAmount,
            FNBReceivable = (decimal)resDueAmount,
            TotalReceivable = combinedDueAmount,

            Remarks = string.Empty
        };

        // computed convenience totals (if VM has these; otherwise you can compute where you render)
        preview.TotalRestaurantCollections =
            restCash + restCard + restMbank + restBank;
        preview.TotalFrontOfficeCollections =
            frontCash + frontCard + frontMbank + frontBank;

        preview.TotalCashCollection = frontCash + restCash;
        preview.TotalCardCollection = frontCard + restCard;
        preview.TotalMobileBankingCollection = frontMbank + restMbank;

        preview.TotalAdvanceCollection = (decimal)frontAdvance;

        int occRooms = preview.TotalRoomsOccupied;
        int availRooms = preview.TotalRoomsAvailable;

        preview.OccupancyPercentage = availRooms > 0
            ? (double)Math.Round(preview.TotalRoomsOccupied / (decimal)availRooms * 100m, 2, MidpointRounding.AwayFromZero)
            : 0.0;

        preview.ADR = occRooms > 0 ? (double)Math.Round(preview.TotalRoomRevenue / occRooms, 2, MidpointRounding.AwayFromZero) : 0.0;
        preview.ARR = occRooms > 0 ? (double)Math.Round(preview.TotalRackRate / occRooms, 2, MidpointRounding.AwayFromZero) : 0.0;
        preview.RevPAR = availRooms > 0 ? (double)Math.Round(preview.TotalRoomRevenue / availRooms, 2, MidpointRounding.AwayFromZero) : 0.0;
        preview.AvgGuestsPerRoom = occRooms > 0 ? (double)Math.Round(preview.TotalGuests / (decimal)occRooms, 2, MidpointRounding.AwayFromZero) : 0.0;

        var businessDay = await Context.HtBusinessDays.FirstOrDefaultAsync(b => b.BusinessDate == targetDate);
        if (businessDay == null)
            throw new InvalidOperationException($"Business day not found for {targetDate:yyyy-MM-dd}");
        if (businessDay != null)
            preview.AuditStatus = businessDay.AuditStatus;

        return preview;
    }

    #endregion

    #region Day Close
    public async Task<bool> CloseBusinessDayAsync(DateTime businessDate, bool applyPreview = true, long currentUserId = 0)
    {
        var targetDate = businessDate.Date;

        var businessDay = await Context.HtBusinessDays.FirstOrDefaultAsync(b => b.BusinessDate == targetDate);
        if (businessDay == null)
            throw new InvalidOperationException($"Business day not found for {targetDate:yyyy-MM-dd}");
        if (businessDay.AuditStatus == 1)
            throw new InvalidOperationException($"Business day {targetDate:yyyy-MM-dd} already closed.");

        BusinessDaySummaryVm preview;
        if (applyPreview)
        {
            preview = await GenerateBusinessDaySummaryPreviewAsync(targetDate);
        }
        else
        {
            preview = await GenerateBusinessDaySummaryPreviewAsync(targetDate);
        }

        var now = DateTime.UtcNow;

        await using var tx = await Context.Database.BeginTransactionAsync();
        try
        {
            var existing = await Context.HtDailyAuditSummaries.FirstOrDefaultAsync(s => s.BusinessDate == targetDate);
            if (existing == null)
            {
                existing = new HtDailyAuditSummary
                {
                    BusinessDate = targetDate,
                    TotalRoomsAvailable = preview.TotalRoomsAvailable,
                    TotalRoomsOccupied = preview.TotalRoomsOccupied,
                    TotalRoomsVacant = preview.TotalRoomsVacant,
                    TotalRoomsOutOfOrder = preview.TotalRoomsOutOfOrder,
                    TotalRoomsHouseUse = preview.TotalRoomsHouseUse,
                    TotalRoomMoves = preview.TotalRoomMoves,
                    TotalCheckIns = preview.TotalCheckIns,
                    TotalCheckOuts = preview.TotalCheckOuts,
                    TotalNoShows = preview.TotalNoShows,
                    TotalAdults = (int)preview.TotalAdults,
                    TotalChildren = (int)preview.TotalChildren,
                    TotalRackRate = (double)preview.TotalRackRate,
                    TotalRoomRevenue = (double)preview.TotalRoomRevenue,
                    TotalRoomServiceCharge = (double)preview.TotalRoomServiceCharge,
                    TotalRoomVat = (double)preview.TotalRoomVat,
                    TotalRoomDiscounts = (double)preview.TotalRoomDiscounts,
                    TotalRoomComplimentary = (double)preview.TotalComplimentary,
                    TotalRoomChargePosted = (double)preview.TotalRoomRevenue,
                    TotalRestaurantRevenue = (double)preview.TotalRestaurantRevenue,
                    TotalRestaurantOrderAmount = (double)preview.TotalRestaurantSales,
                    TotalFNBServiceCharge = (double)preview.TotalFnbServiceCharge,
                    TotalFNBVat = (double)preview.TotalFnbVat,
                    TotalFNBDiscounts = (double)preview.TotalFnbDiscounts,
                    TotalRoomServiceRevenue = (double)preview.TotalOtherRevenue,
                    TotalOtherRevenue = (double)preview.TotalOtherRevenue,
                    TotalCashCollection = preview.TotalCashCollection,
                    TotalCardCollection = preview.TotalCardCollection,
                    TotalMbankingCollection = preview.TotalMobileBankingCollection,
                    TotalCreditCollection = preview.TotalCreditCollection,
                    TotalAdvanceCollection = preview.TotalAdvanceCollection,
                    OccupancyPercentage = (decimal)preview.OccupancyPercentage,

                    TotalBanquetRevenue = (double)preview.TotalBanquetRevenue,

                    TotalFrontOfficeAdvance = preview.TotalFrontOfficeAdvance,
                    TotalFrontOfficeCash = preview.TotalFrontOfficeCash,
                    TotalFrontOfficeCard = preview.TotalFrontOfficeCard,
                    TotalFrontOfficeMbanking = preview.TotalFrontOfficeMbanking,
                    TotalFrontOfficeBank = preview.TotalFrontOfficeBank,

                    TotalRestaurantAdvance = preview.TotalRestaurantAdvance,
                    TotalRestaurantCash = preview.TotalRestaurantCash,
                    TotalRestaurantCard = preview.TotalRestaurantCard,
                    TotalRestaurantMbanking = preview.TotalRestaurantMbanking,
                    TotalRestaurantBank = preview.TotalRestaurantBank,

                    FrontOfficeAdvanceCover = preview.FrontOfficeAdvanceCover,
                    FrontOfficeCashCover = preview.FrontOfficeCashCover,
                    FrontOfficeCardCover = preview.FrontOfficeCardCover,
                    FrontOfficeMbankingCover = preview.FrontOfficeMbankingCover,
                    FrontOfficeBankCover = preview.FrontOfficeBankCover,

                    RestaurantAdvanceCover = preview.RestaurantAdvanceCover,
                    RestaurantCashCover = preview.RestaurantCashCover,
                    RestaurantCardCover = preview.RestaurantCardCover,
                    RestaurantMbankingCover = preview.RestaurantMbankingCover,
                    RestaurantBankCover = preview.RestaurantBankCover,

                    FrontOfficeReceivable = preview.FrontOfficeReceivable,
                    FNBReceivable = preview.FNBReceivable,
                    TotalReceivable = preview.TotalReceivable,

                    ADR = (decimal)preview.ADR,
                    ARR = (decimal)preview.ARR,
                    RevPAR = (decimal)preview.RevPAR,
                    AvgGuestsPerRoom = (decimal)preview.AvgGuestsPerRoom,
                    Remarks = preview.Remarks,
                    ActionDate = now,
                    ActionById = currentUserId
                };

                Context.HtDailyAuditSummaries.Add(existing);
            }
            else
            {
                existing.TotalRoomsAvailable = preview.TotalRoomsAvailable;
                existing.TotalRoomsOccupied = preview.TotalRoomsOccupied;
                existing.TotalRoomsVacant = preview.TotalRoomsVacant;
                existing.TotalRoomsOutOfOrder = preview.TotalRoomsOutOfOrder;
                existing.TotalRoomsHouseUse = preview.TotalRoomsHouseUse;
                existing.TotalRoomMoves = preview.TotalRoomMoves;
                existing.TotalCheckIns = preview.TotalCheckIns;
                existing.TotalCheckOuts = preview.TotalCheckOuts;
                existing.TotalNoShows = preview.TotalNoShows;
                existing.TotalAdults = (int)preview.TotalAdults;
                existing.TotalChildren = (int)preview.TotalChildren;
                existing.TotalRackRate = (double)preview.TotalRackRate;
                existing.TotalRoomRevenue = (double)preview.TotalRoomRevenue;
                existing.TotalRoomServiceCharge = (double)preview.TotalRoomServiceCharge;
                existing.TotalRoomVat = (double)preview.TotalRoomVat;
                existing.TotalRoomDiscounts = (double)preview.TotalRoomDiscounts;
                existing.TotalRoomComplimentary = (double)preview.TotalComplimentary;
                existing.TotalRoomChargePosted = (double)preview.TotalRoomRevenue;
                existing.TotalRestaurantRevenue = (double)preview.TotalRestaurantRevenue;
                existing.TotalRestaurantOrderAmount = (double)preview.TotalRestaurantSales;
                existing.TotalFNBServiceCharge = (double)preview.TotalFnbServiceCharge;
                existing.TotalFNBVat = (double)preview.TotalFnbVat;
                existing.TotalFNBDiscounts = (double)preview.TotalFnbDiscounts;
                existing.TotalRoomServiceRevenue = (double)preview.TotalOtherRevenue;
                existing.TotalOtherRevenue = (double)preview.TotalOtherRevenue;
                existing.TotalCashCollection = preview.TotalCashCollection;
                existing.TotalCardCollection = preview.TotalCardCollection;
                existing.TotalMbankingCollection = preview.TotalMobileBankingCollection;
                existing.TotalCreditCollection = preview.TotalCreditCollection;
                existing.TotalAdvanceCollection = preview.TotalAdvanceCollection;
                existing.OccupancyPercentage = (decimal)preview.OccupancyPercentage;
                existing.TotalBanquetRevenue = (double)preview.TotalBanquetRevenue;
                existing.ADR = (decimal)preview.ADR;
                existing.ARR = (decimal)preview.ARR;
                existing.RevPAR = (decimal)preview.RevPAR;
                existing.AvgGuestsPerRoom = (decimal)preview.AvgGuestsPerRoom;
                existing.Remarks = preview.Remarks;
                existing.ActionDate = now;
                existing.ActionById = currentUserId;

                existing.TotalFrontOfficeAdvance = preview.TotalFrontOfficeAdvance;
                existing.TotalFrontOfficeCash = preview.TotalFrontOfficeCash;
                existing.TotalFrontOfficeCard = preview.TotalFrontOfficeCard;
                existing.TotalFrontOfficeMbanking = preview.TotalFrontOfficeMbanking;
                existing.TotalFrontOfficeBank = preview.TotalFrontOfficeBank;

                existing.TotalRestaurantAdvance = preview.TotalRestaurantAdvance;
                existing.TotalRestaurantCash = preview.TotalRestaurantCash;
                existing.TotalRestaurantCard = preview.TotalRestaurantCard;
                existing.TotalRestaurantMbanking = preview.TotalRestaurantMbanking;
                existing.TotalRestaurantBank = preview.TotalRestaurantBank;

                existing.FrontOfficeAdvanceCover = preview.FrontOfficeAdvanceCover;
                existing.FrontOfficeCashCover = preview.FrontOfficeCashCover;
                existing.FrontOfficeCardCover = preview.FrontOfficeCardCover;
                existing.FrontOfficeMbankingCover = preview.FrontOfficeMbankingCover;
                existing.FrontOfficeBankCover = preview.FrontOfficeBankCover;

                existing.RestaurantAdvanceCover = preview.RestaurantAdvanceCover;
                existing.RestaurantCashCover = preview.RestaurantCashCover;
                existing.RestaurantCardCover = preview.RestaurantCardCover;
                existing.RestaurantMbankingCover = preview.RestaurantMbankingCover;
                existing.RestaurantBankCover = preview.RestaurantBankCover;

                existing.FrontOfficeReceivable = preview.FrontOfficeReceivable;
                existing.FNBReceivable = preview.FNBReceivable;
                existing.TotalReceivable = preview.TotalReceivable;

                Context.HtDailyAuditSummaries.Update(existing);
            }

            businessDay.TotalRoomRevenue = (double)preview.TotalRoomRevenue;
            businessDay.TotalFoodAndBeverageRevenue = (double)preview.TotalRestaurantRevenue;
            businessDay.TotalHallRevenue = businessDay.TotalHallRevenue; // if you compute hall separately, set here
            businessDay.TotalOtherRevenue = (double)preview.TotalOtherRevenue;
            businessDay.EndDateTime = now;
            businessDay.AuditStatus = 1; // closed
            businessDay.ActionDate = now;
            businessDay.ActionById = currentUserId;
            Context.HtBusinessDays.Update(businessDay);

            await Context.SaveChangesAsync();


            var nextDate = targetDate.AddDays(1);

            var existingNext = await Context.HtBusinessDays
                                       .FirstOrDefaultAsync(b => b.BusinessDate == nextDate);

            HtBusinessDay nextBusinessDay;
            if (existingNext != null)
            {
                nextBusinessDay = existingNext;
            }
            else
            {
                var startDateTime = businessDay.EndDateTime ?? now;

                var startDateTimeUtc = startDateTime.Kind == DateTimeKind.Utc
                    ? startDateTime
                    : DateTime.SpecifyKind(startDateTime, DateTimeKind.Utc);

                nextBusinessDay = new HtBusinessDay
                {
                    BusinessDate = nextDate,
                    StartDateTime = startDateTimeUtc,
                    EndDateTime = null,
                    AuditStatus = 0, // open
                    TotalRoomRevenue = 0d,
                    TotalFoodAndBeverageRevenue = 0d,
                    TotalHallRevenue = 0d,
                    TotalOtherRevenue = 0d,
                    ActionDate = now,
                    ActionById = 1
                };

                Context.HtBusinessDays.Add(nextBusinessDay);
                await Context.SaveChangesAsync();
            }

            await tx.CommitAsync();

            return true;
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }
    #endregion

    #region GetBusinessDaySummaryAsync

    public async Task<BusinessDaySummaryVm> GetBusinessDaySummaryAsync(DateTime businessDate)
    {
        var targetDate = businessDate.Date;

        var businessDay = await Context.HtBusinessDays.FirstOrDefaultAsync(b => b.BusinessDate == targetDate);
        if (businessDay == null)
            throw new InvalidOperationException($"Business day not found for {targetDate:yyyy-MM-dd}");
        if (businessDay.AuditStatus == 0)
            throw new Exception("Business Day Not Close Yet...!!");

        var data = await Context.HtDailyAuditSummaries.FirstOrDefaultAsync(x => x.BusinessDate == targetDate);
        if (data == null)
            return new BusinessDaySummaryVm();

        var model = _iMapper.Map<BusinessDaySummaryVm>(data);

        model.TotalRestaurantSales = (decimal)data.TotalRestaurantOrderAmount;

        return model;
    }

    #endregion
}