using AutoMapper;
using Domain.Entities.Admin;
using Domain.Utility.Common;
using Domain.ViewModel.Admin.SetCurrency;
using Interface.Services.Admin;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;

namespace WebMVC.Controllers.Admin
{
    public class SetCurrencyController : AppBaseController
    {
        #region Config
        private readonly ISetCurrencyService _iService;
        private readonly IMapper _iMapper;
        private readonly IUnitOfWork _iUnitOfWork;
        public SetCurrencyController(ISetCurrencyService iService, IMapper iMapper, IUnitOfWork iUnitOfWork) : base(iUnitOfWork)
        {
            _iService = iService;
            _iMapper = iMapper;
            _iUnitOfWork = iUnitOfWork;
        }
        #endregion

        #region Create
        [HttpGet]
        [Authorize(Permissions.SetCurrencies.Create)]
        public IActionResult Create()
        {
            var model = new SetCurrencyVm();
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(SetCurrencyVm modelVm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    SaveFailedMsg("Information Is Not Correct");
                    return View("Create", modelVm);
                }
                _iService.CurrentUserId = UserId;
                var model = _iMapper.Map<SetCurrency>(modelVm);
                model.ActionById = UserId;
                model.ActionDate = Domain.Utility.Utility.GetBdDateTimeNow();

                var isAdded = _iService.Add(model);

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
        [Authorize(Permissions.SetCurrencies.ListView)]
        public IActionResult Search()
        {
            var vm = new SetCurrencySearchVm();
            return View(vm);
        }


        [HttpPost]
        [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
        public async Task<IActionResult>
            Search(DataTablePagination<SetCurrencySearchVm, SetCurrencySearchVm> searchVm = null)
        {
            if (searchVm == null) searchVm = new DataTablePagination<SetCurrencySearchVm, SetCurrencySearchVm>();
            if (searchVm?.SearchModel == null) searchVm.SearchModel = new SetCurrencySearchVm();
            var dataTable = await _iService.SearchAsync(searchVm);
            return dataTable == null ? NotFound() : Ok(dataTable);
        }
        #endregion

        #region Edit
        [HttpGet]
        [Authorize(Permissions.SetCurrencies.Edit)]
        public IActionResult Edit(long id)
        {
            try
            {
                var data = _iService.GetById(id);
                if (data == null)
                {
                    return NotFoundMsg();
                }
                var model = _iMapper.Map<SetCurrencyVm>(data);
                return View(model);
            }
            catch (Exception e)
            {
                ExceptionMsg(e.Message);
                return View("_404");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SetCurrencyVm modelVm)
        {
            try
            {
                if (modelVm.Id <= 0 || !ModelState.IsValid) return View(modelVm);
                var model = _iMapper.Map<SetCurrency>(modelVm);

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
        [Authorize(Permissions.SetCurrencies.Delete)]
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
    }
}
