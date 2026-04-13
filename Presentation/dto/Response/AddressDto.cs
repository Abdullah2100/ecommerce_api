namespace api.Presentation.dto.Response
{
    public class AddressDto
    {
        public Guid Id { get; set; }
        public decimal? Longitude { get; set; } = null;
        public decimal? Latitude { get; set; } = null;
        public string Title { get; set; }
        public bool IsCurrent { get; set; } = false;
    }
}