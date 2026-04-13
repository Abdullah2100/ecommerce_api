using System.ComponentModel.DataAnnotations;

namespace api.Presentation.dto.Request;

public enum EnOrderItemStatusDto
{
    Excepted,
    Cancelled,
    TookByDelivery
}
public class UpdateOrderItemStatusDto
{
    [Required] public Guid Id { get; set; }
    [Required] public EnOrderItemStatusDto Status { get; set; }
}