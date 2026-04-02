using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Admin.FinancialYear;

public class SetFincYearSearchVm : IDataTableSearch
{
    public long Id { get; set; }
    public string YearName { get; set; }
    public DateTime YearStartDate { get; set; }
    public DateTime YearEndDate { get; set; }
    public bool IsActive { get; set; }
    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }
}
