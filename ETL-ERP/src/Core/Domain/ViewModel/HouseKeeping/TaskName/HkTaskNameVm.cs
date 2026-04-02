using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.HouseKeeping.TaskName;

public class HkTaskNameVm
{
    public long Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }
    public long TypeId { get; set; }
    public string TypeName { get; set; }

    public IEnumerable<SelectListItem> TaskTypeLookUp { get; set; }
}
