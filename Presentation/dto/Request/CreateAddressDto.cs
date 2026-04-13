using System.ComponentModel.DataAnnotations;

namespace api.Presentation.dto.Request;

public class CreateAddressDto
{
    [Required]
    public decimal Longitude { get; set; } = 0;

    [Required]
    public decimal Latitude { get; set; } = 0;

    [StringLength(maximumLength: 100, MinimumLength = 3, ErrorMessage = "address title must not be empty")]
    public string Title { get; set; } = string.Empty;
}