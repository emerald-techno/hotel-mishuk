using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Accounting.AccHead
{
    public class AccHeadVm
    {
        public long Id { get; set; }

        public long? ParentHeadId { get; set; }
        public int LevelId { get; set; }

        [StringLength(80, ErrorMessage = "Head name can not be more than 80 characters")]
        [Required(ErrorMessage = "Head name required")]
        [Remote(action: "IsNameExist", controller: "AccHead", AdditionalFields = "InitName")]
        public string HeadName { get; set; }
        public string HeadCode { get; set; }

        [StringLength(120, ErrorMessage = "Description can not be more than 120 characters")]
        public string Description { get; set; }


        public bool BudgetHead { get; set; }
        public bool AssetHead { get; set; }
        public bool PettyHead { get; set; }
        public bool BankHead { get; set; }


        [StringLength(120, ErrorMessage = "Remarks can not be more than 120 characters")]
        public string Remarks { get; set; }

        public bool IsAjaxPost { get; set; }

        //-----------------FK---------------------
        public long GroupId { get; set; }
        public IEnumerable<SelectListItem> AccGroupLookUp { get; set; }
        public IEnumerable<SelectListItem> AccHeadLookUp { get; set; }
    }
}
