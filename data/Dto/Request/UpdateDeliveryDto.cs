using System.ComponentModel.DataAnnotations;

namespace data.dto.Request;

public class UpdateDeliveryDto
{
    public byte[]? Thumbnail { get; set; } = null;
    public decimal? Longitude { get; set; } = null;
    public decimal? Latitude { get; set; } = null;

    [StringLength(maximumLength: 50, ErrorMessage = "Enter Valid Name")]
    public string? Name { get; set; } = null;

    [StringLength(maximumLength: 13, ErrorMessage = "Enter Valid Name")]
    public string? Phone { get; set; } = null;

    public byte[]? UserThumbnail { get; set; } = null;
    public string? Password { get; set; } = null;
    public string? NewPassword { get; set; } = null;
}