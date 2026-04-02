using Domain.Entities.Leave;
using Domain.Utility.Common;
using Domain.ViewModel.Leave.LeaveCf;
using Interface.Base;

namespace Interface.Services.Leave
{
    public interface ILeaveCfService : IService<LeaveCf>
    {
        Task<DataTablePagination<LeaveCfSearchVm, LeaveCfSearchVm>>
                                          SearchAsync(DataTablePagination<LeaveCfSearchVm, LeaveCfSearchVm> model);
        //Task<bool> CalculateDailyCf();
        Task<bool> SetupNewCf();
    }
}
