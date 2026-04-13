namespace api.Presentation.dto.Request;

public class UserDeliveryInfoDto
{
    public required string Name { get; set; } 
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Thumbnail { get; set; }
}