using System.ComponentModel.DataAnnotations;

namespace api.Request;

public record class CreateCategoryApiDto
(
    [StringLength(maximumLength: 50, MinimumLength = 3, ErrorMessage = "Name must not be empty")]
    [Required]
     string Name,

    [Required]  IFormFile Image
    );