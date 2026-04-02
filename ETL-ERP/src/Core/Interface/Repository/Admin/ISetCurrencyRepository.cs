using Domain.Entities.Admin;
using Domain.Utility.Common;
using Domain.ViewModel.Admin.SetCurrency;
using Interface.Base;

namespace Interface.Repository.Admin
{
    public interface ISetCurrencyRepository : IRepository<SetCurrency>
    {
        Task<DataTablePagination<SetCurrencySearchVm, SetCurrencySearchVm>>
            SearchAsync(DataTablePagination<SetCurrencySearchVm, SetCurrencySearchVm> model);
    }
}
