using AutoMapper;
using Domain.Entities.Admin;
using Domain.Utility.Common;
using Domain.ViewModel.Admin.Department;
using Interface.Services.Admin;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;

namespace WebMVC.Controllers.Admin
{
    [Authorize]
    public class DepartmentController : AppBaseController
    {
        #region Config
        private readonly IUnitOfWork _iUnitWork;
        private readonly IDepartmentService _iService;
        private readonly IMapper _iMapper;

        public DepartmentController(IUnitOfWork iUnitOfWork,
                                IDepartmentService iService,
                                IMapper iMapper) : base(iUnitOfWork)
        {
            _iUnitWork = iUnitOfWork;
            _iService = iService;
            _iMapper = iMapper;
        }
        #endregion

        #region Create

        [HttpGet]
        [Authorize(Permissions.Departments.Create)]
        public IActionResult Create()
        {
            var model = new DepartmentVm();
            model.IsAcademic = false;
            return View(model);
        }

        [HttpGet]
        [Authorize(Permissions.Departments.CreateAcademicDepartment)]
        public IActionResult CreateAcademicDepartment()
        {
            var model = new DepartmentVm();
            model.IsAcademic = true;
            return View(model);
        }

        [HttpPost]
        public IActionResult Create(DepartmentVm modelVm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    SaveFailedMsg("Information Is Not Correct");
                    return View("Create", modelVm);
                }
                _iService.CurrentUserId = UserId;
                var model = _iMapper.Map<Department>(modelVm);
                model.IsAcademic = false;
                model.ActionById = UserId;
                model.ActionDate = Domain.Utility.Utility.GetBdDateTimeNow();

                Department existingDepartment = _iService.GetSingleOrDefault(d => d.Name == model.Name && d.Code == model.Code);
                if (existingDepartment != null)
                {
                    ModelState.AddModelError(string.Empty, "Department already exists");
                }
                else
                {
                    var isAdded = _iService.Add(model);
                    if (!isAdded)
                    {
                        SaveFailedMsg();
                        return View("Create", modelVm);
                    }
                    SaveSuccessMsg();


                }
                return RedirectToAction("Create");

            }
            catch (Exception e)
            {
                ExceptionMsg(e.Message);
                return View("Create", modelVm);
            }
        }

        [HttpPost]
        public IActionResult CreateAcademicDepartment(DepartmentVm modelVm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    SaveFailedMsg("Information Is Not Correct");
                    return View("CreateAcademicDepartment", modelVm);
                }
                _iService.CurrentUserId = UserId;
                var model = _iMapper.Map<Department>(modelVm);
                model.IsAcademic = true;
                model.ActionById = UserId;
                model.ActionDate = Domain.Utility.Utility.GetBdDateTimeNow();

                var isAdded = _iService.Add(model);
                if (!isAdded)
                {
                    SaveFailedMsg();
                    return View("CreateAcademicDepartment", modelVm);
                }
                SaveSuccessMsg();

                return RedirectToAction("CreateAcademicDepartment");
            }
            catch (Exception e)
            {
                ExceptionMsg(e.Message);
                return View("CreateAcademicDepartment", modelVm);
            }
        }
        #endregion

        #region Search
        [HttpGet]
        [Authorize(Permissions.Departments.ListView)]
        public IActionResult Search()
        {
            var vm = new DepartmentSearchVm();
            vm.IsAcademic = false;
            return View(vm);
        }

        [HttpGet]
        [Authorize(Permissions.Departments.ListViewAcademicDepartment)]
        public IActionResult SearchAcademicDepartment()
        {
            var vm = new DepartmentSearchVm();
            vm.IsAcademic = true;
            return View(vm);
        }


        [HttpPost]
        [ProducesResponseType(201, Type = typeof(IEnumerable<JsonResult>))]
        public async Task<IActionResult>
            Search(DataTablePagination<DepartmentSearchVm, DepartmentSearchVm> searchVm = null)
        {
            if (searchVm == null) searchVm = new DataTablePagination<DepartmentSearchVm, DepartmentSearchVm>();
            if (searchVm?.SearchModel == null) searchVm.SearchModel = new DepartmentSearchVm();
            var dataTable = await _iService.SearchAsync(searchVm);
            return dataTable == null ? NotFound() : Ok(dataTable);
        }

        #endregion

        #region Edit
        [HttpGet]
        [Authorize(Permissions.Departments.Edit)]
        public IActionResult Edit(long id)
        {
            try
            {
                var data = _iService.GetById(id);
                if (data == null)
                {
                    return NotFoundMsg();
                }
                var model = _iMapper.Map<DepartmentVm>(data);
                model.IsAcademic = false;
                return View(model);
            }
            catch (Exception e)
            {
                ExceptionMsg(e.Message);
                return View("_404");
            }
        }

        [HttpGet]
        [Authorize(Permissions.Departments.EditAcademicDepartment)]
        public IActionResult EditAcademicDepartment(long id)
        {
            try
            {
                var data = _iService.GetById(id);
                if (data == null)
                {
                    return NotFoundMsg();
                }
                var model = _iMapper.Map<DepartmentVm>(data);
                model.IsAcademic = true;
                return View(model);
            }
            catch (Exception e)
            {
                ExceptionMsg(e.Message);
                return View("_404");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DepartmentVm modelVm)
        {
            try
            {
                if (modelVm.Id <= 0 || !ModelState.IsValid) return View(modelVm);
                var model = _iMapper.Map<Department>(modelVm);
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
                if (model.IsAcademic) return RedirectToAction("SearchAcademicDepartment");
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
        [Authorize(Permissions.Departments.Delete)]
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

        [HttpGet]
        [Authorize(Permissions.Departments.DeleteAcademicDepartment)]
        public async Task<IActionResult> DeleteAcademicDepartment(long id)
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
            return RedirectToAction("SearchAcademicDepartment");
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

            var result = await _iService.GetFirstOrDefaultAsync(c => c.Name.Equals(name) && !c.IsDeleted);
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

            var result = await _iService.GetFirstOrDefaultAsync(c => c.Code.Equals(code) && !c.IsDeleted);
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
    }
}
