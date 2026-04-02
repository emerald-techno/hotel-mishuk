using Domain.Entities.Accounting;
using Domain.Utility.Common;
using Domain.ViewModel.Accounting.AccHead;
using Interface.Base;

namespace Interface.Services.Accounts
{
    public interface IAccHeadService : IService<AccHead>
    {
        Task<DataTablePagination<AccHeadSearchVm, AccHeadSearchVm>> SearchAsync(DataTablePagination<AccHeadSearchVm, AccHeadSearchVm> model);
        Task<string> GetAccHeadMaxAutoCode();
        Task<long> GetAccHeadGroupAutoCode(long groupId, long? parentId);
        Task<string> GetAccHeadCodeAsync(long groupId, long? parentId);
    }
}
