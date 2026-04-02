using AutoMapper;
using Domain.Entities.Payroll;
using Domain.Utility.Common;
using Domain.ViewModel.Payroll.PrSalaryPart;
using Interface.Services.Payroll;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using DU = Domain.Utility;

namespace WebMVC.Controllers.Payroll;

public class PrSalaryPartController : AppBaseController
{
    #region Config
    private readonly IMapper _iMapper;
    private readonly IPrSalaryPartService _iService;
    private readonly IUnitOfWork _iUnitWork;
    private readonly DropdownService _dropdownService;
    private readonly CacheStoreService _cacheStoreService;

    public PrSalaryPartController(IPrSalaryPartService iService,
                                IMapper iMapper, IUnitOfWork iUnitOfWork, CacheStoreService cacheStoreService,
                                DropdownService dropDownService) : base(iUnitOfWork)
    {
        _iService = iService;
        _iMapper = iMapper;
        _iUnitWork = iUnitOfWork;
        _cacheStoreService = cacheStoreService;
        _dropdownService = dropDownService;
    }
    #endregion

    #region Search

    [HttpGet]
    [Authorize(Permissions.PrSalaryParts.ListView)]
    public IActionResult Search()
    {
        var vm = new PrSalaryPartSearchVm();
        return View(vm);
    }

    [HttpGet]
    [Authorize(Permissions.PrSalaryParts.ListView)]
    public IActionResult SearchPartial()
    {
        var vm = new PrSalaryPartSearchVm();
        return View(vm);
    }

    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<PrSalaryPartSearchVm, PrSalaryPartSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<PrSalaryPartSearchVm, PrSalaryPartSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new PrSalaryPartSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }

    #endregion

    #region Edit

    [HttpGet]
    [Authorize(Permissions.PrSalaryParts.Edit)]
    public IActionResult Edit(long id)
    {
        try
        {
            var data = _iService.GetById(id);
            if (data == null)
            {
                return NotFoundMsg();
            }
            var model = _iMapper.Map<PrSalaryPartVm>(data);
            model.ValueTypeLookUp = _dropdownService.GetValueTypeSelectListItems();
            model.PartTypeLookUp = _dropdownService.GetSalaryPartTypeSelectListItems();
            return View(model);
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("_404");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit(PrSalaryPartVm modelVm)
    {
        try
        {
            if (modelVm.Id <= 0 || !ModelState.IsValid) return View(modelVm);

            var model = _iMapper.Map<PrSalaryPart>(modelVm);

            model.UpdatedById = UserId;
            model.UpdateDate = DU.Utility.GetBdDateTimeNow();
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
}
