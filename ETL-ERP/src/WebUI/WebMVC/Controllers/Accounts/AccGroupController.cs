using AutoMapper;
using Domain.Entities.Accounting;
using Domain.Utility.Common;
using Domain.ViewModel.Accounting.AccGroup;
using Domain.ViewModel.Accounting.ChartOfAcc;
using Interface.Services.Accounts;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;

namespace WebMVC.Controllers.Accounts
{
    public class AccGroupController : AppBaseController
    {
        #region Config
        private readonly IUnitOfWork _iUnitOfWork;
        private readonly IMapper _iMapper;
        private readonly IAccGroupService _iAccGroupService;
        private readonly IAccLedgerService _iLedgerService;
        private readonly DropdownService _dropdownService;
        public AccGroupController(IUnitOfWork iUnitOfWork,
                                        IAccGroupService iAccGroupService,
                                        IMapper iMapper,
                                        IAccLedgerService iLedgerService,
                                        DropdownService dropdownService) : base(iUnitOfWork)
        {
            _iUnitOfWork = iUnitOfWork;
            _iAccGroupService = iAccGroupService;
            _iMapper = iMapper;
            _dropdownService = dropdownService;
            _iLedgerService = iLedgerService;
        }
        #endregion

        #region Create
        [HttpGet]
        [Authorize(Permissions.AccGroups.Create)]
        public IActionResult Create()
        {
            var model = new AccGroupVm();
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(AccGroupVm modelVm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    SaveFailedMsg("Information Is Not Correct");
                    return View("Create", modelVm);
                }
                _iAccGroupService.CurrentUserId = UserId;
                var model = _iMapper.Map<AccGroup>(modelVm);
                model.ActionById = UserId;
                model.ActionDate = Domain.Utility.Utility.GetBdDateTimeNow();

                var isAdded = _iAccGroupService.Add(model);

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

        #region Edit
        [HttpGet]
        [Authorize(Permissions.AccGroups.Edit)]
        public async Task<IActionResult> Edit(long id)
        {
            try
            {
                var data = await _iAccGroupService.GetByIdAsync(id);
                if (data == null)
                {
                    return NotFoundMsg();
                }
                var model = _iMapper.Map<AccGroupVm>(data);
                return View(model);
            }
            catch (Exception e)
            {
                ExceptionMsg(e.Message);
                return View("_404");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AccGroupVm modelVm)
        {

            try
            {
                if (modelVm.Id <= 0 || !ModelState.IsValid) return View(modelVm);
                var model = _iMapper.Map<AccGroup>(modelVm);

                model.ActionById = UserId;
                model.ActionDate = Domain.Utility.Utility.GetBdDateTimeNow();
                model.UpdatedById = UserId;
                model.UpdateDate = Domain.Utility.Utility.GetBdDateTimeNow();
                var isAdded = await _iAccGroupService.UpdateAsync(model);
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

        #region Search
        [HttpGet]
        [Authorize(Permissions.AccGroups.ListView)]
        public IActionResult Search()
        {
            var vm = new AccGroupSearchVm();
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> ChartOfAcc()
        {
            try
            {
                var vm = new ChartOfAccVm();
                var data = await _iAccGroupService.GetChartOfAcc();
                return View(data);
            }
            catch (Exception ex)
            {
                throw;
            }

        }


        [HttpPost]
        [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
        public async Task<IActionResult>
            Search(DataTablePagination<AccGroupSearchVm, AccGroupSearchVm> searchVm = null)
        {
            if (searchVm == null) searchVm = new DataTablePagination<AccGroupSearchVm, AccGroupSearchVm>();
            if (searchVm?.SearchModel == null) searchVm.SearchModel = new AccGroupSearchVm();
            var dataTable = await _iAccGroupService.SearchAsync(searchVm);
            return dataTable == null ? NotFound() : Ok(dataTable);
        }
        #endregion

        #region Delete
        [HttpGet]
        [Authorize(Permissions.AccGroups.Delete)]
        public async Task<IActionResult> Delete(long id)
        {
            var data = await _iAccGroupService.GetByIdAsync(id);
            if (data == null)
            {
                DeleteFailedMsg();
                return NotFound();
            }
            data.IsDeleted = true;
            DeleteSuccessMsg();
            var isRemove = _iAccGroupService.Update(data);
            return RedirectToAction("Search");
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

            var result = await _iAccGroupService.GetFirstOrDefaultAsync(c => c.Name.Equals(name) && !c.IsDeleted);
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

        #region CHILD LEDGER CHECK

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsLedgerFoundInHead(long Id)
        {
            bool ledgerFound = await _iLedgerService.IsLedgerFoundInHead(Id);
            return Json(ledgerFound);
        }
        #endregion
    }
}
