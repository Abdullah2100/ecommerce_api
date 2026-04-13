namespace api.Presentation.dto.Response;

public class AddressWithTitleDto
{
    public decimal? Longitude { get; set; } = null;
    public decimal? Latitude { get; set; } = null;
    public string Title { get; set; } = string.Empty;
}