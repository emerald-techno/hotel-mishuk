using Domain.Entities.Leave;
using Domain.Utility.Common;
using Domain.ViewModel.Leave.DptLeaveReviewer;
using Interface.Base;

namespace Interface.Repository.Leave
{
    public interface IDptLeaveReviewerRepository : IRepository<DptLeaveReviewer>
    {
        Task<DataTablePagination<DptLeaveReviewerSearchVm, DptLeaveReviewerSearchVm>>
            SearchAsync(DataTablePagination<DptLeaveReviewerSearchVm, DptLeaveReviewerSearchVm> model);
    }
}
