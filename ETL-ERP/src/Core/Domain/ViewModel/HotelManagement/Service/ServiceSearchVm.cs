using Domain.ModelInterface;

namespace Domain.ViewModel.HotelManagement.Service;

public class ServiceSearchVm : IDataTableSearch
{
    public long Id { get; set; }
    public string ServiceName { get; set; }
    public string ServiceCode { get; set; }
    public long? LedgerId { get; set; }
    public string LedgerName { get; set; }
    public string LedgerCode { get; set; }
    public string Description { get; set; }
    public bool IsExtra { get; set; }

    // -- DataTable Propertry

    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }
}