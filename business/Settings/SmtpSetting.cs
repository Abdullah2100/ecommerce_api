namespace api.Settings;

public class SmtpSetting
{
    private const string Name = "smtp_data";
    public string Url { get; set; }
    public string Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}