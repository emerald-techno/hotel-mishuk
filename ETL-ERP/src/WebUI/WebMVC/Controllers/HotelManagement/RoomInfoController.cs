using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Enums.AppEnums;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.RoomInfo;
using Domain.ViewModel.HouseKeeping.RoomAssign;
using Interface.Services.HotelManagement;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using DU = Domain.Utility;

namespace WebMVC.Controllers.HotelManagement;

public class RoomInfoController : AppBaseController
{
    #region Config
    private readonly IUnitOfWork _iUnitWork;
    private readonly IRoomInfoService _iService;
    private readonly IMapper _iMapper;
    private readonly DropdownService _iDropdownService;
    private readonly IRoomFacilityCategoryService _iFacilityCategoryService;

    public RoomInfoController(IUnitOfWork iUnitOfWork,
                            IRoomInfoService iService,
                            IMapper iMapper,
                            DropdownService dropdownService, IRoomFacilityCategoryService iFacilityCategoryService) : base(iUnitOfWork)
    {
        _iUnitWork = iUnitOfWork;
        _iService = iService;
        _iMapper = iMapper;
        _iDropdownService = dropdownService;
        _iFacilityCategoryService = iFacilityCategoryService;
    }
    #endregion

    #region Create
    //[Authorize(Permissions.RoomInfos.Create)]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public async Task<IActionResult> Create()
    {
        var model = new HtRoomInfoVm();
        model.RoomCategoryLookUp = _iDropdownService.GetRoomCategorySelectListItems();
        model.FloorLookUp = _iDropdownService.GetFloorSelectListItems();
        model.BedTypeLookUp = _iDropdownService.GetBedTypeSelectListItems();
        model.RoomFacilityLookup = await _iFacilityCategoryService.GetCategoryWithFacility();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(HtRoomInfoVm modelVm)
    {
        modelVm.RoomCategoryLookUp = _iDropdownService.GetRoomCategorySelectListItems();
        modelVm.FloorLookUp = _iDropdownService.GetFloorSelectListItems();
        modelVm.BedTypeLookUp = _iDropdownService.GetBedTypeSelectListItems();
        modelVm.RoomFacilityLookup = await _iFacilityCategoryService.GetCategoryWithFacility();

        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return View("Create", modelVm);
            }
            _iService.CurrentUserId = UserId;

            var isAdded = await _iService.RoomAddAsync(modelVm);
            if (!isAdded)
            {
                SaveFailedMsg();
                return View("Create", modelVm);
            }

            SaveSuccessMsg();
            return RedirectToAction("Create");
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("Create", modelVm);
        }
    }
    #endregion

    #region Edit
    //[Authorize(Permissions.RoomInfos.Edit)]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public async Task<IActionResult> Edit(long id)
    {
        try
        {
            var data = await _iService.GetByIdAsync(id);
            if (data == null) return NotFoundMsg();

            var model = _iMapper.Map<HtRoomInfoVm>(data);

            model.RoomCategoryLookUp = _iDropdownService.GetRoomCategorySelectListItems();
            model.FloorLookUp = _iDropdownService.GetFloorSelectListItems();
            model.BedTypeLookUp = _iDropdownService.GetBedTypeSelectListItems();
            model.RoomFacilityLookup = await _iFacilityCategoryService.GetCategoryWithFacility();

            return View(model);
        }

        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("_404");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(HtRoomInfoVm modelVm)
    {
        modelVm.RoomCategoryLookUp = _iDropdownService.GetRoomCategorySelectListItems();
        modelVm.FloorLookUp = _iDropdownService.GetFloorSelectListItems();
        modelVm.BedTypeLookUp = _iDropdownService.GetBedTypeSelectListItems();
        modelVm.RoomFacilityLookup = await _iFacilityCategoryService.GetCategoryWithFacility();

        try
        {
            if (modelVm.Id <= 0 || !ModelState.IsValid)
                return View(modelVm);

            _iService.CurrentUserId = UserId;
            var isAdded = await _iService.RoomUpdateAsync(modelVm);

            if (!isAdded)
            {
                UpdateFailedMsg();
                return View("Edit", modelVm);
            }

            SaveSuccessMsg();
            return RedirectToAction("Search");
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("Edit", modelVm);
        }
    }
    #endregion

    #region Search
    [HttpGet]
    //[Authorize(Permissions.RoomInfos.ListView)]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public async Task<IActionResult> Search()
    {
        try
        {
            var vm = new HtRoomInfoSearchVm();
            vm.RoomCategoryLookUp = _iDropdownService.GetRoomCategorySelectListItems();
            vm.FloorLookUp = _iDropdownService.GetFloorSelectListItems();
            vm.BedTypeLookUp = _iDropdownService.GetBedTypeSelectListItems();
            vm.BookedStatusLookUp = _iDropdownService.GetRoomBookingStatusSelectListItems();
            vm.CleanStatusLookUp = _iDropdownService.GetCleanStatusSelectListItems();

            var roomList = await _iService.GetAsync(x => x.IsActive && !x.IsDeleted);
            vm.BookingCount = roomList.Where(x => x.BookingStatus == BookingStatusEnum.Booked).Count();
            vm.AvailableCount = roomList.Where(x => x.BookingStatus == BookingStatusEnum.Available).Count();
            vm.VcCount = roomList.Where(x => x.CleaningStatus == CleaningStatusEnum.VC).Count();
            vm.OccCount = roomList.Where(x => x.CleaningStatus == CleaningStatusEnum.O).Count();
            vm.VdCount = roomList.Where(x => x.CleaningStatus == CleaningStatusEnum.VD).Count();
            vm.CoCount = roomList.Where(x => x.CleaningStatus == CleaningStatusEnum.CO).Count();

            return View(vm);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<HtRoomInfoSearchVm, HtRoomInfoSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<HtRoomInfoSearchVm, HtRoomInfoSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new HtRoomInfoSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion

    #region Room Assign
    //[Authorize(Permissions.RoomInfos.RoomAssign)]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public IActionResult RoomAssign()
    {
        var vm = new RoomAssignVm();
        vm.EmployeeLookUp = _iDropdownService.GetEmployeeSelectListItems();
        return View();
    }
    #endregion

    #region Details
    //[Authorize(Permissions.RoomInfos.Details)]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public IActionResult Details(long id)
    {
        try
        {
            var data = _iService.GetFirstOrDefault(x => x.Id == id, c => c.RoomCategory);
            if (data == null)
                throw new Exception("No Room Found..!");
            var roomModel = _iMapper.Map<HtRoomInfoVm>(data);
            roomModel.RoomCategoryName = data.RoomCategory.CategoryName;
            roomModel.PhotoUrl = data.RoomCategory.PhotoUrl;
            return View(roomModel);
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("_404");
        }
    }

    #endregion

    #region EXISTING CHECK

    [AcceptVerbs("Get", "Post")]
    public async Task<IActionResult> IsRoomNoExist(string roomNo, string initRoomNo)
    {
        if (!string.IsNullOrEmpty(roomNo) && !string.IsNullOrEmpty(initRoomNo) && roomNo.ToUpper().Equals(initRoomNo.ToUpper()))
        {
            return Json(true);
        }

        var result = await _iService.GetFirstOrDefaultAsync(c => c.RoomNo.Equals(roomNo) && !c.IsDeleted);
        if (result == null)
        {
            return Json(true);
        }
        else
        {
            return Json($"Room Number {roomNo} is already exist..!!");
        }

    }

    #endregion

    #region JsonData
    public async Task<IActionResult> GetRoomInfoById(long id)
    {
        var data = await _iService.GetByIdAsync(id);
        if (data == null)
        {
            return BadRequest("Room Not Found..!!!");
        }
        var model = _iMapper.Map<HtRoomInfoVm>(data);
        return Ok(model);
    }

    public async Task<IActionResult> GetRoomByCategoryId(long categoryId, string checkInDate, string checkOutDate)
    {
        var cInDate = (DateTime)(!string.IsNullOrEmpty(checkInDate) ? DU.Utility.ConvertStrToDate(checkInDate) : DateTime.Now);
        var cOutDate = (DateTime)(!string.IsNullOrEmpty(checkOutDate) ? DU.Utility.ConvertStrToDate(checkOutDate) : DateTime.Now);
        var data = await _iService.GetAvailableRoomByCategoryId(categoryId, cInDate, cOutDate);
        return Ok(data);
    }

    public async Task<IActionResult> GetRoomByDateRange(string checkInDate, string checkOutDate)
    {
        var cInDate = (DateTime)(!string.IsNullOrEmpty(checkInDate) ? DU.Utility.ConvertStrToDate(checkInDate) : DateTime.Now);
        var cOutDate = (DateTime)(!string.IsNullOrEmpty(checkOutDate) ? DU.Utility.ConvertStrToDate(checkOutDate) : DateTime.Now);
        var data = await _iService.GetAvailableRoomByDateRange(cInDate, cOutDate);
        return Ok(data);
    }

    public async Task<IActionResult> GetDayWiseRoomInfo(long roomId, string queryDateStr)
    {
        var queryDate = (DateTime)(!string.IsNullOrEmpty(queryDateStr) ? DU.Utility.ConvertStrToDate(queryDateStr) : DateTime.Now);
        var data = await _iService.GetDayWiseRoomInfo(roomId, queryDate);
        return Ok(data);
    }

    [HttpPost]
    public IActionResult GetRoomJsonData()
    {
        var dataList = _iDropdownService.GetRoomDynamicData();
        return Ok(dataList);
    }

    public async Task<IActionResult> GetAvailableRooms(long categoryId, string checkInDate, string checkOutDate)
    {
        var cInDate = (DateTime)(!string.IsNullOrEmpty(checkInDate) ? DU.Utility.ConvertStrToDate(checkInDate) : DateTime.Now);
        var cOutDate = (DateTime)(!string.IsNullOrEmpty(checkOutDate) ? DU.Utility.ConvertStrToDate(checkOutDate) : DateTime.Now);
        var data = await _iService.GetAvailableRooms(cInDate, cOutDate, categoryId);
        return Ok(data);
    }

    #endregion
}
