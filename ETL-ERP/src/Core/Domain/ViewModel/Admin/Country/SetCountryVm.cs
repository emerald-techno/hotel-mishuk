using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Admin.Country
{
    public class SetCountryVm
    {
        public long Id { get; set; }

        [StringLength(100, ErrorMessage = "Name can't be more than 100 character")]
        [Required]
        [DisplayName("Name: *")]

        public string Name { get; set; }

        [StringLength(50)]
        [DisplayName("Abbreviation Name")]
        public string AbbrName { get; set; }

        [StringLength(50)]
        [DisplayName("Code")]
        public string Code { get; set; }

    }
}
