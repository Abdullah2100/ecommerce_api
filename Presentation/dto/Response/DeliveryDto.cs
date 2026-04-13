using api.Presentation.dto.Request;

namespace api.Presentation.dto.Response
{
    public class DeliveryDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public bool IsAvailable { get; set; } = true;
        public DateTime? UpdatedAt { get; set; } = null;
        public string? Thumbnail { get; set; }
        public DeliveryAddressDto? Address { get; set; } = null;
        public DeliveryAnalyseDto? Analyse { get; set; } = null;
        public UserDeliveryInfoDto? User { get; set; } = null;
    }
}