namespace Domain.ViewModel.Admin.SetCurrency
{
    public class SetCurrencySearchVm
    {
        public long Id { get; set; }
        public string CurrencyName { get; set; }
        public string Symbol { get; set; }
        public string CountrySymbol { get; set; }
        public double ExRate { get; set; } = 1;
        public long UserId { get; set; }
        public int SerialNo { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
    }
}
