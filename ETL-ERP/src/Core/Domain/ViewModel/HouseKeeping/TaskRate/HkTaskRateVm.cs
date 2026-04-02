using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.HouseKeeping.TaskRate;

public class HkTaskRateVm
{
    public long Id { get; set; }
    public double Rate { get; set; }
    public long ItemId { get; set; }
    public long TaskId { get; set; }
    public IEnumerable<SelectListItem> TaskNameLookUp { get; set; }
}
