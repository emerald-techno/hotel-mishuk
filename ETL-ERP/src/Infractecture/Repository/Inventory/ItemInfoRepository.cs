using AutoMapper;
using Domain.Entities.Inventory;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.ItemInfo;
using Interface.Repository.Inventory;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Inventory
{
    public class ItemInfoRepository : BaseRepository<ItemInfo>, IItemInfoRepository, IDisposable
    {
        private ApplicationDbContext Context => Db as ApplicationDbContext;
        private readonly IMapper _iMapper;

        public ItemInfoRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
        {
            Db = db;
            _iMapper = iMapper;
        }

        public void Dispose()
        {
            Context.Dispose();
        }

        public async Task<DataTablePagination<ItemInfoSearchVm, ItemInfoSearchVm>>
            SearchAsync(DataTablePagination<ItemInfoSearchVm, ItemInfoSearchVm> vm)
        {
            var searchResult = Context.ItemInfos
                .Include(c => c.Category)
                .Include(c => c.Unit)
                .AsQueryable().Where(c => !c.IsDeleted);

            var model = vm.SearchModel;

            if (model == null) throw new Exception("Search ItemInfo not found");

            if (!string.IsNullOrEmpty(vm.Search.Value))
            {
                var value = vm.Search.Value.Trim().ToLower();
                searchResult = searchResult.Where(c => c.ItemName.ToLower().Contains(value)
                                                    || c.ItemCode.ToLower().Contains(value));
            }

            if (model.CategoryId > 0)
            {
                searchResult = searchResult.Where(x => x.CategoryId == model.CategoryId);
            }

            if (model.UnitId > 0)
            {
                searchResult = searchResult.Where(x => x.UnitId == model.UnitId);
            }

            if (!string.IsNullOrEmpty(model.CategoryType))
            {
                searchResult = searchResult.Where(c => c.Category.CategoryType == model.CategoryType);
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

                vm.data = _iMapper.Map<List<ItemInfoSearchVm>>(data);


                var sl = vm.Start;

                foreach (var searchDto in vm.data)
                {
                    var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                    searchDto.SerialNo = ++sl;
                    searchDto.CategoryName = filterData.Category.CategoryName;
                    searchDto.CategoryType = filterData.Category.CategoryType;
                    searchDto.UnitName = filterData.Unit.UnitName;
                }
            }
            return vm;
        }

        #region Item Details
        public async Task<ItemInfoVm> Details(long id)
        {
            var data = await Context.ItemInfos
                .Include(x => x.Category)
                .Include(x => x.Unit)
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (data == null)
                throw new Exception("No Food Item found");

            var model = _iMapper.Map<ItemInfoVm>(data);
            model.CategoryName = data.Category.CategoryName;
            model.UnitName = data.Unit.UnitName;

            var foodIngredientList = await Context.RsFoodIngredients
                .Include(x => x.Item)
                .Include(x => x.Unit)
                .AsNoTracking()
                .Where(x => x.FoodItemId == data.Id && !x.IsDeleted).ToListAsync();
            return model;
        }
        #endregion
    }
}
