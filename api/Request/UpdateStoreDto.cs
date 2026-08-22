using System.ComponentModel.DataAnnotations;

namespace api.Request;

public class UpdateStoreApiDto
{
    [StringLength(maximumLength: 100, ErrorMessage = "name must not  be empty")]
    public string? Name { get; set; } = null;

    public IFormFile? WallpaperImage { get; set; } = null;
    public IFormFile? SmallImage { get; set; } = null;
    public decimal? Longitude { get; set; }
    public decimal? Latitude { get; set; }
}