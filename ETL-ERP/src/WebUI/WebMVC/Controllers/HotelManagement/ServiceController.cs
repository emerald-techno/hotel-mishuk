using AutoMapper;
using Domain.Entities.HotelManagement;
using Domain.Utility.Common;
using Domain.ViewModel.HotelManagement.Service;
using Interface.Services.HotelManagement;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;

namespace WebMVC.Controllers.HotelManagement;

public class ServiceController : AppBaseController
{
    #region Config
    private readonly IUnitOfWork _iUnitWork;
    private readonly IHtServiceService _iService;
    private readonly IMapper _iMapper;
    private readonly DropdownService _dropdownService;

    public ServiceController(IUnitOfWork iUnitOfWork,
                            IHtServiceService iService,
                            IMapper iMapper,
                            DropdownService dropdownService) : base(iUnitOfWork)
    {
        _iUnitWork = iUnitOfWork;
        _iService = iService;
        _iMapper = iMapper;
        _dropdownService = dropdownService;
    }
    #endregion

    #region Create
    //[Authorize(Permissions.Services.Create)]
    [Authorize(Permissions.SuperAdmin)]
    public async Task<IActionResult> Create()
    {
        var model = new ServiceVm();
        model.ServiceCode = await _iService.GetServiceCode();
        model.LadgerLookUp = _dropdownService.GetAccLedgerSelectListItems();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ServiceVm modelVm)
    {
        modelVm.LadgerLookUp = _dropdownService.GetAccLedgerSelectListItems();
        try
        {
            if (!ModelState.IsValid)
            {
                SaveFailedMsg("Information is not correct");
                return View("Create", modelVm);
            }
            _iService.CurrentUserId = UserId;
            var model = _iMapper.Map<HtService>(modelVm);
            model.ServiceCode = await _iService.GetServiceCode();
            model.ActionById = UserId;
            model.ActionDate = Domain.Utility.Utility.GetBdDateTimeNow();

            HtService existingService = _iService.GetSingleOrDefault(s => s.ServiceName == modelVm.ServiceName);

            if (existingService != null) { ModelState.AddModelError(string.Empty, "Service Name already exist"); }

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
    //[Authorize(Permissions.Services.Edit)]
    [Authorize(Permissions.SuperAdmin)]
    public IActionResult Edit(long id)
    {
        try
        {
            var data = _iService.GetById(id);
            if (data == null)
            {
                return NotFoundMsg();
            }
            var model = _iMapper.Map<ServiceVm>(data);
            model.LadgerLookUp = _dropdownService.GetAccLedgerSelectListItems();
            return View(model);
        }
        catch (Exception ex)
        {
            ExceptionMsg(ex.Message);
            return View("_404");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ServiceVm modelVm)
    {
        modelVm.LadgerLookUp = _dropdownService.GetAccLedgerSelectListItems();
        try
        {
            if (modelVm.Id <= 0 || !ModelState.IsValid)
                return View(modelVm);

            var model = _iMapper.Map<HtService>(modelVm);
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
    //[Authorize(Permissions.Services.Delete)]
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
    //[Authorize(Permissions.Services.ListView)]
    [Authorize(Permissions.Module.HotelManagementModule)]
    public IActionResult Search()
    {
        var vm = new ServiceSearchVm();
        return View(vm);
    }


    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<ServiceSearchVm, ServiceSearchVm> searchVm = null)
    {
        try
        {
            if (searchVm == null) searchVm = new DataTablePagination<ServiceSearchVm, ServiceSearchVm>();
            if (searchVm?.SearchModel == null) searchVm.SearchModel = new ServiceSearchVm();
            var dataTable = await _iService.SearchAsync(searchVm);
            return dataTable == null ? NotFound() : Ok(dataTable);

        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    #endregion

    #region JsonData

    public async Task<IActionResult> GetComplementaryById(long id)
    {
        var data = await _iService.GetByIdAsync(id);
        if (data == null)
        {
            return BadRequest("Service Not Found..!!!");
        }
        var model = _iMapper.Map<ServiceVm>(data);
        return Ok(model);
    }

    [HttpPost]
    public IActionResult GetComplementaryJsonData()
    {
        var dataList = _dropdownService.GetComplementaryDynamicData();
        return Ok(dataList);
    }

    #endregion

    #region EXISTING CHECK

    [AcceptVerbs("Get", "Post")]
    public async Task<IActionResult> IsNameExist(long? id, string serviceName, string initName)
    {
        if (!string.IsNullOrEmpty(serviceName) && !string.IsNullOrEmpty(initName) && serviceName.ToUpper().Equals(initName.ToUpper()))
        {
            return Json(true);
        }

        var result = await _iService.GetFirstOrDefaultAsync(c => c.ServiceName.Equals(serviceName) && c.Id != id && !c.IsDeleted);
        if (result == null)
        {
            return Json(true);
        }
        else
        {
            return Json($"Name {serviceName} Is Already Exist..!!");
        }

    }
    #endregion
}
