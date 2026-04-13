using System.ComponentModel.DataAnnotations;

namespace api.Presentation.dto.Request;

public class UpdateDeliveryDto
{
    public IFormFile? Thumbnail { get; set; } = null;
    public decimal? Longitude { get; set; } = null;
    public decimal? Latitude { get; set; } = null;

    [StringLength(maximumLength: 50 , ErrorMessage = "Enter Valid Name")]
    public string? Name { get; set; } = null;
        
    [StringLength(maximumLength: 13, ErrorMessage = "Enter Valid Name")]
    public string? Phone { get; set; } = null;
    public IFormFile? UserThumbnail { get; set; } = null;
    public string? Password { get; set; } = null;
    public string? NewPassword { get; set; } = null;
}