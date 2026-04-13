namespace api.Presentation.dto.Request;

public class CreatePaymentTypeDto
{
    public string Name { get; set; }
    public bool IsHashCheckOperation { get; set; }
    public IFormFile Thumbnail { get; set; }
}