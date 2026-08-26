using System.ComponentModel.DataAnnotations;

namespace data.dto.Request;

public class UpdateCurrencyDto
{
    public Guid Id { get; set; }
    [MaxLength(20)] public string? Name { get; set; } = null;

    [MaxLength(10)] public String? Symbol { get; set; } = null;
    public int? Value { get; set; } = null;
}