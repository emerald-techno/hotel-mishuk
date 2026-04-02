using AutoMapper;
using Domain.Entities.Admin;
using Domain.Utility.Common;
using Domain.ViewModel.Admin.Designation;
using Interface.Services.Admin;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;

namespace WebMVC.Controllers.Admin
{
    public class DesignationController : AppBaseController
    {
        #region Config
        private readonly IUnitOfWork _iUnitOfWork;
        private readonly IMapper _iMapper;
        private readonly IDesignationService _iDesignationService;
        private readonly DropdownService _dropdownService;
        public DesignationController(IUnitOfWork iUnitOfWork,
                                        IDesignationService iDesignationService,
                                        IMapper iMapper, DropdownService dropdownService) : base(iUnitOfWork)
        {
            _iUnitOfWork = iUnitOfWork;
            _iDesignationService = iDesignationService;
            _iMapper = iMapper;
            _dropdownService = dropdownService;
        }
        #endregion

        #region Create
        [HttpGet]
        [Authorize(Permissions.Designations.Create)]
        public IActionResult Create()
        {
            var model = new DesignationVm();
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(DesignationVm modelVm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    SaveFailedMsg("Information Is Not Correct");
                    return View("Create", modelVm);
                }
                _iDesignationService.CurrentUserId = UserId;
                var model = _iMapper.Map<Designation>(modelVm);
                model.ActionById = UserId;
                model.ActionDate = Domain.Utility.Utility.GetBdDateTimeNow();

                var isAdded = _iDesignationService.Add(model);

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
        [Authorize(Permissions.Designations.ListView)]
        public IActionResult Search()
        {
            var vm = new DesignationSearchVm();
            return View(vm);
        }


        [HttpPost]
        [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
        public async Task<IActionResult>
            Search(DataTablePagination<DesignationSearchVm, DesignationSearchVm> searchVm = null)
        {
            if (searchVm == null) searchVm = new DataTablePagination<DesignationSearchVm, DesignationSearchVm>();
            if (searchVm?.SearchModel == null) searchVm.SearchModel = new DesignationSearchVm();
            var dataTable = await _iDesignationService.SearchAsync(searchVm);
            return dataTable == null ? NotFound() : Ok(dataTable);
        }
        #endregion

        #region Edit
        [HttpGet]
        [Authorize(Permissions.Designations.Edit)]
        public IActionResult Edit(long id)
        {
            try
            {
                var data = _iDesignationService.GetById(id);
                if (data == null)
                {
                    return NotFoundMsg();
                }
                var model = _iMapper.Map<DesignationVm>(data);
                return View(model);
            }
            catch (Exception e)
            {
                ExceptionMsg(e.Message);
                return View("_404");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DesignationVm modelVm)
        {
            try
            {
                if (modelVm.Id <= 0 || !ModelState.IsValid) return View(modelVm);
                var model = _iMapper.Map<Designation>(modelVm);

                model.UpdatedById = UserId;
                model.UpdateDate = Domain.Utility.Utility.GetBdDateTimeNow();
                var isAdded = await _iDesignationService.UpdateAsync(model);
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
        [Authorize(Permissions.Designations.Delete)]
        public async Task<IActionResult> Delete(long id)
        {
            var data = await _iDesignationService.GetByIdAsync(id);
            if (data == null)
            {
                DeleteFailedMsg();
                return NotFound();
            }
            data.IsDeleted = true;
            DeleteSuccessMsg();
            var isRemove = _iDesignationService.Update(data);
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

            var result = await _iDesignationService.GetFirstOrDefaultAsync(c => c.Name.Equals(name) && !c.IsDeleted);
            if (result == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Name {name} Is Already Exist..!!");
            }

        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsCodeExist(string code, string initCode)
        {
            if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(initCode) && code.ToUpper().Equals(initCode.ToUpper()))
            {
                return Json(true);
            }

            var result = await _iDesignationService.GetFirstOrDefaultAsync(c => c.Code.Equals(code) && !c.IsDeleted);
            if (result == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Code {code} Is Already Exist..!!");
            }

        }

        #endregion

        #region DepartmentByDesignation

        public IActionResult GetDeptByDesignationId(long desId)
        {
            if (desId == 0)
            {
                return NotFound();
            }
            var designation = _iDesignationService.GetById(desId);
            if (designation == null) { return NotFound(); }

            var result = _dropdownService.GetNonAcademicDepartmentSelectListItems(false);

            if (designation.IsTeacher)
            {
                result = _dropdownService.GetAcademicDepartmentSelectListItems(false);
            }

            var dynamicData = result.Select(c => new { Id = c.Value, Name = c.Text });
            return Ok(dynamicData);
        }

        #endregion
    }
}
