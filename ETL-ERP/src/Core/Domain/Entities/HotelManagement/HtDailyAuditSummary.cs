using Domain.Entities.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.HotelManagement;

public class HtDailyAuditSummary
{
    public long Id { get; set; }
    public DateTime BusinessDate { get; set; }   // Date Only

    // Rooms / counts
    public int TotalRoomsAvailable { get; set; }    // all rentable rooms
    public int TotalRoomsOccupied { get; set; }     // occupied last night
    public int TotalRoomsVacant { get; set; }       // vacant = available - occupied
    public int TotalRoomsOutOfOrder { get; set; }   // blocked rooms (OOS)
    public int TotalRoomsHouseUse { get; set; }     // hotel staff use
    public int TotalRoomMoves { get; set; }         // room changes
    public int TotalCheckIns { get; set; }
    public int TotalCheckOuts { get; set; }
    public int TotalNoShows { get; set; }
    public int TotalAdults { get; set; }
    public int TotalChildren { get; set; }
    public int TotalGuest => TotalAdults + TotalChildren;

    // Monetary fields (decimal)
    public double TotalRackRate { get; set; }        // original rates, no discount
    public double TotalRoomRevenue { get; set; }     // final room revenue collected
    public double TotalRoomServiceCharge { get; set; }   // total SC from rooms
    public double TotalRoomVat { get; set; }             // total VAT from rooms
    public double TotalRoomDiscounts { get; set; }       // discounts value
    public double TotalRoomComplimentary { get; set; }   // complimentary rooms revenue
    public double TotalRoomChargePosted { get; set; } // note: This represents confirmed, posted, final billing entries ― not expected charges.

    // F&B & other revenue
    public double TotalRestaurantRevenue { get; set; }
    public double TotalRestaurantOrderAmount { get; set; }
    public double TotalFNBServiceCharge { get; set; }     // F&B SC
    public double TotalFNBVat { get; set; }               // F&B VAT
    public double TotalFNBDiscounts { get; set; }
    public double TotalRoomServiceRevenue { get; set; }   // room service charge
    public double TotalBanquetRevenue { get; set; }       // hall revenue
    public double TotalOtherRevenue { get; set; }         // laundry, transport & extra service


    // --- Per-department / per-method fields (Restaurant) ---
    // Restaurant collections by method
    public decimal TotalRestaurantCash { get; set; }
    public decimal TotalRestaurantCard { get; set; }
    public decimal TotalRestaurantMbanking { get; set; }
    public decimal TotalRestaurantBank { get; set; }
    public decimal TotalRestaurantAdvance { get; set; }


    // --- Per-department / per-method fields (Front Office) ---
    public decimal TotalFrontOfficeCash { get; set; }
    public decimal TotalFrontOfficeCard { get; set; }
    public decimal TotalFrontOfficeMbanking { get; set; }
    public decimal TotalFrontOfficeBank { get; set; }
    public decimal TotalFrontOfficeAdvance { get; set; }

    // --- Convenience aggregated department totals (computed, NotMapped) ---
    [NotMapped]
    public decimal TotalRestaurantCollections =>
        TotalRestaurantCash + TotalRestaurantCard + TotalRestaurantMbanking
        + TotalRestaurantBank + TotalRestaurantAdvance;

    [NotMapped]
    public decimal TotalFrontOfficeCollections =>
        TotalFrontOfficeCash + TotalFrontOfficeCard + TotalFrontOfficeMbanking
        + TotalFrontOfficeBank + TotalFrontOfficeAdvance;

    // --- Overall collection totals kept for fast reads (optional duplication) ---
    // You can keep these in sync by computing from department fields when closing the audit
    public decimal TotalCashCollection { get; set; }    // overall cash (restaurant + front office + others)
    public decimal TotalCardCollection { get; set; }
    public decimal TotalMbankingCollection { get; set; }
    public decimal TotalCreditCollection { get; set; }
    public decimal TotalAdvanceCollection { get; set; }

    // Covers
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

    // Receivable / payable
    public decimal FrontOfficeReceivable { get; set; }
    public decimal FNBReceivable { get; set; }
    public decimal TotalReceivable { get; set; }
    public decimal TotalPayable { get; set; }

    // Derived metrics
    public decimal OccupancyPercentage { get; set; }
    public decimal ADR { get; set; }
    public decimal ARR { get; set; }
    public decimal RevPAR { get; set; }
    public decimal AvgGuestsPerRoom { get; set; }

    [StringLength(500)]
    public string Remarks { get; set; }

    public DateTime ActionDate { get; set; } = DateTime.Now;
    public long ActionById { get; set; }
    public ApplicationUser ActionBy { get; set; }
}
