using System.ComponentModel.DataAnnotations;

namespace api.Presentation.dto.Request;

public class UpdateAddressDto
{
    [Required] public Guid Id { get; set; } = Guid.Empty;
    public decimal? Longitude { get; set; } = null;
    public decimal? Latitude { get; set; } = null;
        
    [StringLength(maximumLength: 100 , ErrorMessage = "address title must not be empty")]
    public string? Title { get; set; } = null;
}