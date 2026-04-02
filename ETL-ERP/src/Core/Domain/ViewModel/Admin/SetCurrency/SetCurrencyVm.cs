using System.ComponentModel.DataAnnotations;

namespace Domain.ViewModel.Admin.SetCurrency
{
    public class SetCurrencyVm
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Currency name is required")]
        [StringLength(50, ErrorMessage = "Currency name can not be exceed 50 characters")]
        public string CurrencyName { get; set; }

        [Required(ErrorMessage = "Symbol is required")]
        [StringLength(15, ErrorMessage = "Symbol can not exceed 15 characters")]
        public string Symbol { get; set; }

        [Required(ErrorMessage = "CountrySymbol is required")]
        [StringLength(15, ErrorMessage = "CountrySymbol can not exceed 15 characters")]
        public string CountrySymbol { get; set; }
        public double ExRate { get; set; } = 1;

        //-----------------------------------------
    }
}
