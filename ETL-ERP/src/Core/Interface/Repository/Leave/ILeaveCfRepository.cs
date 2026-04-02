using Domain.Entities.Leave;
using Domain.Utility.Common;
using Domain.ViewModel.Leave.LeaveCf;
using Interface.Base;

namespace Interface.Repository.Leave
{
    public interface ILeaveCfRepository : IRepository<LeaveCf>
    {
        Task<DataTablePagination<LeaveCfSearchVm, LeaveCfSearchVm>>
                             SearchAsync(DataTablePagination<LeaveCfSearchVm, LeaveCfSearchVm> model);
        Task<bool> CalculateDailyCf();
        Task<bool> SetupNewYearCf();
    }
}
