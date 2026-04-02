using Domain.Utility;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Utility.CachingUtility;

namespace WebMVC.Controllers.Base;

public class BaseController : Controller
{
    #region Config

    private long _userId;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly CacheStoreService _cacheStoreService;
    private string EntityName { get; set; }

    public BaseController(IUnitOfWork iUnitOfWork)
    {
        _iUnitOfWork = iUnitOfWork;
    }

    public BaseController(IUnitOfWork iUnitOfWork, CacheStoreService cacheStoreService)
    {
        _iUnitOfWork = iUnitOfWork;
        _cacheStoreService = cacheStoreService;
    }

    #endregion

    public long UserId
    {
        get
        {
            if (_userId > 0) return _userId;
            _userId = _iUnitOfWork.CurrentUserId;
            return _userId = _userId > 0 ? _userId : 0;
        }
        set => _userId = value;
    }

    //For Saved Success BaseMsgHidden
    protected List<TempDataDictionary> SaveSuccessMsg(string title = null)
    {
        return new List<TempDataDictionary> { (TempData["MsgKey"] = MessageEnum.Success) as TempDataDictionary, (TempData["MsgValue"] = $"{EntityName} {title} Information Successfully SAVED.") as TempDataDictionary };
    }
    //For Partially Saved Failed BaseMsgHidden
    protected List<TempDataDictionary> SaveWarningMsg(string title = null)
    {
        return new List<TempDataDictionary> { (TempData["MsgKey"] = MessageEnum.Warning) as TempDataDictionary, (TempData["MsgValue"] = $"{EntityName} {title}") as TempDataDictionary };
    }
    //For Saved Failed BaseMsgHidden
    protected List<TempDataDictionary> SaveFailedMsg(string title = null)
    {
        return new List<TempDataDictionary> { (TempData["MsgKey"] = MessageEnum.Error) as TempDataDictionary, (TempData["MsgValue"] = $"{SetError(title)} Information Save FAILED.") as TempDataDictionary };
    }

    //For Updated Success BaseMsgHidden
    protected List<TempDataDictionary> UpdateSuccessMsg(string title = null)
    {
        return new List<TempDataDictionary> { (TempData["MsgKey"] = MessageEnum.Success) as TempDataDictionary, (TempData["MsgValue"] = $"{EntityName} {title} Information Successfully UPDATED.") as TempDataDictionary };
    }

    //For Updated Failed BaseMsgHidden
    protected List<TempDataDictionary> UpdateFailedMsg(string title = null)
    {
        return new List<TempDataDictionary> { (TempData["MsgKey"] = MessageEnum.Error) as TempDataDictionary, (TempData["MsgValue"] = $"{SetError(title)} Information Update FAILED.") as TempDataDictionary };
    }

    //For Deleted Success BaseMsgHidden
    protected List<TempDataDictionary> DeleteSuccessMsg(string title = null)
    {
        return new List<TempDataDictionary> { (TempData["MsgKey"] = MessageEnum.Success) as TempDataDictionary, (TempData["MsgValue"] = $"{title} Information Successfully DELETED.") as TempDataDictionary };
    }

    //For Deleted Failed BaseMsgHidden
    protected List<TempDataDictionary> DeleteFailedMsg(string title = null)
    {
        return new List<TempDataDictionary> { (TempData["MsgKey"] = MessageEnum.Error) as TempDataDictionary, (TempData["MsgValue"] = $"{SetError(title)} Information Delete FAILED.") as TempDataDictionary };
    }

    //For Failed BaseMsgHidden
    protected List<TempDataDictionary> FailedMsg(string title)
    {
        return new List<TempDataDictionary> { (TempData["MsgKey"] = MessageEnum.Error) as TempDataDictionary, (TempData["MsgValue"] = SetError(title)) as TempDataDictionary };
    }

    //For Exception BaseMsgHidden
    protected dynamic ExceptionMsg(string msg, bool isFromPartial = false)
    {
        if (isFromPartial)
        {
            return Json(SetError(msg));
        }
        return new List<TempDataDictionary> { (TempData["MsgKey"] = MessageEnum.Error) as TempDataDictionary, (TempData["MsgValue"] = SetError(msg)) as TempDataDictionary };
    }

    //For NotFound BaseMsgHidden
    protected dynamic NotFoundMsg()
    {
        var msg = SetError("Sorry, The Information You Are Looking For, That's Not Found, Please Try Another One.");
        return new List<TempDataDictionary> { (TempData["MsgKey"] = MessageEnum.Error) as TempDataDictionary, (TempData["MsgValue"] = SetError(msg)) as TempDataDictionary };
    }

    public ActionResult NotFound(string msg = null)
    {
        FailedMsg(!string.IsNullOrEmpty(msg) ? msg : "Sorry ! The Data You Are Looking For, That's Not Found !");
        return RedirectToAction("Index", "Home", null);
    }

    public string GetModelStateErrors()
    {
        var dataList = ModelState.Values.SelectMany(v => v.Errors);
        var error = dataList.Aggregate("", (c, d) => c + (d.ErrorMessage + " \n"));
        return error;
    }

    protected void SetModelStateError(ModelStateDictionary modelState, IdentityResult result)
    {
        if (result.Errors != null)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.ToString());
            }
        }
    }

    public string SetError(string error)
    {
        if (string.IsNullOrEmpty(error)) return error;
        if (error.Contains("Error: ") && error.Contains(EntityName)) return error;
        if (error.Contains("Error: ")) return "Sorry! " + EntityName + " " + error;
        if (error.Contains("Sorry: ")) return "Error: " + EntityName + " " + error;
        return "Error: Sorry! " + EntityName + " " + error;
    }

    protected string GetBaseUrlNetCore()
    {
        var data = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
        return data;
    }

}
