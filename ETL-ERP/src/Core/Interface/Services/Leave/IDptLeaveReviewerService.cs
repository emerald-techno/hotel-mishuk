using Domain.Entities.Leave;
using Domain.Utility.Common;
using Domain.ViewModel.Leave.DptLeaveReviewer;
using Interface.Base;

namespace Interface.Services.Leave
{
    public interface IDptLeaveReviewerService : IService<DptLeaveReviewer>
    {
        Task<DataTablePagination<DptLeaveReviewerSearchVm, DptLeaveReviewerSearchVm>>
                                          SearchAsync(DataTablePagination<DptLeaveReviewerSearchVm, DptLeaveReviewerSearchVm> model);
    }
}
