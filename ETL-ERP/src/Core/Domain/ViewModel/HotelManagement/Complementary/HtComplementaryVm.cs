using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.HotelManagement.Complementary;

public class HtComplementaryVm
{
    public long Id { get; set; }

    [Required]
    [StringLength(80)]
    public string Title { get; set; }
    public bool IsActive { get; set; }
}