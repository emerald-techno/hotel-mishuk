using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel.Pf.EmpLoan
{
    public class EmployeeLoanReportVm
    {
        public string StrFromDate { get; set; }
        public string StrToDate { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal InterestRate { get; set; }
        public decimal InstallmentAmount { get; set; }
        public DateTime FirstInsDate { get; set; }
        public DateTime? NextInstallmentDate { get; set; }
        public DateTime LoanPassDate { get; set; }
        public DateTime LoanPayDate { get; set; }
        public int TotalInstallment { get; set; }
        public int PaidInstallment { get; set; }
        public int RemainingInstallment { get; set; }
        public decimal PaidLoanAmount { get; set; }
        public decimal RemainingLoanAmount { get;set; }
        public decimal PaidInterestAmount { get; set; }
        public decimal RemainingInterestAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public decimal TotalPayableAmount { get; set; }
        public decimal TotalInterestAmount { get; set; }

        public string StrQueryDate { get; set; }
        public IEnumerable<SelectListItem> EmployeeLookUp { get; set; }
    }
}
