using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Accounting.AccLedger
{
    public class AccLedgerVm
    {
        public long Id { get; set; }

        [StringLength(90, ErrorMessage = "Ledger Name can not be more than 90 characters")]
        [Required(ErrorMessage = "Ledger Name is required")]
        [Remote(action: "IsNameExist", controller: "AccLedger", AdditionalFields = "InitName")]
        public string LedgerName { get; set; }
        public string LedgerCode { get; set; }


        [StringLength(120, ErrorMessage = "Description can not be more than 120 characters")]
        public string Description { get; set; }


        [StringLength(120, ErrorMessage = "Remarks can not be more than 120 characters")]
        public string Remarks { get; set; }
        //----------------------------FK--------------------------
        public long HeadId { get; set; }
        public IEnumerable<SelectListItem> HeadLookUp { get; set; }
        public long? StudentId { get; set; }

        public bool IsAjaxPost { get; set; }
    }
}
