namespace business.Services.Interface;

public interface IMessageService
{
    Task<bool> SendingMessage(string message, string to);
}