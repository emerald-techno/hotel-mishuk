using AutoMapper;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.BillPayment;
using Domain.ViewModel.Inventory.RequsitionInfo;
using Interface.Services.Inventory;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;

namespace WebMVC.Controllers.Inventory;

public class InventoryBillPaymentController : AppBaseController
{
    #region Config
    private readonly IInventoryBillPaymentService _iService;
    private readonly IMapper _iMapper;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly DropdownService _dropdownService;

    public InventoryBillPaymentController(IInventoryBillPaymentService iService, IMapper iMapper,
        IUnitOfWork iUnitOfWork, DropdownService dropdownService)
        : base(iUnitOfWork, "InventoryBillPayment")
    {
        _iService = iService;
        _iMapper = iMapper;
        _iUnitOfWork = iUnitOfWork;
        _dropdownService = dropdownService;
    }
    #endregion

    #region Search
    [HttpGet]
    [Authorize(Permissions.InventoryBillPayment.ListView)]
    public IActionResult Search()
    {
        var vm = new InventoryBillPaymentSearchVm();
        vm.SupplierLookUp = _dropdownService.GetSupplierSelectListItems();
        vm.OrderLookUp = _dropdownService.GetOrderSelectListItems();
        return View(vm);
    }

    [HttpPost]
    [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
    public async Task<IActionResult>
        Search(DataTablePagination<InventoryBillPaymentSearchVm, InventoryBillPaymentSearchVm> searchVm = null)
    {
        if (searchVm == null) searchVm = new DataTablePagination<InventoryBillPaymentSearchVm, InventoryBillPaymentSearchVm>();
        if (searchVm?.SearchModel == null) searchVm.SearchModel = new InventoryBillPaymentSearchVm();
        var dataTable = await _iService.SearchAsync(searchVm);
        return dataTable == null ? NotFound() : Ok(dataTable);
    }
    #endregion

    #region Detail
    [Authorize(Permissions.Issue.Detail)]
    public async Task<ActionResult> Details(long id)
    {
        try
        {
            var model = await _iService.GetBillPaymentDataAsync(id);
            return View(model);
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View("_404");
        }

    }

    #endregion
}
