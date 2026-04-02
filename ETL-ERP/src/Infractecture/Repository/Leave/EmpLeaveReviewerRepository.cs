using AutoMapper;
using Domain.Entities.Leave;
using Domain.Utility.Common;
using Domain.ViewModel.Leave.EmpLeaveReviewer;
using Interface.Repository.Leave;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Leave
{
    public class EmpLeaveReviewerRepository : BaseRepository<EmpLeaveReviewer>, IEmpLeaveReviewerRepository, IDisposable
    {
        #region Config
        private ApplicationDbContext Context => Db as ApplicationDbContext;
        private readonly IMapper _iMapper;

        public EmpLeaveReviewerRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
        {
            Db = db;
            _iMapper = iMapper;
        }
        #endregion

        #region Search

        public async Task<DataTablePagination<EmpLeaveReviewerSearchVm, EmpLeaveReviewerSearchVm>>
            SearchAsync(DataTablePagination<EmpLeaveReviewerSearchVm, EmpLeaveReviewerSearchVm> vm)
        {
            var searchResult = Context.EmpLeaveReviewers
                .Include(c => c.Employee)
                .Include(c => c.Reviewer)
                .Include(c => c.AltReviewer)
                .AsQueryable()
                .Where(c => !c.IsDeleted);
            var model = vm.SearchModel;

            if (model == null) throw new Exception("Search Employee Leave Reviewers not found");

            if (model.EmployeeId > 0)
            {
                searchResult = searchResult.Where(c => c.EmployeeId == model.EmployeeId);
            }

            if (!string.IsNullOrEmpty(vm.Search.Value))
            {
                var value = vm.Search.Value.Trim().ToLower();
                searchResult = searchResult.Where(c => c.Reviewer.FullName.ToLower().Contains(value));
            }
            var totalRecords = await searchResult.CountAsync();
            if (totalRecords > 0)
            {
                vm.recordsTotal = totalRecords;
                vm.recordsFiltered = totalRecords;
                vm.draw = vm.LineDraw ?? 0;

                var data = await searchResult.OrderBy(c => c.SlNo)
                                             .Skip(vm.Start)
                                             .Take(vm.Length)
                                             .ToListAsync();

                vm.data = _iMapper.Map<List<EmpLeaveReviewerSearchVm>>(data);

                var sl = vm.Start;

                foreach (var searchDto in vm.data)
                {
                    var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                    searchDto.SerialNo = ++sl;
                    searchDto.EmployeeName = filterData?.Employee.Name;
                    searchDto.ReviewerName = filterData?.Reviewer.FullName;
                    searchDto.AltReviewerName = filterData?.AltReviewer.FullName;
                }
            }
            return vm;
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            Context.Dispose();
        }

        #endregion
    }
}
