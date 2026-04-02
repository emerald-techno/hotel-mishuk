using AutoMapper;
using Domain.Entities.Leave;
using Domain.Utility.Common;
using Domain.ViewModel.Leave.LvAppReviewer;
using Interface.Repository.Leave;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Leave
{
    public class LvAppReviewerRepository : BaseRepository<LvAppReviewer>, ILvAppReviewerRepository, IDisposable
    {
        #region Config
        private ApplicationDbContext Context => Db as ApplicationDbContext;
        private readonly IMapper _iMapper;

        public LvAppReviewerRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
        {
            Db = db;
            _iMapper = iMapper;
        }
        #endregion

        #region Search

        public async Task<DataTablePagination<LvAppReviewerSearchVm, LvAppReviewerSearchVm>>
            SearchAsync(DataTablePagination<LvAppReviewerSearchVm, LvAppReviewerSearchVm> vm)
        {
            var searchResult = Context.LvAppReviewers
                .Include(c => c.LeaveApp)
                    .ThenInclude(e => e.Employee)
                .Include(c => c.LeaveApp)
                    .ThenInclude(l => l.LeaveType)
                .Include(c => c.Reviewer)
                .Include(c => c.AltReviewer)
                .AsQueryable()
                .Where(c => !c.IsDeleted);
            var model = vm.SearchModel;

            if (model == null) throw new Exception("Search Leave App Revierws Not Found");

            if (model.ReviewerId > 0)
            {
                searchResult = searchResult.Where(c => c.ReviewerId == model.ReviewerId);
            }

            if (!string.IsNullOrEmpty(vm.Search.Value))
            {
                var value = vm.Search.Value.Trim().ToLower();
                searchResult = searchResult.Where(c => c.LeaveApp.ApplicationNo.ToLower().Contains(value));
            }
            var totalRecords = await searchResult.CountAsync();
            if (totalRecords > 0)
            {
                vm.recordsTotal = totalRecords;
                vm.recordsFiltered = totalRecords;
                vm.draw = vm.LineDraw ?? 0;

                var data = await searchResult.OrderBy(c => c.ReceiveTime)
                                             .Skip(vm.Start)
                                             .Take(vm.Length)
                                             .ToListAsync();

                vm.data = _iMapper.Map<List<LvAppReviewerSearchVm>>(data);

                var sl = vm.Start;

                foreach (var searchDto in vm.data)
                {
                    var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                    searchDto.SerialNo = ++sl;
                    searchDto.EmployeeName = filterData?.LeaveApp?.Employee?.Name;
                    searchDto.ApplicationNo = filterData?.LeaveApp?.ApplicationNo;
                    searchDto.AppStatus = filterData?.LeaveApp?.Status;
                    searchDto.LeaveTypeName = filterData?.LeaveApp?.LeaveType?.TypeName;
                    searchDto.AppFromDate = filterData?.LeaveApp?.FromDate;
                    searchDto.AppToDate = filterData?.LeaveApp?.ToDate;
                    searchDto.ReviewerName = filterData?.Reviewer.FullName;
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
