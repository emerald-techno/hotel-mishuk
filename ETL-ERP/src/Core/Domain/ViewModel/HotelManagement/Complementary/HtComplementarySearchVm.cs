using Domain.ModelInterface;

namespace Domain.ViewModel.HotelManagement.Complementary;

public class HtComplementarySearchVm : IDataTableSearch
{
    public long Id { get; set; }
    public string Title { get; set; }

    // -- DataTable Propertry

    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }
}