using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Admin.Division
{
    public class SetDivisionVm
    {
        public int Id { get; set; }
        [StringLength(60)]
        [Required]
        public string Name { get; set; }
        [StringLength(60)]
        public string NameBn { get; set; }
        [StringLength(30)]
        public int Code { get; set; }
        public long CountryId { get; set; }

        //-----------------------------------
    }
}
