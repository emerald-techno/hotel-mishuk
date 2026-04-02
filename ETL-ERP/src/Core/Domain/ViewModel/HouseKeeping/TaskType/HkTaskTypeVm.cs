using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.HouseKeeping.TaskType;

public class HkTaskTypeVm
{
    public long Id { get; set; }

    [Required]
    [StringLength(50)]
    public string TypeName { get; set; }
}
