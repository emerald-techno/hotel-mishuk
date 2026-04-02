using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel.Accounting.AccTranDtl
{
    public class AccTranDtlVm
    {
        public long Id { get; set; }
        public double? AmountDr { get; set; }
        public double? AmountCr { get; set; }
        public double? AmountUsdCr { get; set; }
        public double? AmountUsdDr { get; set; }

        [StringLength(50)]
        public string ChkNo { get; set; }
        public DateTime? ChkDate { get; set; }
        public short SlNo { get; set; }

        [StringLength(120)]
        public string Remarks { get; set; }

        //-----------------------------------------
        public long TranMstId { get; set; }
        public long? LedgerDrId { get; set; }
        public string LedgerDrName { get; set; }
        public long? LedgerCrId { get; set; }
        public string LedgerCrName { get; set; }

    }
}
