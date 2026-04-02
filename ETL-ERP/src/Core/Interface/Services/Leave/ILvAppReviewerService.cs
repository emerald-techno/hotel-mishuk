using Domain.Entities.Leave;
using Domain.Utility.Common;
using Domain.ViewModel.Leave.LvAppReviewer;
using Interface.Base;

namespace Interface.Services.Leave
{
    public interface ILvAppReviewerService : IService<LvAppReviewer>
    {
        Task<bool> LeaveAppReview(LvAppReviewerVm vm);
        Task<bool> LeaveAppReject(LvAppReviewerVm vm);
        Task<DataTablePagination<LvAppReviewerSearchVm, LvAppReviewerSearchVm>>
            SearchAsync(DataTablePagination<LvAppReviewerSearchVm, LvAppReviewerSearchVm> model);
    }
}
