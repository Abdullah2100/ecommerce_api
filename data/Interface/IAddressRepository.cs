using api.domain.entity;

namespace data.Interface;

public interface IAddressRepository : IRepository<Address>
{
    Task<int> GetAddressCount(Guid id);
    Task<Address?> GetAddress(Guid id);
    Task<Address?> GetAddressByOwnerId(Guid id);
    Task<ICollection<Address>> GetAllAddressByOwnerId(Guid id);
    Task MakeAddressNotCurrentToId(Guid ownerId);
    void Delete(Guid id);
}