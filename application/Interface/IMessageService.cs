namespace api.application.Interface;

public interface IMessageService
{
    Task<bool> SendingMessage(string message, string to);
}
