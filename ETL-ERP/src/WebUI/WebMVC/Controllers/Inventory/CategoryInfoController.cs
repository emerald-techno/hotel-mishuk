using AutoMapper;
using Domain.Entities.Inventory;
using Domain.Utility.Common;
using Domain.ViewModel.Inventory.CategoryInfo;
using Interface.Services.Inventory;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using DU = Domain.Utility;

namespace MPSC.Controllers
{
    public class CategoryInfoController : AppBaseController
    {
        #region Config
        private readonly IMapper _iMapper;
        private readonly ICategoryInfoService _iService;
        private readonly IUnitOfWork _iUnitOfWork;
        private readonly DropdownService _dropdownService;
        private readonly CacheStoreService _cacheStoreService;

        public CategoryInfoController(ICategoryInfoService iService, IMapper iMapper,
            IUnitOfWork iUnitOfWork, DropdownService dropdownService,
            CacheStoreService cacheStoreService)
            : base(iUnitOfWork, "SupplierInfo")
        {
            _iService = iService;
            _iMapper = iMapper;
            _iUnitOfWork = iUnitOfWork;
            _dropdownService = dropdownService;
            _cacheStoreService = cacheStoreService;
        }
        #endregion

        #region Create
        [HttpGet]
        [Authorize(Permissions.CategoryInfo.Create)]
        public async Task<IActionResult> Create()
        {
            var model = new CategoryInfoVm();
            model.CategoryTypeLookUp = _dropdownService.GetCategoryTypeSelectListItems();
            model.LadgerLookUp = _dropdownService.GetAccLedgerSelectListItems();
            model.CategoryCode = await _iService.GetCategoryInfoCode();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryInfoVm modelVm)
        {
            modelVm.CategoryTypeLookUp = _dropdownService.GetCategoryTypeSelectListItems();
            modelVm.LadgerLookUp = _dropdownService.GetAccLedgerSelectListItems();

            try
            {
                if (!ModelState.IsValid)
                {
                    SaveFailedMsg("Information is not correct");
                    return View("Create", modelVm);
                }
                var photoFile = modelVm.GetAppFileToUploadFolder();
                _iService.CurrentUserId = UserId;
                var model = _iMapper.Map<CategoryInfo>(modelVm);
                model.ActionById = UserId;
                model.ActionDate = DU.Utility.GetBdDateTimeNow();
                var isAdded = await _iService.AddAsync(model);
                if (!isAdded)
                {
                    SaveFailedMsg();
                    return View("Create", modelVm);
                }
                SaveSuccessMsg();
                await DU.Utility.UploadFileToFolderAsync(photoFile);

                _cacheStoreService.AddOrUpdate<CategoryInfo>(CacheEnum.CategoryInfoList.ToString());

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
        [Authorize(Permissions.CategoryInfo.Edit)]
        public ActionResult Edit(long id)
        {

            try
            {
                var data = _iService.GetById(id);
                if (data == null)
                {
                    return NotFoundMsg();
                }
                var model = _iMapper.Map<CategoryInfoVm>(data);
                var photoDoc = DU.Utility.GetBase64ImageStringFromPath(model.PhotoDocUrl);
                ViewBag.Photo = photoDoc;
                model.CategoryTypeLookUp = _dropdownService.GetCategoryTypeSelectListItems();
                model.LadgerLookUp = _dropdownService.GetAccLedgerSelectListItems();
                return View(model);
            }
            catch (Exception e)
            {
                ExceptionMsg(e.Message);
                return View("_404");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryInfoVm modelVm)
        {
            modelVm.CategoryTypeLookUp = _dropdownService.GetCategoryTypeSelectListItems();
            modelVm.LadgerLookUp = _dropdownService.GetAccLedgerSelectListItems();

            try
            {
                if (modelVm.Id <= 0 || !ModelState.IsValid) return View(modelVm);
                var photoFile = modelVm.GetAppFileToUploadFolder();
                _iService.CurrentUserId = UserId;
                var model = _iMapper.Map<CategoryInfo>(modelVm);
                model.ActionById = UserId;
                model.UpdatedById = UserId;
                model.UpdateDate = Domain.Utility.Utility.GetBdDateTimeNow();
                var isAdded = await _iService.UpdateAsync(model);
                if (!isAdded)
                {
                    UpdateFailedMsg();
                    return View("Edit", modelVm);
                }

                await Domain.Utility.Utility.UploadFileToFolderAsync(photoFile);
                _cacheStoreService.AddOrUpdate<CategoryInfo>(CacheEnum.CategoryInfoList.ToString());

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
        [Authorize(Permissions.CategoryInfo.ListView)]
        public IActionResult Search()
        {
            var vm = new CategoryInfoSearchVm();
            return View(vm);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
        public async Task<IActionResult>
            Search(DataTablePagination<CategoryInfoSearchVm, CategoryInfoSearchVm> searchVm = null)
        {
            if (searchVm == null) searchVm = new DataTablePagination<CategoryInfoSearchVm, CategoryInfoSearchVm>();
            if (searchVm?.SearchModel == null) searchVm.SearchModel = new CategoryInfoSearchVm();
            var dataTable = await _iService.SearchAsync(searchVm);
            return dataTable == null ? NotFound() : Ok(dataTable);
        }
        #endregion

        #region Delete
        [HttpGet]
        [Authorize(Permissions.CategoryInfo.Delete)]
        //[HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var data = await _iService.GetByIdAsync(id);
            if (data == null)
            {
                return NotFound();
            }
            data.IsDeleted = true;
            var isRemoved = _iService.Update(data);
            //return Ok(isRemoved);
            return RedirectToAction("Search");
        }
        #endregion
    }
}
