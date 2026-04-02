using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.Complementary;
using Interface.Repository.HotelManagement;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.HotelManagement;

public class ComplementaryRepository : BaseRepository<HtComplementary>, IComplementaryRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;

    public ComplementaryRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
    {
        Db = db;
        _iMapper = iMapper;
    }
    #endregion

    public async Task<DataTablePagination<HtComplementarySearchVm, HtComplementarySearchVm>> SearchAsync(DataTablePagination<HtComplementarySearchVm, HtComplementarySearchVm> vm)
    {
        var searchResult = Context.HtComplementaries.AsQueryable().Where(c => !c.IsDeleted);
        var model = vm.SearchModel;

        if (model == null) throw new Exception("Search bed type not found");

        if (!string.IsNullOrEmpty(vm.Search.Value))
        {
            var value = vm.Search.Value.Trim().ToLower();
            searchResult = searchResult.Where(c => c.Title.ToLower().Contains(value));
        }
        var totalRecords = await searchResult.CountAsync();
        if (totalRecords > 0)
        {
            vm.recordsTotal = totalRecords;
            vm.recordsFiltered = totalRecords;
            vm.draw = vm.LineDraw ?? 0;

            var data = await searchResult.OrderBy(c => c.Title)
                                         .Skip(vm.Start)
                                         .Take(vm.Length)
                                         .ToListAsync();

            vm.data = _iMapper.Map<List<HtComplementarySearchVm>>(data);

            var sl = vm.Start;

            foreach (var searchDto in vm.data)
            {
                var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                searchDto.SerialNo = ++sl;
            }
        }
        return vm;
    }

    #region Dispose

    public void Dispose()
    {
        Context.Dispose();
    }

    #endregion
}