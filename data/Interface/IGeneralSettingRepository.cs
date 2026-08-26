using api.domain.entity;

namespace data.Interface;

public interface IGeneralSettingRepository : IRepository<GeneralSetting>
{
    Task<GeneralSetting?> GetGeneralSetting(Guid id);
    Task<ICollection<GeneralSetting>> Getgenralsettings(int page, int length);

    Task<bool> IsExist(Guid id);
    Task<bool> IsExist(string name);
    Task<bool> IsExist(Guid id, string name);

    void Delete(Guid id);
}