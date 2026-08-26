using System.ComponentModel.DataAnnotations;

namespace data.dto.Request;

public class CreateDeliveryDto
{
    [Required] public Guid UserId { get; set; }
    public byte[]? Thumbnail { get; set; } = null;
}