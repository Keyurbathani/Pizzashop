using System.Security.Claims;
using Pizzashop.Service.Interfaces;

namespace Pizzashop.Service.Implementations;

public class TokenEmailService : ITokenEmailService
{
    private readonly IJwtService _jwtservice;

    public TokenEmailService(IJwtService jwtservice)
    {
        _jwtservice = jwtservice;
    }

    

    public int GetIdFromToken(string token)
    {
        if (!string.IsNullOrEmpty(token))
        {
            var principal = _jwtservice.ValidateToken(token);
            if (principal != null)
            {
        
                var IdValue = principal.Claims.First(claim => claim.Type == "Id");
                var id = IdValue.Value;

                return  int.Parse(id);
            }
        }

        return  0;
    }
    
}
