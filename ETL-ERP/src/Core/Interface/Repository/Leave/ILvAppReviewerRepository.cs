using Domain.Entities.Leave;
using Domain.Utility.Common;
using Domain.ViewModel.Leave.LvAppReviewer;
using Interface.Base;

namespace Interface.Repository.Leave
{
    public interface ILvAppReviewerRepository : IRepository<LvAppReviewer>
    {
        Task<DataTablePagination<LvAppReviewerSearchVm, LvAppReviewerSearchVm>>
            SearchAsync(DataTablePagination<LvAppReviewerSearchVm, LvAppReviewerSearchVm> vm);
    }
}
