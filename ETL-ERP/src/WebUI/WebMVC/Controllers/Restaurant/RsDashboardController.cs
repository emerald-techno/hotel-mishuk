using AutoMapper;
using Interface.Services.Restaurant;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Mvc;
using Utility.CachingUtility;
using WebMVC.Controllers.Base;

namespace WebMVC.Controllers.Restaurant
{
    public class RsDashboardController : AppBaseController
    {
        #region Config
        private readonly IUnitOfWork _iUnitWork;
        private readonly IMapper _iMapper;

        public RsDashboardController(IUnitOfWork iUnitOfWork,
                                IMapper iMapper) : base(iUnitOfWork)
        {
            _iUnitWork = iUnitOfWork;
            _iMapper = iMapper;
        }
        #endregion

        public IActionResult Index()
        {
            return View();
        }
    }
}
