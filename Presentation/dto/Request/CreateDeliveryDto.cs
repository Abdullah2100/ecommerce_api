using System.ComponentModel.DataAnnotations;

namespace api.Presentation.dto.Request;

public class CreateDeliveryDto
{
    [Required] public Guid UserId { get; set; }
    public IFormFile? Thumbnail { get; set; } = null;
}