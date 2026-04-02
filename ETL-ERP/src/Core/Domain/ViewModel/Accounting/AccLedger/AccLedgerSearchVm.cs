using Domain.ModelInterface;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Accounting.AccLedger
{
    public class AccLedgerSearchVm : IDataTableSearch
    {
        public long Id { get; set; }
        public string LedgerName { get; set; }
        public string LedgerCode { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }

        //------------------------FK---------------------
        public long HeadId { get; set; }
        public string HeadName { get; set; }
        public string HeadCode { get; set; }
        public IEnumerable<SelectListItem> HeadLookUp { get; set; }
        public long? StudentId { get; set; }
        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
    }
}
