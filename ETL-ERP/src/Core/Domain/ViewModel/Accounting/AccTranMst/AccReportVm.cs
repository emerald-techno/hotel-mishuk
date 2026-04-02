using Domain.Entities.Admin;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Accounting.AccTranMst;

public class AccReportVm
{
    public string StrFromDate { get; set; }
    public string StrToDate { get; set; }
    public long LedgerId { get; set; }
    public string LedgerCode { get; set; }
    public string LedgerName { get; set; }
    public long FinYearId { get; set; }
    public long HeadId { get; set; }
    public string HeadCode { get; set; }
    public string HeadName { get; set; }
    public long? MishukLedgerId { get; set; }
    public SetFincYear FincYear { get; set; }
    public IEnumerable<SelectListItem> LedgerLookup { get; set; }
    public IEnumerable<SelectListItem> FincYearLookup { get; set; }
    public IEnumerable<SelectListItem> AccountLookup { get; set; }
    public IEnumerable<SelectListItem> HeadLookup { get; set; }
}
