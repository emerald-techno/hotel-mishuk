using AutoMapper;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.Transaction;
using Interface.Services.Admin;
using Interface.Services.Inventory;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;

namespace WebMVC.Controllers.Inventory;

public class InventoryOpeningController : AppBaseController
{
    #region Config

    private readonly IUnitOfWork _iUnitWork;
    private readonly ITranService _iService;
    private readonly DropdownService _dropdownService;
    private readonly IDepartmentService _iDepartmentService;
    private readonly IMapper _iMapper;

    public InventoryOpeningController(IUnitOfWork iUnitOfWork,
                            ITranService iService,
                            DropdownService dropdownService,
                            IDepartmentService iDepartmentService,
                            IMapper iMapper) : base(iUnitOfWork)
    {
        _iUnitWork = iUnitOfWork;
        _iService = iService;
        _dropdownService = dropdownService;
        _iDepartmentService = iDepartmentService;
        _iMapper = iMapper;
    }

    #endregion

    #region Create

    [HttpGet]
    //[Authorize(Permissions.InventoryOpening.CreateOrEdit)]
    [Authorize(Permissions.Module.InventoryModule)]
    public IActionResult CreateOrEdit()
    {
        var vm = new TransactionVm();
        vm.TranType = TransType.Opening;
        vm.TranDateStr = DateTime.Now.ToString("dd/MM/yyyy");
        vm.ItemLookUp = _dropdownService.GetItemSelectListItems();
        vm.UnitLookUp = _dropdownService.GetUnitSelectListItems();
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrEdit(TransactionVm modelVm)
    {
        modelVm.ItemLookUp = _dropdownService.GetItemSelectListItems();
        modelVm.UnitLookUp = _dropdownService.GetUnitSelectListItems();

        try
        {
            //if (!ModelState.IsValid)
            //{
            //    SaveFailedMsg("Information Is Not Correct");
            //    return View("CreateOrEdit", modelVm);
            //}
            _iService.CurrentUserId = UserId;

            long isAddOrUpdate = 0;

            if (modelVm.IsOpenignUpdate)
            {
                isAddOrUpdate = await _iService.UpdateOpening(modelVm);
            }
            else
            {
                isAddOrUpdate = await _iService.AddOpening(modelVm);
            }

            if (isAddOrUpdate == 0)
            {
                SaveFailedMsg();
                return View("CreateOrEdit", modelVm);
            }

            SaveSuccessMsg();
            return RedirectToAction("CreateOrEdit");
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("CreateOrEdit", modelVm);
        }
    }

    #endregion

    #region HkOpening

    [HttpGet]
    //[Authorize(Permissions.InventoryOpening.CreateOrEdit)]
    [Authorize(Permissions.Module.HouseKeepingModule)]
    public async Task<IActionResult> HkCreateOrEdit()
    {
        var vm = new TransactionVm();

        var housekeeperDpt = await _iDepartmentService.GetFirstOrDefaultAsync(x => x.Code == DepartmentCode.HouseKeeper);
        if (housekeeperDpt == null)
            throw new Exception("HouserKeeper Department Entry Not Found..!!");

        vm.IssueDeptId = housekeeperDpt.Id;
        vm.TranType = TransType.Opening;
        vm.TranDateStr = DateTime.Now.ToString("dd/MM/yyyy");
        vm.ItemLookUp = _dropdownService.GetItemSelectListItems();
        vm.UnitLookUp = _dropdownService.GetUnitSelectListItems();
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> HkCreateOrEdit(TransactionVm modelVm)
    {
        modelVm.ItemLookUp = _dropdownService.GetItemSelectListItems();
        modelVm.UnitLookUp = _dropdownService.GetUnitSelectListItems();

        try
        {
            //if (!ModelState.IsValid)
            //{
            //    SaveFailedMsg("Information Is Not Correct");
            //    return View("CreateOrEdit", modelVm);
            //}
            _iService.CurrentUserId = UserId;

            long isAddOrUpdate = 0;

            if (modelVm.IsOpenignUpdate)
            {
                isAddOrUpdate = await _iService.UpdateOpening(modelVm);
            }
            else
            {
                isAddOrUpdate = await _iService.AddOpening(modelVm);
            }

            if (isAddOrUpdate == 0)
            {
                SaveFailedMsg();
                return View("HkCreateOrEdit", modelVm);
            }

            SaveSuccessMsg();
            return RedirectToAction("HkCreateOrEdit");
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("HkCreateOrEdit", modelVm);
        }
    }

    #endregion

    #region RsOpening

    [HttpGet]
    //[Authorize(Permissions.InventoryOpening.CreateOrEdit)]
    [Authorize(Permissions.Module.RestaurantModule)]
    public async Task<IActionResult> RsCreateOrEditAsync()
    {
        var vm = new TransactionVm();

        var resturantDpt = await _iDepartmentService.GetFirstOrDefaultAsync(x => x.Code == DepartmentCode.Resturant);
        if (resturantDpt == null)
            throw new Exception("Restaurant Department Entry Not Found..!!");

        vm.IssueDeptId = resturantDpt.Id;
        vm.TranType = TransType.Opening;
        vm.TranDateStr = DateTime.Now.ToString("dd/MM/yyyy");
        vm.ItemLookUp = _dropdownService.GetItemSelectListItems();
        vm.UnitLookUp = _dropdownService.GetUnitSelectListItems();
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> RsCreateOrEdit(TransactionVm modelVm)
    {
        modelVm.ItemLookUp = _dropdownService.GetItemSelectListItems();
        modelVm.UnitLookUp = _dropdownService.GetUnitSelectListItems();

        try
        {
            //if (!ModelState.IsValid)
            //{
            //    SaveFailedMsg("Information Is Not Correct");
            //    return View("CreateOrEdit", modelVm);
            //}
            _iService.CurrentUserId = UserId;

            long isAddOrUpdate = 0;

            if (modelVm.IsOpenignUpdate)
            {
                isAddOrUpdate = await _iService.UpdateOpening(modelVm);
            }
            else
            {
                isAddOrUpdate = await _iService.AddOpening(modelVm);
            }

            if (isAddOrUpdate == 0)
            {
                SaveFailedMsg();
                return View("RsCreateOrEdit", modelVm);
            }

            SaveSuccessMsg();
            return RedirectToAction("RsCreateOrEdit");
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("RsCreateOrEdit", modelVm);
        }
    }

    #endregion

    #region JsonData

    public async Task<IActionResult> GetInventoryOpenignData()
    {
        try
        {
            var result = await _iService.GetInventoryOpeningDataAsync();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }

    public async Task<IActionResult> GetInventoryOpenignDataByDeptId(long dptId)
    {
        try
        {
            var result = await _iService.GetInventoryOpeningDataByDeptIdAsync(dptId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }

    #endregion
}
