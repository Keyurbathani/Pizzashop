using Pizzashop.Entity.Data;

namespace Pizzashop.Service.Interfaces;

public interface IEmailService
{
      void SendEmailAsync(string email , string body , string subject);
}
