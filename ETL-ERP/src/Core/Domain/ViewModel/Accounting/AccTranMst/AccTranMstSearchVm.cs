using Domain.ModelInterface;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Accounting.AccTranMst
{
    public class AccTranMstSearchVm : IDataTableSearch
    {
        public long Id { get; set; }
        public DateTime VcDate { get; set; }
        public string VcType { get; set; }
        public string VcNo { get; set; }
        public string Narration { get; set; }
        public string Remarks { get; set; }
        public double TotalAmount { get; set; }
        public bool IsApproved { get; set; }
        public bool IsAudited { get; set; }
        public bool IsBankClear { get; set; }
        public DateTime ApprovedDate { get; set; }
        public DateTime AuditDate { get; set; }
        public DateTime BankClearDate { get; set; }
        public bool IsAuto { get; set; }
        public int CurrencyId { get; set; }
        public string RefNo { get; set; }
        public string SubVacType { get; set; }
        public string FormDateStr { get; set; }
        public string ToDateStr { get; set; }

        //---------------------------FK------------------
        public long? AuditedById { get; set; }
        public long? ApprovedById { get; set; }
        public long? BankClearById { get; set; }
        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
        public IEnumerable<SelectListItem> VcTypeLookUp { get; set; }

        public string StrTotalAmount
        {
            get { return TotalAmount.ToString("N2"); }
        }

    }
}
