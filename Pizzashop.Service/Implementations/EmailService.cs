using System.Net;
using System.Net.Mail;
using Pizzashop.Repository.Interfaces;
using Pizzashop.Service.Interfaces;

namespace Pizzashop.Service.Implementations;

public class EmailService : IEmailService
{
    private readonly IUserRepository _userRepository;

    public EmailService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }


    public  void SendEmailAsync(string email, string body, string subject)
    {

            string senderEmail = "test.dotnet@etatvasoft.com";
            string senderPassword = "P}N^{z-]7Ilp";

            SmtpClient client = new SmtpClient();
            client.Credentials = new NetworkCredential(senderEmail, senderPassword);
            client.Port = 587;
            client.Host = "mail.etatvasoft.com";
            client.EnableSsl = true;

            MailMessage mailMessage = new MailMessage(senderEmail, email, subject, body);
            mailMessage.IsBodyHtml = true;

            try
            {
                client.Send(mailMessage);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


    }





}


