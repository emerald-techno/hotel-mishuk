using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.RoomBooking;
using Domain.ViewModel.HotelManagement.RoomCategory;
using Interface.Services.HotelManagement;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using DU = Domain.Utility;

namespace WebMVC.Controllers.HotelManagement;

public class RoomCategoryController : AppBaseController
{
    #region Config
    private readonly IUnitOfWork _iUnitWork;
    private readonly IRoomCategoryService _iService;
    private readonly IRoomCategoryDiscountMapService _iRoomCategoryDiscountMapService;
    private readonly IMapper _iMapper;
    private readonly DropdownService _dropdownService;

    public RoomCategoryController(IUnitOfWork iUnitOfWork,
                            IRoomCategoryService iService,
                            IMapper iMapper,
                            IRoomCategoryDiscountMapService iRoomCategoryDiscountMapService,
                            DropdownService dropdownService) : base(iUnitOfWork)
    {
        _iUnitWork = iUnitOfWork;
        _iService = iService;
        _iMapper = iMapper;
        _iRoomCategoryDiscountMapService = iRoomCategoryDiscountMapService;
        _dropdownService = dropdownService;
    }
    #endregion

    #region Create
    //[Authorize(Permissions.RoomCategories.Create)]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public IActionResult Create()
    {
        var model = new HtRoomCategoryVm();
        model.BedTypeLookUp = _dropdownService.GetBedTypeSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(HtRoomCategoryVm modelVm)
    {
        modelVm.BedTypeLookUp = _dropdownService.GetBedTypeSelectListItems();

        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return View("Create", modelVm);
            }
            _iService.CurrentUserId = UserId;

            var photoFile = modelVm.GetAppFileToUploadFolder();

            var isAdded = await _iService.RoomCategoryAddAsync(modelVm);

            if (!isAdded)
            {
                SaveFailedMsg();
                return View("Create", modelVm);
            }

            await DU.Utility.UploadFileToFolderAsync(photoFile);

            SaveSuccessMsg();

            return RedirectToAction("Search");
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("Create", modelVm);
        }
    }
    #endregion

    #region Edit
    [HttpGet]
    //[Authorize(Permissions.RoomCategories.Edit)]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public IActionResult Edit(long id)
    {
        try
        {
            var data = _iService.GetById(id);
            if (data == null)
            {
                return NotFoundMsg();
            }
            var model = _iMapper.Map<HtRoomCategoryVm>(data);

            var photoDoc = DU.Utility.GetBase64ImageStringFromPath(model.PhotoUrl);
            ViewBag.Photo = photoDoc;

            model.BedTypeLookUp = _dropdownService.GetBedTypeSelectListItems();

            return View(model);
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("_404");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(HtRoomCategoryVm modelVm)
    {
        modelVm.BedTypeLookUp = _dropdownService.GetBedTypeSelectListItems();

        try
        {
            if (modelVm.Id <= 0 || !ModelState.IsValid)
                return View(modelVm);

            var photoFile = modelVm.GetAppFileToUploadFolder();

            var model = _iMapper.Map<HtRoomCategory>(modelVm);

            model.UpdatedById = UserId;
            model.UpdateDate = DU.Utility.GetBdDateTimeNow();
            var isAdded = await _iService.UpdateAsync(model);
            if (!isAdded)
            {
                UpdateFailedMsg();
                return View("Edit", modelVm);
            }

            await DU.Utility.UploadFileToFolderAsync(photoFile);

            UpdateSuccessMsg();

            return RedirectToAction("Search");
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View(modelVm);
        }
    }
    #endregion

    #region Delete
    [HttpGet]
    //[Authorize(Permissions.RoomCategories.Delete)]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public async Task<IActionResult> Delete(long id)
    {
        var data = await _iService.GetByIdAsync(id);
        if (data == null)
        {
            DeleteFailedMsg();
            return NotFound();
        }
        data.IsDeleted = true;
        DeleteSuccessMsg();
        var isRemove = _iService.Update(data);
        return RedirectToAction("Search");
    }
    #endregion

    #region Search
    [HttpGet]
    //[Authorize(Permissions.RoomCategories.ListView)]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public IActionResult Search()
    {
        var vm = new HtRoomCategorySearchVm();
        return View(vm);
    }


    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<HtRoomCategorySearchVm, HtRoomCategorySearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<HtRoomCategorySearchVm, HtRoomCategorySearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new HtRoomCategorySearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion

    #region Discount Setup
    [HttpGet]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public async Task<IActionResult> DiscountSetup()
    {
        try
        {
            var model = new List<HtRoomCategoryDiscountSetUpVm>();
            return View(model);
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("_404");
        }
    }

    [HttpPost]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public async Task<IActionResult> DiscountSetupPrepareData(DateTime fromDate, DateTime toDate, DiscountTypeEnum discountType, double discountAmount)
    {
        var model = await _iService.GetDiscountedCategories(fromDate, toDate, discountType, discountAmount);
        return PartialView("_DiscountTable", model);
        //return Json(model);
    }

    [HttpPost]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public async Task<bool> SubmitDiscountSetupData(List<HtRoomCategoryDiscountMap> discountSetupList)
     {
        
        if (!ModelState.IsValid)
        {
            SaveFailedMsg("Information Is Not Valid");
        }
        try
        {
            var isAdded =await _iRoomCategoryDiscountMapService.SubmitDiscountSetupDataAsync(discountSetupList);
            return isAdded;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
        
    }

    #endregion

    #region JsonData

    public async Task<IActionResult> GetRoomCatgoryInfoById(long id)
    {
        var data = await _iService.GetByIdAsync(id);
        if (data == null)
        {
            return BadRequest("Room Category Not Found..!!!");
        }
        var model = _iMapper.Map<HtRoomCategoryVm>(data);
        return Ok(model);
    }

    [HttpPost]
    public IActionResult GetRoomCategoryJsonData()
    {
        var dataList = _dropdownService.GetRoomCategoryDynamicData();
        return Ok(dataList);
    }

    #endregion
}
