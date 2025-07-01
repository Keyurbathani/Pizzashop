using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Pizzashop.Entity.Data;

namespace Pizzashop.Service.Utils;

public static class CookieUtils
{

    // Save JWT Token to Cookies
    public static void SaveJWTToken(HttpResponse response, string token)
    {
        response.Cookies.Append("AuthToken", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = DateTime.UtcNow.AddHours(24)
        });
    }

    // for remember me
    public static void SaveJWTTokenWithRememberMe(HttpResponse response, string token)
    {
        response.Cookies.Append("AuthToken", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = DateTime.UtcNow.AddMonths(1)
            
        });
    }
   

    // public static string? GetJWTToken(HttpRequest request)
    // {
    //     _ = request.Cookies.TryGetValue("AuthToken", out string? token);
    //     return token;
    // }


    // Save User data to Cookies
    // public static void SaveUserData(HttpResponse response, User user)
    // {
    //     string userData = JsonSerializer.Serialize(user);

    //     // Store user details in a cookie for 3 days
    //     var cookieOptions = new CookieOptions
    //     {
    //         Expires = DateTime.UtcNow.AddDays(3),
    //         HttpOnly = true,
    //         Secure = true,
    //         IsEssential = true
    //     };
    //     response.Cookies.Append("UserData", userData, cookieOptions);
    // }

    
}

