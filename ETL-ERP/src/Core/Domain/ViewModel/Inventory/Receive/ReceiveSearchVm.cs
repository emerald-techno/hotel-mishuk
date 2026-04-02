using Domain.ModelInterface;
using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Inventory.Receive
{
    public class ReceiveSearchVm : IDataTableSearch
    {
        public long Id { get; set; }

        [Required]
        [StringLength(50)]
        public string TranNo { get; set; }
        public DateTime TranDate { get; set; }

        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
    }
}
