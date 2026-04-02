using AutoMapper;
using Domain.Entities.Inventory;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.UnitInfo;
using Interface.Repository.Inventory;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Inventory
{
    public class UnitInfoRepository : BaseRepository<UnitInfo>, IUnitInfoRepository, IDisposable
    {
        #region CONFIG
        private ApplicationDbContext Context => Db as ApplicationDbContext;
        private readonly IMapper _iMapper;

        public UnitInfoRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
        {
            Db = db;
            _iMapper = iMapper;
        }
        #endregion


        public void Dispose()
        {
            Context.Dispose();
        }

        public async Task<DataTablePagination<UnitInfoSearchVm, UnitInfoSearchVm>>
            SearchAsync(DataTablePagination<UnitInfoSearchVm, UnitInfoSearchVm> vm)
        {
            var searchResult = Context.UnitInfos
                .AsQueryable().Where(c => !c.IsDeleted);

            var model = vm.SearchModel;

            if (model == null) throw new Exception("Search Unitinfo not found");

            if (!string.IsNullOrEmpty(vm.Search.Value))
            {
                var value = vm.Search.Value.Trim().ToLower();
                searchResult = searchResult.Where(c => c.UnitName.ToLower().Contains(value)
                                                  || c.UnitCode.ToLower().Contains(value));
            }

            var totalRecords = await searchResult.CountAsync();
            if (totalRecords > 0)
            {
                vm.recordsTotal = totalRecords;
                vm.recordsFiltered = totalRecords;
                vm.draw = vm.LineDraw ?? 0;

                var data = await searchResult.OrderByDescending(c => c.ActionDate)
                                             .Skip(vm.Start)
                                             .Take(vm.Length)
                                             .ToListAsync();

                vm.data = _iMapper.Map<List<UnitInfoSearchVm>>(data);

                var sl = vm.Start;

                foreach (var searchDto in vm.data)
                {
                    searchDto.SerialNo = ++sl;
                }
            }
            return vm;
        }
    }
}
