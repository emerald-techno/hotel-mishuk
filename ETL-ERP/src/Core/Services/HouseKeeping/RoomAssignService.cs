using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Domain.Entities.HotelManagement;
using Domain.Entities.HouseKeeping;
using Domain.Enums.AppEnums;
using Domain.Utility;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.HotelReport;
using Domain.ViewModel.HouseKeeping.Reports;
using Domain.ViewModel.HouseKeeping.RoomAssign;
using Interface.Repository.HotelManagement;
using Interface.Repository.HouseKeeping;
using Interface.Repository.Hr;
using Interface.Services.HotelManagement;
using Interface.Services.HouseKeeping;
using Interface.UnitOfWork;
using Services.Base;
using Services.HotelManagement;
using System.Transactions;

namespace Services.HouseKeeping;

public class RoomAssignService : BaseService<HkRoomAssign>, IRoomAssignService
{
    #region Config
    private IRoomAssignRepository _iRepository;
    private ITaskNameRepository _iTaskNameRepository;
    private ITaskAssignRepository _iTaskAssignRepository;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly IRoomInfoRepository _iRoomInfoRepository;
    private readonly IRoomInfoService _iRoomInfoService;

    public RoomAssignService(IRoomAssignRepository iRepository,
        IMapper iMapper,
        IUnitOfWork iUnitOfWork,
        ITaskNameRepository iTaskNameRepository,
        IRoomInfoRepository iRoomInfoRepository,
        IRoomInfoService iRoomInfoService,
        ITaskAssignRepository iTaskAssignRepository) : base(iRepository, iUnitOfWork)
    {
        _iRepository = iRepository;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
        _iRoomInfoRepository = iRoomInfoRepository;
        _iRoomInfoService = iRoomInfoService;
        _iTaskNameRepository = iTaskNameRepository;
        _iTaskAssignRepository = iTaskAssignRepository;
    }
    #endregion

