using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Admin.PoliceStation
{
    public class SetPoliceStationVm
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        [StringLength(80, ErrorMessage = "Name can not be more than 80 characters")]
        public string Name { get; set; }

        [StringLength(80, ErrorMessage = "Bangla Name can not be more than 80 characters")]
        public string NameBn { get; set; }
        [StringLength(30, ErrorMessage = "Code can not be more than 30 characters")]
        public string Code { get; set; }

        public long DistrictId { get; set; }
    }
}
