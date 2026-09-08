using api.domain.entity;
using data.dto.Response;

namespace data.Interface;

public interface IGeneralSettingRepository : IRepository<GeneralSetting>
{
    Task<GeneralSetting?> GetGeneralSetting(Guid id);
    Task<ICollection<GeneralSettingDto>> GeneralSettings(int page, int length);

    Task<bool> IsExist(Guid id);
    Task<bool> IsExist(string name);
    Task<bool> IsExist(Guid id, string name);

    void Delete(Guid id);
}