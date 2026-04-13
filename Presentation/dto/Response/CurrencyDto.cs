namespace api.Presentation.dto.Response;

public class CurrencyDto
{
    public Guid Id { get; set; } 
    public string Name { get; set; }
    public string Symbol { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; } = null;
    public int Value { get; set; }
    public bool IsDefault { get; set; }
}