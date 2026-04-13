namespace api.Presentation.dto.Response
{
    
    
    public class ProductDto
    {
        public Guid Id { get; set; }
        public String Symbol { get; set; } 
        public string Name { get; set; }
        public string Description { get; set; }
        public string  Thumbnail { get; set; }
        public Guid SubcategoryId { get; set; }
        public Guid CategoryId { get; set; }
        public Guid StoreId { get; set; }
        public string StoreName { get; set; }
        public int Price { get; set; }
        public List<List<ProductVariantDto>>? ProductVariants { get; set; }
        public List<string> ProductImages { get; set; }
    }
}