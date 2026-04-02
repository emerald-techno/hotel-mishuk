namespace Domain.ViewModel.HotelManagement.BusinessDaySummary;

public class BusinessDaySummaryVm
{
    public DateTime BusinessDate { get; set; }

    // counts
    public int TotalRoomsAvailable { get; set; }
    public int TotalRoomsOccupied { get; set; }
    public int TotalRoomsVacant { get; set; }
    public int TotalRoomsOutOfOrder { get; set; }
    public int TotalRoomsHouseUse { get; set; }
    public int TotalCheckIns { get; set; }
    public int TotalCheckOuts { get; set; }
    public int TotalNoShows { get; set; }
    public int TotalRoomMoves { get; set; }

    // guests
    public long TotalAdults { get; set; }
    public long TotalChildren { get; set; }
    public long TotalGuests => TotalAdults + TotalChildren;

    // money (decimals for preview; you may cast to double when persisting to your current schema)
    public decimal TotalRackRate { get; set; }
    public decimal TotalBaseRate { get; set; }
    public decimal TotalRoomRevenue { get; set; }
    public decimal TotalRoomServiceCharge { get; set; }
    public decimal TotalRoomVat { get; set; }
    public decimal TotalRoomDiscounts { get; set; }
    public decimal TotalComplimentary { get; set; }

    public decimal TotalRestaurantSales { get; set; }
    public decimal TotalRestaurantRevenue { get; set; }
    public decimal TotalFnbServiceCharge { get; set; }
    public decimal TotalFnbVat { get; set; }
    public decimal TotalFnbDiscounts { get; set; }

    public decimal TotalBanquetRevenue { get; set; }
    
    public decimal TotalOtherRevenue { get; set; } // services/laundry/transport
    public decimal TotalOtherServiceCharge { get; set; }
    public decimal TotalOtherVat { get; set; }
    public decimal TotalCashCollection { get; set; }
    public decimal TotalCardCollection { get; set; }
    public decimal TotalMobileBankingCollection { get; set; }
    public decimal TotalCreditCollection { get; set; }
    public decimal TotalAdvanceCollection { get; set; }

    public decimal TotalRestaurantCash { get; set; }
    public decimal TotalRestaurantCard { get; set; }
    public decimal TotalRestaurantBank { get; set; }
    public decimal TotalRestaurantMbanking { get; set; }
    public decimal TotalRestaurantAdvance { get; set; }

    public decimal TotalFrontOfficeCash { get; set; }
    public decimal TotalFrontOfficeCard { get; set; }
    public decimal TotalFrontOfficeMbanking { get; set; }
    public decimal TotalFrontOfficeBank { get; set; }
    public decimal TotalFrontOfficeAdvance { get; set; }

    //Covers
    public decimal RestaurantCashCover { get; set; }
    public decimal RestaurantCardCover { get; set; }
    public decimal RestaurantMbankingCover { get; set; }
    public decimal RestaurantBankCover { get; set; }
    public decimal RestaurantAdvanceCover { get; set; }

    public decimal FrontOfficeCashCover { get; set; }
    public decimal FrontOfficeCardCover { get; set; }
    public decimal FrontOfficeMbankingCover { get; set; }
    public decimal FrontOfficeBankCover { get; set; }
    public decimal FrontOfficeAdvanceCover { get; set; }

    public decimal TotalRestaurantCollections { get; set; }
    public decimal TotalFrontOfficeCollections { get; set; }

    public decimal FrontOfficeReceivable { get; set; }
    public decimal FNBReceivable { get; set; }
    public decimal TotalReceivable { get; set; }

    // derived metrics
    public double OccupancyPercentage { get; set; }
    public double ADR { get; set; }
    public double ARR { get; set; }
    public double RevPAR { get; set; }
    public double AvgGuestsPerRoom { get; set; }

    public string Remarks { get; set; }

    //business day

    public short AuditStatus { get; set; }
}
