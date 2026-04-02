using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.HouseKeeping.TaskType;

public class HkTaskTypeSearchVm : IDataTableSearch
{
    public long Id { get; set; }

    [Required]
    [StringLength(50)]
    public string TypeName { get; set; }

    // -- DataTable Propertry

    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }
}