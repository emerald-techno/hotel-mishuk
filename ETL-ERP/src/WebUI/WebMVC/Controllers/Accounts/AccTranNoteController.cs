using AutoMapper;
using Domain.Entities.Accounting;
using Domain.ViewModel.Accounting.AccTranNote;
using Interface.Services.Accounts;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;
using DU = Domain.Utility;

namespace WebMVC.Controllers.Accounts
{
    public class AccTranNoteController : AppBaseController
    {
        #region Config

        private readonly IUnitOfWork _iUnitWork;
        private readonly IMapper _iMapper;
        private readonly IAccTranNoteService _iService;
        private readonly DropdownService _dropdownService;
        private readonly IWebHostEnvironment _iWebHostEnvironment;

        public AccTranNoteController(IAccTranNoteService iService,
                                IMapper iMapper,
                                IUnitOfWork iUnitOfWork,
                                DropdownService dropdownService,
                                IWebHostEnvironment iWebHostEnvironment) : base(iUnitOfWork)
        {
            _iService = iService;
            _iMapper = iMapper;
            _iUnitWork = iUnitOfWork;
            _dropdownService = dropdownService;
            _iWebHostEnvironment = iWebHostEnvironment;
        }

        #endregion

        #region Create

        [HttpGet]
        public ActionResult Create()
        {
            var model = new AccTranNoteVm();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AccTranNoteVm modelVm)
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


                var model = _iMapper.Map<AccTranNote>(modelVm);
                model.NoteDate = DU.Utility.GetBdDateTimeNow();
                model.NoteById = UserId;
                model.ActionById = UserId;
                model.ActionDate = DU.Utility.GetBdDateTimeNow();

                var isAdded = await _iService.AddAsync(model);

                if (modelVm.IsAjaxPost)
                {
                    return Ok(isAdded);
                }

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

        #region GetVoucherNotes

        [HttpGet]
        public async Task<ActionResult> GetVcNotes(long vcId)
        {
            try
            {
                var vcNotes = await _iService.GetAsync(c => c.TranMstId == vcId && !c.IsDeleted);
                var dataList = _iMapper.Map<List<AccTranNoteVm>>(vcNotes);
                return Ok(dataList);
            }
            catch (Exception ex)
            {
                return Ok(SetError(ex.Message));
            }
        }

        #endregion

        #region Delete
        [HttpGet]
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
