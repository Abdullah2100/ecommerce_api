using System.ComponentModel.DataAnnotations;

namespace api.Presentation.dto.Request;

public class CreatePaymentTypeDto
{
    public required string Name { get; set; }
    public bool IsHashCheckOperation { get; set; }
    public required IFormFile Thumbnail { get; set; }
}