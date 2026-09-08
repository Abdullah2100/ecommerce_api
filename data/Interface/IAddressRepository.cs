using api.domain.entity;
using data.dto.Response;

namespace data.Interface;

public interface IAddressRepository : IRepository<Address>
{
    Task<int> GetAddressCount(Guid id);
    Task<Address?> GetAddress(Guid id);
    Task<Address?> GetAddressByOwnerId(Guid id);
    Task MakeAddressNotCurrentToId(Guid ownerId);
    void Delete(Guid id);
}