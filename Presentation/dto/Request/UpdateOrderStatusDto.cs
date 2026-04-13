namespace api.Presentation.dto.Request;

public class UpdateOrderStatusDto
{
    public Guid Id { get; set; }
    public int Status { get; set; }
}