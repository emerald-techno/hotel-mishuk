using AutoMapper;
using Domain.Entities.Accounting;
using Domain.Utility.Common;
using Domain.ViewModel.Accounting.AccHead;
using Interface.Repository.Accounts;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Accounts
{
    public class AccHeadRepository : BaseRepository<AccHead>, IAccHeadRepository, IDisposable
    {
        #region Config
        private ApplicationDbContext Context => Db as ApplicationDbContext;
        private readonly IMapper _iMapper;

        public AccHeadRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
        {
            Db = db;
            _iMapper = iMapper;
        }

        #endregion

        #region Search

        public async Task<DataTablePagination<AccHeadSearchVm, AccHeadSearchVm>> SearchAsync(DataTablePagination<AccHeadSearchVm, AccHeadSearchVm> vm)
        {
            var searchResult = Context.AccHeads.Include(x => x.Group).AsQueryable().Where(c => !c.IsDeleted);
            var model = vm.SearchModel;

            if (model == null) throw new Exception("Account Heads not found");

            if (model.GroupId > 0)
            {
                searchResult = searchResult.Where(c => c.GroupId == model.GroupId);
            }
            if (!string.IsNullOrEmpty(model.HeadName))
            {
                searchResult = searchResult.Where(c => c.HeadName == model.HeadName);
            }
            if (!string.IsNullOrEmpty(model.HeadCode))
            {
                searchResult = searchResult.Where(c => c.HeadCode == model.HeadCode);
            }
            if (!string.IsNullOrEmpty(vm.Search.Value))
            {
                var value = vm.Search.Value.Trim().ToLower();
                searchResult = searchResult.Where(c => c.HeadName.ToLower().Contains(value));
            }
            var totalRecords = await searchResult.CountAsync();
            if (totalRecords > 0)
            {
                vm.recordsTotal = totalRecords;
                vm.recordsFiltered = totalRecords;
                vm.draw = vm.LineDraw ?? 0;

                var data = await searchResult.OrderBy(c => c.HeadCode)
                                             .Skip(vm.Start)
                                             .Take(vm.Length)
                                             .ToListAsync();

                vm.data = _iMapper.Map<List<AccHeadSearchVm>>(data);

                var sl = vm.Start;

                foreach (var searchDto in vm.data)
                {
                    var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                    searchDto.SerialNo = ++sl;
                    searchDto.GroupName = filterData?.Group.Name;

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
