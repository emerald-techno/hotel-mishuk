using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.HotelManagement.Service;

public class ServiceVm
{
    public long Id { get; set; }

    [Required]
    [StringLength(80, ErrorMessage = "Maximum 80 carecters can be given")]
    [Remote(action: "IsNameExist", controller: "Service", AdditionalFields = "InitName,Id")]
    public string ServiceName { get; set; }
    public string ServiceCode { get; set; }
    public string Description { get; set; }
    public long LedgerId { get; set; }

    public IEnumerable<SelectListItem> LadgerLookUp { get; set; }
}
