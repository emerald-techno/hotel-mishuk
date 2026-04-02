using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.RoomFacility;
using Interface.Services.HotelManagement;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;

namespace WebMVC.Controllers.HotelManagement;

public class RoomFacilityController : AppBaseController
{
    #region Config
    private readonly IUnitOfWork _iUnitWork;
    private readonly IRoomFacilityService _iService;
    private readonly IMapper _iMapper;
    private readonly DropdownService _iDropdownService;

    public RoomFacilityController(IUnitOfWork iUnitOfWork,
                            IRoomFacilityService iService,
                            IMapper iMapper,
                            DropdownService dropdownService) : base(iUnitOfWork)
    {
        _iUnitWork = iUnitOfWork;
        _iService = iService;
        _iMapper = iMapper;
        _iDropdownService = dropdownService;
    }
    #endregion

    #region Create
    //[Authorize(Permissions.RoomFacilities.Create)]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public IActionResult Create()
    {
        var model = new HtRoomFacilityVm();
        model.FacilitCategoryLookUp = _iDropdownService.GetRoomFacilityCategorySelectListItems();
        return View(model);
    }

    [HttpPost]
    public IActionResult Create(HtRoomFacilityVm modelVm)
    {
        modelVm.FacilitCategoryLookUp = _iDropdownService.GetRoomFacilityCategorySelectListItems();
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return View("Create", modelVm);
            }
            _iService.CurrentUserId = UserId;
            var model = _iMapper.Map<HtRoomFacility>(modelVm);
            model.ActionById = UserId;
            model.ActionDate = Domain.Utility.Utility.GetBdDateTimeNow();
            model.IsActive = true;

            HtRoomFacility existingRoomFacility = _iService.GetSingleOrDefault(d => d.FacilityName == model.FacilityName);

            if (existingRoomFacility != null)
            {
                ModelState.AddModelError(string.Empty, "Room facility already exists");
            }
            else
            {
                var isAdded = _iService.Add(model);
                if (!isAdded)
                {
                    SaveFailedMsg();
                    return View("Create", modelVm);
                }
                SaveSuccessMsg();
            }
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
    //[Authorize(Permissions.RoomFacilities.Edit)]
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
            var model = _iMapper.Map<HtRoomFacilityVm>(data);
            model.FacilitCategoryLookUp = _iDropdownService.GetRoomFacilityCategorySelectListItems();
            return View(model);
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("_404");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(HtRoomFacilityVm modelVm)
    {
        modelVm.FacilitCategoryLookUp = _iDropdownService.GetRoomFacilityCategorySelectListItems();
        try
        {
            if (modelVm.Id <= 0 || !ModelState.IsValid)
                return View(modelVm);

            var model = _iMapper.Map<HtRoomFacility>(modelVm);
            model.ActionById = UserId;
            model.ActionDate = Domain.Utility.Utility.GetBdDateTimeNow();
            model.UpdatedById = UserId;
            model.UpdateDate = Domain.Utility.Utility.GetBdDateTimeNow();
            var isAdded = await _iService.UpdateAsync(model);
            if (!isAdded)
            {
                UpdateFailedMsg();
                return View("Edit", modelVm);
            }
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
    //[Authorize(Permissions.RoomFacilities.Delete)]
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
    //[Authorize(Permissions.RoomFacilities.ListView)]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public IActionResult Search()
    {
        var vm = new HtRoomFacilitySearchVm();
        return View(vm);
    }


    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<HtRoomFacilitySearchVm, HtRoomFacilitySearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<HtRoomFacilitySearchVm, HtRoomFacilitySearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new HtRoomFacilitySearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion

    #region EXISTING CHECK

    [AcceptVerbs("Get", "Post")]
    public async Task<IActionResult> IsNameExist(string name, string initName)
    {
        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(initName) && name.ToUpper().Equals(initName.ToUpper()))
        {
            return Json(true);
        }

        var result = await _iService.GetFirstOrDefaultAsync(c => c.FacilityName.Equals(name) && !c.IsDeleted);
        if (result == null)
        {
            return Json(true);
        }
        else
        {
            return Json($"Name {name} Is Already Exist..!!");
        }

    }
    #endregion
}