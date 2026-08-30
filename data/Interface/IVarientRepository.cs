using api.domain.entity;

namespace data.Interface;

public interface IVariantRepository : IRepository<Variant>
{
    Task<Variant?> GetVariant(Guid id);
    Task<ICollection<Variant>> GetVariants(int page, int lenght);
    Task<int> GetVariantCount(int variantPerPage);
    Task<bool> IsExist(Guid id);
    Task<bool> IsExist(string name);
    Task<bool> IsExist(string name, Guid id);
    Task Delete(Guid id);
}