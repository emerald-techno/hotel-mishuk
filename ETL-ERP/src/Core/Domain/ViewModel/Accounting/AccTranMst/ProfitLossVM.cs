using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel.Accounting.AccTranMst
{
    public class ProfitLossVM
    {
        public string ActionType { get; set; }
        public string HeadType { get; set; }
        public long HeadId { get; set; }
        public string HeadName { get; set; }
        public string HeadCode { get; set; }
        public decimal AmountDr { get; set; } = 0;
        public decimal AmountCr { get; set; } = 0;
        public string BalanceType { get; set; }
        public decimal BalanceAmount { get; set; } = 0;
    }
}
