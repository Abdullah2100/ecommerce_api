using Microsoft.Build.Framework;

namespace api.Presentation.dto.Request;

public class CreateGeneralSettingDto
{
    [Required] public Guid Id { get; set; }
    [Required] public string Name { get; set; }
    [Required] public decimal Value { get; set; }
}