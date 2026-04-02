using Domain.Entities.Leave;
using Domain.Utility.Common;
using Domain.ViewModel.Leave.EmpLeaveReviewer;
using Interface.Base;

namespace Interface.Repository.Leave
{
    public interface IEmpLeaveReviewerRepository : IRepository<EmpLeaveReviewer>
    {
        Task<DataTablePagination<EmpLeaveReviewerSearchVm, EmpLeaveReviewerSearchVm>>
                                SearchAsync(DataTablePagination<EmpLeaveReviewerSearchVm, EmpLeaveReviewerSearchVm> model);
    }
}
