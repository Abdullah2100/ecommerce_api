namespace api.Request;

public class CreatePaymentTypeApiDto
{
    public required string Name { get; set; }
    public bool IsHashCheckOperation { get; set; }
    public required IFormFile Thumbnail { get; set; }
}