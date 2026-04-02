using AutoMapper;
using Domain.Entities.Accounting;
using Domain.Utility.Common;
using Domain.ViewModel.Accounting.AccLedger;
using Interface.Services.Accounts;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;

namespace WebMVC.Controllers.Accounts
{
    public class AccLedgerController : AppBaseController
    {
        #region Config
        private readonly IUnitOfWork _iUnitOfWork;
        private readonly IMapper _iMapper;
        private readonly IAccLedgerService _iAccLedgerService;
        private readonly DropdownService _dropdownService;
        public AccLedgerController(IUnitOfWork iUnitOfWork,
                                        IAccLedgerService iAccLedgerService,
                                        IMapper iMapper, DropdownService dropdownService) : base(iUnitOfWork)
        {
            _iUnitOfWork = iUnitOfWork;
            _iAccLedgerService = iAccLedgerService;
            _iMapper = iMapper;
            _dropdownService = dropdownService;
        }
        #endregion

        #region Create
        [HttpGet]
        [Authorize(Permissions.AccLedgers.Create)]
        public IActionResult Create()
        {
            var model = new AccLedgerVm();
            model.HeadLookUp = _dropdownService.GetAccHeadSelectListItems();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AccLedgerVm modelVm)
        {
            modelVm.HeadLookUp = _dropdownService.GetAccHeadSelectListItems();
            try
            {
                if (!ModelState.IsValid)
                {
                    if (modelVm.IsAjaxPost) return BadRequest("Information Is Not Correct");

                    SaveFailedMsg("Information Is Not Correct");
                    return View("Create", modelVm);
                }
                _iAccLedgerService.CurrentUserId = UserId;
                var model = _iMapper.Map<AccLedger>(modelVm);
                model.LedgerCode = await _iAccLedgerService.GetAccLedgerCode(model.HeadId);
                model.LedgerGroupCode = await _iAccLedgerService.GetAccLedgerGroupCode(model.HeadId);
                model.ActionById = UserId;
                model.ActionDate = Domain.Utility.Utility.GetBdDateTimeNow();

                var isAdded = _iAccLedgerService.Add(model);

                if (!isAdded)
                {
                    if (modelVm.IsAjaxPost) return BadRequest("Save Failed.. !!");

                    SaveFailedMsg();
                    return View("Create", modelVm);
                }

                if (modelVm.IsAjaxPost) return Ok(true);

                SaveSuccessMsg();
                return RedirectToAction("Create");
            }
            catch (Exception e)
            {
                if (modelVm.IsAjaxPost) return BadRequest(e.Message);

                ExceptionMsg(e.Message);
                return View("Create", modelVm);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateFromCoa(long headId, string ledgerName)
        {
            try
            {
                _iAccLedgerService.CurrentUserId = UserId;
                var model = new AccLedger();
                model.HeadId = headId;
                model.LedgerName = ledgerName;
                model.LedgerCode = await _iAccLedgerService.GetAccLedgerCode(model.HeadId);
                model.LedgerGroupCode = await _iAccLedgerService.GetAccLedgerGroupCode(model.HeadId);
                model.ActionById = UserId;
                model.ActionDate = Domain.Utility.Utility.GetBdDateTimeNow();

                var isAdded = _iAccLedgerService.Add(model);
                return Ok(isAdded);
            }
            catch (Exception e)
            {
                ExceptionMsg(e.Message);
                return BadRequest(e.Message);
            }
        }

        #endregion

        #region Search
        [HttpGet]
        [Authorize(Permissions.AccLedgers.ListView)]
        public IActionResult Search()
        {
            var vm = new AccLedgerSearchVm();
            vm.HeadLookUp = _dropdownService.GetAccHeadSelectListItems();
            return View(vm);
        }


        [HttpPost]
        [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
        public async Task<IActionResult>
            Search(DataTablePagination<AccLedgerSearchVm, AccLedgerSearchVm> searchVm = null)
        {
            if (searchVm == null) searchVm = new DataTablePagination<AccLedgerSearchVm, AccLedgerSearchVm>();
            if (searchVm?.SearchModel == null) searchVm.SearchModel = new AccLedgerSearchVm();
            var dataTable = await _iAccLedgerService.SearchAsync(searchVm);
            return dataTable == null ? NotFound() : Ok(dataTable);
        }
        #endregion

        #region Edit
        [HttpGet]
        [Authorize(Permissions.AccLedgers.Edit)]
        public IActionResult Edit(long id)
        {
            try
            {
                var data = _iAccLedgerService.GetById(id);
                if (data == null)
                {
                    return NotFoundMsg();
                }
                var model = _iMapper.Map<AccLedgerVm>(data);
                model.HeadLookUp = _dropdownService.GetAccHeadSelectListItems();
                return View(model);
            }
            catch (Exception e)
            {
                ExceptionMsg(e.Message);
                return View("_404");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AccLedgerVm modelVm)
        {
            modelVm.HeadLookUp = _dropdownService.GetAccHeadSelectListItems();
            try
            {
                if (modelVm.Id <= 0 || !ModelState.IsValid) return View(modelVm);
                var model = _iMapper.Map<AccLedger>(modelVm);

                model.ActionById = UserId;
                model.ActionDate = Domain.Utility.Utility.GetBdDateTimeNow();
                model.UpdatedById = UserId;
                model.UpdateDate = Domain.Utility.Utility.GetBdDateTimeNow();
                var isAdded = await _iAccLedgerService.UpdateAsync(model);
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
        [Authorize(Permissions.AccLedgers.Delete)]
        public async Task<IActionResult> Delete(long id)
        {
            var data = await _iAccLedgerService.GetByIdAsync(id);
            if (data == null)
            {
                DeleteFailedMsg();
                return NotFound();
            }
            data.IsDeleted = true;
            DeleteSuccessMsg();
            var isRemove = _iAccLedgerService.Update(data);
            return RedirectToAction("Search");
        }
        #endregion

        #region EXISTING CHECK

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsNameExist(string ledgerName, string initName)
        {
            if (!string.IsNullOrEmpty(ledgerName) && !string.IsNullOrEmpty(initName) && ledgerName.ToUpper().Equals(initName.ToUpper()))
            {
                return Json(true);
            }

            var result = await _iAccLedgerService.GetFirstOrDefaultAsync(c => c.LedgerName.Equals(ledgerName) && !c.IsDeleted);
            if (result == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Ledger Name {ledgerName} Is Already Exist..!!");
            }

        }

        #endregion
    }
}
