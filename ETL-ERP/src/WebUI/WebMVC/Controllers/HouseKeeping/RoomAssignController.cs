using AutoMapper;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.HotelReport;
using Domain.ViewModel.HotelManagement.RoomInfo;
using Domain.ViewModel.HouseKeeping.Reports;
using Domain.ViewModel.HouseKeeping.RoomAssign;
using Interface.Services.HotelManagement;
using Interface.Services.HouseKeeping;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using Utility.Export;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using DU = Domain.Utility;

namespace WebMVC.Controllers.HouseKeeping;

public class RoomAssignController : AppBaseController
{
    #region Config
    private readonly IUnitOfWork _iUnitWork;
    private readonly IRoomAssignService _iService;
    private readonly IRoomInfoService _iRoomService;
    private readonly IMapper _iMapper;
    private readonly DropdownService _dropdownService;
    private readonly IHttpContextAccessor _iHttpContextAccessor;

    public RoomAssignController(IUnitOfWork iUnitOfWork,
                            IRoomAssignService iService,
                            IMapper iMapper,
                            DropdownService dropdownService,
                            IRoomInfoService iRoomService,
                            IHttpContextAccessor iHttpContextAccessor) : base(iUnitOfWork)
    {
        _iUnitWork = iUnitOfWork;
        _iService = iService;
        _iMapper = iMapper;
        _dropdownService = dropdownService;
        _iRoomService = iRoomService;
        _iHttpContextAccessor = iHttpContextAccessor;
    }
    #endregion

