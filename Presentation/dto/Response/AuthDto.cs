namespace api.Presentation.dto.Response
{
    public class AuthDto
    {
        public string? Token { get; set; }
        public Guid RefreshToken { get; set; }
    }
}