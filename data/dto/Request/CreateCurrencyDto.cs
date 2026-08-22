using System.ComponentModel.DataAnnotations;

namespace data.dto.Request;

public class CreateCurrencyDto
{
    [MaxLength(20)] public string Name { get; set; }
    [MaxLength(10)] public String Symbol { get; set; }
    public int Value { get; set; }
    public bool IsDefault { get; set; }
}