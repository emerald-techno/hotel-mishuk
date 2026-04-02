using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Accounting.AccTranMst
{
    public class JournalDtlVm
    {
        public long Id { get; set; }
        public double AmountDr { get; set; }
        public double AmountCr { get; set; }
        public long TranMstId { get; set; }
        public long LedgerId { get; set; }
        public string LedgerName { get; set; }
        public string LedgerCode { get; set; }
        public string LedgerType { get; set; }

    }

    public class JournalMstVm
    {
        public long Id { get; set; }
        public string VcDateStr { get; set; }
        public DateTime VcDate { get; set; }
        public string VcType { get; set; }
        public string VcNo { get; set; }
        [Required]
        public string Narration { get; set; }
        public string Remarks { get; set; }
        public double TotalAmount { get; set; }
        public bool IsApproved { get; set; }
        public bool IsAudited { get; set; }
        public bool IsBankClear { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? AuditDate { get; set; }
        public DateTime? BankClearDate { get; set; }
        public bool IsAuto { get; set; }
        public string RefNo { get; set; }
        public string SubVacType { get; set; }

        //-----------------------FK-------------------------

        public long CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public long FinYearId { get; set; }
        public string FinYearName { get; set; }
        public long? AuditedById { get; set; }
        public long? ApprovedById { get; set; }
        public long? BankClearById { get; set; }
        public ICollection<JournalDtlVm> JournalDtlVms { get; set; }
    }
}
