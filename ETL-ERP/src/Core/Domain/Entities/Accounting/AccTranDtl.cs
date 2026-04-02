using Domain.Entities.Identity;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Accounting
{
    public class AccTranDtl : IAuditable
    {
        public long Id { get; set; }
        public double AmountDr { get; set; }
        public double AmountCr { get; set; }
        public double AmountUsdCr { get; set; }
        public double AmountUsdDr { get; set; }

        [StringLength(50)]
        public string ChkNo { get; set; }

        public DateTime? ChkDate { get; set; }
        public short SlNo { get; set; }

        [StringLength(120)]
        public string Remarks { get; set; }

        //-----------------------------------------
        public long TranMstId { get; set; }
        public AccTranMst TranMst { get; set; }

        public long? LedgerDrId { get; set; }
        public AccLedger LedgerDr { get; set; }

        public long? LedgerCrId { get; set; }
        public AccLedger LedgerCr { get; set; }

        public ApplicationUser ActionBy { get; set; }
        public DateTime ActionDate { get; set; }
        public long ActionById { get; set; }

        public ApplicationUser UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public long? UpdatedById { get; set; }
        public bool IsDeleted { get; set; }
    }
}
