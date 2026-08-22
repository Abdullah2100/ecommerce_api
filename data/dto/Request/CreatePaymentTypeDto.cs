namespace data.dto.Request;

public class CreatePaymentTypeDto
{
    public required string Name { get; set; }
    public bool IsHashCheckOperation { get; set; }
    public required byte[] Thumbnail { get; set; }
}