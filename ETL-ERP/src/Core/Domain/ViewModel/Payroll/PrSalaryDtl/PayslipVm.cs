using Domain.ViewModel.Payroll.PrEmpSalaryPart;
using Domain.ViewModel.Payroll.PrSalaryPart;

namespace Domain.ViewModel.Payroll.PrSalaryDtl;

public class PayslipVm
{
    public long Id { get; set; }
    public double GrossSalary { get; set; }
    public double NetSalary { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? PaidDate { get; set; }
    public double BasicSalary { get; set; }
    public double TotalDeduction { get; set; }
    public double ColA { get; set; }
    public double ColB { get; set; }
    public double ColC { get; set; }
    public double ColD { get; set; }
    public double ColE { get; set; }
    public double ColF { get; set; }
    public double ColG { get; set; }
    public double ColH { get; set; }
    public double ColI { get; set; }
    public double ColJ { get; set; }
    public double ColK { get; set; }
    public double ColL { get; set; }
    public double ColM { get; set; }
    public double ColN { get; set; }
    public double ColO { get; set; }
    public double ColP { get; set; }
    public double ColQ { get; set; }
    public double ColR { get; set; }
    public double ColS { get; set; }
    public double ColT { get; set; }
    public double ColU { get; set; }
    public string Remarks { get; set; }

    public long SalaryMstId { get; set; }
    public short SalaryMonth { get; set; }
    public short SalaryYear { get; set; }
    public DateTime SalaryDate { get; set; }
    public long EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public string EmployeeCode { get; set; }
    public double EmployeeSalary { get; set; }
    public string EmpPhotoUrl { get; set; }
    public string EmpDepartmentName { get; set; }
    public string EmpDesignationName { get; set; }
    public DateTime? EmpJoinDate { get; set; }
    public long? PaidById { get; set; }
    public ICollection<PrSalaryPartVm> PrSalaryParts { get; set; }
    public ICollection<PrEmpSalaryPartVm> PrEmpSalaryParts { get; set; }
}
