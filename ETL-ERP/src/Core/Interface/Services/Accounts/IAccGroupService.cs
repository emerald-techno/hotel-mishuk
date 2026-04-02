using Domain.Entities.Accounting;
using Domain.Utility.Common;
using Domain.ViewModel.Accounting.AccGroup;
using Domain.ViewModel.Accounting.ChartOfAcc;
using Interface.Base;

namespace Interface.Services.Accounts
{
    public interface IAccGroupService : IService<AccGroup>
    {
        Task<DataTablePagination<AccGroupSearchVm, AccGroupSearchVm>> SearchAsync(DataTablePagination<AccGroupSearchVm, AccGroupSearchVm> model);
        Task<ChartOfAccVm> GetChartOfAcc();
    }
}
