using Domain.Entities.Inventory;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.Issue;
using Domain.ViewModel.Inventory.RequsitionInfo;
using Interface.Base;

namespace Interface.Services.Inventory
{
    public interface IRequsitionInfoService : IService<RequsitionInfo>
    {
        Task<bool> AddAsync(RequsitionInfoVm vm);
        Task<bool> AddOrUpdate(RequsitionInfoVm vm);

        Task<DataTablePagination<RequsitionInfoSearchVm, RequsitionInfoSearchVm>>
            SearchAsync(DataTablePagination<RequsitionInfoSearchVm, RequsitionInfoSearchVm> model);
        Task<RequsitionInfoVm> GetRequsitionInfoDataAsync(long id);
        dynamic GetItemsByRequsitionId(long id);
        Task<bool> ReviewUpdate(RequsitionApprovalVm vm, string ntfLink);
        Task<string> GetRequsitionInfoNo();
        dynamic GetRequsitionByDptId(long dptId);
        dynamic GetRequsitionByEmpId(long empId);
        Task<List<IssueDtlVm>> GetIssueItemByReqId(long reqId);
        Task<string> GetRequsitionByIdAsyncHtml(long id);
    }
}
