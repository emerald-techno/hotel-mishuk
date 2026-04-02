using Domain.Entities.Accounting;
using Domain.Utility.Common;
using Domain.ViewModel.Accounting.AccLedger;
using Interface.Base;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Interface.Services.Accounts
{
    public interface IAccLedgerService : IService<AccLedger>
    {
        Task<DataTablePagination<AccLedgerSearchVm, AccLedgerSearchVm>> SearchAsync(DataTablePagination<AccLedgerSearchVm, AccLedgerSearchVm> model);
        Task<string> GetAccLedgerCode(long headId);
        Task<long> GetAccLedgerGroupCode(long? headId);
        Task<bool> IsLedgerFoundInHead(long headId);
        Task<IEnumerable<SelectListItem>> GetMishukLedgerSelectListItems(bool isDefaultSelectAdd = true);
        Task<long> GetGuestLedgerId(long guestId);
    }
}
