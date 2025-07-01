using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Repository.Interfaces;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;

namespace Pizzashop.Web.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public AuthController(IAuthService authService, IJwtService jwtService, IUserRepository userRepository)
    {
        _authService = authService;
        _jwtService = jwtService;
        _userRepository = userRepository;
    }
    public IActionResult Login()
    {
        var authtoken = Request.Cookies["AuthToken"];
       

        if (authtoken != null)
        {
            var validAuthToken = _jwtService.ValidateToken(authtoken);


            if (validAuthToken != null)
            {
                return RedirectToAction("Index", "Dashboard");
            }

        }

        return View();

    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginFormModel model)
    {

        if (ModelState.IsValid)
        {
            var response = await _authService.AuthenticateUser(model);

           if (!response.Success)
            {
                TempData["error"] = response.Message;
                return View(model);
            }

            var loginResponse = response.Data!;

            if (model.RememberMe)
            {
                CookieUtils.SaveJWTTokenWithRememberMe(Response, loginResponse.AuthToken);
            }
            else
            {
                CookieUtils.SaveJWTToken(Response, loginResponse.AuthToken);
            }

            TempData["success"] = response.Message;
            var user = await _userRepository.GetUserByEmail(model.Email);
            if (!user.IsFirstLogin.GetValueOrDefault())
            {
                return RedirectToAction("Index", "Dashboard");
            }
            
            return RedirectToAction("ChangePassword", "Dashboard");


        }

        return View(model);
    }

    public IActionResult ForgetPassword(string email)
    {
       
        ForgotPasswordModel forgotPasswordModel = new()
        {
            Email = email
        };

        return View(forgotPasswordModel);

    }


    [HttpPost]
    public async Task<IActionResult> ForgetPassword(ForgotPasswordModel model)
    {

        if (ModelState.IsValid)
        {
            

            var response = await _authService.SendResetPasswordEmail(model, Request);

             if (!response.Success)
            {
              TempData["error"] = response.Message;
              return View(model);
            }
            
            
            TempData["success"] = response.Message;
            
        }

        return View(model);

    }

    public async Task<IActionResult> ResetPassword(string email, Guid token)
    {

        var isTokenValid = await _authService.IsTokenValidAsync(email , token);

        if (!isTokenValid)
        {
            TempData["error"] = "Invalid reset password link";
            return RedirectToAction("Login");
        }

        ResetPasswordModel model = new()
        {
            Email = email,
            Token = token
        };

        return View(model);

    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
    {
        if (ModelState.IsValid)
        {
            var response = await _authService.ResetUserPassword(model) ;

        if (response.Success)
        {
            TempData["success"] = response.Message;
             return RedirectToAction("Login");
        }
        else
        {
            TempData["error"] = response.Message;
        }
            
        }
        return View(model);
    }



}





