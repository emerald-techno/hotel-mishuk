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

namespace WebMVC.Controllers.Inventory
{
    public class ConsumptionController : AppBaseController
    {
        #region Config

        private readonly IUnitOfWork _iUnitWork;
        private readonly ITranService _iService;
        private readonly DropdownService _dropdownService;
        private readonly IMapper _iMapper;
        private readonly IDepartmentService _iDepartmentService;

        public ConsumptionController(IUnitOfWork iUnitOfWork,
                                ITranService iService,
                                DropdownService dropdownService,
                                IMapper iMapper,
                                IDepartmentService iDepartmentService) : base(iUnitOfWork)
        {
            _iUnitWork = iUnitOfWork;
            _iService = iService;
            _dropdownService = dropdownService;
            _iMapper = iMapper;
            _iDepartmentService = iDepartmentService;
        }

        #endregion

        #region Create

        [HttpGet]
        [Authorize(Permissions.Module.InventoryModule)]
        public async Task<IActionResult> Create()
        {
            try
            {
                var vm = new TransactionVm();
                vm.TranDateStr = DateTime.Now.ToString("dd/MM/yyyy");
                vm.IssueDptLookUp = _dropdownService.GetDepartmentSelectListItems();
                vm.ItemLookUp = _dropdownService.GetItemSelectListItems();
                vm.UnitLookUp = _dropdownService.GetUnitSelectListItems();
                vm.TranType = TransType.Consumption;
                vm.TranNo = await _iService.GetTransAutoCode(vm.TranType);
                return View(vm);
            }
            catch (Exception ex)
            {
                ExceptionMsg(ex.Message);
                return View("Search");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(TransactionVm modelVm)
        {
            modelVm.IssueDptLookUp = _dropdownService.GetDepartmentSelectListItems();
            modelVm.ItemLookUp = _dropdownService.GetItemSelectListItems();
            modelVm.UnitLookUp = _dropdownService.GetUnitSelectListItems();

            try
            {
                if (!ModelState.IsValid)
                {
                    SaveFailedMsg("Information Is Not Correct");
                    return View("Create", modelVm);
                }
                _iService.CurrentUserId = UserId;
                var isAdded = await _iService.AddAsync(modelVm);

                if (!isAdded)
                {
                    SaveFailedMsg();
                    return View("Create", modelVm);
                }

                SaveSuccessMsg();
                return RedirectToAction("Create");
            }
            catch (Exception e)
            {
                ExceptionMsg(e.Message);
                return View("Create", modelVm);
            }
        }

        #endregion

        #region Search

        [HttpGet]
        [Authorize(Permissions.Module.InventoryModule)]
        public IActionResult Search()
        {
            var model = new TransactionSearchVm();
            model.TranType = TransType.Consumption;
            model.IssueDptLookup = _dropdownService.GetDepartmentSelectListItems();
            return View(model);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
        public async Task<IActionResult>
            Search(DataTablePagination<TransactionSearchVm, TransactionSearchVm> searchVm = null)
        {
            if (searchVm == null) searchVm = new DataTablePagination<TransactionSearchVm, TransactionSearchVm>();
            if (searchVm?.SearchModel == null) searchVm.SearchModel = new TransactionSearchVm();
            var dataTable = await _iService.SearchAsync(searchVm);
            return dataTable == null ? NotFound() : Ok(dataTable);
        }

        #endregion

        #region Detail
        //[Authorize(Permissions.Consumption.Detail)]
        [Authorize(Permissions.Module.InventoryModule)]
        public async Task<ActionResult> Details(long id)
        {
            try
            {
                var model = await _iService.GetTransInfoDataAsync(id);
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
}
