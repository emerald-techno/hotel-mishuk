using AutoMapper;
using Domain.Entities.HouseKeeping;
using Domain.Enums.AppEnums;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.HotelReport;
using Domain.ViewModel.HouseKeeping.Reports;
using Domain.ViewModel.HouseKeeping.RoomAssign;
using Interface.Repository.HouseKeeping;
using Microsoft.EntityFrameworkCore;
using Persistence.ContextModel;
using Persistence.DapperModel;
using Repository.Base;
using DU = Domain.Utility;

namespace Repository.HouseKeeping;

public class RoomAssignRepository : BaseRepository<HkRoomAssign>, IRoomAssignRepository, IDisposable
{
    #region Config
    private ApplicationDbContext Context => Db as ApplicationDbContext;
    private readonly IApplicationReadDbConnection _iReadDbConnection;
    private readonly IMapper _iMapper;

    public RoomAssignRepository(ApplicationDbContext db, IMapper iMapper, IApplicationReadDbConnection iReadDbConnection) : base(db)
    {
        Db = db;
        _iMapper = iMapper;
        _iReadDbConnection = iReadDbConnection;
    }
    #endregion

    #region AssignRooms

    public async Task<DataTablePagination<RoomAssignSearchVm, RoomAssignSearchVm>> AssignRooms(DataTablePagination<RoomAssignSearchVm, RoomAssignSearchVm> vm)
    {
        var searchResult = Context.HkRoomAssigns
            .Include(c => c.Room)
                .ThenInclude(x => x.RoomCategory)
             .Include(x => x.Room)
                .ThenInclude(f => f.Floor)
            .AsQueryable().Where(c => !c.IsDeleted);

        var model = vm.SearchModel;

        if (model == null)
        {
            throw new Exception("Search assign room not found..!!");
        }

        if (model.HouseKeeperId > 0)
        {
            searchResult = searchResult.Where(c => c.HouseKeeperId == model.HouseKeeperId && (c.Status != RoomAssignEnum.Completed || c.Status != RoomAssignEnum.Cancel));
        }

        if (!string.IsNullOrEmpty(vm.Search.Value))
        {
            var value = vm.Search.Value.Trim().ToLower();
            searchResult = searchResult.Where(c => c.Room.RoomNo.ToLower().Contains(value) || c.Room.RoomCategory.CategoryName.ToLower().Contains(value));
        }

        var totalRecords = await searchResult.CountAsync();

        if (totalRecords > 0)
        {
            vm.recordsTotal = totalRecords;
            vm.recordsFiltered = totalRecords;
            vm.draw = vm.LineDraw ?? 0;

            var data = await searchResult.OrderBy(c => c.Room.RoomNo)
                                         .Skip(vm.Start)
                                         .Take(vm.Length)
                                         .ToListAsync();

            vm.data = _iMapper.Map<List<RoomAssignSearchVm>>(data);

            var sl = vm.Start;

            foreach (var searchDto in vm.data)
            {
                var filterData = data.Where(c => c.Id == searchDto.Id).FirstOrDefault();

                searchDto.SerialNo = ++sl;
                searchDto.RoomNo = filterData.Room.RoomNo;
                searchDto.RoomCategoryName = filterData.Room.RoomCategory.CategoryName;
                searchDto.RoomFloorName = filterData.Room.Floor.FloorName;
            }
        }
        return vm;
    }

    #endregion

    #region GetRoomCleaningReportData
    public async Task<List<RoomCleaningReportVm>> GetRoomCleaningReportData(RoomCleaningReportVm vm)
    {
        vm.StrFromDate = (string.IsNullOrEmpty(vm.StrFromDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrFromDate;
        vm.StrToDate = (string.IsNullOrEmpty(vm.StrFromDate)) ? DateTime.Today.ToString("dd/MM/yyyy") : vm.StrToDate;

        var fromDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrFromDate)).ToString("dd/MMM/yyyy");
        var toDate = Convert.ToDateTime(DU.Utility.ConvertStrToDate(vm.StrToDate)).ToString("dd/MMM/yyyy");

        string fromDateFilter = $" convert(date,ta.AssignDate) >= '{fromDate}'";
        string toDateFilter = $" and convert(date,ta.AssignDate) <= '{toDate}'";
        string houseKeeperFilter = vm.HouseKeeperId != null && vm.HouseKeeperId > 0 ? @$"and e.Id = {vm.HouseKeeperId}" : "";
        string roomNoFilter = !string.IsNullOrEmpty(vm.RoomNo) ? @$"and ri.RoomNo like '%{vm.RoomNo}%'" : "";
        string statusFilter = vm.Status != null && vm.Status > -1 ? @$"and ra.Status = {vm.Status}" : "";


        string query = $@"select ri.RoomNo,ISNULL(e.Name, '') HouseKeeperName, e.Id HouseKeeperId,CONVERT(date, ta.AssignDate) AssignDate, ISNULL(Convert(date,ta.CompleteDate), null)                          CompleteDate,ISNULL(Convert(date,ta.AuditDate), null) AuditDate , ra.Status
                            from HkRoomAssigns ra 
                            join HkTaskAssigns ta on ra.Id = ta.AssignId 
                            join HtRoomInfos ri on ri.Id = ra.RoomId
                            join Employees e on e.Id = ra.HouseKeeperId
                            where {fromDateFilter} {toDateFilter} {houseKeeperFilter} {roomNoFilter} {statusFilter};";

        var data = await _iReadDbConnection.QueryAsync<RoomCleaningReportVm>(query);
        return data.ToList();
    }
    #endregion

    #region Dispose

    public void Dispose()
    {
        Context.Dispose();
    }

    #endregion
}