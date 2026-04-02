using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Inventory.UnitInfo
{
    public class UnitInfoVm
    {
        public long Id { get; set; }

        [Required]
        [DisplayName("Name *")]
        public string UnitName { get; set; }
        public string UnitCode { get; set; }
        public string Remarks { get; set; }
        public bool IsDeleted { get; set; }
    }
}
