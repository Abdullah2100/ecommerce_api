namespace api.Settings;

public class CredentialSetting
{
    public const string Name = "credentials";
    public string key { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
}