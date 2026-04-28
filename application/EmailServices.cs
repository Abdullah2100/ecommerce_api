using System.Net;
using System.Net.Mail;
using api.application.Interface;
using api.Infrastructure;

namespace api.application;

public class EmailServices(IConfig config) : IMessageService 
{
    public async Task<bool> SendingMessage(string message, string to)
    {
        try
        {
            var serverUrl = config.GetKey("smtp_data:url");
            var userName = config.GetKey("smtp_data:username");
            var password = config.GetKey("smtp_data:password");
            var port = config.GetKey("smtp_data:port");

            var client = new SmtpClient(serverUrl, Convert.ToInt32((port)))
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(userName, password)
            };
            await client.SendMailAsync(
                new MailMessage(
                    userName
                    , to,
                    "Otp Valdiation",
                    message)
            );
            return true;
        }
        catch (System.Exception ex)
        {
            Console.WriteLine("this error from sending otp to user " + ex.Message);
            return false;
        }
    }
}