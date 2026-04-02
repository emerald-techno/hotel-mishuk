using Domain.Entities.Accounting;
using Domain.Utility.Common;
using Domain.ViewModel.Accounting.AccReviewer;
using Interface.Base;

namespace Interface.Repository.Accounts
{
    public interface IAccReviewerRepository : IRepository<AccReviewer>
    {
        Task<DataTablePagination<AccReviewerSearchVm, AccReviewerSearchVm>>
                                                        SearchAsync(DataTablePagination<AccReviewerSearchVm, AccReviewerSearchVm> model);
    }
}
