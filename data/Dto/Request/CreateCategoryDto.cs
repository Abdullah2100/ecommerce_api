using System.ComponentModel.DataAnnotations;

namespace data.dto.Request;

public record class CreateCategoryDto ([StringLength(maximumLength: 50, MinimumLength = 3, ErrorMessage = "Name must not be empty")]
    [Required]
     string Name,  byte[] Image);