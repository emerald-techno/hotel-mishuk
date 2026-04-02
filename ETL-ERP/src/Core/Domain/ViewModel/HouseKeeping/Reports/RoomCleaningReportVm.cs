using Domain.Enums.AppEnums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.HouseKeeping.Reports;

public class RoomCleaningReportVm
{
    public string StrFromDate { get; set; }
    public string StrToDate { get; set; }
    public string RoomNo { get; set; }
    public string HouseKeeperName { get; set; }
    public long? HouseKeeperId { get; set; }
    public DateTime? AssignDate { get; set; }
    public DateTime? CompleteDate { get; set; }
    public DateTime? AuditDate { get; set; }
    public int? Status { get; set; }

    public IEnumerable<SelectListItem> HouseKeeperLookup { get; set; }
    public IEnumerable<SelectListItem> StatusLookup { get; set; }
}
