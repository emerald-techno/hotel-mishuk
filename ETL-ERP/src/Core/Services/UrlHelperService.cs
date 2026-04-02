using Interface.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;

namespace Services;

public class UrlHelperService : IUrlHelperService
{
    private readonly IUrlHelper _urlHelper;
    private readonly IHttpContextAccessor _iHttpContextAccessor;

    public UrlHelperService(IUrlHelperFactory urlHelperFactory, IHttpContextAccessor iHttpContextAccessor)
    {
        var actionContext = new ActionContext(iHttpContextAccessor.HttpContext,
                                              iHttpContextAccessor.HttpContext.GetRouteData(),
                                              new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
        _urlHelper = urlHelperFactory.GetUrlHelper(actionContext);
        _iHttpContextAccessor = iHttpContextAccessor;
    }

    public string BookingDetailUrl(long id)
    {
        return _urlHelper.Action("Details", "BookingService", new { id = id },
                                 _iHttpContextAccessor.HttpContext.Request.Scheme);
    }
}