    #region AssignRoom
    //[Authorize(Permissions.RoomAssigns.AssignRoom)]
    [Authorize(Permissions.Module.HouseKeepingModule)]
    public IActionResult AssignRoom()
    {
        var model = new RoomAssignVm();
        model.EmployeeLookUp = _dropdownService.GetEmployeeSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AssignRoom(RoomAssignVm modelVm)
    {
        try
        {
            _iService.CurrentUserId = UserId;
            var isAdded = await _iService.AddAssignRoom(modelVm);
            return Ok(isAdded);
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                message = "Entry Failed..!!",
                errors = e.Message
            });
        }
    }

    #endregion

    #region UnassignRoom
    //[Authorize(Permissions.RoomAssigns.UnassignRoom)]
    [Authorize(Permissions.Module.HouseKeepingModule)]
    [HttpPost]
    public async Task<IActionResult> UnassignRoom(RoomAssignVm modelVm)
    {
        try
        {
            _iService.CurrentUserId = UserId;
            var isAdded = await _iService.RemoveAssignRoom(modelVm);
            return Ok(isAdded);
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                message = "Entry Failed..!!",
                errors = e.Message
            });
        }
    }

    #endregion

    #region AssignSingleRoom
    //[Authorize(Permissions.RoomAssigns.AssignSingleRoom)]
    [Authorize(Permissions.Module.HouseKeepingModule)]
    public async Task<IActionResult> AssignSingleRoom(SingleRoomAssignVm vm)
    {
        try
        {
            _iService.CurrentUserId = UserId;
            var isAdded = await _iService.AssignSingleRoom(vm);
            return Ok(isAdded);
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                message = "Entry Failed..!!",
                errors = e.Message
            });
        }
    }

    #endregion

    #region UnassignSingleRoom
    //[Authorize(Permissions.RoomAssigns.UnassignSingleRoom)]
    [Authorize(Permissions.Module.HouseKeepingModule)]
    public async Task<IActionResult> UnassignSingleRoom(long id)
    {
        var data = await _iService.GetByIdAsync(id);
        if (data == null)
        {
            DeleteFailedMsg();
            return NotFound();
        }
        data.IsDeleted = true;
        DeleteSuccessMsg();
        _iService.Remove(data);
        var isRemove = _iUnitWork.Complete();
        return RedirectToAction("RoomView");
    }

    #endregion

    #region CleanStatusChange
    //[Authorize(Permissions.RoomAssigns.CleaningStatusChange)]
    [Authorize(Permissions.Module.HouseKeepingModule)]
    public async Task<IActionResult> CleanStatusChange(long roomId, int cleanStatus)
    {
        try
        {
            _iRoomService.CurrentUserId = UserId;
            var isUpdate = await _iRoomService.RoomCleanStatusUpdate(roomId, cleanStatus);
            return Ok(isUpdate);
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                message = "Change Failed..!!",
                errors = e.Message
            });
        }
    }

    #endregion

    #region AvailabilityStatusChange
    [HttpPost]
    //[Authorize(Permissions.RoomAssigns.AvailabilityStatusChange)]
    [Authorize(Permissions.Module.HouseKeepingModule)]
    public async Task<IActionResult> AvailabilityStatusChange(long roomId, int availabilityStatus)
    {
        try
        {
            _iRoomService.CurrentUserId = UserId;
            var isUpdate = await _iRoomService.RoomAvailabilityStatusUpdate(roomId, availabilityStatus);
            return Ok(isUpdate);
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                message = "Change Failed..!!",
                errors = e.Message
            });
        }
    }

    #endregion

    #region RoomView
    [HttpGet]
    //[Authorize(Permissions.RoomAssigns.ListView)]
    [Authorize(Permissions.Module.HouseKeepingModule)]
    public IActionResult RoomView()
    {
        var vm = new HtRoomInfoSearchVm();
        vm.RoomCategoryLookUp = _dropdownService.GetRoomCategorySelectListItems();
        vm.AvailabilityStatusLookUp = _dropdownService.GetRoomAvailabilityStatusSelectListItems();
        vm.CleanStatusLookUp = _dropdownService.GetCleanStatusSelectListItems();
        vm.HouseKeeperLookUp = _dropdownService.GetEmployeeSelectListItems();
        return View(vm);
    }

    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        RoomViewSearch(DataTablePagination<HtRoomInfoSearchVm, HtRoomInfoSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<HtRoomInfoSearchVm, HtRoomInfoSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new HtRoomInfoSearchVm();
        var dataTable = await _iRoomService.HouseKeeperViewSearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion

    #region GetAssignRooms
    //[Authorize(Permissions.RoomAssigns.GetAssignRooms)]
    [Authorize(Permissions.Module.HouseKeepingModule)]
    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
            GetAssignRooms(DataTablePagination<RoomAssignSearchVm, RoomAssignSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<RoomAssignSearchVm, RoomAssignSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new RoomAssignSearchVm();
        var dataTable = await _iService.GetAssignRooms(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }
    #endregion

    #region HkRoomViewPrint

    public async Task<ActionResult> HkRoomViewPrint(long? roomCategoryId, short? houseKeeperStatus, short? cleaningStatus)
    {
        var searchVm = new DataTablePagination<HtRoomInfoSearchVm, HtRoomInfoSearchVm>();
        searchVm.SearchModel = new HtRoomInfoSearchVm();

        searchVm.SearchModel.RoomCategoryId = roomCategoryId ?? 0;
        searchVm.SearchModel.HouseKeeperStatus = houseKeeperStatus;
        searchVm.SearchModel.CleaningStatus = cleaningStatus;

        var html = await _iRoomService.HkRoomViewPrintHtml(searchVm);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "HouseKepper Room View Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");

        string reportName = "HouseKepper Room View Report_" + DateTime.Now.ToString("dd/MM/yyyy");

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 50, top: 80, left: 10, right: 10, isLandScape: false), "application/pdf");
    }

    #endregion

    #region MultipleRoomStatusUpdate
    [HttpPost]
    //[Authorize(Permissions.RoomAssigns.AvailabilityStatusChange)]
    [Authorize(Permissions.Module.HouseKeepingModule)]
    public async Task<IActionResult> MultipleRoomStatusUpdate(List<long> roomIds, int? availabilityStatus, int? cleanStatus)
    {
        try
        {
            _iRoomService.CurrentUserId = UserId;
            var isUpdate = await _iRoomService.MultipleRoomStatusUpdate(roomIds, cleanStatus);
            return Ok(isUpdate);
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                message = "Change Failed..!!",
                errors = e.Message
            });
        }
    }

    #endregion

    #region AssignMultipleRoom

    [Authorize(Permissions.Module.HouseKeepingModule)]
    public async Task<IActionResult> AssignMultipleRoom(List<long> roomIds, int houseKeeperId, bool isClean)
    {
        try
        {
            _iService.CurrentUserId = UserId;
            var isAdded = await _iService.AssignMultipleRoom(roomIds: roomIds, houseKeeperId: houseKeeperId, isClean : isClean);
            return Ok(isAdded);
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                message = "Entry Failed..!!",
                errors = e.Message
            });
        }
    }

    #endregion

    #region MultipleRoomMakeOOO
    [HttpPost]
    //[Authorize(Permissions.RoomAssigns.AvailabilityStatusChange)]
    [Authorize(Permissions.Module.HouseKeepingModule)]
    public async Task<IActionResult> MultipleRoomMakeOOO(List<long> roomIds, string remarks)
    {
        try
        {
            _iRoomService.CurrentUserId = UserId;
            var isUpdate = await _iRoomService.MultipleRoomMakeOOO(roomIds, remarks);
            return Ok(isUpdate);
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                message = "Change Failed..!!",
                errors = e.Message
            });
        }
    }

    #endregion

    #region MultipleRoomMakeAvailable
    [HttpPost]
    //[Authorize(Permissions.RoomAssigns.AvailabilityStatusChange)]
    [Authorize(Permissions.Module.HouseKeepingModule)]
    public async Task<IActionResult> MultipleRoomMakeAvailable(List<long> roomIds)
    {
        try
        {
            _iRoomService.CurrentUserId = UserId;
            var isUpdate = await _iRoomService.MultipleRoomMakeAvailable(roomIds);
            return Ok(isUpdate);
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                message = "Change Failed..!!",
                errors = e.Message
            });
        }
    }

    #endregion

    #region RoomCleaningReport
    [HttpGet]
    public ActionResult RoomCleaningReport()
    {
        var today = DateTime.Today;
        var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        var model = new RoomCleaningReportVm
        {
            StrFromDate = startDate.ToString("dd/MM/yyyy"),
            StrToDate = today.ToString("dd/MM/yyyy"),
            HouseKeeperLookup = _dropdownService.GetEmployeeSelectListItems(),
            StatusLookup = _dropdownService.GetHKRoomAssignStatusSelectListItems()
        };
        return View(model);
    }

    [HttpPost]
    public async Task<ActionResult> RoomCleaningReport(RoomCleaningReportVm model)
    {
        try
        {
            var data = await _iService.GetRoomCleaningReportHtml(model);
            return Ok(data);

        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }

    public async Task<ActionResult> RoomCleaningReportPrint(string strFromDate,string strToDate, string roomNo, int? status, long? houseKeeperId)
    {
        var model = new RoomCleaningReportVm
        {
            StrFromDate = strFromDate,
            StrToDate = strToDate,
            HouseKeeperId = houseKeeperId,
            RoomNo = roomNo,
            Status = status
        };

        string html = await _iService.GetRoomCleaningReportHtml(model);

        ExportToPDF export = new ExportToPDF(_iHttpContextAccessor);
        ExportDataTitle reportTitle = new ExportDataTitle();
        reportTitle.Header = PrintInfo.CompanyName;
        reportTitle.AddressOne = PrintInfo.CompanyAddress;
        reportTitle.ReportTitle = "Room Cleaning Report";
        reportTitle.SignatureList = new List<string>();
        reportTitle.SignatureList.Add("Sign One");
        reportTitle.SignatureList.Add("Sign Two");
        reportTitle.SignatureList.Add("Sign Three");
        reportTitle.IsSignature = false;

        string reportName = "Room Cleaning Report";

        return File(export.ExportContentToPdf(html, reportName, reportTitle: reportTitle, bottom: 60, top: 80, left: 18, right: 18, isLandScape: true, width: 1500), "application/pdf");

    }
    #endregion

}