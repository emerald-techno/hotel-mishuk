using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Admin.District
{
    public class SetDistrictVm
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Name is Required")]
        [StringLength(60, ErrorMessage = "Name can't be more than 60 characters")]
        public string Name { get; set; }


        [StringLength(60, ErrorMessage = "Name can't be more than 60 characters")]
        public string NameBn { get; set; }


        [StringLength(30, ErrorMessage = "Code can't be more than 30 characters")]
        public string Code { get; set; }

        //------------------------------------------
        public long DivisionId { get; set; }
    }
}
