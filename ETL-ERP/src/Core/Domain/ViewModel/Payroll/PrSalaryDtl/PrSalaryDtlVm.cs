using Domain.Utility.Common;

namespace Domain.ViewModel.Payroll.PrSalaryDtl;

public class PrSalaryDtlVm
{
    public long Id { get; set; }
    public double GrossSalary { get; set; }
    public double NetSalary { get; set; }
    public bool IsPaid { get; set; }
    public bool IsHeldUp { get; set; }
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

    public double? DeductionValue { get; set; }
    public double? AbsentDeductionValue { get; set; }


    //----------------------FK--------------------
    public long SalaryMstId { get; set; }
    public long EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public string EmployeeCode { get; set; }
    public string EmpAccount { get; set; }
    public long? EmpDepartmentId { get; set; }
    public string EmpDepartmentName { get; set; }
    public string EmpDepartmentCode { get; set; }
    public string EmpDepartmentSlNo { get; set; }
    public string DptLadgerCode { get; set; }
    public string EmpDesignationName { get; set; }
    public string EmpDesignationCode { get; set; }
    public long? PaidById { get; set; }

    public void SetDepartmentSlNo()
    {
        var mapping = new Dictionary<string, string>
            {
                { DepartmentCode.HotelMishukAdmin, "01" },
                { DepartmentCode.Accounting, "02" },
                { DepartmentCode.Store, "03" },
                { DepartmentCode.FrontDesk, "04" },
                { DepartmentCode.Security, "05" },
                { DepartmentCode.HouseKeeper, "06" },
                { DepartmentCode.Resturant, "07" },
                { DepartmentCode.StaffKitchen, "08" },
                { DepartmentCode.AmariResort, "09" }
            };

        if (!string.IsNullOrEmpty(EmpDepartmentCode) && mapping.TryGetValue(EmpDepartmentCode, out var slNo))
        {
            EmpDepartmentSlNo = slNo;
        }
        else
        {
            EmpDepartmentSlNo = null; // or handle default/fallback case
        }
    }
}
