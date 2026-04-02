using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.ViewModel.Accounting.AccTranMst;

public class QuickVoucherVm
{
    public long VcId { get; set; }
    public string VcNo { get; set; }
    public DateTime VcDate { get; set; }
    public string VcDateStr { get; set; }
    public long LedgerId { get; set; }

    [StringLength(1)]
    public string DrCr { get; set; } // D = Debit, C = Credit
    public string Narration { get; set; } = string.Empty;
    public double Amount { get; set; }
    public IEnumerable<SelectListItem> FinYearLookup { get; set; }
    public IEnumerable<SelectListItem> LedgerLookup { get; set; }
    public IEnumerable<SelectListItem> DrCrLookup { get; set; }

    [NotMapped]
    public long? MishukLedgerId { get; set; }
    
    [NotMapped]
    public long? CurrentFincYearId { get; set; }

    [NotMapped]
    public long? DefaultLedgerId { get; set; } //Restaurant , Amari Resort, Staff Kitchen

    public string DefaultDrCrValue { get; set; } = "C";
}

public class PayrollAutoVoucherVm
{
    public long PayrollId { get; set; }
    public double PaidAmount { get; set; }
    public DateTime PaidDate { get; set; }
    public string LedgerCode { get; set; }
}