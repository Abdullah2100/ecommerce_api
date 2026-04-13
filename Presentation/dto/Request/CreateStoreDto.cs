using System.ComponentModel.DataAnnotations;

namespace api.Presentation.dto.Request;

public class CreateStoreDto
{
    [StringLength(maximumLength: 100, MinimumLength = 2, ErrorMessage = "Name must not be empty")]
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required] public  IFormFile WallpaperImage { get; set; }
    [Required] public IFormFile SmallImage { get; set; }

    [Required]
    public decimal Longitude { get; set; }
        
    [Required]
    public decimal Latitude { get; set; }
}