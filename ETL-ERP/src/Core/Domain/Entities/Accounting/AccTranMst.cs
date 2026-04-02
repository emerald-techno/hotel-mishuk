using Domain.Entities.Admin;
using Domain.Entities.HotelManagement;
using Domain.Entities.Identity;
using Domain.Entities.Inventory;
using Domain.Entities.Payroll;
using Domain.Entities.Pf;
using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Accounting
{
    public class AccTranMst : IAuditable
    {
        public long Id { get; set; }
        public DateTime VcDate { get; set; }

        [Required]
        [StringLength(1)]
        public string VcType { get; set; } // J = Jounal, P = Petty cash, O = Opening, D = Bank Debit Voucher, C = Bank Credit Voucher. S = Cash Debit Vouchar, H = Cash Credit Vouchar

        [Required]
        [StringLength(60)]
        public string VcNo { get; set; }


        [StringLength(300)]
        public string Narration { get; set; }


        [StringLength(120)]
        public string Remarks { get; set; }

        public double TotalAmount { get; set; }

        public bool IsApproved { get; set; }

        public bool IsAudited { get; set; }

        public bool IsBankClear { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public DateTime? AuditDate { get; set; }

        public DateTime? BankClearDate { get; set; }

        public bool IsAuto { get; set; }



        [StringLength(60)]
        public string RefNo { get; set; }

        [Required]
        [StringLength(1)]
        public string SubVacType { get; set; }



        //-----------------------------------------
        public long? AccAccountId { get; set; }
        public long CurrencyId { get; set; }
        public SetCurrency Currency { get; set; }

        public long FinYearId { get; set; }
        public SetFincYear FinYear { get; set; }

        public long? AuditedById { get; set; }
        public ApplicationUser AuditedBy { get; set; }

        public long? ApprovedById { get; set; }
        public ApplicationUser ApprovedBy { get; set; }

        public long? BankClearById { get; set; }
        public ApplicationUser BankClearBy { get; set; }

        public long? PaymentId { get; set; }
        public HtBookingPayment Payment { get; set; }

        public long? RsPaymentId { get; set; }
        public RsOrderPayments RsPayment { get; set; }

        public long? InvPaymentId { get; set; }
        public InventoryBillPayment InvPayment { get; set; }

        public long? EmpLoanMstId { get; set; }
        public EmpLoanMst EmpLoanMst { get; set; }

        public long? EmpLoanDtlId { get; set; }
        public EmpLoanDtl EmpLoanDtl { get; set; }
        public long? PrSalaryDtlId { get; set; }
        public PrSalaryDtl PrSalaryDtl { get; set; }       
        public ApplicationUser ActionBy { get; set; }
        public DateTime ActionDate { get; set; }
        public long ActionById { get; set; }
        public ApplicationUser UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public long? UpdatedById { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<AccTranDtl> AccTranDtls { get; set; }
    }
}
