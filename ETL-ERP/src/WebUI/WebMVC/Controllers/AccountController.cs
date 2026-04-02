using AutoMapper;
using Domain.Entities.Identity;
using Domain.ViewModel.UserAccount;
using Interface.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebMVC.Controllers.Base;

namespace WebMVC.Controllers;

public class AccountController : AppBaseController
{
    #region Config

    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly IUnitOfWork _iUnitOfWork;
    private readonly IMapper _iMapper;

    public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager,
                            RoleManager<ApplicationRole> roleManager,
                            IUnitOfWork iUnitOfWork, IMapper iMapper) : base(iUnitOfWork)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _iUnitOfWork = iUnitOfWork;
        _iMapper = iMapper;
    }

    [HttpGet]
    public IActionResult Login(string msg, string returnUrl)
    {

        ViewBag.Msg = msg;
        var model = new LoginViewModel()
        {
            Message = msg,
            ReturnUrl = returnUrl
        };

        return View(model);
    }

    #endregion



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        try
        {
            //var user = await _iAccountService.GetUser(model.UserName);

            if (ModelState.IsValid)
            {
                //var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
                ApplicationUser signedUser = await _userManager.FindByEmailAsync(model.Email);

                if (signedUser.IsActive && !signedUser.IsDeleted)
                {
                    var result = await _signInManager.PasswordSignInAsync(signedUser.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        var roles = await _userManager.GetRolesAsync(signedUser);

                        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                        {
                            return Redirect(model.ReturnUrl);
                        }
                        else
                        {
                            if (roles.Contains("Hk Manager"))
                            {
                                return RedirectToAction("HKDashBoardV2", "HkDashboard");
                            }
                            else
                            {
                                return RedirectToAction("Index", "Home");
                            }
                        }
                    }

                    ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

                }
                else
                {
                    throw new Exception("Invalid Email");
                }

            }
            return View(model);
        }
        catch (Exception e)
        {
            ExceptionMsg(e.Message);
            return View();
        }
    }

    [HttpPost]
    public async Task<IActionResult> LogOff()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
