using Domain.ModelInterface;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Domain.ViewModel.Accounting.AccHead
{
    public class AccHeadSearchVm : IDataTableSearch
    {
        public long Id { get; set; }
        public long ParentHeadId { get; set; }
        public int LevelId { get; set; }
        public string HeadName { get; set; }
        public string HeadCode { get; set; }

        public string Description { get; set; }
        public bool BudgetHead { get; set; }
        public bool AssetHead { get; set; }
        public bool PettyHead { get; set; }
        public bool BankHead { get; set; }
        public string Remarks { get; set; }

        public string ParentHeadName { get; set; }

        //------------------FK-----------------------
        public long GroupId { get; set; }
        public IEnumerable<SelectListItem> AccGroupLookUp { get; set; }
        public IEnumerable<SelectListItem> AccHeadLookUp { get; set; }
        public string GroupName { get; set; }
        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
    }
}
