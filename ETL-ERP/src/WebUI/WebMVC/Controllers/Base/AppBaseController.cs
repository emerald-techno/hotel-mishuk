using Interface.UnitOfWork;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace WebMVC.Controllers.Base;

public class AppBaseController : BaseController
{
    #region Config

    private readonly IUnitOfWork _iUnitOfWork;

    //public AppBaseController(IUnitOfWork iUnitOfWork, IAccountService iAccountService) : base(iUnitOfWork, iAccountService)
    //{
    //    _iUnitOfWork = iUnitOfWork;
    //    _iAccountService = iAccountService;
    //}

    //public AppBaseController(IUnitOfWork iUnitOfWork, string entityName, IAccountService iAccountService) : this(iUnitOfWork, iAccountService)
    //{
    //    _iUnitOfWork = iUnitOfWork;
    //}

    public AppBaseController(IUnitOfWork iUnitOfWork) : base(iUnitOfWork)
    {
        _iUnitOfWork = iUnitOfWork;
    }

    public AppBaseController(IUnitOfWork iUnitOfWork, string entityName) : this(iUnitOfWork)
    {
        _iUnitOfWork = iUnitOfWork;
    }

    #endregion

}
