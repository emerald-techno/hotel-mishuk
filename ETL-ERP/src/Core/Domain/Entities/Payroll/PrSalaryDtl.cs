using Domain.Entities.HR;
using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Payroll
{
    public class PrSalaryDtl : IAuditable
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

        [StringLength(120)]
        public string Remarks { get; set; }

        //-----------------------------------------

        public long SalaryMstId { get; set; }
        public PrSalaryMst SalaryMst { get; set; }

        public long EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public ApplicationUser PaidBy { get; set; }
        public long? PaidById { get; set; }

        public ApplicationUser ActionBy { get; set; }
        public DateTime ActionDate { get; set; }
        public long ActionById { get; set; }

        public ApplicationUser UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public long? UpdatedById { get; set; }
        public bool IsDeleted { get; set; }
    }
}
