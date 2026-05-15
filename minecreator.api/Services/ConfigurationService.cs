using minecreator.api.Helpers;

namespace minecreator.api.Services
{
    public interface IConfigurationService
    {
        AppConfig GetConfig();
    }

    public class ConfigurationService : IConfigurationService
    {
        private readonly AppConfig _config;
        public ConfigurationService()
        {
            _config = new AppConfig();

            ColorHelper.Init(_config.MaxColorCount, _config.MaxPalleteSize);
        }
        public AppConfig GetConfig()
        {
            return _config;
        }

    }
}
