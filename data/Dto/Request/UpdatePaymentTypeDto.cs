namespace data.dto.Request;

public class UpdatePaymentTypeDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; } = null;
    public bool? IsHashCheckOperation { get; set; } = null;
    public byte[]? Thumbnail { get; set; } = null;
}