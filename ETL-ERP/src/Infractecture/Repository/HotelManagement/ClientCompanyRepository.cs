using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.ClientCompany;
using Domain.ViewModel.HotelManagement.RoomInfo;
using Domain.ViewModel.Inventory.ItemConversion;
using Interface.Repository.HotelManagement;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Repository.Base;

namespace Repository.HotelManagement;

public class ClientCompanyRepository : BaseRepository<ClientCompany>, IClientCompanyRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IMapper _iMapper;

    public ClientCompanyRepository(ApplicationDbContext db, IMapper iMapper) : base(db)
    {
        Db = db;
        _iMapper = iMapper;
    }
    #endregion

    #region Search
    public async Task<DataTablePagination<ClientCompanySearchVm, ClientCompanySearchVm>> SearchAsync(DataTablePagination<ClientCompanySearchVm, ClientCompanySearchVm> vm)
    {
        var searchResult = Context.ClientCompanies
                .AsQueryable().Where(c => !c.IsDeleted); ;
        var model = vm.SearchModel;

        if (model == null) throw new Exception("Search Company not found");

        if (!string.IsNullOrEmpty(vm.Search.Value))
        {
            var value = vm.Search.Value.Trim().ToLower();
            searchResult = searchResult.Where(c => c.Name.ToLower().Contains(value));
        }
        var totalRecords = await searchResult.CountAsync();
        if (totalRecords > 0)
        {
            vm.recordsTotal = totalRecords;
            vm.recordsFiltered = totalRecords;
            vm.draw = vm.LineDraw ?? 0;

            var data = await searchResult.OrderBy(c => c.Name)
                                         .Skip(vm.Start)
                                         .Take(vm.Length)
                                         .ToListAsync();

            vm.data = _iMapper.Map<List<ClientCompanySearchVm>>(data);

            var sl = vm.Start;

            foreach (var searchDto in vm.data)
            {
                var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                searchDto.SerialNo = ++sl;
                searchDto.Name = filterData.Name;
                searchDto.Address = filterData.Address;
                searchDto.Mobile = filterData.Mobile;
                searchDto.Email = filterData.Email;
                searchDto.WebSite = filterData.WebSite;
                searchDto.LogoUrl = filterData.LogoUrl;

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
