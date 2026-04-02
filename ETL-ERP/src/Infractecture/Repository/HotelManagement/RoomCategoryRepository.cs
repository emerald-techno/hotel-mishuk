using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.RoomCategory;
using Domain.ViewModel.HotelManagement.RoomFacility;
using Domain.ViewModel.Website;
using Interface.Repository.HotelManagement;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Persistence.DapperModel;
using Repository.Base;

namespace Repository.HotelManagement;

public class RoomCategoryRepository : BaseRepository<HtRoomCategory>, IRoomCategoryRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IApplicationReadDbConnection _iReadDbConnection;
    private readonly IMapper _iMapper;


    public RoomCategoryRepository(ApplicationDbContext db, IApplicationReadDbConnection iReadDbConnection, IMapper iMapper) : base(db)
    {
        Db = db;
        _iMapper = iMapper;
        _iReadDbConnection = iReadDbConnection;
    }
    #endregion

    #region Search
    public async Task<DataTablePagination<HtRoomCategorySearchVm, HtRoomCategorySearchVm>> SearchAsync(DataTablePagination<HtRoomCategorySearchVm, HtRoomCategorySearchVm> vm)
    {
        var searchResult = Context.HtRoomCategories
            .Include(x => x.BedType)
            .AsQueryable().Where(c => !c.IsDeleted);
        var model = vm.SearchModel;

        if (model == null) throw new Exception("Search Department not found");

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

            vm.data = _iMapper.Map<List<HtRoomCategorySearchVm>>(data);

            var sl = vm.Start;

            foreach (var searchDto in vm.data)
            {
                var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                searchDto.SerialNo = ++sl;
                searchDto.BedTypeName = filterData.BedType.TypeName;
            }
        }
        return vm;
    }
    #endregion

    public async Task<List<RoomFacilityVm>> GetRoomFacilityData()
    {
        var query = $@"select distinct rc.Id CategoryId, rc.CategoryName, FacilityName 
                        from HtRoomFacilities rf
                        join HtRoomFacilityCategories fc on  rf.FacilityCategoryId = fc.Id
                        join HtRoomFacilityMaps fm on fm.FacilityId = rf.Id
                        join HtRoomInfos ri on fm.RoomId = ri.Id
                        join HtRoomCategories rc on rc.Id = ri.RoomCategoryId";


        var data = await _iReadDbConnection.QueryAsync<RoomFacilityVm>(query);
        return data.ToList();
    }

    public async Task<List<HtRoomCategoryVm>> GetRoomCategoryPublicData()
    {
        var data = await Context.HtRoomCategories
            .Include(x => x.BedType)
            .AsQueryable()
            .Where(c => !c.IsDeleted && c.IsActive && c.IsWebSiteShow).ToListAsync();

        var discountData = await Context.HtRoomCategoryDiscountMaps
            .AsQueryable()
            .Where(c => !c.IsDeleted && c.IsActive && c.DiscountAmount > 0)
            .ToListAsync();

        var rooms = await Context.HtRoomInfos
            .AsQueryable()
            .Where(c => !c.IsDeleted).ToListAsync();

        var roomCategoryIds = rooms.Select(x => x.RoomCategoryId).Distinct().ToList();
        data = data.Where(x => roomCategoryIds.Contains(x.Id)).ToList();

        var roomCategoryFacilityList = await Context.HtRoomFacilityMaps
            .Include(x => x.Facility)
            .ThenInclude(c => c.FacilityCategory)
            .AsQueryable()
            .Where(c => !c.IsDeleted).ToListAsync();

        var dataList = _iMapper.Map<List<HtRoomCategoryVm>>(data);

        if (dataList.Count > 0)
        {
            dataList = dataList.OrderBy(c => c.CategoryName).ToList();

            foreach (var searchDto in dataList)
            {
                var facilityMapList = roomCategoryFacilityList.Where(c => c.RoomCategoryId == searchDto.Id);

                var facilityList = facilityMapList.Select(x => x.Facility).ToList();

                searchDto.RoomFacilityList = _iMapper.Map<List<HtRoomFacilityVm>>(facilityList);
            }
        }

        foreach (var item in dataList)
        {
            var filterData = discountData.FirstOrDefault(x => x.RoomCategoryId == item.Id);

            if (filterData != null)
            {
                item.HasDiscount = true;
                item.DiscountValue = filterData.DiscountAmount;
                item.DiscountStart = filterData.FromDate;
                item.DiscountEnd = filterData.ToDate;
                item.DiscountType = (DiscountType)filterData.DiscountType;
            }
        }

        return dataList;
    }

    #region Dispose

    public void Dispose()
    {
        Context.Dispose();
    }


    #endregion
}
