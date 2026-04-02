using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel.Accounting.AccTranMst
{
    public class AccLedgerReportVm
    {
        public short Sl { get; set; }
        public long AccTranMstId { get; set; }
        public short SlNo { get; set; }
        public long LedgerId { get; set; }
        public string LedgerName { get; set; }
        public string VcDate { get; set; }
        public string nr { get; set; }
        public string Narration { get; set; }
        public string VcNo { get; set; }
        public short IsApproved { get; set; }
        public bool IsAuto { get; set; }
        public string VcStatus { get; set; }
        public string OpBal { get; set; }

        public string AmountDr { get; set; }
        public string AmountCr { get; set; }
        public string Balance { get; set; }

        public decimal AmountDrNum { get; set; }
        public decimal AmountCrNum { get; set; }
        public decimal BalanceNum { get; set; }
    }
}
