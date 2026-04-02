using AutoMapper;
using Domain.Entities.Restaurant;
using Domain.Utility.Common;
using Domain.ViewModel.Restaurant.FoodIngredient;
using Interface.Repository.Restaurant;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Restaurant
{
    public class FoodIngredientRepository : BaseRepository<RsFoodIngredient>, IFoodIngredientRepository, IDisposable
    {
        #region Config
        private ApplicationDbContext Context => Db as ApplicationDbContext;
        private readonly IMapper _iMapper;

        public FoodIngredientRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
        {
            Db = db;
            _iMapper = iMapper;
        }
        #endregion

        #region Search
        public async Task<DataTablePagination<FoodIngredientSearchVM, FoodIngredientSearchVM>> SearchAsync(DataTablePagination<FoodIngredientSearchVM, FoodIngredientSearchVM> vm)
        {
            var searchResult = Context.RsFoodIngredients
           .Include(x => x.FoodItem)
           .AsQueryable().Where(c => !c.IsDeleted);
            var model = vm.SearchModel;

            if (model == null) throw new Exception("Search Food Ingredient not found");

            if (!string.IsNullOrEmpty(vm.Search.Value))
            {
                var value = vm.Search.Value.Trim().ToLower();
                searchResult = searchResult.Where(c => c.Item.ItemName.ToLower().Contains(value));
            }
            var totalRecords = await searchResult.CountAsync();
            if (totalRecords > 0)
            {
                vm.recordsTotal = totalRecords;
                vm.recordsFiltered = totalRecords;
                vm.draw = vm.LineDraw ?? 0;

                var data = await searchResult.OrderBy(c => c.Item.ItemName)
                                             .Skip(vm.Start)
                                             .Take(vm.Length)
                                             .ToListAsync();

                vm.data = _iMapper.Map<List<FoodIngredientSearchVM>>(data);

                var sl = vm.Start;

                foreach (var searchDto in vm.data)
                {
                    var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                    searchDto.SerialNo = ++sl;
                    searchDto.ItemName = filterData?.Item?.ItemName;
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
