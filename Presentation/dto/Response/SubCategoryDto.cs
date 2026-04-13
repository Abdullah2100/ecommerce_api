namespace api.Presentation.dto.Response
{

public class SubCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid CategoryId { get; set; } 
    public Guid StoreId { get; set; } 

}
}
