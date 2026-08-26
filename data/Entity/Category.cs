using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.domain.entity;

public class Category : GeneralShredInfo
{
    public string Name { get; set; }
    public Guid OwnerId { get; set; }
    public bool IsBlocked { get; set; } = false;
    public string Image { get; set; }
    public virtual User User { get; set; } = null!;
    public virtual ICollection<SubCategory> SubCategories { get; set; } = new List<SubCategory>();
}