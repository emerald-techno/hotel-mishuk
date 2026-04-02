using Domain.ModelInterface;

namespace Domain.ViewModel.Payroll.PrEmpSalaryPart;

public class PrEmpSalaryPartSearchVm : IDataTableSearch
{
    public long Id { get; set; }
    public string PartType { get; set; }
    public string ValueType { get; set; }
    public double Value { get; set; }
    public string Remarks { get; set; }

    //--------------FK---------------------
    public long SalaryPartId { get; set; }
    public string SalaryPartsName { get; set; }
    public long EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public string EmployeeCode { get; set; }
    public string EmpDesignation { get; set; }
    public string EmpDesignationCode { get; set; }
    public string EmpDepartment { get; set; }
    public int EmpPartCount { get; set; }
    public long UserId { get; set; }
    public int SerialNo { get; set; }
    public bool CanCreate { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanView { get; set; }
    public bool CanDelete { get; set; }
}

public class EmpSalaryPartExcelModel
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string PartCode { get; set; }
    public string Value { get; set; }
}
