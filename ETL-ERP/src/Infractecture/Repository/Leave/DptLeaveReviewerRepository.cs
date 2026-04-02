using AutoMapper;
using Domain.Entities.Leave;
using Domain.Utility.Common;
using Domain.ViewModel.Leave.DptLeaveReviewer;
using Interface.Repository.Leave;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Leave
{
    public class DptLeaveReviewerRepository : BaseRepository<DptLeaveReviewer>, IDptLeaveReviewerRepository, IDisposable
    {
        #region Config
        private ApplicationDbContext Context => Db as ApplicationDbContext;
        private readonly IMapper _iMapper;

        public DptLeaveReviewerRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
        {
            Db = db;
            _iMapper = iMapper;
        }
        #endregion

        #region Search

        public async Task<DataTablePagination<DptLeaveReviewerSearchVm, DptLeaveReviewerSearchVm>>
            SearchAsync(DataTablePagination<DptLeaveReviewerSearchVm, DptLeaveReviewerSearchVm> vm)
        {
            var searchResult = Context.DptLeaveReviewers
                .Include(c => c.Department)
                .Include(c => c.Reviewer)
                .Include(c => c.AltReviewer)
                .AsQueryable()
                .Where(c => !c.IsDeleted);
            var model = vm.SearchModel;

            if (model == null) throw new Exception("Search Department Leave Revierws not found");

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

                vm.data = _iMapper.Map<List<DptLeaveReviewerSearchVm>>(data);

                var sl = vm.Start;

                foreach (var searchDto in vm.data)
                {
                    var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                    searchDto.SerialNo = ++sl;
                    searchDto.DepartmentName = filterData?.Department.Name;
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
