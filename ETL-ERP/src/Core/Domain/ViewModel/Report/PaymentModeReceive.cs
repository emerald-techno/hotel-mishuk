using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel.Report
{
    public class PaymentModeReceive
    {
        public string StrPaidDate { get; set; }
        public DateTime PaidDate { get; set; }
        public decimal CashPayment { get; set; } = 0;
        public decimal BankPayment { get; set; } = 0;
        public decimal CardPayment { get; set; } = 0;
        public decimal BkashPayment { get; set; } = 0;
        public decimal TotalPayment { get; set; } = 0;

    }
}
