using System.ComponentModel.DataAnnotations;

namespace data.dto.Response;

public class PaymentRequirementData
{
    public long Amount { get; set; }
    [Required] public string Currency { get; set; }
}