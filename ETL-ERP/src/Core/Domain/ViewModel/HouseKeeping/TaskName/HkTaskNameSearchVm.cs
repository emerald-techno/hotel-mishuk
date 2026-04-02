using Domain.ModelInterface;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.HouseKeeping.TaskName;

public class HkTaskNameSearchVm : IDataTableSearch
{
    public long Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    //DataTable
    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }

    //Fk
    public long TypeId { get; set; }
    public string TypeName { get; set; }

    public IEnumerable<SelectListItem> TaskTypeLookUp { get; set; }
}
