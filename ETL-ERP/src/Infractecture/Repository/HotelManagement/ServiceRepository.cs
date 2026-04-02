using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.Service;
using Interface.Repository.HotelManagement;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.HotelManagement;

public class ServiceRepository : BaseRepository<HtService>, IServiceRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;

    public ServiceRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
    {
        Db = db;
        _iMapper = iMapper;
    }
    #endregion

    public async Task<DataTablePagination<ServiceSearchVm, ServiceSearchVm>> SearchAsync(DataTablePagination<ServiceSearchVm, ServiceSearchVm> vm)
    {
        var searchResult = Context.HtServices
                                  .Include(l=>l.Ledger)
            .AsQueryable().Where(c => !c.IsDeleted);
        var model = vm.SearchModel;

        if (model == null) throw new Exception("Search service not found");

        if (!string.IsNullOrEmpty(vm.Search.Value))
        {
            var value = vm.Search.Value.Trim().ToLower();
            searchResult = searchResult.Where(c => c.ServiceName.ToLower().Contains(value));
        }
        var totalRecords = await searchResult.CountAsync();
        if (totalRecords > 0)
        {
            vm.recordsTotal = totalRecords;
            vm.recordsFiltered = totalRecords;
            vm.draw = vm.LineDraw ?? 0;

            var data = await searchResult.OrderBy(c => c.ServiceName)
                                         .Skip(vm.Start)
                                         .Take(vm.Length)
                                         .ToListAsync();

            vm.data = _iMapper.Map<List<ServiceSearchVm>>(data);

            var sl = vm.Start;

            foreach (var searchDto in vm.data)
            {
                var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                searchDto.SerialNo = ++sl;
                searchDto.LedgerName = filterData?.Ledger?.LedgerName;
                searchDto.LedgerCode = filterData?.Ledger?.LedgerCode;

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
