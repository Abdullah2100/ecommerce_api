using System.ComponentModel.DataAnnotations;

namespace api.Request;

public class UpdateCategoryApiDto
{
    [Required] public Guid Id { get; set; }

    [StringLength(maximumLength: 50, ErrorMessage = "Name must not be empty")]
    public string? Name { get; set; } = null;

    public IFormFile? Image { get; set; } = null;
}