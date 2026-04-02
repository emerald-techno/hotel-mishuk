using AutoMapper;
using Domain.Entities.Accounting;
using Domain.Utility.Common;
using Domain.ViewModel.Accounting.AccHead;
using Interface.Services.Accounts;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;

namespace WebMVC.Controllers.Accounts
{
    public class AccHeadController : AppBaseController
    {
        #region Config

        private readonly IUnitOfWork _iUnitOfWork;
        private readonly IMapper _iMapper;
        private readonly IAccHeadService _iAccHeadService;
        private readonly DropdownService _dropdownService;

        public AccHeadController(IUnitOfWork iUnitOfWork, IMapper iMapper, IAccHeadService iAccHeadService, DropdownService dropdownService) : base(iUnitOfWork)
        {
            _iUnitOfWork = iUnitOfWork;
            _iMapper = iMapper;
            _iAccHeadService = iAccHeadService;
            _dropdownService = dropdownService;
        }

        #endregion

        #region Create
        [HttpGet]
        [Authorize(Permissions.AccHeads.Create)]
        public async Task<IActionResult> Create()
        {
            var model = new AccHeadVm();
            model.AccGroupLookUp = _dropdownService.GetAccGroupSelectListItems();
            model.AccHeadLookUp = _dropdownService.GetSubAccHeadSelectListItems();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AccHeadVm modelVm)
        {
            modelVm.AccGroupLookUp = _dropdownService.GetAccGroupSelectListItems();
            modelVm.AccHeadLookUp = _dropdownService.GetSubAccHeadSelectListItems();
            try
            {
                if (!ModelState.IsValid)
                {
                    if (modelVm.IsAjaxPost) return BadRequest("Information Is Not Correct");

                    SaveFailedMsg("Information Is Not Correct");
                    return View("Create", modelVm);
                }
                _iAccHeadService.CurrentUserId = UserId;

                int levelId = 1;
                if (modelVm.ParentHeadId > 0)
                {
                    AccHead parent = _iAccHeadService.GetById(Convert.ToInt32(modelVm.ParentHeadId));
                    levelId = parent.LevelId + 1;
                }

                var model = _iMapper.Map<AccHead>(modelVm);
                model.HeadCode = await _iAccHeadService.GetAccHeadCodeAsync(model.GroupId, model.ParentHeadId);
                model.HeadGroupCode = await _iAccHeadService.GetAccHeadGroupAutoCode(model.GroupId, model.ParentHeadId);
                model.ActionById = UserId;
                model.ActionDate = Domain.Utility.Utility.GetBdDateTimeNow();
                model.LevelId = levelId;


                var isAdded = await _iAccHeadService.AddAsync(model);

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
        public async Task<IActionResult> CreateFromCoa(long parentHeadId, string headName)
        {
            try
            {
                _iAccHeadService.CurrentUserId = UserId;
                var model = new AccHead();

                if (parentHeadId > 0)
                {
                    AccHead parent = _iAccHeadService.GetById(Convert.ToInt32(parentHeadId));
                    model.LevelId = parent.LevelId + 1;
                    model.GroupId = parent.GroupId;
                    model.HeadName = headName;
                    model.ParentHeadId = parentHeadId;
                    model.HeadCode = await _iAccHeadService.GetAccHeadCodeAsync(model.GroupId, model.ParentHeadId);
                    model.HeadGroupCode = await _iAccHeadService.GetAccHeadGroupAutoCode(model.GroupId, model.ParentHeadId);
                    model.ActionById = UserId;
                    model.ActionDate = Domain.Utility.Utility.GetBdDateTimeNow();


                    var isAdded = await _iAccHeadService.AddAsync(model);
                    return Ok(isAdded);
                }
                return BadRequest("Parent head id missing");
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
        [Authorize(Permissions.AccHeads.ListView)]
        public IActionResult Search()
        {
            var vm = new AccHeadSearchVm();
            vm.AccGroupLookUp = _dropdownService.GetAccGroupSelectListItems();
            return View(vm);
        }


        [HttpPost]
        [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
        public async Task<IActionResult>
            SearchAsync(DataTablePagination<AccHeadSearchVm, AccHeadSearchVm> searchVm = null)
        {
            if (searchVm == null) searchVm = new DataTablePagination<AccHeadSearchVm, AccHeadSearchVm>();
            if (searchVm?.SearchModel == null) searchVm.SearchModel = new AccHeadSearchVm();
            var dataTable = await _iAccHeadService.SearchAsync(searchVm);
            return dataTable == null ? NotFound() : Ok(dataTable);
        }
        #endregion

        #region Edit
        [HttpGet]
        [Authorize(Permissions.AccHeads.Edit)]
        public async Task<IActionResult> Edit(long id)
        {
            try
            {
                var data = await _iAccHeadService.GetByIdAsync(id);
                if (data == null)
                {
                    return NotFoundMsg();
                }
                var model = _iMapper.Map<AccHeadVm>(data);
                model.AccGroupLookUp = _dropdownService.GetAccGroupSelectListItems();
                return View(model);
            }
            catch (Exception e)
            {
                ExceptionMsg(e.Message);
                return View("_404");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AccHeadVm modelVm)
        {
            modelVm.AccGroupLookUp = _dropdownService.GetAccGroupSelectListItems();

            try
            {
                if (modelVm.Id <= 0 || !ModelState.IsValid) return View(modelVm);
                var model = _iMapper.Map<AccHead>(modelVm);

                model.ActionById = UserId;
                model.ActionDate = Domain.Utility.Utility.GetBdDateTimeNow();
                model.UpdatedById = UserId;
                model.UpdateDate = Domain.Utility.Utility.GetBdDateTimeNow();
                var isAdded = await _iAccHeadService.UpdateAsync(model);
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
        [Authorize(Permissions.AccHeads.Delete)]
        public async Task<IActionResult> Delete(long id)
        {
            var data = await _iAccHeadService.GetByIdAsync(id);
            if (data == null)
            {
                DeleteFailedMsg();
                return NotFound();
            }
            data.IsDeleted = true;
            DeleteSuccessMsg();
            var isRemove = _iAccHeadService.Update(data);
            return RedirectToAction("Search");
        }
        #endregion

        #region JSON Data

        [HttpPost]
        public IActionResult GetHeadJsonData(long groupId)
        {
            var dataList = _dropdownService.GetAccHeadDynamicData(groupId);
            return Ok(dataList);
        }

        #endregion

        #region EXISTING CHECK

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsNameExist(string headName, string initName)
        {
            if (!string.IsNullOrEmpty(headName) && !string.IsNullOrEmpty(initName) && headName.ToUpper().Equals(initName.ToUpper()))
            {
                return Json(true);
            }

            var result = await _iAccHeadService.GetFirstOrDefaultAsync(c => c.HeadName.Equals(headName) && !c.IsDeleted);
            if (result == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Head Name {headName} Is Already Exist..!!");
            }

        }

        #endregion
    }
}
