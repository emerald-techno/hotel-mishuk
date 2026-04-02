using AutoMapper;
using Domain.Entities.Inventory;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.CategoryInfo;
using Interface.Repository.Inventory;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.Inventory;

public class CategoryInfoRepository : BaseRepository<CategoryInfo>, ICategoryInfoRepository, IDisposable
{

    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;


    public CategoryInfoRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
    {
        Db = db;
        _iMapper = iMapper;
    }



    public void Dispose()
    {
        Context.Dispose();
    }

    public async Task<DataTablePagination<CategoryInfoSearchVm, CategoryInfoSearchVm>>
        SearchAsync(DataTablePagination<CategoryInfoSearchVm, CategoryInfoSearchVm> vm)
    {
        var searchResult = Context.CategoryInfos
            .Include(x=>x.Ledger)
            .AsQueryable().Where(c => !c.IsDeleted);
        var model = vm.SearchModel;
        if (model == null) throw new Exception("Search SupplierInfo not found");

        if (!string.IsNullOrEmpty(vm.Search.Value))
        {
            var value = vm.Search.Value.Trim().ToLower();
            searchResult = searchResult.Where(c => c.CategoryName.ToLower().Contains(value)
                                                || c.CategoryCode.ToLower().Contains(value));
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

            vm.data = _iMapper.Map<List<CategoryInfoSearchVm>>(data);

            var sl = vm.Start;

            foreach (var searchDto in vm.data)
            {
                var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                searchDto.SerialNo = ++sl;
                searchDto.LedgerName = filterData.Ledger?.LedgerName;
            }
        }
        return vm;
    }
}
