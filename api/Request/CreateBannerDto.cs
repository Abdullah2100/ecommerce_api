using System.ComponentModel.DataAnnotations;

namespace api.Request;

public class CreateBannerApiDto
{
    [Required] public IFormFile Image { get; set; }
    [Required] public DateTime EndAt { get; set; }
}