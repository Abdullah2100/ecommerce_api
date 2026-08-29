using api.domain.entity;

namespace data.Interface;

public interface IVariantRepository : IRepository<Variant>
{
    Task<Variant?> GetVarient(Guid id);
    Task<ICollection<Variant>> GetVarients(int page, int lenght);
    Task<int> GetVarientCount(int variantPerPage);
    Task<bool> IsExist(Guid id);
    Task<bool> IsExist(string name);
    Task<bool> IsExist(string name, Guid id);
    void Delete(Guid id);
}