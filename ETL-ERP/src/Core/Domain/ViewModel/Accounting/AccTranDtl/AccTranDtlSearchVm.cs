using Domain.ModelInterface;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ViewModel.Accounting.AccTranDtl
{
    public class AccTranDtlSearchVm : IDataTableSearch
    {
        public long Id { get; set; }
        public string Ledger { get; set; }
        public double? AmountDr { get; set; }
        public double? AmountCr { get; set; }
        public double? AmountUsdCr { get; set; }
        public double? AmountUsdDr { get; set; }
        public string ChkNo { get; set; }
        public DateTime? ChkDate { get; set; }
        public short SlNo { get; set; }
        public string Remarks { get; set; }
        public string VcType { get; set; }

        //-----------------------------------------
        public long TranMstId { get; set; }
        public long? LedgerDrId { get; set; }
        public long? LedgerCrId { get; set; }
        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
    }
}
