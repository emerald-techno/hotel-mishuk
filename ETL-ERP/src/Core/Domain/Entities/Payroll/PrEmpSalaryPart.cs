using Domain.Entities.HR;
using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Payroll
{
    public class PrEmpSalaryPart : IAuditable
    {
        public long Id { get; set; }

        [Required]
        [StringLength(1)]
        public string PartType { get; set; }

        [Required]
        [StringLength(1)]
        public string ValueType { get; set; } // P=Percent, A=Amount / DEAFULT = P
        public double Value { get; set; }

        [StringLength(120)]
        public string Remarks { get; set; }

        //------------------FK---------------------
        public long SalaryPartId { get; set; }
        public PrSalaryPart SalaryPart { get; set; }

        public long EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public ApplicationUser ActionBy { get; set; }
        public DateTime ActionDate { get; set; }
        public long ActionById { get; set; }

        public ApplicationUser UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public long? UpdatedById { get; set; }
        public bool IsDeleted { get; set; }
    }
}
