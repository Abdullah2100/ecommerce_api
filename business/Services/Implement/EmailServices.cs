using System.Net;
using System.Net.Mail;
using api.application.Services.Interface;
using api.Settings;
using business.Services.Interface;
using Microsoft.Extensions.Options;

namespace business.Services.Implement;

public class EmailServices(IOptions<SmtpSetting> smtp) : IMessageService
{
    public async Task<bool> SendingMessage(string message, string to)
    {
        try
        {
            var serverUrl = smtp.Value.Url;
            var userName = smtp.Value.Username;
            var password = smtp.Value.Password;
            var port = smtp.Value.Port;

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
                    "Otp Validation",
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