using api.Infrastructure;

namespace api.application;

public class ConfigurationImplement(IConfiguration configurationService) : IConfig
{

        private readonly IConfiguration? _configurationService = configurationService;

        public string GetKey(string key)
        {
            string result = "";
            if (_configurationService !=  null)
            {
                result = _configurationService[key]!;
            }
            return result;
        }
        

}