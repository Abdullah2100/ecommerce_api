using System.ComponentModel.DataAnnotations;

namespace api.Presentation.dto.Request;

public class CreateBannerDto
{
    [Required] public IFormFile Image { get; set; }
    [Required] public DateTime EndAt { get; set; }
}