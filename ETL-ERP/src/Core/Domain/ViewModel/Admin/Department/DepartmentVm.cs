using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Admin.Department
{
    public class DepartmentVm
    {
        public long Id { get; set; }
        [StringLength(120, ErrorMessage = "Name can not be more than 120 characters")]
        [Required(ErrorMessage = "Name is required")]
        [Remote(action: "IsNameExist", controller: "Department", AdditionalFields = "InitName")]
        public string Name { get; set; }

        [StringLength(100, ErrorMessage = "Code can not be more than 100 characters")]
        [Required(ErrorMessage = "Code is required")]
        [Remote(action: "IsCodeExist", controller: "Department", AdditionalFields = "InitCode")]
        public string Code { get; set; }

        [StringLength(120, ErrorMessage = "Remarks can not be more than 120 characters")]
        public string Remarks { get; set; }
        public bool IsAcademic { get; set; }

        //------------------FK------------------
    }
}
