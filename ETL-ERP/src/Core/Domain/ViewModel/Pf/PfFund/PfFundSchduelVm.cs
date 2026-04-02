using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel.Pf.PfFund
{
    public class PfFundSchduelVm
    {
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeePhoto { get; set; }
        public string EmployeeCode { get; set; }
        public int DesignationId { get; set; }
        public string DesignationName { get; set; }
        public int DeparmentId { get; set; }
        public string DepartmentName { get; set; }
        public DateTime JoinDate { get; set; }
        public DateTime? PfStartDate { get; set; }
        public DateTime? PfEndDate { get; set; }
        public decimal EmpConOp { get; set; } = 0;
        public decimal CompConOp { get; set; } = 0;
        public decimal EmpIntOp { get; set; } = 0;
        public decimal CompIntOp { get; set; } = 0;
        public decimal TotalEmpOp { get; set; } = 0;
        public decimal TotalCompOp { get; set; } = 0;
        public decimal TotalAmountOp { get; set; } = 0;
        public decimal EmpCon { get; set; } = 0;
        public decimal CompCon { get; set; } = 0;
        public decimal EmpInt { get; set; } = 0;
        public decimal CompInt { get; set; } = 0;
        public decimal TotalAmount { get; set; } = 0;



    }
}
