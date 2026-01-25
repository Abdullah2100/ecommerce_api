using Microsoft.Build.Framework;

namespace api.Presentation.dto;

public class PaymentRequirementData
{
    public long Amount{ get; set; }
   [Required]  public string Currency{ get; set; }
}