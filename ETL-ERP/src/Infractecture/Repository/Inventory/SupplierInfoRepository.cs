using AutoMapper;
using Domain.Entities.Inventory;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.SupplierInfo;
using Interface.Repository.Inventory;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Inventory
{
    public class SupplierInfoRepository : BaseRepository<SupplierInfo>, ISupplierInfoRepository, IDisposable
    {
        #region Config
        private ApplicationDbContext Context => Db as ApplicationDbContext;
        private readonly IMapper _iMapper;

        public SupplierInfoRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
        {
            Db = db;
            _iMapper = iMapper;
        }
        #endregion

        public void Dispose()
        {
            Context.Dispose();
        }

        public async Task<DataTablePagination<SupplierInfoSearchVm, SupplierInfoSearchVm>>
            SearchAsync(DataTablePagination<SupplierInfoSearchVm, SupplierInfoSearchVm> vm)
        {
            var searchResult = Context.SupplierInfos
                .AsQueryable().Where(c => !c.IsDeleted);
            var model = vm.SearchModel;
            if (model == null) throw new Exception("Search SupplierInfo not found");

            if (!string.IsNullOrEmpty(vm.Search.Value))
            {
                var value = vm.Search.Value.Trim().ToLower();
                searchResult = searchResult.Where(c => c.SupplierName.ToLower().Contains(value)
                                                    || c.SupplierCode.ToLower().Contains(value)
                                                    || c.Mobile.ToLower().Contains(value)
                                                    || c.Phone.ToLower().Contains(value)
                                                    || c.Email.ToLower().Contains(value)
                                                    || c.Address.ToLower().Contains(value));
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

                vm.data = _iMapper.Map<List<SupplierInfoSearchVm>>(data);

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
