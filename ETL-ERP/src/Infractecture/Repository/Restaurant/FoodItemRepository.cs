using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.Restaurant.FoodIngredient;
using Domain.ViewModel.Restaurant.FoodItem;
using Interface.Repository.Restaurant;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Restaurant;

public class FoodItemRepository : BaseRepository<RsFoodItem>, IFoodItemRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;

    public FoodItemRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
    {
        Db = db;
        _iMapper = iMapper;
    }
    #endregion

    #region Search
    public async Task<DataTablePagination<FoodItemSearchVm, FoodItemSearchVm>> SearchAsync(DataTablePagination<FoodItemSearchVm, FoodItemSearchVm> vm)
    {
        var searchResult = Context.RsFoodItems
                .Include(x => x.Category)
                .AsQueryable().Where(c => !c.IsDeleted);
        var model = vm.SearchModel;

        if (model == null) throw new Exception("Search Food Category not found");



        if (model.CategoryId > 0)
        {
            searchResult = searchResult.Where(c => c.CategoryId == model.CategoryId);
        }


        if (!string.IsNullOrEmpty(vm.Search.Value))
        {
            var value = vm.Search.Value.Trim().ToLower();
            searchResult = searchResult.Where(c => c.ItemName.ToLower().Contains(value));
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

            vm.data = _iMapper.Map<List<FoodItemSearchVm>>(data);

            var sl = vm.Start;

            foreach (var searchDto in vm.data)
            {
                var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                searchDto.SerialNo = ++sl;
                searchDto.CategoryName = filterData.Category?.CategoryName;
                //searchDto.ReqDate = filterData.Req?.ReqDate;
                //searchDto.DeliveryDate = filterData?.DeliveryDeadline;
                //searchDto.SupplierName = filterData.Supplier?.SupplierName;
            }
        }
        return vm;
    }
    #endregion

    #region Food Details
    public async Task<FoodItemVm> FoodDetails(long id)
    {
        var data = await Context.RsFoodItems
            .Include(x => x.Category)
            .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        if (data == null)
            throw new Exception("No Food Item found");

        var model = _iMapper.Map<FoodItemVm>(data);
        model.CategoryName = data.Category.CategoryName;

        var foodIngredientList = await Context.RsFoodIngredients
            .Include(x => x.Item)
            .Include(x => x.Unit)
            .AsNoTracking()
            .Where(x => x.FoodItemId == data.Id && !x.IsDeleted).ToListAsync();

        model.FoodIngredientVMs = _iMapper.Map<List<FoodIngredientVM>>(foodIngredientList);

        if (model.FoodIngredientVMs.Count > 0)
        {
            foreach (var ingredient in model.FoodIngredientVMs)
            {
                var filterData = foodIngredientList.FirstOrDefault(x => x.Id == ingredient.Id);

                ingredient.FoodItemName = data.ItemName;
                ingredient.ItemName = filterData.Item?.ItemName;
                ingredient.UnitName = filterData.Unit?.UnitName;
            }
        }

        //var itemExistInOrder = await Context.RsFoodOrderItems
        //    .SingleOrDefaultAsync(x => x.FoodId == data.Id && !x.IsDeleted);

        var foodSetMenutList = await Context.RsFoodSetItems
            .Include(x => x.SetFoodItem)
            .AsNoTracking()
            .Where(x => x.FoodItemId == data.Id && !x.IsDeleted).ToListAsync();

        model.FoodSetItemVms = _iMapper.Map<List<FoodSetItemVm>>(foodSetMenutList);

        if (model.FoodSetItemVms.Count > 0)
        {
            foreach (var setItem in model.FoodSetItemVms)
            {
                var filterData = foodSetMenutList.FirstOrDefault(x => x.Id == setItem.Id);

                setItem.SetFoodItemName = filterData.SetFoodItem.ItemName;
                setItem.SetFoodItemPrice = filterData.SetFoodItem.NetRate;
            }
        }

        return model;
    }
    #endregion

    #region GetSetMenuItems
    public async Task<List<FoodSetItemVm>> GetSetMenuItems(long foodId)
    {
        var data = await Context.RsFoodItems
            .FirstOrDefaultAsync(c => c.Id == foodId && !c.IsDeleted);

        if (data == null)
            throw new Exception("No Food Item found");

        if (data.IsSetMenuItem != true)
            throw new Exception("Food item is not set menu");

        var foodSetMenutList = await Context.RsFoodSetItems
            .Include(x => x.SetFoodItem)
            .AsNoTracking()
            .Where(x => x.FoodItemId == data.Id && !x.IsDeleted).ToListAsync();

        var foodSetItemVms = _iMapper.Map<List<FoodSetItemVm>>(foodSetMenutList);

        if (foodSetItemVms.Count > 0)
        {
            foreach (var setItem in foodSetItemVms)
            {
                var filterData = foodSetMenutList.FirstOrDefault(x => x.Id == setItem.Id);

                setItem.SetFoodItemName = filterData.SetFoodItem.ItemName;
                setItem.SetFoodItemPrice = filterData.SetFoodItem.NetRate;
            }
        }

        return foodSetItemVms;
    }

    #endregion

    #region Dispose

    public void Dispose()
    {
        Context.Dispose();
    }

    //public async Task<bool> IsFoodItemInOrderAsync()
    //{
    //     var isAvailableInOrder = await Context.RsFoodOrderItems.Include(x=>x.Food).AnyAsync(item => item.FoodId == item.Food.Id && item.Food.IsSetMenuItem==true);
    //    return isAvailableInOrder;
    //}

    #endregion
}
