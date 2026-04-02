using Domain.ModelInterface;

namespace Domain.ViewModel.Restaurant.Waiter;

public class WaiterSearchVm : IDataTableSearch
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string Mobile { get; set; }
    public string Email { get; set; }
    public double Salary { get; set; }
    public string DobStr { get; set; }
    public string JoinDateStr { get; set; }
    public bool IsActive { get; set; }
    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }
}
