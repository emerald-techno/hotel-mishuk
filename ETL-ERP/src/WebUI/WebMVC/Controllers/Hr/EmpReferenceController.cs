using AutoMapper;
using Domain.Entities.HR;
using Domain.ViewModel.Hr.EmpReference;
using Interface.Services.Hr;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Controllers.Base;
using WebMVC.Models.IdentityModels;
using DU = Domain.Utility;

namespace WebMVC.Controllers.Hr
{
    public class EmpReferenceController : AppBaseController
    {
        #region Config


        private readonly IMapper _iMapper;
        private readonly IEmpReferenceService _iService;
        private readonly IUnitOfWork _iUnitWork;

        public EmpReferenceController(IEmpReferenceService iService, IMapper iMapper, IUnitOfWork iUnitOfWork) : base(iUnitOfWork, "EmpReference")
        {
            _iService = iService;
            _iMapper = iMapper;
            _iUnitWork = iUnitOfWork;
        }

        #endregion

        #region Create

        [HttpGet]
        [Authorize(Permissions.EmpReference.Create)]
        public ActionResult Create()
        {
            var model = new EmpReferenceVm();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] EmpReferenceVm modelVm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    if (modelVm.IsAjaxPost)
                    {
                        return Ok(false);
                    }

                    SaveFailedMsg("Information Is Not Correct");
                    return View("Create", modelVm);
                };

                var model = _iMapper.Map<EmpReference>(modelVm);
                model.ActionById = UserId;
                model.ActionDate = DU.Utility.GetBdDateTimeNow();
                model.Dob = !string.IsNullOrEmpty(modelVm.DobStr) ? DU.Utility.ConvertStrToDate(modelVm.DobStr) : null;

                var isAdded = await _iService.AddAsync(model);
                if (modelVm.IsAjaxPost) return Ok(isAdded);
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

        #region GetEmployeeReference

        [HttpGet]
        [Authorize(Permissions.EmpReference.GetEmployeeReference)]
        public async Task<ActionResult> GetReferenceByEmpId(long empId, short refType)
        {
            try
            {
                var references = await _iService.GetReferenceByEmpId(empId, refType);
                return Ok(references);
            }
            catch (Exception ex)
            {
                return Ok(SetError(ex.Message));
            }
        }

        #endregion

        #region Delete
        [HttpGet]
        [Authorize(Permissions.EmpReference.Delete)]
        public async Task<IActionResult> Delete(long id)
        {
            var data = await _iService.GetByIdAsync(id);
            if (data == null)
            {
                return NotFound();
            }
            data.IsDeleted = true;
            var isRemove = _iService.Update(data);
            return Ok(isRemove);
        }
        #endregion
    }
}
