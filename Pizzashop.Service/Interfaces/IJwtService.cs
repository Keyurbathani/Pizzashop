using System.Security.Claims;

namespace Pizzashop.Service.Interfaces;

public interface IJwtService
{
    string GenerateJwtToken(string email,int id, string role , string FirstLogin, bool? RememberMe);
    ClaimsPrincipal? ValidateToken(string token);
}
