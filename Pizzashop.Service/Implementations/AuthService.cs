using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Repository.Interfaces;
using Pizzashop.Service.Interfaces;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    private readonly IJwtService _jwtService;

    public AuthService(IUserRepository userRepository, IEmailService emailService , IJwtService jwtService)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _jwtService = jwtService;
    }

    public async Task<Response<LoginResponse?>> AuthenticateUser(LoginFormModel model)
    {
        var email = model.Email;
        var password = model.Password;

        var user = await _userRepository.GetUserByEmail(email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
             return Response<LoginResponse?>.FailureResponse("Invalid User Credintials!");
        }
        if (!user.IsActive.GetValueOrDefault() || user.IsDeleted.GetValueOrDefault())
        {
            return Response<LoginResponse?>.FailureResponse("Login Failed !! User is currently INACTIVE Or DELETED!");
        }

        var token = _jwtService.GenerateJwtToken(user.Email, user.Id , user.Role.Name , user.IsFirstLogin.ToString()!, model.RememberMe);

        return Response<LoginResponse?>.SuccessResponse(new LoginResponse
        {
            AuthToken = token,
           
        }, "Login successfull!");

    }

    public async Task<Response<User>> SendResetPasswordEmail(ForgotPasswordModel model, HttpRequest request)
    {

        var user = await _userRepository.GetUserByEmail(model.Email);

         if (user == null)
            {
                return Response<User?>.FailureResponse("User does not exist!");
            }


            Guid resetCode = Guid.NewGuid();

            var urlHelperFactory = (IUrlHelperFactory)request.HttpContext.RequestServices.GetService(typeof(IUrlHelperFactory));
            var actionContext = new ActionContext(request.HttpContext, request.HttpContext.GetRouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
            var urlHelper = urlHelperFactory.GetUrlHelper(actionContext);

            var verifyUrl = urlHelper.Action("ResetPassword", "Auth", new { email = user.Email, token = resetCode });

            var resetLink = $"{request.Scheme}://{request.Host}{verifyUrl}";
            user.Token = resetCode;
            user.TokenExpiary = DateTime.Now.AddMinutes(30);
            await _userRepository.UpdateUser(user);


            string subject = "Forgot Password";
            string body = "<div style='background-color: #0066A7; padding: 10px; display: flex; justify-content: center; align-items: center; gap: 2rem;'>" +
          "<img src='http://localhost:5105/images/pizzashop_logo.png' alt=''  style='width:60px; background-color:white; border-radius:50%;'> " +
           "<h1 style='color: white; font-family: sans-serif;'>PIZZASHOP</h1>" +
           "</div>" +
          "<p>" +
          "Pizza shop, <br><br>" +
          "Please click <a style='color : blue' href=" + resetLink + ">here</a> for reset your password. <br><br>" +
          "If you encounter any issues or have any questions, please do not hesitate to contact our support team. <br><br>" +
          "<span style='color: orange;'>Important Note :</span> For security reasons, the link will expire in 24 hours. If you did not  request a password reset, please ignore this email or contact our support team immediately." +
          "</p>";


            _emailService.SendEmailAsync(model.Email, body, subject);

            return Response<User?>.SuccessResponse(user, "Reset password link has been sent to your email id.");

    }

    public async Task<bool> IsTokenValidAsync(string email, Guid token)
    {
        try
        {
            var user = await _userRepository.GetUserByEmail(email);
            if (
                user == null
                || user.Token != token
                || user.TokenExpiary < DateTime.Now)
            {
                return false;
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }


    public async Task<Response<bool>> ResetUserPassword(ResetPasswordModel model)
    {

            var email = model.Email;
            var token = model.Token;
            var newPassword = model.NewPassword;

            var user = await _userRepository.GetUserByEmail(email);

            if (user == null)
            {
                return Response<bool>.FailureResponse("User does not exist!");
            }

            if ((token != user.Token) || (user.TokenExpiary < DateTime.Now))
            {
                return Response<bool>.FailureResponse("Token is invalid or expired!");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            user.Password = passwordHash;
            user.Token = null;
            user.TokenExpiary = null;
            await _userRepository.UpdateUser(user);

            return Response<bool>.SuccessResponse(true, "Password Reseted Successfully!");
     

    }



}


