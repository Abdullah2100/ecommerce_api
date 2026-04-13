namespace api.Presentation.dto.Response
{
    public class ProductVariantDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string? Name { get; set; }
        public int Percentage { get; set; }
        public Guid VariantId { get; set; }
    }
}