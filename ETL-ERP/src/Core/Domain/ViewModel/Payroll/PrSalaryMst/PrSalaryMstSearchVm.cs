using Domain.ModelInterface;

namespace Domain.ViewModel.Payroll.PrSalaryMst;

public class PrSalaryMstSearchVm : IDataTableSearch
{
    public long Id { get; set; }
    public DateTime SalaryDate { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public short TotalEmployee { get; set; }
    public double TotalSalary { get; set; }
    public string Remarks { get; set; }

    //--------------FK-----------------
    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }
}
