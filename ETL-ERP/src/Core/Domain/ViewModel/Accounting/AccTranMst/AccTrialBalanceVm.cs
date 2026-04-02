using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel.Accounting.AccTranMst
{
    public class AccTrialBalanceVm
    {
        public long LedgerId { get; set; }
        public string LedgerName { get; set; }
        public decimal OpDr { get; set; }
        public decimal OpCr { get; set; }
        public decimal AmountDr { get; set; }
        public decimal AmountCr { get; set; }
        public decimal BalanceDr { get; set; }
        public decimal BalanceCr { get; set; }
        public string LedgerCode { get; set; }
        public string HeadCode { get; set; }
        public long? HeadId { get; set; }
        public string HeadName { get; set; }

        public string HeadCode2 { get; set; }
        public long? HeadId2 { get; set; }
        public string HeadName2 { get; set; }


        public string HeadCode3 { get; set; }
        public long? HeadId3 { get; set; }
        public string HeadName3 { get; set; }


        public string HeadCode4 { get; set; }
        public long? HeadId4 { get; set; }
        public string HeadName4 { get; set; }


        public string HeadCode5 { get; set; }
        public long? HeadId5 { get; set; }
        public string HeadName5 { get; set; }


        public string HeadCode6 { get; set; }
        public long? HeadId6 { get; set; }
        public string HeadName6 { get; set; }


        public string HeadCode7 { get; set; }
        public long? HeadId7 { get; set; }
        public string HeadName7 { get; set; }




    }
}
