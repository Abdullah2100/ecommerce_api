using System.ComponentModel.DataAnnotations;

namespace data.dto.Request;

public class UpdateCategoryDto
{
    [Required] public Guid Id { get; set; }

    [StringLength(maximumLength: 50, ErrorMessage = "Name must not be empty")]
    public string? Name { get; set; } = null;

    public byte[]? Image { get; set; } = null;
}