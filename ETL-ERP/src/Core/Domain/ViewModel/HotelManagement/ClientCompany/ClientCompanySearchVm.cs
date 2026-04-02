using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.HotelManagement.ClientCompany;

public class ClientCompanySearchVm : IDataTableSearch
{
    public long Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; }

    [StringLength(350)]
    public string Address { get; set; }

    [StringLength(30)]
    public string Mobile { get; set; }

    [StringLength(100)]
    public string Email { get; set; }

    [StringLength(100)]
    public string WebSite { get; set; }

    [StringLength(100)]
    public string LogoUrl { get; set; }

    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }
}
