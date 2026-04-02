using Domain.Entities.Leave;
using Domain.Utility.Common;
using Domain.ViewModel.Leave.EmpLeaveReviewer;
using Interface.Base;

namespace Interface.Services.Leave
{
    public interface IEmpLeaveReviewerService : IService<EmpLeaveReviewer>
    {
        Task<DataTablePagination<EmpLeaveReviewerSearchVm, EmpLeaveReviewerSearchVm>>
                                          SearchAsync(DataTablePagination<EmpLeaveReviewerSearchVm, EmpLeaveReviewerSearchVm> model);
    }
}
