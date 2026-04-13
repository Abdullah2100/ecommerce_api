using Microsoft.Build.Framework;

namespace api.Presentation.dto.Response;

public class PaymentRequirementData
{
    public long Amount{ get; set; }
   [Required]  public string Currency{ get; set; }
}