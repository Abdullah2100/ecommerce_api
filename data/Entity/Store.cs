using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.domain.entity;

public class Store : GeneralShredInfo
{
    public string Name { get; set; }
    public string WallpaperImage { get; set; }
    public string SmallImage { get; set; }
    public bool IsBlock { get; set; } = true;
    public Guid UserId { get; set; }
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
    public virtual ICollection<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
    public virtual ICollection<Banner> Banners { get; set; } = new List<Banner>();
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    public virtual ICollection<OrderItem> OddrderItems { get; set; } = new List<OrderItem>();

    public virtual User user { get; set; } = null!;
}