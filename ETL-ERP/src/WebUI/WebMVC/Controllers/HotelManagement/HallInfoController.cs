using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.RoomInfo;
using Interface.Services.HotelManagement;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using DU = Domain.Utility;

namespace WebMVC.Controllers.HotelManagement;

public class HallInfoController : AppBaseController
{
    #region Config
    private readonly IUnitOfWork _iUnitWork;
    private readonly IHallInfoService _iService;
    private readonly IMapper _iMapper;
    private readonly DropdownService _iDropdownService;

    public HallInfoController(IUnitOfWork iUnitOfWork,
                            IHallInfoService iService,
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
    [Authorize(Permissions.Module.HotelManagementModule)]
    public IActionResult Create()
  {
        var model = new HtHallInfoVm();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(HtHallInfoVm modelVm)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return View("Create", modelVm);
            }
            _iService.CurrentUserId = UserId;

            var photoFile = modelVm.GetAppFileToUploadFolder();

            var isAdded = await _iService.HallAddAsync(modelVm);
            if (!isAdded)
            {
                SaveFailedMsg();
                return View("Create", modelVm);
            }

            await DU.Utility.UploadFileToFolderAsync(photoFile);

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
    [Authorize(Permissions.Module.HotelManagementModule)]
    public async Task<IActionResult> Edit(long id)
    {
        try
        {
            var data = await _iService.GetByIdAsync(id);
            if (data == null) return NotFoundMsg();

            var model = _iMapper.Map<HtHallInfoVm>(data);

            var photoDoc = DU.Utility.GetBase64ImageStringFromPath(model.PhotoUrl);
            ViewBag.Photo = photoDoc;

            return View(model);
        }

        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("_404");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(HtHallInfoVm modelVm)
    {
        try
        {
            if (modelVm.Id <= 0 || !ModelState.IsValid) return View(modelVm);

            var photoFile = modelVm.GetAppFileToUploadFolder();

            var model = _iMapper.Map<HtHallInfo>(modelVm);

            model.ActionById = UserId;
            model.ActionDate = DU.Utility.GetBdDateTimeNow();
            model.UpdatedById = UserId;
            model.UpdateDate = DU.Utility.GetBdDateTimeNow();

            var isAdded = await _iService.UpdateAsync(model);

            if (!isAdded)
            {
                UpdateFailedMsg();
                return View("Edit", modelVm);
            }
            await DU.Utility.UploadFileToFolderAsync(photoFile);

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
    public IActionResult Search()
    {
        var vm = new HtHallInfoSearchVm();

        return View(vm);
    }


    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<HtHallInfoSearchVm, HtHallInfoSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<HtHallInfoSearchVm, HtHallInfoSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new HtHallInfoSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
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

    #region Details
    //[Authorize(Permissions.RoomInfos.Details)]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public IActionResult Details(long id)
    {
        try
        {
            var data = _iService.GetFirstOrDefault(x => x.Id == id);
            if (data == null)
                throw new Exception("No Hall Found..!");
            var hallModel = _iMapper.Map<HtHallInfoVm>(data);
            return View(hallModel);
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
    public async Task<IActionResult> IsHallNameExist(string hallName, string initHallName)
    {
        if (!string.IsNullOrEmpty(hallName) && !string.IsNullOrEmpty(initHallName) && hallName.ToUpper().Equals(initHallName.ToUpper()))
        {
            return Json(true);
        }

        var result = await _iService.GetFirstOrDefaultAsync(c => c.HallName.Equals(hallName) && !c.IsDeleted);
        if (result == null)
        {
            return Json(true);
        }
        else
        {
            return Json($"Hall Name: {hallName} is already exist..!!");
        }

    }

    #endregion

    #region JsonData
    public async Task<IActionResult> GetHallInfoById(long id)
    {
        var data = await _iService.GetByIdAsync(id);
        if (data == null)
        {
            return BadRequest("Hall Not Found..!!!");
        }
        var model = _iMapper.Map<HtHallInfoVm>(data);
        return Ok(model);
    }

    #endregion
}
