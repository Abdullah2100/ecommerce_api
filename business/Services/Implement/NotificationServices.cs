using business.Services.Interface;
using FirebaseAdmin.Messaging;

namespace business.Services.Implement;

public class NotificationServices : IMessageService
{
    public async Task<bool> SendingMessage(string message, string to)
    {
        var messageObj = new Message
        {
            Notification = new Notification()
            {
                Title = message,
            },
            Token = to
        };


        var response = await FirebaseMessaging.DefaultInstance.SendAsync(messageObj);


        return (response is null) switch
        {
            true => true,
            _ => false,
        };
    }
}