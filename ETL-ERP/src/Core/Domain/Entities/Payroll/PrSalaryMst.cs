using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Payroll
{
    public class PrSalaryMst : IAuditable
    {
        public long Id { get; set; }
        public DateTime SalaryDate { get; set; }
        public short Year { get; set; }
        public short Month { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public short WorkingDays { get; set; }
        public short TotalEmployee { get; set; }
        public double TotalSalary { get; set; }
        public bool IsLocked { get; set; }

        [StringLength(120)]
        public string Remarks { get; set; }

        [StringLength(120)]
        public string ApprovedRemarks { get; set; }

        public DateTime ActionDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public bool IsDeleted { get; set; }

        //-----------------FK----------------------
        public long? ApprovedById { get; set; }
        public ApplicationUser ApprovedBy { get; set; }
        public long ActionById { get; set; }
        public ApplicationUser ActionBy { get; set; }
        public long? UpdatedById { get; set; }
        public ApplicationUser UpdatedBy { get; set; }
        public ICollection<PrSalaryDtl> PrSalaryDtls { get; set; }
    }
}
