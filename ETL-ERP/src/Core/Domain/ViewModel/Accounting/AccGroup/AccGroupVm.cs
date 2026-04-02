using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Accounting.AccGroup
{
    public class AccGroupVm
    {
        public long Id { get; set; }

        [StringLength(60, ErrorMessage = "Group Name can not be more than 60 characters")]
        [Required(ErrorMessage = "Group Name required")]
        [DisplayName("Group Name: ")]
        [Remote(action: "IsNameExist", controller: "AccGroup", AdditionalFields = "InitName")]
        public string Name { get; set; }


        [StringLength(60, ErrorMessage = "Group Code can not be more than 60 characters")]
        [Required(ErrorMessage = "Group Code required")]
        [DisplayName("Group Code: ")]
        public string GroupCode { get; set; }


        [StringLength(40, ErrorMessage = "Group Short Code can not be more than 40 characters")]
        [Required(ErrorMessage = "Group short code required")]
        [DisplayName("Group Short Code: ")]
        public string GroupShortCode { get; set; }

        [DisplayName("Serial No: ")]
        public int SlNo { get; set; }


        [StringLength(120, ErrorMessage = "Remarks can not be more than 120 characters")]
        [DisplayName("Remarks: ")]
        public string Remarks { get; set; }

        //---------------------FK-----------------------------
    }
}
