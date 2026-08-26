using System.ComponentModel.DataAnnotations;

namespace data.dto.Request;

public class CreateGeneralSettingDto
{
    [Required] public Guid Id { get; set; }
    [Required] public string Name { get; set; }
    [Required] public decimal Value { get; set; }
}