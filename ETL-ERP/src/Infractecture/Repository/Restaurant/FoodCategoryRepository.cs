using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.Restaurant.FoodCategory;
using Interface.Repository.Restaurant;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;
namespace Repository.Restaurant;

public class FoodCategoryRepository : BaseRepository<RsFoodCategory>, IFoodCategoryRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;

    public FoodCategoryRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
    {
        Db = db;
        _iMapper = iMapper;
    }
    #endregion

    #region Search
    public async Task<DataTablePagination<FoodCategorySearchVm, FoodCategorySearchVm>> SearchAsync(DataTablePagination<FoodCategorySearchVm, FoodCategorySearchVm> vm)
    {
        var searchResult = Context.RsFoodCategories
            .Include(x => x.ParentCategory)
            .AsQueryable().Where(c => !c.IsDeleted);
        var model = vm.SearchModel;

        if (model == null) throw new Exception("Search Food Category not found");

        if (!string.IsNullOrEmpty(vm.Search.Value))
        {
            var value = vm.Search.Value.Trim().ToLower();
            searchResult = searchResult.Where(c => c.CategoryName.ToLower().Contains(value));
        }
        var totalRecords = await searchResult.CountAsync();
        if (totalRecords > 0)
        {
            vm.recordsTotal = totalRecords;
            vm.recordsFiltered = totalRecords;
            vm.draw = vm.LineDraw ?? 0;

            var data = await searchResult.OrderBy(c => c.CategoryName)
                                         .Skip(vm.Start)
                                         .Take(vm.Length)
                                         .ToListAsync();

            vm.data = _iMapper.Map<List<FoodCategorySearchVm>>(data);

            var sl = vm.Start;

            foreach (var searchDto in vm.data)
            {
                var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                searchDto.SerialNo = ++sl;
                searchDto.ParentCategoryName = filterData?.ParentCategory?.CategoryName;
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