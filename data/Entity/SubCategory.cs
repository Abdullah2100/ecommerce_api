using System.ComponentModel.DataAnnotations.Schema;

namespace api.domain.entity;

public class SubCategory : GeneralShredInfo
{
    public string Name { get; set; }
    public Guid StoreId { get; set; }
    public Guid CategoryId { get; set; }
    public virtual Store? Store { get; set; } = null;
    public virtual Category? Category { get; set; } = null;
    public  virtual ICollection<Product> Products { get; set; } = new List<Product>();
}