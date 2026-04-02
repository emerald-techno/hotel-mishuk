using Domain.ViewModel.Accounting.AccTranDtl;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.ViewModel.Accounting.AccTranMst
{
    public class AccTranMstVm
    {
        public long Id { get; set; }

        public string VcDateStr { get; set; }
        public DateTime VcDate { get; set; }

        [StringLength(1)]
        [Required]
        public string VcType { get; set; }

        [StringLength(60)]
        [Required]
        public string VcNo { get; set; }

        [Required(ErrorMessage = "Narration can't be empty")]
        [StringLength(300, ErrorMessage = "Narration can not be more than 300 characters")]
        public string Narration { get; set; }

        [StringLength(120, ErrorMessage = "Remarks can not be more than 120 characters")]
        public string Remarks { get; set; }

        public double TotalAmount { get; set; }
        public bool IsApproved { get; set; }
        public bool IsAudited { get; set; }
        public bool IsBankClear { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? AuditDate { get; set; }
        public DateTime? BankClearDate { get; set; }
        public bool IsAuto { get; set; }
        public long CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public long FinYearId { get; set; }
        public string FinYearName { get; set; }


        [StringLength(60, ErrorMessage = "Referance nunmber can not be more than 60 characters")]
        public string RefNo { get; set; }


        [StringLength(1)]
        public string SubVacType { get; set; }

        [NotMapped]
        public bool IsOpenignUpdate { get; set; }

        //-----------------------FK-------------------------
        public long? AuditedById { get; set; }
        public long? ApprovedById { get; set; }
        public long? BankClearById { get; set; }

        [Required(ErrorMessage = "Account Information Is Mandatory")]
        public long? AccAccountId { get; set; }
        public string AccAccountName { get; set; }
        public IEnumerable<SelectListItem> CurrencyLookup { get; set; }
        public IEnumerable<SelectListItem> FinYearLookup { get; set; }
        public IEnumerable<SelectListItem> LedgerLookup { get; set; }
        public IEnumerable<SelectListItem> NoteTypeLookup { get; set; }
        public IEnumerable<SelectListItem> AccountLookup { get; set; }
        public ICollection<AccTranDtlVm> AccTranDtls { get; set; }
        public ICollection<JournalDtlVm> JournalDtlVms { get; set; }

        public string VcTypeText => VcType switch { "J" => "Jounal Voucher", "D" => "Bank Debit Voucher", "C" => "Bank Credit Voucher", "S" => "Cash Debit Voucher", "H" => "Cash Credit Voucher", "O" => "Opening Voucher", _ => "" };

    }
}
