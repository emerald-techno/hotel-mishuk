using Domain.Entities.Admin;
using Domain.Utility.Common;
using Domain.ViewModel.Admin.SetCurrency;
using Interface.Base;

namespace Interface.Services.Admin
{
    public interface ISetCurrencyService : IService<SetCurrency>
    {
        Task<DataTablePagination<SetCurrencySearchVm, SetCurrencySearchVm>>
                                    SearchAsync(DataTablePagination<SetCurrencySearchVm, SetCurrencySearchVm> model);
    }
}
