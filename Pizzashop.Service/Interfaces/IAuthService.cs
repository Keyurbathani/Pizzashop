using Microsoft.AspNetCore.Http;
using Pizzashop.Entity.Data;
using Pizzashop.Entity.ViewModels;
using Pizzashop.Service.Utils;

namespace Pizzashop.Service.Interfaces;

public interface IAuthService
{
    Task<Response<LoginResponse?>> AuthenticateUser(LoginFormModel model);
    
    Task<Response<User>> SendResetPasswordEmail(ForgotPasswordModel model, HttpRequest request);

    Task<Response<bool>> ResetUserPassword(ResetPasswordModel model);

    Task<bool> IsTokenValidAsync(string email, Guid token);
   
}


