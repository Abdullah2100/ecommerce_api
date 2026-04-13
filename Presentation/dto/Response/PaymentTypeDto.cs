namespace api.Presentation.dto.Response;

public class PaymentTypeDto
{
   public Guid Id { get; set; }
   public string Name { get; set; }
   public bool IsHashCheckOperation { get; set; }
   public string Thumbnail { get; set; }
}