using Domain.Entities.Accounting;
using Domain.Entities.Identity;
using Domain.Entities.Payroll;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Pf
{
    public class PfFundMst : IAuditable
    {
        public long Id { get; set; }

        [Required]
        [StringLength(1)]
        public string FundType { get; set; } // M=Monthly. I=Interest, P=Profit, O=Others
        public DateTime FundFromDate { get; set; }
        public DateTime FundToDate { get; set; }
        public DateTime EntryDate { get; set; }
        public double TotalEmpCon { get; set; }
        public double TotalCompCon { get; set; }
        public double Interest { get; set; }
        public double EmpConPer { get; set; }
        public double CompConPer { get; set; }
        public double PfAmount { get; set; } // Default = 0

        [StringLength(120)]
        public string Remarks { get; set; }

        //-----------------------------------------
        public long? VoucherId { get; set; }
        public AccTranMst Voucher { get; set; }
        public long? PrMstId { get; set; }
        public PrSalaryMst PrMst { get; set; }
        public long? InvestmentId { get; set; } // table not added - relation added future
        public long ActionById { get; set; }
        public ApplicationUser ActionBy { get; set; }
        public DateTime ActionDate { get; set; }

        public long? UpdatedById { get; set; }
        public ApplicationUser UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }

        public bool IsDeleted { get; set; }
        public ICollection<PfFundDtl> PfFundDtls { get; set; }
    }
}
