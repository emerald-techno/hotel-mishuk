using Domain.ModelInterface;

namespace Domain.ViewModel.Payroll.PrArrearMst;

public class PrArrearMstSearchVm : IDataTableSearch
{
    public long Id { get; set; }
    public short Year { get; set; }
    public short Month { get; set; }
    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public int EmployeeCount { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }
}
