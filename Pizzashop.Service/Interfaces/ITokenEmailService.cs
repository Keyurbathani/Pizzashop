namespace Pizzashop.Service.Interfaces;

public interface ITokenEmailService
{
    int GetIdFromToken(string token);
}
