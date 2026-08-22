using System.ComponentModel.DataAnnotations;

namespace api.Request;

public class CreateDeliveryApiDto
{
    [Required] public Guid UserId { get; set; }
    public IFormFile? Thumbnail { get; set; } = null;
}