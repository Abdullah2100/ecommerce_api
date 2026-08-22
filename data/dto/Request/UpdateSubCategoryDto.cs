using System.ComponentModel.DataAnnotations;

namespace data.dto.Request;

public class UpdateSubCategoryDto
{
    [Required] public Guid Id { get; set; }

    [StringLength(maximumLength: 50, ErrorMessage = "name must not  be empty")]
    public string? Name { get; set; } = null;

    public Guid? CategoryId { get; set; } = null;
}