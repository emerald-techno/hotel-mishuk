using Domain.ModelInterface;

namespace Domain.ViewModel.Pf.PfFund
{
    public class PfFundMstSearchVm : IDataTableSearch
    {
        public long Id { get; set; }
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
        public string Remarks { get; set; }

        public short Year { get; set; }
        public short Month { get; set; }

        //-----------------------------------------
        public long? VoucherId { get; set; }
        public long? PrMstId { get; set; }
        public string PrMstMonth { get; set; }
        public long? InvestmentId { get; set; } // table not added - relation added future
        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
    }
}