    #region AddOrUpdate
    public async Task<bool> AddOrUpdate(RoomAssignVm vm)
    {
        if (vm?.RoomAssignVms == null || vm.RoomAssignVms.Count <= 0) throw new Exception("Sorry! No rooms found to assign!");

        List<HkRoomAssign> addableList = null;
        List<HkRoomAssign> updateableList = null;
        List<HkRoomAssign> deletableList = null;

        if (vm?.RoomAssignVms?.Count > 0)
        {
            var keeperId = vm.RoomAssignVms?.Select(c => c.HouseKeeperId).FirstOrDefault();
            var dataListForAdd = vm?.RoomAssignVms?.Where(c => c.Id == 0).ToList();

            var updatableItemIds = vm?.RoomAssignVms?.Where(c => c.Id > 0).Select(c => c.Id).ToList();
            updateableList = (await _iRepository.GetAsync(c => updatableItemIds.Contains(c.Id))).ToList();

            if (updateableList?.Count > 0)
            {
                foreach (var updateRoom in updateableList)
                {
                    var filterData = vm.RoomAssignVms.Where(c => c.Id == updateRoom.Id).FirstOrDefault();

                    updateRoom.UpdateDate = DateTime.Now;
                    updateRoom.UpdatedById = CurrentUserId;
                }

            }

            var oldIds = updateableList?.Select(c => c.Id).ToList();

            deletableList = (await _iRepository.GetAsync(c => c.HouseKeeperId == keeperId && !oldIds.Contains(c.Id))).ToList();

            if (dataListForAdd?.Count > 0)
            {
                addableList = _iMapper.Map<List<HkRoomAssign>>(dataListForAdd);

                foreach (var (v, i) in addableList.GetItemWithIndex())
                {
                    v.ActionDate = DateTime.Now;
                    v.ActionById = CurrentUserId;
                }
            }

        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        if (addableList?.Count > 0)
        {
            _iRepository.AddRange(addableList);
        }

        if (updateableList?.Count > 0)
        {
            _iRepository.UpdateRange(updateableList);
        }

        if (deletableList?.Count > 0)
        {
            _iRepository.RemoveRange(deletableList);
        }

        var isAdded = await _iUnitOfWork.CompleteAsync();
        if (!isAdded) { return false; }
        ts.Complete();
        return true;
    }
    #endregion

    #region AssignRoomAdd
    public async Task<bool> AddAssignRoom(RoomAssignVm vm)
    {
        if (vm?.RoomAssignVms == null || vm.RoomAssignVms.Count <= 0) throw new Exception("Sorry! No rooms found to assign!");

        List<HkRoomAssign> addableList = null;


        if (vm?.RoomAssignVms?.Count > 0)
        {
            var keeperId = vm.RoomAssignVms?.Select(c => c.HouseKeeperId).FirstOrDefault();
            var dataListForAdd = vm?.RoomAssignVms?.Where(c => c.Id == 0).ToList();

            var updatableItemIds = vm?.RoomAssignVms?.Where(c => c.Id > 0).Select(c => c.Id).ToList();

            if (dataListForAdd?.Count > 0)
            {
                addableList = _iMapper.Map<List<HkRoomAssign>>(dataListForAdd);

                foreach (var (v, i) in addableList.GetItemWithIndex())
                {
                    v.ActionDate = DateTime.Now;
                    v.ActionById = CurrentUserId;
                }
            }

        }

        List<HtRoomInfo> changeStatusRoomList = new List<HtRoomInfo>();

        if (addableList?.Count > 0)
        {
            foreach (var item in addableList)
            {
                var roomInfo = await _iRoomInfoRepository.GetFirstOrDefaultAsync(x => x.Id == item.RoomId && !x.IsDeleted);
                if (roomInfo == null)
                    throw new Exception("Room Not Found To assinged");

                roomInfo.CleaningStatus = CleaningStatusEnum.VD;

                changeStatusRoomList.Add(roomInfo);
            }
        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        if (addableList?.Count > 0)
        {
            _iRepository.AddRange(addableList);
        }

        if (changeStatusRoomList.Count > 0)
        {
            _iRoomInfoRepository.UpdateRange(changeStatusRoomList);
        }

        var isAdded = await _iUnitOfWork.CompleteAsync();
        if (!isAdded) { return false; }
        ts.Complete();
        return true;
    }
    #endregion

    #region RemoveAssignRoom
    public async Task<bool> RemoveAssignRoom(RoomAssignVm vm)
    {
        if (vm?.RoomAssignVms == null || vm.RoomAssignVms.Count <= 0) throw new Exception("Sorry! No rooms found to assign!");

        List<HkRoomAssign> removeList = new List<HkRoomAssign>();

        if (vm?.RoomAssignVms?.Count > 0)
        {
            var keeperId = vm.RoomAssignVms?.Select(c => c.HouseKeeperId).FirstOrDefault();

            foreach (var assignRoom in vm.RoomAssignVms)
            {
                var assignedRoom = await _iRepository.GetFirstOrDefaultAsync(c => c.HouseKeeperId == keeperId
                && c.RoomId == assignRoom.RoomId
                && (c.Status == RoomAssignEnum.Assigned || c.Status == RoomAssignEnum.Running), r => r.Room);

                if (assignedRoom == null)
                    throw new Exception("Sorry! No assigned rooms found to remove!");

                removeList.Add(assignedRoom);
            }
        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        if (removeList?.Count > 0)
        {
            _iRepository.RemoveRange(removeList);
        }

        var isAdded = await _iUnitOfWork.CompleteAsync();
        if (!isAdded) { return false; }
        ts.Complete();
        return true;
    }
    #endregion

    #region AssignRooms

    public async Task<DataTablePagination<RoomAssignSearchVm, RoomAssignSearchVm>>
        GetAssignRooms(DataTablePagination<RoomAssignSearchVm, RoomAssignSearchVm> model)
    {
        var dataList = await _iRepository.AssignRooms(model);
        return dataList;
    }

    #endregion

    #region AssignSingleRoom

    public async Task<bool> AssignSingleRoom(SingleRoomAssignVm vm)
    {
        if (vm == null)
            throw new Exception("Information is not correct");

        var model = _iMapper.Map<HkRoomAssign>(vm);
        model.Status = RoomAssignEnum.Assigned;
        model.ActionDate = DateTime.Now;
        model.ActionById = CurrentUserId;

        await _iRepository.AddAsync(model);

        var isAdded = await _iUnitOfWork.CompleteAsync();
        if (!isAdded) { return false; }
        return true;
    }

    #endregion

    #region AssignMultipleRoom

    public async Task<bool> AssignMultipleRoom(List<long> roomIds, int houseKeeperId, bool isClean)
    {
        if (roomIds == null || roomIds.Count == 0)
            throw new Exception("No Room Data Found");

        if (houseKeeperId <= 0)
            throw new Exception("House Keeper Information is not correct");
        var assignList = new List<HkRoomAssign>();
        var taskAssignList = new List<HkTaskAssign>();


        var task = await _iTaskNameRepository.GetFirstOrDefaultAsync(x => x.Name == HKTaskName.RoomCleaning);

        if (task == null)
            throw new Exception("Room Cleaning Task Not Found");

        foreach (var roomId in roomIds)
        {
            var model = new HkRoomAssign
            {
                RoomId = roomId,
                HouseKeeperId = houseKeeperId,
                Status = RoomAssignEnum.Assigned,
                ActionDate = DateTime.Now,
                ActionById = CurrentUserId
            };

            assignList.Add(model);
        }
        if (assignList.Any())
        {
            await _iRepository.AddRangeAsync(assignList);
            await _iUnitOfWork.CompleteAsync();

            foreach (var item in assignList)
            {
                var taskAssignModel = new HkTaskAssign
                {
                    AssignId = item.Id,
                    TaskId = task.Id,
                    AssignDate = Utility.GetBdDateTimeNow(),
                    AssignRemarks = $@"Assigned on {Utility.GetBdDateTimeNow()} ",
                    ActionDate = Utility.GetBdDateTimeNow(),
                    ActionById = CurrentUserId,
                };
                taskAssignList.Add(taskAssignModel);
            }
        }
        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var isAssigned = false;
        var isCleaned = false;
        var isAdded = false;
        if (taskAssignList.Any())
        {
            await _iTaskAssignRepository.AddRangeAsync(taskAssignList);
            isAssigned = await _iUnitOfWork.CompleteAsync();
            isAdded = isAssigned;
        }
        if (isClean == true && isAssigned)
        {
            _iRoomInfoService.CurrentUserId = CurrentUserId;
            isCleaned = await _iRoomInfoService.MultipleRoomStatusUpdate(roomIds, (int?)CleaningStatusEnum.VC);
            isAdded = isAssigned && isCleaned;
        }

        ts.Complete();

        return isAdded;
    }


    #endregion

    #region Cleaning Report
    public async Task<string> GetRoomCleaningReportHtml(RoomCleaningReportVm vm)
    {

        string fullHtml = "";
        List<RoomCleaningReportVm> objDataList = await _iRepository.GetRoomCleaningReportData(vm);

        fullHtml += $@"
                        <div style='text-align:center;margin-bottom:20px;'>
                    
                            <p style='margin:4px 0;color:#555;font-size:14px;'>
                                <b>From:</b> {vm.StrFromDate}  ||  <b>To:</b> {vm.StrToDate}
                            </p>
                            <hr style='border:1px solid #ddd;width:80%;margin:10px auto;' />
                        </div>";
        if (objDataList != null && objDataList.Count > 0)
        {

            fullHtml += "<table class='table report-table table-bordered' id='print_table' style='width:100%; padding-bottom:10px; repeat-header:yes;' border='1'>";
            fullHtml += "<thead>";
            fullHtml += "<tr style='height:30px; background:#f2f2f2;'>";
            fullHtml += "<th style='width:40px; text-align:center;'>SL</th>";
            fullHtml += "<th style='width:120px; text-align:center;'>Room No</th>";
            fullHtml += "<th style='width:160px; text-align:center;'>Housekeeper</th>";
            fullHtml += "<th style='width:120px; text-align:center;'>Assign Date</th>";
            fullHtml += "<th style='width:120px; text-align:center;'>Complete Date</th>";
            fullHtml += "<th style='width:120px; text-align:center;'>Audit Date</th>";
            fullHtml += "<th style='width:100px; text-align:center;'>Status</th>";
            fullHtml += "</tr>";
            fullHtml += "</thead>";
            fullHtml += "<tbody>";

            for (int i = 0; i < objDataList.Count; i++)
            {
                var data = objDataList[i];

                string roomNo = string.IsNullOrEmpty(data.RoomNo) ? "" : $@"Room-{data.RoomNo}";
                string hkName = data.HouseKeeperName ?? "";
                string assignDate = data.AssignDate == null ? "" : data.AssignDate?.ToString("dd/MMM/yyyy");
                string completeDate = data.CompleteDate == null ? "" : data.CompleteDate?.ToString("dd/MMM/yyyy");
                string auditDate = data.AuditDate == null ? null : data.AuditDate?.ToString("dd/MMM/yyyy");
                RoomAssignEnum status = (RoomAssignEnum)data?.Status;

                fullHtml += "<tr>";
                fullHtml += $@"<td style='text-align:center;'>{i + 1}</td>";
                fullHtml += $@"<td style='text-align:center;'>{roomNo}</td>";
                fullHtml += $@"<td style='text-align:left;padding-left:6px;'>{hkName}</td>";
                fullHtml += $@"<td style='text-align:center;'>{assignDate}</td>";
                fullHtml += $@"<td style='text-align:center;'>{completeDate}</td>";
                fullHtml += $@"<td style='text-align:center;'>{auditDate}</td>";
                fullHtml += $@"<td style='text-align:center;'>{status}</td>";
                fullHtml += "</tr>";
            }

            fullHtml += "</tbody>";
            fullHtml += "</table>";
        }
        else
        {
            fullHtml = $"<div style='padding:12px; text-align:center; font-size:0.95rem;'>No records found for the selected criteria</div>";
        }
        fullHtml += $"<div style='text-align:center; font-size:15px; color:#666; margin-top:6px;'>Generated: {DateTime.Today:dd/MMM/yyyy}</div>";
        return fullHtml;
    }

    #endregion
}
