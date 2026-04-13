namespace api.Presentation.dto.Request;

public class UpdatePaymentTypeDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; } = null;
    public bool? IsHashCheckOperation { get; set; } = null;
    public IFormFile? Thumbnail { get; set; } = null;
}