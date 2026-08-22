using Microsoft.Build.Framework;

namespace api.Request;

public class CreateGeneralSettingApiDto
{
    [Required] public Guid Id { get; set; }
    [Required] public string Name { get; set; }
    [Required] public decimal Value { get; set; }
}