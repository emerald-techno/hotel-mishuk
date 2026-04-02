using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Payroll
{
    public class PrSalaryPart : IAuditable
    {
        public long Id { get; set; }

        [Required]
        [StringLength(60)]
        public string PartName { get; set; }

        [Required]
        [StringLength(60)]
        public string PartCode { get; set; }

        [Required]
        [StringLength(1)]
        public string PartType { get; set; } // A=Addition, D=Deduction

        [Required]
        [StringLength(1)]
        public string ValueType { get; set; } // P=Percent, A=Amount

        [StringLength(1)]
        public string PartLink { get; set; } // A=Arrear, L=Loan, B=Basic, P=Pf
        public double Value { get; set; }
        public bool IsEmpWise { get; set; }
        public bool IsEnable { get; set; }
        public short SlNo { get; set; }

        [StringLength(120)]
        public string Remarks { get; set; }

        //------------------FK---------------------
        public ApplicationUser ActionBy { get; set; }
        public DateTime ActionDate { get; set; }
        public long ActionById { get; set; }

        public ApplicationUser UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public long? UpdatedById { get; set; }
        public bool IsDeleted { get; set; }
    }
}
