using Domain.ModelInterface;

namespace Domain.ViewModel.Payroll.PrSalaryPart;

public class PrSalaryPartSearchVm : IDataTableSearch
{
    public long Id { get; set; }
    public string PartName { get; set; }
    public string PartCode { get; set; }
    public string PartType { get; set; }
    public string PartLink { get; set; }
    public string ValueType { get; set; }
    public double Value { get; set; }
    public bool IsEmpWise { get; set; }
    public bool IsEnable { get; set; }
    public string Remarks { get; set; }
    public bool IsDeleted { get; set; }

    //----------------FK-----------------

    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }


}
