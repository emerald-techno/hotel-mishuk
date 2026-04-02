using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Pf.PfFund
{
    public class PfFundMstVm
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

        public short Year { get; set; }
        public short Month { get; set; }

        //-----------------------------------------
        public long? VoucherId { get; set; }
        public long? PrMstId { get; set; }
        public long? InvestmentId { get; set; } // table not added - relation added future
        public ICollection<PfFundDtlVm> PfFundDtls { get; set; }



        public string FormDateStr { get; set; }
        public string ToDateStr { get; set; }
        public IEnumerable<SelectListItem> DepatmentLookup { get; set; }
        public IEnumerable<SelectListItem> DesignationLookup { get; set; }
    }
}
