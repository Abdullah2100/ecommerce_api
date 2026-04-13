namespace api.Presentation.dto.Response
{
    public class BannerDto
    {
        public Guid Id { get; set; }
        public String Image { get; set; }
        public DateTime EndAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid StoreId { get; set; }
    }
}