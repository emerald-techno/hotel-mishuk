using Domain.Entities.Accounting;
using Domain.Utility.Common;
using Domain.ViewModel.Accounting.AccReviewer;
using Interface.Base;

namespace Interface.Services.Accounts
{
    public interface IAccReviewerService : IService<AccReviewer>
    {
        Task<DataTablePagination<AccReviewerSearchVm, AccReviewerSearchVm>> SearchAsync(DataTablePagination<AccReviewerSearchVm, AccReviewerSearchVm> model);
    }
}
